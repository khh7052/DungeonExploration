using UnityEngine;

public class ArrangeChildrenGrid : MonoBehaviour
{
    [SerializeField] private float spacingX = 1f;  // X축 간격
    [SerializeField] private float spacingZ = 1f;  // Z축 간격
    [SerializeField] private int rowCount = 3;     // 한 줄에 배치할 자식 수 (행 개수)

    [ContextMenu("Arrange Grid")]
    public void ArrangeGrid()
    {
        int count = transform.childCount;

        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);

            int row = i / rowCount;     // 몇 번째 행인지
            int column = i % rowCount;  // 몇 번째 열인지

            float posX = column * spacingX;
            float posZ = row * spacingZ;

            child.localPosition = new Vector3(posX, child.localPosition.y, posZ);
        }
    }
}
