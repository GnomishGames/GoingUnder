using UnityEngine;

public class Camera : MonoBehaviour
{
    //camera
    Camera cam;

    //mouse
    Vector2 mousePos;
    float mouseTranslation = 0.1f;  //Translate camera with mouse
    float mouseRotation = 2f;       //Rotate camera with mouse
    float smoothSpeed = 5f;         //smoothen the camera movement
    float deadzoneRadius = 0.1f;    //radius of the deadzone for camera mouse follow
    float mouseSensitivity = 1f;    //sensitivity of the camera mouse follow

    private Vector3 targetTranslation;
    private Quaternion targetRotation;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        GetMousePosition(); //constantly update mouse position variable for use in other methods

    }

    Vector2 GetMousePosition()
    {
        mousePos = new Vector2(
            Input.mousePosition.x / Screen.width - 0.5f,
            Input.mousePosition.y / Screen.height - 0.5f);
        return mousePos;
    }

    void HandleTranslation()
    {
        if (mouseTranslation > 0)
        {
            targetTranslation = new Vector3(mousePos.x * mouseTranslation, mousePos.y * mouseTranslation, 0f);
        }
    }
}
