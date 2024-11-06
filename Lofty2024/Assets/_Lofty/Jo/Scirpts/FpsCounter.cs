using UnityEngine;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText; // Reference to the TextMeshProUGUI component
    private float deltaTime = 0.0f;

    private void Update()
    {
        // Calculate the time it takes to render a frame
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // Calculate FPS
        float fps = 1.0f / deltaTime;

        // Update the UI Text element
        fpsText.text = $"{fps:0.} FPS"; // Format the FPS to display without decimal places
    }

    // Method to update the FPS limit display (optional)
    public void UpdateFpsLimitDisplay(int fpsLimit)
    {
        // This method can be used to show FPS limit if desired
    }
}