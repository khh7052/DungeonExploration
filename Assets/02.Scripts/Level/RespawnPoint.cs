using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] private Vector3 spawnOffset;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 spawnPoint = transform.position + spawnOffset;
            GameManager.Instance.SetRespawnPoint(spawnPoint);
        }
    }
}
