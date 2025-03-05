using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI instructionText;
    public GameObject gameOverPanel;

    private LevelManager levelManager;
    private MovingPlatform platform;
    private bool isGameActive = true;
    private int score = 0;
    private Dictionary<Block.BlockType, int> caughtBlocks = new Dictionary<Block.BlockType, int>();

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        levelManager = FindFirstObjectByType<LevelManager>();
        platform = FindFirstObjectByType<MovingPlatform>();

        // Initialize UI
        UpdateScoreDisplay();
        gameOverPanel.SetActive(false);

        // Start the spawner if it exists
        BlockSpawner spawner = FindFirstObjectByType<BlockSpawner>();
        if (spawner != null)
        {
            spawner.StartSpawning();
        }

        // Initialize the caught blocks dictionary
        ResetCaughtBlocks();
    }

    void Update()
    {
        // Check for game over conditions
        if (platform.IsTimeUp() && isGameActive)
        {
            GameOver("Time's Up!");
        }

        // Restart game on R key
        if (Input.GetKeyDown(KeyCode.R) && !isGameActive)
        {
            RestartGame();
        }
    }

    private void ResetCaughtBlocks()
    {
        caughtBlocks.Clear();

        // Initialize with zero for all block types
        foreach (Block.BlockType type in System.Enum.GetValues(typeof(Block.BlockType)))
        {
            caughtBlocks[type] = 0;
        }
    }

    public void BlockCaught(Block block)
    {
        // Check if the caught block is needed for the target shape
        List<Block.BlockType> targetTypes = levelManager.GetTargetBlockTypes();
        bool isNeededBlock = targetTypes.Contains(block.type);

        if (isNeededBlock)
        {
            // Play success sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySuccessSound();
            }

            // Increment the count for this block type
            caughtBlocks[block.type]++;

            Debug.Log("Caught needed block: " + block.type + ", Total of this type: " + caughtBlocks[block.type]);

            // Check if player has caught the target shape
            CheckForTargetMatch();
        }
        else
        {
            // Play wrong block sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayWrongSound();
            }

            // Penalty for catching wrong block type
            score = Mathf.Max(0, score - 50);
            UpdateScoreDisplay();

            Debug.Log("Caught wrong block: " + block.type + ", -50 points!");

            // Show feedback for wrong block
            StartCoroutine(ShowInstructionText("Wrong Block! -50 Points", 1f));
        }
    }

    public void BlockMissed(Block block)
    {
        // Don't end game when a block is missed, deduct points instead
        if (isGameActive)
        {
            // Play missed sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayMissedSound();
            }

            // Deduct points but keep score non-negative
            score = Mathf.Max(0, score - 50);
            UpdateScoreDisplay();

            // Show feedback for missed block
            StartCoroutine(ShowInstructionText("Block Missed! -50 Points", 1f));
        }
    }

    private void CheckForTargetMatch()
    {
        // Get target blocks from level manager
        List<Block.BlockType> targetBlocks = levelManager.GetTargetBlockTypes();
        bool hasAllTargetBlocks = true;

        // Check if we have at least one of each target block type
        foreach (Block.BlockType type in targetBlocks)
        {
            if (caughtBlocks[type] < 1)
            {
                hasAllTargetBlocks = false;
                break;
            }
        }

        // If we have all target blocks, complete the level
        if (hasAllTargetBlocks)
        {
            // Level completed
            LevelComplete();
        }
    }

    private void LevelComplete()
    {
        // Play level complete sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayLevelCompleteSound();
        }

        int levelBonus = 100 * levelManager.GetCurrentLevel();
        score += levelBonus;
        UpdateScoreDisplay();

        // Reset caught blocks for next level
        ResetCaughtBlocks();

        // Generate new level
        levelManager.LevelCompleted();

        // Show success message briefly
        StartCoroutine(ShowInstructionText("Level Complete! +" + levelBonus + " Points", 2f));
    }

    private void UpdateScoreDisplay()
    {
        scoreText.text = "Score: " + score;
    }

    private void GameOver(string message)
    {
        isGameActive = false;

        // Play game over sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayGameOverSound();
        }

        // Stop block spawning
        FindFirstObjectByType<BlockSpawner>().StopSpawning();

        // Show game over panel
        gameOverPanel.SetActive(true);
        gameOverText.text = message + "\nFinal Score: " + score + "\nPress R to Restart";
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }

    public int GetCurrentLevel()
    {
        return levelManager.GetCurrentLevel();
    }

    // Coroutine to show instruction text for a limited time
    private IEnumerator ShowInstructionText(string text, float duration)
    {
        instructionText.text = text;
        instructionText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        instructionText.gameObject.SetActive(false);
    }
}