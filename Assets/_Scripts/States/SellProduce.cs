using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellProduce : IState
{
    private float _timeElapsed = 0f;
    private float _timeDelay = 0f;
    public bool IsProduceSold = false;
    
    private readonly IntReference _resourceReference;
    private readonly IntReference _moneyReference;

    public SellProduce(IntReference resourceReference_, IntReference moneyReference_)
    {
        _resourceReference = resourceReference_;
        _moneyReference = moneyReference_;
    }
    public void Tick()
    {
        if (IsProduceSold) return;
        
        _timeElapsed += Time.deltaTime;
        if (_timeElapsed >= _timeDelay)
        {
            _resourceReference.Value--;
            _moneyReference.Value += Random.Range(1, 3);
            UIManager.Instance.UpdateHUD();
            
            _timeElapsed = 0f;
            _timeDelay = Random.Range(1f, 3f);
            
            if (_resourceReference.Value <= 0) IsProduceSold = true;
        }
    }

    public void OnEnter()
    {
        _timeDelay = Random.Range(1f, 3f);
    }

    public void OnExit()
    {
        _timeDelay = 0f;
        _timeElapsed = 0f;
        IsProduceSold = false;
    }
}
