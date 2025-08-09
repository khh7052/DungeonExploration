using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LazerTrap : MonoBehaviour
{
    public Action OnTrapped;
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
                OnTrapped?.Invoke();
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
