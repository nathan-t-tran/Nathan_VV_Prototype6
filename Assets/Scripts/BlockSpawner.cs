using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameObject blockPrefab;
    public float initialSpawnRate = 2f; // Slightly faster spawn interval
    public float minX = -4f;
    public float maxX = 4f;
    public float spawnY = 6f;

    // Fall speed settings
    public float baseFallSpeed = 2.5f; // Increased from 1.0f to 2.5f
    public float fallSpeedIncreasePerLevel = 0.2f; // How much to increase fall speed per level

    private Dictionary<Block.BlockType, Vector2[]> blockShapes;
    private bool isSpawning = false;
    private Coroutine spawnCoroutine;

    void Start()
    {
        InitializeBlockShapes();
        // Start spawning coroutine
        StartSpawning();
    }

    void OnEnable()
    {
        // Make sure we're spawning when enabled
        if (!isSpawning)
        {
            StartSpawning();
        }
    }

    void OnDisable()
    {
        // Stop spawning when disabled
        StopSpawning();
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            // Start the coroutine and store its reference
            spawnCoroutine = StartCoroutine(SpawnBlockRoutine());
        }
    }

    public void StopSpawning()
    {
        if (isSpawning && spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            isSpawning = false;
            spawnCoroutine = null;
        }
    }

    public bool IsSpawning()
    {
        return isSpawning;
    }

    private IEnumerator SpawnBlockRoutine()
    {
        // Wait a bit before first spawn
        yield return new WaitForSeconds(1f);

        int spawnCount = 0;

        while (isSpawning)
        {
            // Only spawn if game is active
            if (GameManager.Instance != null && GameManager.Instance.IsGameActive())
            {
                spawnCount++;
                Debug.Log("Spawning block #" + spawnCount);

                SpawnRandomBlock();

                // Use a fixed spawn interval of 2 seconds
                yield return new WaitForSeconds(initialSpawnRate);
            }
            else
            {
                // If game is not active, just wait a bit
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private void InitializeBlockShapes()
    {
        blockShapes = new Dictionary<Block.BlockType, Vector2[]>();

        // Define block shapes as relative positions of blocks
        blockShapes[Block.BlockType.I] = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(3, 0) };
        blockShapes[Block.BlockType.J] = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(2, 0) };
        blockShapes[Block.BlockType.L] = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, 1) };
        blockShapes[Block.BlockType.O] = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
        blockShapes[Block.BlockType.S] = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1) };
        blockShapes[Block.BlockType.T] = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(1, 1) };
        blockShapes[Block.BlockType.Z] = new Vector2[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(2, 0) };
    }

    private void SpawnRandomBlock()
    {
        // Choose random block type
        Block.BlockType blockType = (Block.BlockType)Random.Range(0, System.Enum.GetValues(typeof(Block.BlockType)).Length);

        // Random x position for spawning with more restricted range
        float spawnX = Random.Range(-3f, 3f); // More centered spawn
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);

        Debug.Log("Spawning block at position: " + spawnPosition);

        // Create parent object for the block shape
        GameObject blockParent = new GameObject("Block_" + blockType.ToString());
        blockParent.transform.position = spawnPosition;
        blockParent.AddComponent<BoxCollider2D>();
        Rigidbody2D rb = blockParent.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        // Add Block component
        Block blockScript = blockParent.AddComponent<Block>();
        blockScript.type = blockType;

        // Calculate fall speed based on level (faster than before)
        int currentLevel = GameManager.Instance.GetCurrentLevel();
        blockScript.fallSpeed = baseFallSpeed + (currentLevel * fallSpeedIncreasePerLevel);

        // Generate the blocks based on shape
        Vector2[] shape = blockShapes[blockType];
        float blockSize = 0.5f; // Size of individual blocks

        foreach (Vector2 pos in shape)
        {
            GameObject blockPiece = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity);
            blockPiece.transform.localScale = new Vector3(blockSize, blockSize, 1f);
            blockPiece.transform.SetParent(blockParent.transform);
            blockPiece.transform.localPosition = new Vector3(pos.x * blockSize, pos.y * blockSize, 0);

            // Ensure SpriteRenderer exists and has vibrant color
            SpriteRenderer renderer = blockPiece.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = blockPiece.AddComponent<SpriteRenderer>();
            }

            blockPiece.tag = "Block";
        }

        // Tag the parent object
        blockParent.tag = "Block";

        // Set block type (assigns color) - ensure this works
        blockScript.SetBlockType(blockType);
    }

    // Method to spawn a specific block type (used for target shape)
    public GameObject SpawnSpecificBlock(Block.BlockType blockType, Vector3 position)
    {
        Debug.Log("Spawning target display block of type: " + blockType);

        // Make sure shapes are initialized
        if (blockShapes == null || blockShapes.Count == 0)
        {
            InitializeBlockShapes();
        }

        // Verify we have this shape defined
        if (!blockShapes.ContainsKey(blockType))
        {
            Debug.LogError("Missing shape definition for block type: " + blockType);
            return null;
        }

        GameObject blockParent = new GameObject("Target_" + blockType.ToString());
        blockParent.transform.position = position;

        // Generate the blocks based on shape
        Vector2[] shape = blockShapes[blockType];
        float blockSize = 0.5f; // Size of individual blocks

        foreach (Vector2 pos in shape)
        {
            GameObject blockPiece = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity);
            blockPiece.transform.localScale = new Vector3(blockSize, blockSize, 1f);
            blockPiece.transform.SetParent(blockParent.transform);
            blockPiece.transform.localPosition = new Vector3(pos.x * blockSize, pos.y * blockSize, 0);

            // Make sure it has a SpriteRenderer with color
            SpriteRenderer renderer = blockPiece.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = blockPiece.AddComponent<SpriteRenderer>();
                renderer.sprite = Resources.Load<Sprite>("Square"); // Default Unity sprite
            }

            // Make it semi-transparent but clearly visible
            switch (blockType)
            {
                case Block.BlockType.I:
                    renderer.color = new Color(0f, 1f, 1f, 0.7f); // Cyan
                    break;
                case Block.BlockType.J:
                    renderer.color = new Color(0f, 0f, 1f, 0.7f); // Blue
                    break;
                case Block.BlockType.L:
                    renderer.color = new Color(1f, 0.5f, 0f, 0.7f); // Orange
                    break;
                case Block.BlockType.O:
                    renderer.color = new Color(1f, 1f, 0f, 0.7f); // Yellow
                    break;
                case Block.BlockType.S:
                    renderer.color = new Color(0f, 1f, 0f, 0.7f); // Green
                    break;
                case Block.BlockType.T:
                    renderer.color = new Color(0.5f, 0f, 0.5f, 0.7f); // Purple
                    break;
                case Block.BlockType.Z:
                    renderer.color = new Color(1f, 0f, 0f, 0.7f); // Red
                    break;
            }

            // Set high sorting order to ensure it's visible in UI
            renderer.sortingOrder = 100;
        }

        return blockParent;
    }
}