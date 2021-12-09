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
        float v = value / 100;
        if (fill.fillAmount < v)
        {
            flasher.fillAmount = v;
            fill.fillAmount = Mathf.MoveTowards(fill.fillAmount, v, fillSpeed * Time.deltaTime);
        }
        else if (flasher.fillAmount > v)
        {
            fill.fillAmount = v;
            flasher.fillAmount = Mathf.MoveTowards(flasher.fillAmount, v, fillSpeed * Time.deltaTime);
        }
    }
}
