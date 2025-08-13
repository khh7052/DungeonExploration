using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

public class LazerTrap : MonoBehaviour
{
    public UnityEvent<IDamageable> OnTrapped;
    [SerializeField] private float rayDistance = 5f;
    private LineRenderer lineRenderer;
    private bool isTrapped = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + transform.forward * rayDistance);
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, rayDistance) && hit.rigidbody != null)
        {
            if (!isTrapped)
            {
                isTrapped = true;
                IDamageable damageable = hit.rigidbody.GetComponent<IDamageable>();
                OnTrapped?.Invoke(damageable);
            }
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            if (isTrapped)
            {
                isTrapped = false;
            }
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.forward * rayDistance);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * rayDistance);
    }
}
