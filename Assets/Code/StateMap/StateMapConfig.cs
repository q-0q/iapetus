using UnityEngine;

public class StateMapConfig
{
    public StateMap<string> Name;
    public StateMap<float> Duration;
    public StateMap<float> GravityStrengthMod;
    public StateMap<bool> IsAbstract;
    public StateMap<string> AnimationTrigger;
    public StateMap<bool> LockSpringCollider;
    public StateMap<Vector3> TightropeLineOffset;
    public StateMap<float> TightropeLineYLerpStrength;
    public StateMap<bool> CutscenePlayerDisabled;
    public StateMap<bool> CutsceneCameraDisabled;
}