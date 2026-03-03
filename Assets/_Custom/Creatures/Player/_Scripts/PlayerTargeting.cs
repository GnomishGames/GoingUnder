using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTargeting : MonoBehaviour
{
    Camera cam;
    [SerializeField] LayerMask interactableLayerMask;
    public Interactable currentTarget;
    TargetPanel targetPanel;

    //dice
    AttackDie attackDie;
    DamageDie damageDie;
    
    void Start()
    {
        cam = GetComponentInChildren<Camera>(); //camera component is on a child object of the player
        targetPanel = GetComponentInChildren<TargetPanel>(true); //target panel is on a child object of the player, but is inactive at the start of the game
        targetPanel.gameObject.SetActive(false); //make sure the target panel is inactive at the start of the game

        // Find AttackDie and DamageDie in the scene
        attackDie = FindFirstObjectByType<AttackDie>();
        damageDie = FindFirstObjectByType<DamageDie>();
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
                    targetPanel.SetNewTarget(creature);
                    targetPanel.gameObject.SetActive(true);
                }
            }
            else
            {
                // Clicked on empty space, clear target
                currentTarget = null;
                targetPanel.SetNewTarget(null);
                targetPanel.gameObject.SetActive(false);
                damageDie.SetDieValue(0); // Clear damage die display when no target is selected
                attackDie.SetDieValue(0); // Clear attack die display when no target is selected
            }
        }
    }
}
