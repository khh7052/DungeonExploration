using UnityEngine;

public class PlayerLookController : MonoBehaviour
{
    private InputManager input;
    [SerializeField] private ThirdPersonCamera thirdPersonCamera;

    public Transform CameraTransform { get => thirdPersonCamera.transform; }

    private void Awake()
    {
        input = InputManager.Instance;
    }

    private void LateUpdate()
    {
        float mouseX = input.LookInput.x;
        float mouseY = input.LookInput.y;

        thirdPersonCamera.Rotate(mouseX, mouseY);
        thirdPersonCamera.Zoom(input.ZoomInput); // 줌 입력 처리
    }
}
