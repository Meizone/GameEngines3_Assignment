using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityButton : MonoBehaviour
{
    [SerializeField]
    private Image cooldownImage;
    [SerializeField]
    private Button button;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private Color disabledColour;
    
    private AbilitySlot slot;
    public void Assign(AbilitySlot newSlot)
    {
        if (slot != null)
        {
            button.onClick.RemoveListener(slot.ChooseTarget);
            newSlot.OnAvailabilityChanged -= UpdateClickable;
        }

        slot = newSlot;

        if (slot != null && slot.ability.isTargetted)
        {
            gameObject.SetActive(true);
            text.text = slot.ability.abilityName;
            button.onClick.AddListener(slot.ChooseTarget);
            newSlot.OnAvailabilityChanged += UpdateClickable;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private bool UpdateClickable(float cooldown, bool payable)
    {
        if (payable && cooldown <= 0.0f)
        {
            button.enabled = true;
            text.color = Color.white;
        }
        else
        {
            button.enabled = false;
            text.color = disabledColour;
        }

        cooldownImage.fillAmount = cooldown;

        return (payable && cooldown <= 0.0f);
    }
}
