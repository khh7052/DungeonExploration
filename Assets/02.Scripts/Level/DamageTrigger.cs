using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private float damage = 10f;

    public void TakeDamage(IDamageable damageable)
    {
        if (damageable == null) return;
        damageable.TakeDamage(damage);
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.collider.GetComponent<IDamageable>();
        TakeDamage(damageable);
    }
}
