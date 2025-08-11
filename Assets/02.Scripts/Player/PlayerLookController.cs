using UnityEngine;

public class PlayerLookController : MonoBehaviour
{
    private InputManager input;
    [SerializeField] private ThirdPersonCamera thirdPersonCamera;

    private void Awake()
    {
        input = InputManager.Instance;
    }

    private void Update()
    {
        float mouseX = input.LookInput.x;
        float mouseY = input.LookInput.y;

        thirdPersonCamera.Rotate(mouseX, mouseY);
    }
}
