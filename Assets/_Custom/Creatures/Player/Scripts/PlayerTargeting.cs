using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTargeting : MonoBehaviour
{
    Camera cam;
    [SerializeField] LayerMask interactableLayerMask;
    [SerializeField] Interactable currentTarget;
    
    void Start()
    {
        cam = GetComponentInChildren<Camera>(); //camera component is on a child object of the player
    }

    void Update()
    {
        LeftClick();
    }

    void LeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Don't click through the UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayerMask))
            {
                // Hit a creature
                CreatureStats creature = hit.collider.GetComponent<CreatureStats>();

                if (creature != null)
                {
                    // If the hit object has a CreatureStats component, set it as the target
                    currentTarget = creature.GetComponent<Interactable>();
                }
            }
            else
            {
                // Clicked on empty space, clear target
                currentTarget = null;
            }
        }
    }
}
