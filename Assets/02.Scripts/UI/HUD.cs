using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Constants;

public class HUD : BaseUI
{
    private PlayerController playerController;
    public TMP_Text hpText;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerController.characterStats.GetStat(StatType.CurrentHP).RegisterBaseValueChanged(UpdateHP);

        UpdateHP(0, playerController.Health);
    }

    public void UpdateHP(float baseValue, float finalValue)
    {
        if (hpText != null)
        {
            hpText.text = "HP: " + finalValue.ToString();
        }
    }

}
