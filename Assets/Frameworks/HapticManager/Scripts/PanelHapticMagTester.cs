using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PanelHapticMagTester : MonoBehaviour
{
    [SerializeField] MySlider sliderSharpness, sliderIntensity;

    [Space]
    [SerializeField] Toggle isContinuous;

    [Space]
    [SerializeField] MySlider sliderDuration;
    [SerializeField] MySlider sliderDelay;

    private void Start()
    {
        sliderIntensity.Title = "Intensity";
        sliderSharpness.Title = "Sharpness";
        sliderDuration.Title = "Duration";
        sliderDelay.Title = "Delay In Between";

        isContinuous.onValueChanged.AddListener(OnContinuityChanged);
    }

    private void OnContinuityChanged(bool continuous)
    {
        sliderDelay.gameObject.SetActive(!continuous);
    }

    //public void Play()
    //{
    //    FPG.HapticController.PlayHapticWithMagnitude(sliderMagnitude.Value, sliderIntensity.Value, sliderSharpness.Value);
    //}

    bool hapticPlaying = false;
    double timePassed = 0;
    public void PlayHaptic()
    {
        if (hapticPlaying) return;

        hapticPlaying = true;
        if(isContinuous.isOn) FPG.HapticController.PlayContinuousHaptic(sliderIntensity.Value, sliderSharpness.Value, sliderDuration.Value);
        StartCoroutine(StopHaptic());
    }

    private IEnumerator StopHaptic()
    {
        yield return new WaitForSeconds(sliderDuration.Value);
        hapticPlaying = false;
        timePassed = 0;
    }

    private void Update()
    {
        if (hapticPlaying && !isContinuous.isOn)
        {
            if (timePassed >= sliderDelay.Value) {
                FPG.HapticController.PlayDiscreteHaptic(sliderIntensity.Value, sliderSharpness.Value);
                timePassed -= sliderDelay.Value;
            }

            timePassed += Time.deltaTime;
        }
    }
}
