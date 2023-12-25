using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourceText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private Traveler _traveler;
    public static UIManager Instance { get; private set; }
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateHUD();
    }

    public void UpdateHUD()
    {
        _resourceText.text = "Resource: " + _traveler.Resource;
        _moneyText.text = "Money: " + _traveler.Money;
    }
}
