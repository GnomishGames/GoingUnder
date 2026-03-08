using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public float updateInterval = 0.5f;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval

    private float fps;
    private bool showFPS = true;

    public void SetFPSVisibility(bool visible)
    {
        showFPS = visible;
    }

    void Start()
    {
        timeleft = updateInterval;
    }

    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            fps = accum / frames;
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

    void OnGUI()
    {
        if (showFPS)
        {
            GUI.Label(new Rect(10, 10, 100, 25), "FPS: " + fps.ToString("F2"));
        }
    }
}