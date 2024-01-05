using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectResource : IState
{
    private float _timeElapsed = 0f;
    private float _timeDelay = 0f;
    public bool IsResourceCollected = false;
    
    private readonly IntReference _resourceReference;

    public CollectResource(IntReference resourceReference_)
    {
        _resourceReference = resourceReference_;
    }
    public void Tick()
    {
        if (IsResourceCollected) return;
        
        _timeElapsed += Time.deltaTime;
        if (_timeElapsed >= _timeDelay)
        {
            _resourceReference.Value++;
            UIManager.Instance.UpdateHUD();
            IsResourceCollected = true;
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
        IsResourceCollected = false;
    }
}
