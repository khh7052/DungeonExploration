using UnityEngine;

public class PlayerInteractController : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f; // 상호작용 거리
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
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayerMask))
        {
            float distance = Vector3.Distance(transform.position, hit.point);
            if (distance > interactRange)
            {
                currentInteractable = null; // 범위를 벗어나면 초기화
                return;
            }

            currentInteractable = hit.collider.GetComponent<IInteractable>();
        }
        else
            currentInteractable = null;
    }

    private void UpdatePromptText()
    {
        HUD hud = uiManager.HUD;
        if (currentInteractable != null)
            hud.UpdatePromptText(currentInteractable.GetPrompt());
        else
            hud.UpdatePromptText("");
    }
    private void Interact()
    {
        if (currentInteractable == null) return;
        currentInteractable.Interact(playerController);
        currentInteractable = null; // 상호작용 후 초기화
    }
}
