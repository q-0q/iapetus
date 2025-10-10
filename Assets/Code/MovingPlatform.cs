using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Cycle Settings")]
    [SerializeField] private float cycleDuration = 2f;
    [Range(0f, 1f)]
    [SerializeField] private float cycleOffset = 0f;
    [SerializeField] private float cycleSnapping = 0f;

    [Header("Movement")]
    [SerializeField] private Vector3 positionDestination = Vector3.zero;
    [SerializeField] private Vector3 rotationDestination = Vector3.zero;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }

    private void Update()
    {
        if (cycleDuration <= 0f) return;

        float time = Time.time + cycleOffset * cycleDuration;
        float normalizedTime = (time % cycleDuration) / cycleDuration;

        // Sine wave between -1 and 1
        float sine = Mathf.Sin(normalizedTime * Mathf.PI * 2f);

        // Normalize to [0,1]
        float t = (sine * 0.5f) + 0.5f;

        // Apply symmetric sharpening using sigmoid shaping
        float shapedT = SharpSymmetric(t, cycleSnapping);

        // Lerp position and rotation
        Vector3 targetPos = Vector3.Lerp(Vector3.zero, positionDestination, shapedT);
        Quaternion targetRot = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(rotationDestination), shapedT);

        transform.localPosition = startPosition + targetPos;
        transform.localRotation = startRotation * targetRot;
    }


    
    private float SharpSymmetric(float t, float snapping)
    {
        // Map snapping [0,1] → sharpness [0.1, ~60]
        float k = Mathf.Lerp(0.1f, 60f, snapping);

        // Sigmoid centered at 0.5
        float x = (t - 0.5f) * 2f; // x in [-1, 1]
        float y = 1f / (1f + Mathf.Exp(-k * x));

        // Normalize based on min/max of logistic over [-1,1]
        float min = 1f / (1f + Mathf.Exp(k));     // value at x = -1
        float max = 1f / (1f + Mathf.Exp(-k));    // value at x = +1

        return Mathf.InverseLerp(min, max, y);
    }



}