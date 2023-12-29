using System;
using System.Collections;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class Traveler : MonoBehaviour
{
    public Transform TownALocation;
    public Transform TownBLocation;
    public Transform ForestLocation;
    [SerializeField] private float _speed = 15;
    [SerializeField] private float _slowestSpeed = 1;
    [SerializeField] private float _initialRotationSpeed = 10;
    [SerializeField] private float _turnSpeed = 3;
    [SerializeField] private float _turnDistance = 5;
    [SerializeField] private float _stoppingDistance = 10;
    [SerializeField] private AnimationCurve _stoppingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private IntReference _resource = new IntReference();
    [SerializeField] private IntReference _money = new IntReference();
    [SerializeField] private bool _drawPath;
    public int Resource => _resource.Value;
    public int Money => _money.Value;
    
    private StateMachine _stateMachine;
    private AnimatorController _animatorController;
    
    private void Start()
    {
        _animatorController = GetComponent<AnimatorController>();
        _stateMachine = new StateMachine();
        
        var moveToForest = new MoveTo(transform, ForestLocation.position, _speed, _turnDistance, _turnSpeed, _stoppingDistance, _slowestSpeed, _initialRotationSpeed, _stoppingCurve);
        var collectResource = new CollectResource(_resource);
        var moveToTownA = new MoveTo(transform, TownALocation.position, _speed, _turnDistance, _turnSpeed, _stoppingDistance, _slowestSpeed, _initialRotationSpeed, _stoppingCurve);
        var moveToTownB = new MoveTo(transform, TownBLocation.position, _speed, _turnDistance, _turnSpeed, _stoppingDistance, _slowestSpeed, _initialRotationSpeed, _stoppingCurve);
        var sellProduce = new SellProduce(_resource, _money);
        
        _stateMachine.AddTransition(moveToForest, collectResource, WaitedInForestForOverASecond());
        _stateMachine.AddTransition(collectResource, moveToTownA, ResourceIsCollected());
        _stateMachine.AddTransition(moveToTownA, moveToTownB, WaitedInTownAForOverASecond());
        _stateMachine.AddTransition(moveToTownB, sellProduce, WaitedInTownBForOverASecond());
        _stateMachine.AddTransition(sellProduce, moveToForest, ProduceIsSold());
        
        _stateMachine.SetState(moveToForest);
        
        Func<bool> WaitedInForestForOverASecond() => () => moveToForest.IsDestinationReached;
        Func<bool> ResourceIsCollected() => () => collectResource.IsResourceCollected;
        Func<bool> WaitedInTownAForOverASecond() => () => moveToTownA.IsDestinationReached;
        Func<bool> WaitedInTownBForOverASecond() => () => moveToTownB.IsDestinationReached;
        Func<bool> ProduceIsSold() => () => sellProduce.IsProduceSold;
    }

    private void Update() => _stateMachine.Tick();

    void OnDrawGizmos()
    {
        if (_stateMachine != null) _stateMachine.DrawGizmos();
    }
    
}
