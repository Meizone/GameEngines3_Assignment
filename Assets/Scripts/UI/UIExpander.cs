using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// UI Expander by Kiera Bacon
/// Version 1.0 created  October 6th, 2021
/// 
/// Component to transition the scale width/height of a UI object 
/// when selected over a specified number of seconds.
/// 
/// Note: The base size that it transitions back to when deselected 
/// is read only at Start.
/// </summary>
public class UIExpander : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Transition Parameters")]
    [SerializeField]
    private Vector2 expandedSize;
    [SerializeField, Min(0)]
    private float transitionTime;

    [Header("Transition Triggers")]
    [SerializeField]
    private bool expandOnHover;
    private bool isHovered;
    [SerializeField]
    private bool expandOnSelect;
    private bool isSelected;

    // Internal variables
    private Vector2 baseSize;
    private float transitionDistance;
    private Coroutine expansionCoroutine;
    private RectTransform rectTransform;

    private bool allowStateChange = true;
    public bool AllowStateChange {
        get { return allowStateChange; }
        set {
            allowStateChange = value;
            ChangeSize(isSelected || isHovered);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (rectTransform == null) // We can't do these steps if they've already been set, because we can't reliably know what the original baseSize might have been.
        {
            rectTransform = GetComponent<RectTransform>();
            baseSize = rectTransform.sizeDelta;
        }

        transitionDistance = Vector2.Distance(baseSize, expandedSize) / transitionTime;
    }
#endif

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        baseSize = rectTransform.sizeDelta;
        transitionDistance = Vector2.Distance(baseSize, expandedSize) / transitionTime;
    }

    private IEnumerator TransitionSize(Vector2 newSize)
    {
        while (rectTransform.sizeDelta != newSize)
        {
            // Using Vector2.MoveTowards will move at a constant speed, so only that speed needs to be calculated in advance.
            // There is no need to calculate the transition time, since it will obviously be shorter if the transition starts in the middle.
            rectTransform.sizeDelta = Vector2.MoveTowards(rectTransform.sizeDelta, newSize, transitionDistance * Time.deltaTime);
            
            yield return null;
        }

        expansionCoroutine = null;
    }

    private void ChangeSize(bool expand)
    {
        if (!allowStateChange)
            return;

        if (expansionCoroutine != null) // Remove the existing transition before starting a new one, so they aren't fighting for control.
            StopCoroutine(expansionCoroutine);

        expansionCoroutine = StartCoroutine(TransitionSize(expand ? expandedSize : baseSize));
    }

    public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
    {
        isHovered = true;
        if (expandOnHover)
        {
            ChangeSize(true);
        }
    }

    public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
    {
        isHovered = false;
        if (expandOnHover && (!expandOnSelect || !isSelected)) // Don't shrink again on pointer exit if the button is still selected.
        {
            ChangeSize(false);
        }
    }

    public void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
    {
        isSelected = true;
        if (expandOnSelect)
        {
            ChangeSize(true);
        }
    }

    public void OnDeselect(UnityEngine.EventSystems.BaseEventData eventData)
    {
        isSelected = false;
        if (expandOnSelect && (!expandOnHover || !isHovered)) // Don't shrink again on deselect exit if the button is still hovered.
        {
            ChangeSize(false);
        }
    }
}
