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
        if (isLaunching) yield break; // 이미 발사 중이거나 대상이 없으면 종료
        isLaunching = true; // 발사 시작 상태 설정

        yield return new WaitForSeconds(launchDelay);

        foreach (var rigidbody in targetRigidbodies)
            rigidbody.AddForce(launchDirection.normalized * launchForce, ForceMode.VelocityChange);

        targetRigidbodies.Clear(); // 발사 후 대상 목록 초기화
        isLaunching = false; // 발사 완료 후 상태 초기화
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
