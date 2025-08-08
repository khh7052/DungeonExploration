using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Constants;

public class HUD : BaseUI
{
    private PlayerController playerController;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text promptText;

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

    public void UpdatePromptText(string text)
    {
        if (promptText != null)
        {
            promptText.text = text;
        }
    }

}
