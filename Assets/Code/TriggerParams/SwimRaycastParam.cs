using UnityEngine;

public class SwimRaycastParam : Wasp.TriggerParams
{
    public Vector3 point;
    public float distance;
    public GameObject obj;
    public WaterHazardType.Type WaterHazardType;
}
