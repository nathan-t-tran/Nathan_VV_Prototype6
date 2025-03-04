using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum BlockType { I, J, L, O, S, T, Z }

    public BlockType type;
    public float fallSpeed = 3f;
    public bool isLocked = false;

    private Rigidbody2D rb;
    private bool isBeingDestroyed = false; // Prevent multiple destroy calls

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // We'll handle falling manually
    }

    void Update()
    {
        if (!isLocked)
        {
            // Debug.Log("Block position: " + transform.position); // Add debug to see positions

            // Make block fall
            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

            // Check if block is below the screen (missed)
            if (transform.position.y < -6f && !isBeingDestroyed)
            {
                isBeingDestroyed = true;
                GameManager.Instance.BlockMissed(this);
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if block collided with platform
        if (collision.gameObject.CompareTag("Platform"))
        {
            isLocked = true;
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;

            // Make block a child of the platform
            transform.SetParent(collision.transform);

            // Inform GameManager about the caught block
            GameManager.Instance.BlockCaught(this);
        }

        // If block collides with another locked block
        if (collision.gameObject.CompareTag("Block") && collision.gameObject.GetComponent<Block>().isLocked)
        {
            isLocked = true;
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;

            // Make block a child of the platform
            Transform platform = GameObject.FindGameObjectWithTag("Platform").transform;
            transform.SetParent(platform);

            // Inform GameManager about the caught block
            GameManager.Instance.BlockCaught(this);
        }
    }

    // Set the color based on block type
    public void SetBlockType(BlockType newType)
    {
        type = newType;

        // Apply color to all child block pieces
        foreach (Transform child in transform)
        {
            SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                switch (type)
                {
                    case BlockType.I:
                        renderer.color = new Color(0f, 1f, 1f); // Bright Cyan
                        break;
                    case BlockType.J:
                        renderer.color = new Color(0f, 0f, 1f); // Bright Blue
                        break;
                    case BlockType.L:
                        renderer.color = new Color(1f, 0.5f, 0f); // Bright Orange
                        break;
                    case BlockType.O:
                        renderer.color = new Color(1f, 1f, 0f); // Bright Yellow
                        break;
                    case BlockType.S:
                        renderer.color = new Color(0f, 1f, 0f); // Bright Green
                        break;
                    case BlockType.T:
                        renderer.color = new Color(0.5f, 0f, 0.5f); // Bright Purple
                        break;
                    case BlockType.Z:
                        renderer.color = new Color(1f, 0f, 0f); // Bright Red
                        break;
                }
            }
            else
            {
                Debug.LogError("SpriteRenderer missing on block piece!");
            }
        }

        Debug.Log("Set block type to " + newType + " with " + transform.childCount + " children");
    }
}