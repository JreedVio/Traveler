using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light _directionalLight;
    [SerializeField] private LightingPreset _preset;
    
    [Tooltip("Time of a full day cycle in minutes")]
    [SerializeField, Range(0.1f, 60f)] private float _dayCycle;
    
    [Tooltip("Time of day in hours")]
    [Range(0, 24)] public float TimeOfDay;
    
    private float DayCycleInSeconds => _dayCycle * 60f;

    private void Update()
    {
        float currentTime = Time.timeSinceLevelLoad;
        float timeOfDayInPercent = currentTime % DayCycleInSeconds / DayCycleInSeconds;
        TimeOfDay = timeOfDayInPercent * 24f;
        
        UpdateLighting();
    }

    private void UpdateLighting()
    {
        float timeOfDayInPercent = TimeOfDay / 24f;
        RenderSettings.ambientLight = _preset.AmbientColor.Evaluate(timeOfDayInPercent);
        RenderSettings.fogColor = _preset.FogColor.Evaluate(timeOfDayInPercent);
        
        _directionalLight.color = _preset.DirectionalColor.Evaluate(timeOfDayInPercent);
        _directionalLight.transform.localRotation = Quaternion.Euler(new Vector3(timeOfDayInPercent * 360f - 90f, 170, 0));
    }
    
}
