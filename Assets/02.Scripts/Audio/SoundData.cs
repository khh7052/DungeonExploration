using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

[CreateAssetMenu(fileName = "SoundData", menuName = "Data/SoundData")]
public class SoundData : ScriptableObject
{
    public AudioClip[] clips;
    public float volume = 1f;
    public SoundType soundType = SoundType.SFX;
}
