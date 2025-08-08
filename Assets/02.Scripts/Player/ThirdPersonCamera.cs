using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;  // ���� ĳ����
    [SerializeField] private LayerMask obstacleMask; // ��ֹ� ���̾� ����ũ
    [SerializeField] private float distance = 5.0f;
    [SerializeField] private float xSpeed = 120.0f;
    [SerializeField] private float ySpeed = 120.0f;

    [SerializeField] private float yMinLimit = -20f;
    [SerializeField] private float yMaxLimit = 80f;

    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0); // ī�޶��� ���� ����

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
        // ȸ�� ���� ����
        x += mouseX * xSpeed * Time.deltaTime;
        y = Mathf.Clamp(y - mouseY * ySpeed * Time.deltaTime, yMinLimit, yMaxLimit);

        // ȸ�� �� ��ġ ���
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 desiredPos = target.position + offset - rotation * Vector3.forward * distance;

        // ��ġ ���� �� ����
        transform.SetPositionAndRotation(AdjustForObstacles(desiredPos), rotation);
    }

    private Vector3 AdjustForObstacles(Vector3 desiredPos)
    {
        Vector3 startPos = target.position + offset;
        Vector3 camDir = desiredPos - startPos;

        if (Physics.Raycast(startPos, camDir.normalized, out RaycastHit hit, distance, obstacleMask))
        {
            // ǥ�鿡 �ʹ� ���� �ʵ��� ��¦ ���
            return hit.point - camDir.normalized * 0.1f;
        }

        return desiredPos;
    }
}
