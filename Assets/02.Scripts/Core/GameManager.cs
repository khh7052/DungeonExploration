using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SoundData bgm;
    [SerializeField] private Vector3 respawnPoint;
    [SerializeField] private float respawnHeight = -10.0f;

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

    public void SetRespawnPoint(Vector3 newRespawn) => respawnPoint = newRespawn;

    void Respawn()
    {
        if (playerController == null || respawnPoint == null) return;
        playerController.RigidBody.MovePosition(respawnPoint);
        // playerController.transform.position = respawnPoint;
    }
}
