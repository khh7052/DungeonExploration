using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SoundData bgm;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnHeight = -10.0f;

    private void Start()
    {
        AudioManager.Instance.PlayBGM(bgm);
    }


    void Update()
    {
        if (HasFallenBelowRespawnHeight())
            Respawn();
    }

    bool HasFallenBelowRespawnHeight() => playerController.transform.position.y <= respawnHeight;

    public void SetRespawnPoint(Transform newRespawn) => respawnPoint = newRespawn;

    void Respawn()
    {
        if (playerController == null || respawnPoint == null) return;

        playerController.transform.position = respawnPoint.position;
        playerController.transform.rotation = respawnPoint.rotation;
    }
}
