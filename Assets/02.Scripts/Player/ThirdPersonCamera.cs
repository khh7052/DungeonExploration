using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform target;                  // ���� ĳ���� Ʈ������
    [SerializeField] private float xSpeed = 120f;                // �¿� ȸ�� �ӵ�
    [SerializeField] private float ySpeed = 120f;                // ���� ȸ�� �ӵ�
    [SerializeField] private float yMinLimit = -20f;             // ���� ȸ�� �ּ� ����
    [SerializeField] private float yMaxLimit = 80f;              // ���� ȸ�� �ִ� ����
    [SerializeField] private Vector3 offset = new(0, 1.5f, 0); // ĳ���� ���� ī�޶� ���� ����

    [Header("Zoom")]
    [SerializeField] private float defaultDistance = 4f;         // �ʱ� ī�޶� �Ÿ�
    [SerializeField] private float zoomMin = 2f;                 // �ּ� �� �Ÿ�
    [SerializeField] private float zoomMax = 10f;                 // �ִ� �� �Ÿ�
    [SerializeField] private float zoomSpeed = 2f;               // �� �ӵ�
    [SerializeField] private float zoomLerpSpeed = 8f;           // �� ���� �ӵ�
    [SerializeField] private LayerMask obstacleMask;             // ��ֹ� ���̾� ����ũ

    private float currentDistance;                               // ���� ī�޶� �Ÿ�
    private float targetDistance;                                // ��ǥ ī�޶� �Ÿ�

    private float rotationX = 0f;                                // �¿� ȸ�� ���� (Yaw)
    private float rotationY = 0f;                                // ���� ȸ�� ���� (Pitch)

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
            // ī�޶� ��ֹ��� �ʹ� ���� �ʵ��� ��¦ ����߸�
            return hit.point - direction * 0.1f;
        }
        return toPos;
    }
}
