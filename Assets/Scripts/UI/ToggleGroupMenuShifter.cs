using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleGroupMenuShifter : MonoBehaviour
{
    [SerializeField]
    private bool allowOnlyOneOn;
    [SerializeField]
    private UnityEvent<bool> OnAnyTogglesOn;

    private bool isAnyOn = false;
    public bool IsAnyOn { get { return isAnyOn; } }
    private Toggle[] toggles = null;
    private int currentOnIndex = -1;

    private void GetToggles()
    {
        if (toggles == null)
        {
            toggles = GetComponentsInChildren<Toggle>();
            foreach (Toggle toggle in toggles)
            {
                if (toggle.isOn)
                    toggle.isOn = false;
            }
        }
    }

    private void OnEnable()
    {
        GetToggles();
        foreach (Toggle toggle in toggles)
            toggle.onValueChanged.AddListener(CheckAnyTogglesOn);
    }

    private void OnDisable()
    {
        GetToggles();
        foreach (Toggle toggle in toggles)
            toggle.onValueChanged.RemoveListener(CheckAnyTogglesOn);
    }

    private void CheckAnyTogglesOn(bool value)
    {
        if (value && allowOnlyOneOn && currentOnIndex > 0)
            toggles[currentOnIndex].isOn = false;

        currentOnIndex = -1;
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                currentOnIndex = i;
                break;
            }
        }

        if (isAnyOn != (currentOnIndex > 0))
        {
            isAnyOn = (currentOnIndex > 0);
            Debug.Log("calling OnAnyTogglesOn with value " + isAnyOn);
            OnAnyTogglesOn?.Invoke(isAnyOn);
        }
    }
}
