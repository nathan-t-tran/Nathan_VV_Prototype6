using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITargetDisplay : MonoBehaviour
{
    [Header("Block Colors")]
    public Color colorI = Color.cyan;
    public Color colorJ = Color.blue;
    public Color colorL = new Color(1f, 0.5f, 0f); // Orange
    public Color colorO = Color.yellow;
    public Color colorS = Color.green;
    public Color colorT = new Color(0.5f, 0f, 0.5f); // Purple
    public Color colorZ = Color.red;

    [Header("Display Settings")]
    public float blockSize = 40f;
    public float blockSpacing = 10f; // Space between individual blocks
    public float shapeSpacing = 150f; // Space between different tetromino shapes

    // Template for creating blocks
    private GameObject blockTemplate;
    private List<GameObject> currentBlocks = new List<GameObject>();

    void Awake()
    {
        // Create a template block (a simple UI Image)
        blockTemplate = new GameObject("BlockTemplate");
        blockTemplate.SetActive(false);

        // Add a UI Image component
        Image image = blockTemplate.AddComponent<Image>();
        image.color = Color.white;

        // Set as child of this object but keep it hidden
        blockTemplate.transform.SetParent(transform, false);

        // Make sure this game object has a RectTransform
        if (GetComponent<RectTransform>() == null)
        {
            gameObject.AddComponent<RectTransform>();
        }
    }

    // Clear all displayed blocks
    public void ClearDisplay()
    {
        foreach (GameObject block in currentBlocks)
        {
            Destroy(block);
        }
        currentBlocks.Clear();
    }

    // Display multiple block types with proper spacing
    public void DisplayBlockTypes(List<Block.BlockType> blockTypes)
    {
        ClearDisplay();

        if (blockTypes == null || blockTypes.Count == 0)
            return;

        // Calculate total width to center all shapes
        float totalWidth = (blockTypes.Count - 1) * shapeSpacing;
        float startX = -totalWidth / 2;

        // Display each block type
        for (int i = 0; i < blockTypes.Count; i++)
        {
            float xPosition = startX + (i * shapeSpacing);
            DisplaySingleBlockType(blockTypes[i], new Vector2(xPosition, 0));
        }
    }

    // Display a single tetromino shape
    private void DisplaySingleBlockType(Block.BlockType blockType, Vector2 centerPosition)
    {
        // Get the appropriate data for this block type
        Color color = GetColorForBlockType(blockType);
        Vector2Int[] blockPositions = GetNormalizedPositionsForBlockType(blockType);

        // Find the bounds to center the shape
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        foreach (Vector2Int pos in blockPositions)
        {
            minX = Mathf.Min(minX, pos.x);
            maxX = Mathf.Max(maxX, pos.x);
            minY = Mathf.Min(minY, pos.y);
            maxY = Mathf.Max(maxY, pos.y);
        }

        // Calculate the center offset to position the shape
        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;

        // Calculate the full width and height (including spacing)
        float totalWidth = (maxX - minX + 1) * (blockSize + blockSpacing) - blockSpacing;
        float totalHeight = (maxY - minY + 1) * (blockSize + blockSpacing) - blockSpacing;

        // Debug information
        Debug.Log($"Shape {blockType}: Bounds ({minX},{minY}) to ({maxX},{maxY}), Center: ({centerX},{centerY})");

        // Create each block
        foreach (Vector2Int pos in blockPositions)
        {
            // Calculate the actual position relative to center
            float xPos = centerPosition.x + (pos.x - centerX) * (blockSize + blockSpacing);
            float yPos = centerPosition.y - (pos.y - centerY) * (blockSize + blockSpacing); // Y is inverted in UI

            // Create the block
            GameObject blockObj = Instantiate(blockTemplate);
            blockObj.SetActive(true);
            blockObj.transform.SetParent(transform, false);
            blockObj.name = $"Block_{blockType}_{pos.x}_{pos.y}";

            // Position and size
            RectTransform rectTransform = blockObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(xPos, yPos);
            rectTransform.sizeDelta = new Vector2(blockSize, blockSize);

            // Set color
            Image image = blockObj.GetComponent<Image>();
            image.color = color;

            // Add to managed list
            currentBlocks.Add(blockObj);

            // Debug
            Debug.Log($"Created block at grid ({pos.x},{pos.y}), screen pos: ({xPos},{yPos})");
        }
    }

    // Get color for block type
    private Color GetColorForBlockType(Block.BlockType blockType)
    {
        switch (blockType)
        {
            case Block.BlockType.I: return colorI;
            case Block.BlockType.J: return colorJ;
            case Block.BlockType.L: return colorL;
            case Block.BlockType.O: return colorO;
            case Block.BlockType.S: return colorS;
            case Block.BlockType.T: return colorT;
            case Block.BlockType.Z: return colorZ;
            default: return Color.white;
        }
    }

    // Get normalized grid positions for each block type
    private Vector2Int[] GetNormalizedPositionsForBlockType(Block.BlockType blockType)
    {
        switch (blockType)
        {
            case Block.BlockType.I:
                return new Vector2Int[] {
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                    new Vector2Int(2, 1),
                    new Vector2Int(3, 1)
                };

            case Block.BlockType.J:
                return new Vector2Int[] {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                    new Vector2Int(2, 1)
                };

            case Block.BlockType.L:
                return new Vector2Int[] {
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                    new Vector2Int(2, 1),
                    new Vector2Int(2, 0)
                };

            case Block.BlockType.O:
                return new Vector2Int[] {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1)
                };

            case Block.BlockType.S:
                return new Vector2Int[] {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(1, 1),
                    new Vector2Int(2, 1)
                };

            case Block.BlockType.T:
                return new Vector2Int[] {
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                    new Vector2Int(2, 1),
                    new Vector2Int(1, 0)
                };

            case Block.BlockType.Z:
                return new Vector2Int[] {
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0)
                };

            default:
                return new Vector2Int[] { new Vector2Int(0, 0) };
        }
    }
}