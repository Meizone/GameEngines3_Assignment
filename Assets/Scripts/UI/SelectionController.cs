using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class SelectionController : MonoBehaviour
{
    public enum Allowance
    {
        Any,
        None,
    }
    [SerializeField]
    public Allowance allow;
    [SerializeField]
    public GameObject firstSelected;

    private void SetFirstSelected()
    {
        if (allow != Allowance.None && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    private void Start()
    {
        SetFirstSelected();
    }

    private void OnEnable()
    {
        SetFirstSelected();
    }
}
