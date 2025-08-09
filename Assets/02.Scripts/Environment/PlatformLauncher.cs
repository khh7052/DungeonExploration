using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLauncher : MonoBehaviour
{
    [SerializeField] private Vector3 launchDirection;
    [SerializeField] private float launchForce = 20f;
    [SerializeField] private float launchDelay = 0.5f;
    private List<Rigidbody> targetRigidbodies;
    private bool isLaunching = false;

    private void Start()
    {
        targetRigidbodies = new();
    }

    private IEnumerator Launch()
    {
        if (isLaunching) yield break; // �̹� �߻� ���̰ų� ����� ������ ����
        isLaunching = true; // �߻� ���� ���� ����

        yield return new WaitForSeconds(launchDelay);

        foreach (var rigidbody in targetRigidbodies)
            rigidbody.AddForce(launchDirection.normalized * launchForce, ForceMode.VelocityChange);

        targetRigidbodies.Clear(); // �߻� �� ��� ��� �ʱ�ȭ
        isLaunching = false; // �߻� �Ϸ� �� ���� �ʱ�ȭ
    }

    void AddTarget(Rigidbody rigidbody)
    {
        if (rigidbody == null || targetRigidbodies.Contains(rigidbody)) return;
        if (targetRigidbodies.Count == 0) StartLauncher();

        targetRigidbodies.Add(rigidbody);
    }

    void RemoveTarget(Rigidbody rigidbody)
    {
        targetRigidbodies.Remove(rigidbody);
    }

    void StartLauncher()
    {
        StartCoroutine(Launch());
    }

    private void OnCollisionEnter(Collision collision)
    {
        AddTarget(collision.rigidbody);
    }

    private void OnCollisionExit(Collision collision)
    {
        RemoveTarget(collision.rigidbody);
    }

}
