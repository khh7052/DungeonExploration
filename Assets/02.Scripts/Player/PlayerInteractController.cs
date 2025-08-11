using UnityEngine;

public class PlayerInteractController : MonoBehaviour
{
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayerMask;

    private InputManager input;
    private UIManager uiManager;
    private PlayerController playerController;
    private IInteractable currentInteractable;

    private void Awake()
    {
        input = InputManager.Instance;
        uiManager = UIManager.Instance;
        playerController = GetComponent<PlayerController>();
        input.InteractAction += Interact;
    }

    private void Update()
    {
        CheckInteractableObject();
        UpdatePromptText();
    }

    private void CheckInteractableObject()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayerMask))
        {
            currentInteractable = hit.collider.GetComponent<IInteractable>();
        }
        else
        {
            currentInteractable = null;
        }
    }

    private void Interact()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact(playerController);
            currentInteractable = null;
        }
    }

    private void UpdatePromptText()
    {
        HUD hud = uiManager.HUD;
        if (currentInteractable != null)
            hud.UpdatePromptText(currentInteractable.GetPrompt());
        else
            hud.UpdatePromptText("");
    }
}
