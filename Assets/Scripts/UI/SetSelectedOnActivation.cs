using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetSelectedOnActivation : MonoBehaviour
{
    [SerializeField]
    GameObject selection;

    private void OnEnable()
    {
        if (selection != null)
            EventSystem.current.SetSelectedGameObject(selection);
    }
}
