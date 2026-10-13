using UnityEngine;

public class Tools
{
    public static bool IsSameLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}