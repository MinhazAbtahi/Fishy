using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MySlider : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI txtTitle;
    [SerializeField] TextMeshProUGUI txtSliderValue;
    [SerializeField] Slider slider;

    public float Value { get { return slider ? (float)Math.Round(slider.value, 2) : 0f; } }
    public string Title { get { return txtTitle ? txtTitle.text : string.Empty; } set { if (txtTitle) txtTitle.text = value;} }

    private void Start()
    {
        OnSliderValueChanged(slider.value);
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        txtSliderValue.text = string.Format("{0:0.00}", value);
    }
}
