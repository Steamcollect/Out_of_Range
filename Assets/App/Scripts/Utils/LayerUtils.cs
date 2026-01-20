using UnityEngine;

public static class LayerUtils
{
    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
    
    public static void IgnoreLayerMaskCollision(LayerMask layerMaskA, LayerMask layerMaskB, bool ignore)
    {
        for (int i = 0; i < 32; i++)
        {
            if (!layerMaskA.Contains(i)) continue;
            for (int j = 0; j < 32; j++)
            {
                if (layerMaskB.Contains(j))
                {
                    Physics.IgnoreLayerCollision(i, j, ignore);
                }
            }
        }
    }
}