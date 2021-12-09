using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    [SerializeField]
    private Image fill;
    [SerializeField]
    private Image flasher;
    [SerializeField]
    private float fillSpeed;
    public float value;
    public Color Color { get { return fill.color; } set { fill.color = value; } }

    private void Update()
    {
        if (fill.fillAmount < value)
        {
            flasher.fillAmount = value;
            fill.fillAmount = Mathf.MoveTowards(fill.fillAmount, value, fillSpeed * Time.deltaTime);
        }
        else if (flasher.fillAmount > value)
        {
            fill.fillAmount = value;
            flasher.fillAmount = Mathf.MoveTowards(flasher.fillAmount, value, fillSpeed * Time.deltaTime);
        }
    }
}
