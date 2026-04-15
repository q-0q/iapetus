using UnityEngine;

public class WaterHazardType : MonoBehaviour
{
    public enum Type
    {
        InstantDrown,
        Freeze,
        None,
    }

    public Type type;
}
