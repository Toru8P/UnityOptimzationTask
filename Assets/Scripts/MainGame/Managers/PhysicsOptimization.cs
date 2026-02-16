using UnityEngine;

public static class PhysicsOptimization
{
    public const int PROJECTILE_LAYER = 8;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Apply()
    {
        Physics.IgnoreLayerCollision(PROJECTILE_LAYER, PROJECTILE_LAYER, true);
    }
}
