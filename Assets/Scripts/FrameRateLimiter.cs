using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    // Desired frame rate
    public int targetFPS = 60;
    private float deltaTime = 0f;
    public float currentFPS = 0f;

    void Start()
    {
        // Set the target frame rate
        Application.targetFrameRate = targetFPS;

        // Optional: Log the set frame rate
        Debug.Log("Target Frame Rate set to: " + targetFPS + " FPS");
    }

    void OnDisable()
    {
        // Reset to default frame rate when the script is disabled or the GameObject is destroyed
        Application.targetFrameRate = -1;
        Debug.Log("Target Frame Rate reset to default");
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        currentFPS = Mathf.Ceil(1.0f / deltaTime);
    }
}
