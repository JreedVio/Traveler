using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourceText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private Traveler _traveler;
    
    [SerializeField] private TextMeshProUGUI _timeSpeedText;
    [SerializeField] private Slider _timeSpeedSlider;
    public static UIManager Instance { get; private set; }
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _timeSpeedSlider.onValueChanged.AddListener(OnValueChange);
        UpdateHUD();
    }

    void OnValueChange(float value)
    {
        _timeSpeedText.text = value.ToString();
        Time.timeScale = value;
    }

    public void UpdateHUD()
    {
        _resourceText.text = "Resource: " + _traveler.Resource;
        _moneyText.text = "Money: " + _traveler.Money;
    }
}
