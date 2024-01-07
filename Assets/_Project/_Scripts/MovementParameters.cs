using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Movement Parameters", menuName = "Scriptables/Movement Parameters", order = 1)]
public class MovementParameters : ScriptableObject
{
    public float Speed;
    public float SlowestSpeed;
    public float InitialRotationSpeed;
    public float TurnSpeed;
    public float TurnDistance;
    public float StoppingDistance;
    public AnimationCurve StoppingCurve;
    public AnimationCurve TurningCurve;
}
