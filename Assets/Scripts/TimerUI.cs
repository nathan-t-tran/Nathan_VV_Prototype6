using UnityEngine;
using TMPro; // Use TextMeshPro for better UI

public class TimerUI : MonoBehaviour
{
    public MovingPlatform platformScript; // Reference to MovingPlatform script
    public TextMeshProUGUI timerText; // UI text element

    void Update()
    {
        if (platformScript != null && timerText != null)
        {
            timerText.text = "Time: " + Mathf.Ceil(platformScript.GetRemainingTime()).ToString();
        }
    }
}
