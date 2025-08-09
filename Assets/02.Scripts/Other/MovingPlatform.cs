using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform[] movingPoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private bool loop = true;
    [SerializeField] private int startPointIndex = 0;
    private int currentPointIndex;

    private void Start()
    {
        if (movingPoints == null || movingPoints.Length == 0) return;
        currentPointIndex = Mathf.Clamp(startPointIndex, 0, movingPoints.Length - 1);
        transform.position = movingPoints[currentPointIndex].position;
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (true)
        {
            int nextPointIndex = (currentPointIndex + 1) % movingPoints.Length;
            Vector3 targetPos = movingPoints[nextPointIndex].position;

            while ((transform.position - targetPos).sqrMagnitude > 0.0001f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;
            currentPointIndex = nextPointIndex;

            yield return new WaitForSeconds(waitTime);

            if (!loop && currentPointIndex == movingPoints.Length - 1)
                yield break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        collision.transform.SetParent(transform);
    }

    private void OnCollisionExit(Collision collision)
    {
        collision.transform.SetParent(null);
    }

}
