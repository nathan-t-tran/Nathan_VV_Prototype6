using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI targetShapeText;
    public UITargetDisplay targetDisplay; // Reference to the UI target display component

    // Number of blocks to include in each target shape
    public int minBlocksInTarget = 2;
    public int maxBlocksInTarget = 3;

    private int currentLevel = 1;
    private List<Block.BlockType> targetBlockTypes = new List<Block.BlockType>();
    private BlockSpawner blockSpawner;

    void Start()
    {
        blockSpawner = FindFirstObjectByType<BlockSpawner>();

        // Verify target display is assigned
        if (targetDisplay == null)
        {
            Debug.LogError("UITargetDisplay component is not assigned to Level Manager!");
        }

        GenerateNewLevel();
    }

    public void GenerateNewLevel()
    {
        // Update UI
        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel;
        }

        // Generate a random combination of blocks for the target
        GenerateRandomTargetShape();

        // Display target shape visually
        DisplayTargetShape();

        // Reset timer in MovingPlatform
        MovingPlatform platform = FindFirstObjectByType<MovingPlatform>();
        if (platform != null)
        {
            platform.ResetTimer();
        }

        // Clear existing blocks on platform
        ClearExistingBlocks();

        // Make sure block spawner is running
        BlockSpawner spawner = FindFirstObjectByType<BlockSpawner>();
        if (spawner != null && !spawner.IsSpawning())
        {
            spawner.StartSpawning();
        }
    }

    private void GenerateRandomTargetShape()
    {
        // Clear previous target
        targetBlockTypes.Clear();

        // Determine number of blocks for this target (increases with level)
        int numBlocks = Mathf.Min(minBlocksInTarget + (currentLevel - 1) / 3, maxBlocksInTarget);
        numBlocks = Mathf.Clamp(numBlocks, minBlocksInTarget, maxBlocksInTarget);

        // Get all available block types
        List<Block.BlockType> availableTypes = new List<Block.BlockType>();
        foreach (Block.BlockType type in System.Enum.GetValues(typeof(Block.BlockType)))
        {
            availableTypes.Add(type);
        }

        // Randomly select unique block types for the target
        for (int i = 0; i < numBlocks; i++)
        {
            if (availableTypes.Count == 0) break;

            int randomIndex = Random.Range(0, availableTypes.Count);
            targetBlockTypes.Add(availableTypes[randomIndex]);
            availableTypes.RemoveAt(randomIndex); // Remove to ensure uniqueness
        }

        // Update the target text
        if (targetShapeText != null)
        {
            targetShapeText.text = "Collect:";
        }

        Debug.Log("Generated target shape with " + targetBlockTypes.Count + " blocks:");
        foreach (Block.BlockType type in targetBlockTypes)
        {
            Debug.Log("- " + type.ToString());
        }
    }

    private void DisplayTargetShape()
    {
        if (targetDisplay == null)
        {
            Debug.LogError("Target display component is missing!");
            return;
        }

        // Use the new method to display all blocks at once with proper spacing
        targetDisplay.DisplayBlockTypes(targetBlockTypes);

        Debug.Log("Displayed " + targetBlockTypes.Count + " target shapes");
    }

    private void ClearExistingBlocks()
    {
        // Find all blocks that are children of the platform
        GameObject platformObj = GameObject.FindGameObjectWithTag("Platform");
        if (platformObj == null)
        {
            Debug.LogWarning("Platform not found when clearing blocks");
            return;
        }

        Transform platform = platformObj.transform;
        List<GameObject> blocksToRemove = new List<GameObject>();

        foreach (Transform child in platform)
        {
            if (child.CompareTag("Block"))
            {
                blocksToRemove.Add(child.gameObject);
            }
        }

        // Destroy all found blocks
        foreach (GameObject block in blocksToRemove)
        {
            Destroy(block);
        }
    }

    private void ClearAllBlocks()
    {
        // Find and destroy ALL blocks in the scene
        GameObject[] allBlocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in allBlocks)
        {
            Destroy(block);
        }
    }

    public void LevelCompleted()
    {
        currentLevel++;

        // Stop previous block spawning
        BlockSpawner spawner = FindFirstObjectByType<BlockSpawner>();
        if (spawner != null)
        {
            spawner.StopSpawning();
        }

        // Clear any floating blocks before starting new level
        ClearAllBlocks();

        // Generate new level
        GenerateNewLevel();

        // Restart block spawning for new level
        if (spawner != null)
        {
            spawner.StartSpawning();
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public List<Block.BlockType> GetTargetBlockTypes()
    {
        return targetBlockTypes;
    }
}