using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform target;                  // 따라갈 캐릭터 트랜스폼
    [SerializeField] private float xSpeed = 120f;                // 좌우 회전 속도
    [SerializeField] private float ySpeed = 120f;                // 상하 회전 속도
    [SerializeField] private float yMinLimit = -20f;             // 상하 회전 최소 각도
    [SerializeField] private float yMaxLimit = 80f;              // 상하 회전 최대 각도
    [SerializeField] private Vector3 offset = new(0, 1.5f, 0); // 캐릭터 기준 카메라 높이 보정

    [Header("Zoom")]
    [SerializeField] private float defaultDistance = 4f;         // 초기 카메라 거리
    [SerializeField] private float zoomMin = 2f;                 // 최소 줌 거리
    [SerializeField] private float zoomMax = 10f;                 // 최대 줌 거리
    [SerializeField] private float zoomSpeed = 2f;               // 줌 속도
    [SerializeField] private float zoomLerpSpeed = 8f;           // 줌 보간 속도
    [SerializeField] private LayerMask obstacleMask;             // 장애물 레이어 마스크

    private float currentDistance;                               // 현재 카메라 거리
    private float targetDistance;                                // 목표 카메라 거리

    private float rotationX = 0f;                                // 좌우 회전 각도 (Yaw)
    private float rotationY = 0f;                                // 상하 회전 각도 (Pitch)

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;

        currentDistance = defaultDistance;
        targetDistance = defaultDistance;
    }

    private void LateUpdate()
    {
        SmoothZoom();
        UpdateCameraPosition();
    }

    private void SmoothZoom()
    {
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, zoomLerpSpeed * Time.deltaTime);
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0f);
        Vector3 desiredPos = target.position + offset - rotation * Vector3.forward * currentDistance;
        Vector3 adjustedPos = AdjustForObstacles(target.position + offset, desiredPos);

        transform.SetPositionAndRotation(adjustedPos, rotation);
    }

    public void Rotate(float mouseX, float mouseY)
    {
        rotationX += mouseX * xSpeed * Time.deltaTime;
        rotationY -= mouseY * ySpeed * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, yMinLimit, yMaxLimit);
    }

    public void Zoom(float zoomInput)
    {
        if (Mathf.Approximately(zoomInput, 0f))
            return;

        targetDistance -= zoomInput * zoomSpeed * Time.deltaTime;
        targetDistance = Mathf.Clamp(targetDistance, zoomMin, zoomMax);
    }

    private Vector3 AdjustForObstacles(Vector3 fromPos, Vector3 toPos)
    {
        Vector3 direction = toPos - fromPos;
        float distance = direction.magnitude;
        direction.Normalize();

        if (Physics.Raycast(fromPos, direction, out RaycastHit hit, distance, obstacleMask))
        {
            // 카메라가 장애물에 너무 붙지 않도록 살짝 떨어뜨림
            return hit.point - direction * 0.1f;
        }
        return toPos;
    }
}
