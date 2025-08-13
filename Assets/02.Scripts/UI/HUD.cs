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
    [SerializeField] private Image dashFillImage;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private TMP_Text descriptionText;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerController.characterStats.GetStat(StatType.CurrentHP).FinalValueChanged += UpdateHP;
        playerController.MovementController.DashAction += StartDashFill;

        UpdateHP(playerController.Health);
    }

    public void UpdateHP(float finalValue)
    {
        if (hpFillImage == null) return;

        float maxHealth = playerController.MaxHealth;
        float fillAmount = finalValue / maxHealth;
        hpFillImage.fillAmount = fillAmount;
    }

    void StartDashFill()
    {
        StartCoroutine(DashFill());
    }

    IEnumerator DashFill()
    {
        dashFillImage.fillAmount = 0f;
        float cooldown = playerController.DashCooldown;
        float elapsedTime = 0f;
        while (elapsedTime < cooldown)
        {
            elapsedTime += Time.deltaTime;
            dashFillImage.fillAmount = Mathf.Clamp01(elapsedTime / cooldown);
            yield return null;
        }

        dashFillImage.fillAmount = 1f;
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
