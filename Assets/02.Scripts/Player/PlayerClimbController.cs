using UnityEngine;

public class PlayerClimbController : MonoBehaviour
{
    [SerializeField] private float climbForwardCheckDistance = 0.5f;
    [SerializeField] private float climbDownCheckDistance = 1f;
    [SerializeField] private Vector3 climbDownCheckOffset = new(0, 1f, 0);
    [SerializeField] private Transform climbCheckPoint;
    [SerializeField] private LayerMask climbableLayerMask;

    private AnimationHandler animHandler;
    private Rigidbody rigd;

    private bool isClimbing = false;
    private Vector3 climbPoint;

    private InputManager input;

    private void Awake()
    {
        rigd = GetComponent<Rigidbody>();
        animHandler = GetComponentInChildren<AnimationHandler>();
        input = InputManager.Instance;
    }

    private void Update()
    {
        if (!isClimbing)
        {
            if (TryStartClimbing())
            {
                // 클라이밍 시작
            }
        }
        else
        {
            if (input.JumpInput)
            {
                ClimbUp();
            }
        }
    }

    private bool TryStartClimbing()
    {
        Ray forwardRay = new(climbCheckPoint.position, climbCheckPoint.forward);
        Ray downRay = new(climbCheckPoint.position + transform.TransformDirection(climbDownCheckOffset), Vector3.down);

        bool isForwardHit = Physics.Raycast(forwardRay, out RaycastHit forwardHit, climbForwardCheckDistance, climbableLayerMask);
        bool isDownHit = Physics.Raycast(downRay, out RaycastHit downHit, climbDownCheckDistance, climbableLayerMask);

        if (isForwardHit && isDownHit)
        {
            isClimbing = true;
            animHandler.Climb(true);
            rigd.isKinematic = true;

            climbPoint = new Vector3(forwardHit.point.x, downHit.point.y + 1f, downHit.point.z);
            return true;
        }
        return false;
    }

    private void ClimbUp()
    {
        if (!isClimbing) return;

        animHandler.Climb(false);
        ClimbUpEnd();
    }

    public void ClimbUpEnd()
    {
        isClimbing = false;
        rigd.isKinematic = false;
        transform.position = climbPoint;
    }
}
