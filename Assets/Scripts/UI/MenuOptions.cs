using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Menu Options by Kiera Bacon
/// Version 1.0 created  October 6th, 2021
/// 
/// Component containing all menu options.
/// </summary>
public class MenuOptions : MonoBehaviour
{
    public void LoadScene(LevelMetaData level)
    {
        LevelLoader.LoadLevel(level);
    }


    // This function is for use in the main menu, and causes the menu to shift to the left or right on toggle.
    private GameObject parentOfHeirarchy = null;
    public void Shift(bool value)
    {
        if (parentOfHeirarchy == null)
        {
            Transform parent = transform;
            while (parent.parent != null)
                parent = parent.parent;
           parentOfHeirarchy = parent.gameObject;
        }

        parentOfHeirarchy.GetComponent<Animator>().SetBool("Shift", value);
    }

    // This function is for any toggled button, and makes it so that its highlight will not unhighlight.
    Selectable selectable = null;
    UIExpander expander = null;
    public void DisableTransitions(bool value)
    {
        if (selectable == null) selectable = GetComponent<Selectable>();
        if (selectable != null)
            selectable.transition = value ? Selectable.Transition.None : Selectable.Transition.SpriteSwap;
        
        if (expander == null) expander = GetComponent<UIExpander>();
        if (expander != null)
            expander.AllowStateChange = !value;
    }

    Toggle[] siblings;
    Toggle toggleOnThis;
    // This function can be called by any toggle in a layout group, and it will unflip all other toggles in the same layout group.
    public void UnflipSiblings()
    {
        if (siblings.Length < 1)
        {
            LayoutGroup layoutGroup = transform.parent.GetComponent<LayoutGroup>();
            siblings = layoutGroup.GetComponentsInChildren<Toggle>();
        }

        Toggle currentActive = null;
        foreach (Toggle toggle in siblings)
        {
            if (toggle.isOn && toggle != toggleOnThis)
                currentActive = toggle;
        }

        currentActive.isOn = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
