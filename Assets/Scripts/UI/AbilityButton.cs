using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AbilityButton : MonoBehaviour
{
    #region "Member variables and properties"
    [SerializeField] private Image cooldownImage;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color disabledColour;
    private AbilityData _abilityData;
    #endregion

    #region "Public functions"
    public void Assign(AbilityData abilityData)
    {
        if (_abilityData != null)
        {
            button.onClick.RemoveListener(ChooseTarget);
            abilityData.onAvailabilityChanged -= UpdateClickable;
        }

        _abilityData = abilityData;

        if (_abilityData != null && _abilityData.ability.isTargetted)
        {
            gameObject.SetActive(true);
            text.text = _abilityData.ability.displayName;
            button.onClick.AddListener(ChooseTarget);
            abilityData.onAvailabilityChanged += UpdateClickable;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region "Private functions"
    private void ChooseTarget()
    {

    }

    private void UpdateClickable(uint cooldownValue, float cooldownPercent, bool payable)
    {
        if (payable && cooldownPercent <= 0.0f)
        {
            button.enabled = true;
            text.color = Color.white;
        }
        else
        {
            button.enabled = false;
            text.color = disabledColour;
        }

        cooldownImage.fillAmount = cooldownPercent;
    }
    #endregion
}
