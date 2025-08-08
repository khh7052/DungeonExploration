using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;  // 따라갈 캐릭터
    [SerializeField] private LayerMask obstacleMask; // 장애물 레이어 마스크
    [SerializeField] private float distance = 5.0f;
    [SerializeField] private float xSpeed = 120.0f;
    [SerializeField] private float ySpeed = 120.0f;

    [SerializeField] private float yMinLimit = -20f;
    [SerializeField] private float yMaxLimit = 80f;

    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0); // 카메라의 높이 조정

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    public void Rotate(float mouseX, float mouseY)
    {
        // 회전 각도 갱신
        x += mouseX * xSpeed * Time.deltaTime;
        y = Mathf.Clamp(y - mouseY * ySpeed * Time.deltaTime, yMinLimit, yMaxLimit);

        // 회전 → 위치 계산
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 desiredPos = target.position + offset - rotation * Vector3.forward * distance;

        // 위치 보정 후 적용
        transform.SetPositionAndRotation(AdjustForObstacles(desiredPos), rotation);
    }

    private Vector3 AdjustForObstacles(Vector3 desiredPos)
    {
        Vector3 startPos = target.position + offset;
        Vector3 camDir = desiredPos - startPos;

        if (Physics.Raycast(startPos, camDir.normalized, out RaycastHit hit, distance, obstacleMask))
        {
            // 표면에 너무 붙지 않도록 살짝 띄움
            return hit.point - camDir.normalized * 0.1f;
        }

        return desiredPos;
    }
}
