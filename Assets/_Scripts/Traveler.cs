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
    public float Speed = 30;
    
    private StateMachine _stateMachine;
    private AnimatorController _animatorController;

    [SerializeField] private IntReference _resource = new IntReference();
    [SerializeField] private IntReference _money = new IntReference();
    public int Resource => _resource.Value;
    public int Money => _money.Value;
    
    private void Start()
    {
        _animatorController = GetComponent<AnimatorController>();
        _stateMachine = new StateMachine();
        
        var moveToForest = new MoveTo(transform, ForestLocation.position, Speed);
        var collectResource = new CollectResource(_resource);
        var moveToTownA = new MoveTo(transform, TownALocation.position, Speed);
        var moveToTownB = new MoveTo(transform, TownBLocation.position, Speed);
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
    
}
