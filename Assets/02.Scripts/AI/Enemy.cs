using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float updateRate = 0.2f; // Ÿ�� ��ġ ������Ʈ �ֱ�
    private NavMeshAgent agent;
    private Transform target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameManager.Instance.PlayerController.transform;
    }

    private void Start()
    {
        StartCoroutine(UpdateDestination());
    }
    bool CanMove(Vector3 targetPos)
    {
        NavMeshPath path = new();
        return agent.CalculatePath(targetPos, path) && path.status == NavMeshPathStatus.PathComplete;
    }

    IEnumerator UpdateDestination()
    {
        while (target)
        {
            if(CanMove(target.position))
                agent.SetDestination(target.position);
            else
                agent.ResetPath();
            yield return new WaitForSeconds(updateRate);
        }
    }
}
