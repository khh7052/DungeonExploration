using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class PlatformLauncher : MonoBehaviour
{
    [SerializeField] private Vector3 launchDirection;
    [SerializeField] private float launchForce = 20f;
    [SerializeField] private float launchDelay = 0.5f;
    [SerializeField] private SoundData bounceSFX;
    private Animator anim;
    private List<Rigidbody> targetRigidbodies;
    private bool isLaunching = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        targetRigidbodies = new();
    }

    private IEnumerator Launch()
    {
        if (isLaunching) yield break; // �̹� �߻� ���̰ų� ����� ������ ����
        isLaunching = true; // �߻� ���� ���� ����
        anim.SetTrigger(AnimatorHash.LaunchReadyHash); // �ִϸ��̼� Ʈ���� ����

        yield return new WaitForSeconds(launchDelay);

        anim.SetTrigger(AnimatorHash.LaunchHash); // �ִϸ��̼� Ʈ���� ����
        AudioManager.Instance.PlaySFX(bounceSFX);

        Vector3 dir = transform.TransformDirection(launchDirection.normalized);
        Vector3 force = dir * launchForce;

        foreach (var rigidbody in targetRigidbodies)
        {
            if (rigidbody.CompareTag("Player"))
            {
                PlayerMovementController playerController = rigidbody.GetComponent<PlayerMovementController>();
                playerController.AddExternalVelocity(force, ForceMode.VelocityChange);
            }
            else
            {
                rigidbody.AddForce(force, ForceMode.VelocityChange);
            }
        }

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
