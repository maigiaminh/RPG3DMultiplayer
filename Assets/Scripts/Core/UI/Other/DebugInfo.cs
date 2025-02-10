using TMPro;
using UnityEngine;

public class DebugInfo : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    private float deltaTime;
    private float timer;

    void Update()
    {
        timer = 0;
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        int fps = (int)(1.0f / deltaTime);
        fpsText.text = $"FPS: {fps}";
    }
}
