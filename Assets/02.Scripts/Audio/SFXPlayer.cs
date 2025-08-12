using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    [SerializeField] private SoundData soundData;
    private void OnEnable()
    {
        if (soundData == null) return;

        AudioManager.Instance.PlaySFX(soundData);
    }
}
