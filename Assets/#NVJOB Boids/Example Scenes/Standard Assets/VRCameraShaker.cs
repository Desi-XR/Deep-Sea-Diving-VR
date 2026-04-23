using UnityEngine;

public class VRCameraShaker : MonoBehaviour
{
    [Header("Shake Settings")]
    [Tooltip("Drag the new 'Shake Pivot' object here!")]
    public Transform shakePivot; 
    
    public float maxShakeMagnitude = 0.5f; 
    public float shakeSpeed = 35f; 

    private float currentFrameIntensity = 0f;
    private float smoothedIntensity = 0f;

    // THE FIX: Only accept a shake request if it is STRONGER than the current shake this frame.
    // This stops distant sharks from cancelling out the close sharks!
    public void SetShakeIntensity(float intensity)
    {
        if (intensity > currentFrameIntensity)
        {
            currentFrameIntensity = Mathf.Clamp01(intensity);
        }
    }

    // LateUpdate runs AFTER all sharks have finished their Update() logic
    void LateUpdate()
    {
        if (shakePivot == null) return;

        // Smoothly transition between intensities so it doesn't violently snap in VR
        smoothedIntensity = Mathf.Lerp(smoothedIntensity, currentFrameIntensity, Time.deltaTime * 10f);

        if (smoothedIntensity > 0.01f)
        {
            // Calculate chaotic shaking
            float x = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) * 2f - 1f);
            float y = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) * 2f - 1f);
            float z = (Mathf.PerlinNoise(Time.time * shakeSpeed, Time.time * shakeSpeed) * 2f - 1f);

            // Apply intensity based on smoothed value
            Vector3 shakeOffset = new Vector3(x, y, z) * maxShakeMagnitude * smoothedIntensity;
            
            // Apply strictly to localPosition so it doesn't break player movement
            shakePivot.localPosition = shakeOffset; 
        }
        else
        {
            // Smoothly snap the pivot exactly back to center when safe
            shakePivot.localPosition = Vector3.Lerp(shakePivot.localPosition, Vector3.zero, Time.deltaTime * 10f);
        }

        // CRITICAL: Reset the intensity to 0 for the next frame.
        // Sharks must actively request a shake every frame they are close enough.
        currentFrameIntensity = 0f; 
    }
}