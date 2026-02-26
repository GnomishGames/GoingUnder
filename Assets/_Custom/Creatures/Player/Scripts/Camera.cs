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

    Vector3 targetTranslation;
    Quaternion targetRotation;
    Vector3 targetPosition;
    Vector3 originalPosition;
    Quaternion originalRotation;

    void Start()
    {
        cam = GetComponent<Camera>();
        originalPosition = cam.transform.localPosition;
        originalRotation = cam.transform.localRotation;
    }

    void Update()
    {
        GetMousePosition(); //constantly update mouse position variable for use in other methods
        HandleTranslation();
        HandleRotation();
    }

    Vector2 GetMousePosition()
    {
        mousePos = new Vector2(
            Input.mousePosition.x / Screen.width - 0.5f,
            Input.mousePosition.y / Screen.height - 0.5f
        );
        return mousePos;
    }

    void HandleTranslation()
    {
        if (mouseTranslation > 0)
        {
            targetTranslation = originalPosition + new Vector3(
                mousePos.x * mouseTranslation,
                mousePos.y * mouseTranslation,
                0f
            );

            cam.transform.localPosition = Vector3.Lerp(
                cam.transform.localPosition,
                targetTranslation,
                smoothSpeed * Time.deltaTime
            );
        }
    }

    void HandleRotation()
    {
        if (mouseRotation > 0)
        {
            targetRotation = originalRotation * Quaternion.Euler(
                -mousePos.y * mouseRotation,
                mousePos.x * mouseRotation,
                0f
            );

            cam.transform.localRotation = Quaternion.Slerp(
                cam.transform.localRotation,
                targetRotation,
                smoothSpeed * Time.deltaTime
            );
        }
    }
}
