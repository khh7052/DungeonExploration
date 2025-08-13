using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Constants;
using UnityEngine.UI;

public class HUD : BaseUI
{
    private PlayerController playerController;
    [SerializeField] private Image hpFillImage;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private TMP_Text descriptionText;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerController.characterStats.GetStat(StatType.CurrentHP).FinalValueChanged += UpdateHP;

        UpdateHP(playerController.Health);
    }

    public void UpdateHP(float finalValue)
    {
        if (hpFillImage == null) return;

        float maxHealth = playerController.MaxHealth;
        float fillAmount = finalValue / maxHealth;
        hpFillImage.fillAmount = fillAmount;
    }

    public void UpdatePromptText(string text)
    {
        if (promptText == null) return;
        promptText.text = text;
    }

    public void UpdateDescriptionText(string text)
    {
        if (descriptionText == null) return;
        descriptionText.text = text;
    }

}
