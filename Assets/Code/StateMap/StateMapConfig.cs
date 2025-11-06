using UnityEngine;

public class StateMapConfig
{
    public StateMap<string> Name;
    public StateMap<float> Duration;
    public StateMap<float> GravityStrengthMod;
    public StateMap<bool> IsAbstract;
    public StateMap<string> AnimationTrigger;
    public StateMap<bool> LockSpringCollider;
    public StateMap<float> TightropeLineYOffset;
    public StateMap<float> TightropeLineYLerpStrength;
}