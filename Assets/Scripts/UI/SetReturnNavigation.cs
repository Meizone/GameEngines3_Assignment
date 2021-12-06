using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetReturnNavigation : MonoBehaviour
{
    [SerializeField]
    Selectable up;
    [SerializeField]
    Selectable down;
    [SerializeField]
    Selectable left;
    [SerializeField]
    Selectable right;

    private void SetNavigation()
    {
        foreach (Selectable selectable in GetComponentsInChildren<Selectable>())
        {
            Navigation nav = selectable.navigation;
            nav.selectOnLeft = left;
            //
            //if (up != null)
            //    nav.selectOnUp = up;
            //if (down != null)
            //    nav.selectOnDown = down;
            //if (left != null)
            //    nav.selectOnLeft = left;
            //if (right != null)
            //    nav.selectOnRight = right;
            selectable.navigation = nav;
        }
    }

    private void OnValidate()
    {
        SetNavigation();

    }

    private void Start()
    {
        SetNavigation();
    }
}
