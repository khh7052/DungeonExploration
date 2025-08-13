using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SoundData bgm;
    [SerializeField] private Vector3 respawnPoint;
    [SerializeField] private float respawnHeight = -10.0f;

    [SerializeField] private GameObject saveEffect;
    [SerializeField] private SoundData saveSFX;

    private void Start()
    {
        AudioManager.Instance.PlayBGM(bgm);
        InputManager.Instance.RestartAction += Respawn;
    }


    void Update()
    {
        if (HasFallenBelowRespawnHeight())
            Respawn();
    }

    bool HasFallenBelowRespawnHeight() => playerController.transform.position.y <= respawnHeight;

    public void SetRespawnPoint(Vector3 newRespawn)
    {
        respawnPoint = newRespawn;
        if (saveEffect != null)
            ObjectPoolingManager.Instance.Get(saveEffect, respawnPoint, Quaternion.identity);
        if (saveSFX != null)
            AudioManager.Instance.PlaySFX(saveSFX);
    }

    public void Respawn()
    {
        if (playerController == null || respawnPoint == null) return;
        playerController.RigidBody.MovePosition(respawnPoint);
        playerController.RigidBody.velocity = Vector3.zero; // 클라이밍 후 속도를 0으로 설정
        // playerController.transform.position = respawnPoint;
    }
}
