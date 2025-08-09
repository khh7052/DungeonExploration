using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;  // ���� ĳ����
    [SerializeField] private LayerMask obstacleMask; // ��ֹ� ���̾� ����ũ
    [SerializeField] private float initDistance = 4f; // �ʱ� �Ÿ�
    private float currentDistance = 5f;
    [SerializeField] private float xSpeed = 120f;
    [SerializeField] private float ySpeed = 120f;

    [SerializeField] private float yMinLimit = -20f;
    [SerializeField] private float yMaxLimit = 80f;

    [SerializeField] private float zoomSpeed = 2f; // �� �ӵ�
    [SerializeField] private float zoomLerpSpeed = 2f; // �� �ӵ�
    [SerializeField] private float zoomMin = 2f; // �ּ� �� �Ÿ�
    [SerializeField] private float zoomMax = 10f; // �ִ� �� �Ÿ�

    [SerializeField] private Vector3 offset = new(0, 1.5f, 0); // ī�޶��� ���� ����
    private float targetDistance; // ���� ��ǥ �Ÿ�

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        currentDistance = initDistance;
        targetDistance = initDistance;
    }
    private void LateUpdate()
    {
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, zoomLerpSpeed * Time.deltaTime);

        // ��ġ �� ȸ�� ������Ʈ (Rotate �Լ� ���� �Ϻ� ���)
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 desiredPos = target.position + offset - rotation * Vector3.forward * currentDistance;
        transform.SetPositionAndRotation(AdjustForObstacles(desiredPos), rotation);
    }

    public void Rotate(float mouseX, float mouseY)
    {
        x += mouseX * xSpeed * Time.deltaTime;
        y = Mathf.Clamp(y - mouseY * ySpeed * Time.deltaTime, yMinLimit, yMaxLimit);
    }

    public void Zoom(float zoomAmount)
    {
        if (zoomAmount == 0) return;

        zoomAmount = Mathf.Clamp(zoomAmount, -1f, 1f);

        targetDistance -= zoomAmount * zoomSpeed * Time.deltaTime;
        targetDistance = Mathf.Clamp(targetDistance, zoomMin, zoomMax);
    }


    private Vector3 AdjustForObstacles(Vector3 desiredPos)
    {
        Vector3 startPos = target.position + offset;
        Vector3 camDir = desiredPos - startPos;

        if (Physics.Raycast(startPos, camDir.normalized, out RaycastHit hit, currentDistance, obstacleMask))
        {
            // ǥ�鿡 �ʹ� ���� �ʵ��� ��¦ ���
            return hit.point - camDir.normalized * 0.1f;
        }

        return desiredPos;
    }
}
