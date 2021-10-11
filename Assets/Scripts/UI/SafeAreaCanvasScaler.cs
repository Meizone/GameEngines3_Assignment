using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SafeAreaCanvasScaler : MonoBehaviour
{
    RectTransform rectTransform;
    Rect safeArea;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        safeArea = new Rect(0, 0, 0, 0);
    }

    private void Update()
    {
        if (safeArea != Screen.safeArea)
        {
            OnScreenOrientationChanged();
        }
    }

    private void OnScreenOrientationChanged()
    {
        Rect safeArea = Screen.safeArea;
        Vector2 screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        rectTransform.anchorMin = new Vector2(safeArea.min.x / screenSize.x, safeArea.min.y / screenSize.y);
        rectTransform.anchorMax = new Vector2(safeArea.max.x / screenSize.x, safeArea.max.y / screenSize.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(Screen.safeArea.width, Screen.safeArea.height, 0));
    }
}
