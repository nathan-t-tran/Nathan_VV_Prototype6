using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingPlatform : MonoBehaviour
{
    public float moveSpeed = 5f;  // Speed of movement
    public float timeLimit = 10f; // Timer countdown

    private float timer;
    private bool gameOver = false;
    private float minX, maxX; // Camera bounds

    void Start()
    {
        timer = timeLimit; // Initialize timer
        SetCameraBounds(); // Get screen limits
    }

    void Update()
    {
        if (!gameOver && !GameManager.Instance.IsGameActive())
        {
            gameOver = true;
        }

        if (!gameOver)
        {
            MovePlatform();
            UpdateTimer();
        }
    }

    void MovePlatform()
    {
        float moveInput = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
        float newX = transform.position.x + (moveInput * moveSpeed * Time.deltaTime);

        // Clamp position so platform stays within camera view
        newX = Mathf.Clamp(newX, minX, maxX);

        transform.position = new Vector2(newX, transform.position.y);

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void UpdateTimer()
    {
        timer -= Time.deltaTime; // Decrease time

        if (timer <= 0)
        {
            gameOver = true;
            Debug.Log("Time's Up! Game Over!");
        }
    }

    void SetCameraBounds()
    {
        Camera cam = Camera.main;
        float halfWidth = cam.orthographicSize * cam.aspect; // Get half width of the camera

        minX = cam.transform.position.x - halfWidth + 1f; // Offset to avoid edge clipping
        maxX = cam.transform.position.x + halfWidth - 1f;
    }

    public float GetRemainingTime()
    {
        return timer;
    }

    public void ResetTimer()
    {
        timer = timeLimit;
        gameOver = false;
    }

    public bool IsTimeUp()
    {
        return timer <= 0;
    }
}
