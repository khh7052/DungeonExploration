using UnityEngine;

public class PlayerClimbController : MonoBehaviour
{
    [SerializeField] private float climbForwardCheckDistance = 0.5f;
    [SerializeField] private float climbDownCheckDistance = 1f;
    [SerializeField] private Vector3 climbDownCheckOffset = new(0, 1f, 0);
    [SerializeField] private Transform climbCheckPoint;
    [SerializeField] private LayerMask climbableLayerMask;
    [SerializeField] private Vector3 climbPointOffset;

    private AnimationHandler animHandler;
    private Rigidbody rigd;

    private bool isClimbing = false;
    private Vector3 climbPoint;

    private InputManager input;

    public bool IsClimbing => isClimbing;

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
            if (input.IsMoving)
            {
                TryStartClimbing();
            }
        }
        else
        {
            if (input.IsMoving)
            {
                ClimbUp();
            }
        }
    }

    private bool TryStartClimbing()
    {
        Ray forwardRay = new(climbCheckPoint.position, climbCheckPoint.forward);
        Ray downRay = new(climbCheckPoint.position + transform.TransformDirection(climbDownCheckOffset), Vector3.down);

        bool isForwardHit = Physics.Raycast(forwardRay, out RaycastHit forwardHit, climbForwardCheckDistance, climbableLayerMask, QueryTriggerInteraction.Ignore);
        bool isDownHit = Physics.Raycast(downRay, out RaycastHit downHit, climbDownCheckDistance, climbableLayerMask, QueryTriggerInteraction.Ignore);

        if (isForwardHit && isDownHit)
        {
            isClimbing = true;
            animHandler.Climb(true);
            rigd.isKinematic = true;

            climbPoint = new Vector3(downHit.point.x, downHit.point.y, downHit.point.z) + transform.TransformDirection(climbPointOffset);
            return true;
        }
        return false;
    }

    private void ClimbUp()
    {
        if (!isClimbing) return;

        animHandler.Climb(false);
    }

    public void ClimbUpEnd()
    {
        isClimbing = false;
        rigd.isKinematic = false;
        transform.position = climbPoint;
    }

    private void OnDrawGizmosSelected()
    {

        if (climbCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(climbCheckPoint.position, climbCheckPoint.forward * climbForwardCheckDistance); // 클라이밍 체크 전방 방향 그리기
            Gizmos.DrawRay(climbCheckPoint.position + transform.TransformDirection(climbDownCheckOffset), Vector3.down * climbDownCheckDistance); // 클라이밍 체크 아래 방향 그리기

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(climbPoint, 0.5f); // 클라이밍 체크 구체 그리기
        }

    }
}
