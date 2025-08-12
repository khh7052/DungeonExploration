using UnityEngine;

public class ArrangeChildrenGrid : MonoBehaviour
{
    [SerializeField] private float spacingX = 1f;  // X�� ����
    [SerializeField] private float spacingZ = 1f;  // Z�� ����
    [SerializeField] private int rowCount = 3;     // �� �ٿ� ��ġ�� �ڽ� �� (�� ����)

    [ContextMenu("Arrange Grid")]
    public void ArrangeGrid()
    {
        int count = transform.childCount;

        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);

            int row = i / rowCount;     // �� ��° ������
            int column = i % rowCount;  // �� ��° ������

            float posX = column * spacingX;
            float posZ = row * spacingZ;

            child.localPosition = new Vector3(posX, child.localPosition.y, posZ);
        }
    }
}
