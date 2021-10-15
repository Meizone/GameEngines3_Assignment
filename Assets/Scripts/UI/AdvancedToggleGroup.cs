using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AdvancedToggleGroup : MonoBehaviour
{
    public bool allowOnlyOne;
    public bool allowNone;
    public UnityEvent<Toggle> OnAnyToggleValueChanged;
    public UnityEvent<bool> OnIsAnyToggleActiveChanged;
    private Toggle[] _toggles = null;
    private BitArray _activeToggles = null;
    private int lastOnIndex = -1;
    private int lastOffIndex = 0;
    private bool _isAnyToggleActive = false;
    public bool isAnyToggleActive { get { return _isAnyToggleActive; } set { SetAnyOn(value); } }
    public IEnumerable<Toggle> activeToggles { 
        get {
            int count = _activeToggles.Count;
            Toggle[] result = new Toggle[count]; // Count returns the number that are set to true.
            for (int i = 0, j = 0; i < _toggles.Length && j < count; i++)
                if (_activeToggles[j])
                    result[j++] = _toggles[i]; // Increment happens after result is added to.
            return result;
        } 
    }

    private void Register()
    {
        if (_toggles == null)
        {
            _toggles = GetComponentsInChildren<Toggle>();
            _activeToggles = new BitArray(_toggles.Length);
        }

        if (_toggles.Length < 1)
            return;

        for (int i = 0; i < _toggles.Length; i++)
        {
            _toggles[i].onValueChanged.AddListener(IsAnyOn);
            _activeToggles[i] = _toggles[i].isOn;
        }

        if (allowOnlyOne)
            SetAnyOn(false);
        if (!allowNone)
            IsAnyOn(false);
    }

    private void Deregister()
    {
        if (_toggles == null)
            return;

        foreach (Toggle toggle in _toggles)
            toggle.onValueChanged.RemoveListener(IsAnyOn);
    }

    private void OnEnable()
    {
        Register();
    }

    private void OnDisable()
    {
        Deregister();
    }

    private void OnTransformChildrenChanged()
    {
        Deregister();
        Register();
    }

    private void SetAnyOn(bool value, bool callback = true)
    {
        if (value != _isAnyToggleActive)
        {
            _isAnyToggleActive = value;
            OnIsAnyToggleActiveChanged?.Invoke(_isAnyToggleActive);
        }

        if (value)
        {
            if (callback)
                _toggles[lastOffIndex].isOn = true;
            else
                _toggles[lastOffIndex].SetIsOnWithoutNotify(true);
        }
        else
        {
            foreach (Toggle toggle in _toggles)
            {
                if (callback)
                    toggle.SetIsOnWithoutNotify(false);
                else
                    toggle.isOn = false;
            }
        }
    }

    private void IsAnyOn(bool value)
    {
        if (value && allowOnlyOne && lastOnIndex > 0)
            _toggles[lastOnIndex].isOn = false;
        if (!value && !allowNone)
            _toggles[lastOffIndex].isOn = true;

        lastOnIndex = -1;
        for (int i = 0; i < _toggles.Length; i++)
        {
            bool isOn = _toggles[i].isOn;
            if (isOn)
            {
                if (!_activeToggles[i])
                {
                    lastOnIndex = i;
                    OnAnyToggleValueChanged?.Invoke(_toggles[i]);
                }
            }
            else if (_activeToggles[i])
            {
                lastOffIndex = i;
                OnAnyToggleValueChanged?.Invoke(_toggles[i]);
            }
        }

        if (_isAnyToggleActive != (lastOnIndex > 0))
        {
            _isAnyToggleActive = lastOnIndex > 0;
            OnIsAnyToggleActiveChanged?.Invoke(_isAnyToggleActive);
        }
    }
}
