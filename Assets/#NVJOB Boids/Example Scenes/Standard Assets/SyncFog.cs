using UnityEngine;

[ExecuteInEditMode]
public class FogClipSyncer : MonoBehaviour
{
    public Camera targetCamera;
    [Range(1.1f, 1.5f)]
    public float bufferFactor = 1.2f; // Adds a small gap so the cut is never seen

    void Update()
    {
        if (targetCamera == null) targetCamera = GetComponent<Camera>();
        if (!RenderSettings.fog) return;

        float opaqueDistance = 0f;

        if (RenderSettings.fogMode == FogMode.Linear)
        {
            opaqueDistance = RenderSettings.fogEndDistance;
        }
        else if (RenderSettings.fogMode == FogMode.Exponential)
        {
            // d = -ln(0.01) / density
            opaqueDistance = 4.605f / RenderSettings.fogDensity;
        }
        else if (RenderSettings.fogMode == FogMode.ExponentialSquared)
        {
            // d = sqrt(-ln(0.01)) / density
            opaqueDistance = 2.146f / RenderSettings.fogDensity;
        }

        // Set the Far Clip Plane slightly further than the fog distance
        targetCamera.farClipPlane = opaqueDistance * bufferFactor;
    }
}