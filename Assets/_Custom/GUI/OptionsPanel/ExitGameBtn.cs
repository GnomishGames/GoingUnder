using UnityEngine;

public class ExitGameBtn : MonoBehaviour
{
    public void ExitGame()
    {
        // Exit the application
        Application.Quit();

        // If running in the Unity editor, stop playing
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}