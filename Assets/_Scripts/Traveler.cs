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
    [SerializeField] private MovementParameters _movementParameters;

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
        
        var moveToTownA = new MoveTo(transform, TownALocation.position, _movementParameters);
        var moveToForest = new MoveTo(transform, ForestLocation.position, _movementParameters);
        var collectResource = new CollectResource(_resource);
        var moveToTownB = new MoveTo(transform, TownBLocation.position, _movementParameters);
        var sellProduce = new SellProduce(_resource, _money);
        
        _stateMachine.AddTransition(moveToTownA, moveToForest, WaitedInTownAForOverASecondWithNoResources());
        _stateMachine.AddTransition(moveToForest, collectResource, WaitedInForestForOverASecond());
        _stateMachine.AddTransition(collectResource, moveToTownA, ResourceIsCollected());
        _stateMachine.AddTransition(moveToTownA, moveToTownB, WaitedInTownAForOverASecondWithResources());
        _stateMachine.AddTransition(moveToTownB, sellProduce, WaitedInTownBForOverASecond());
        _stateMachine.AddTransition(sellProduce, moveToTownA, ProduceIsSold());
        
        _stateMachine.SetState(moveToTownA);
        
        Func<bool> WaitedInTownAForOverASecondWithNoResources() => () => moveToTownA.IsDestinationReached && Resource == 0;
        Func<bool> ResourceIsCollected() => () => collectResource.IsResourceCollected;
        Func<bool> WaitedInForestForOverASecond() => () => moveToForest.IsDestinationReached;
        Func<bool> WaitedInTownAForOverASecondWithResources() => () => moveToTownA.IsDestinationReached && Resource > 0;
        Func<bool> WaitedInTownBForOverASecond() => () => moveToTownB.IsDestinationReached;
        Func<bool> ProduceIsSold() => () => sellProduce.IsProduceSold;
    }

    private void Update() => _stateMachine.Tick();

    void OnDrawGizmos()
    {
        if (_stateMachine != null && _drawPath) _stateMachine.DrawGizmos();
    }
    
}
