using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Cycle Settings")]
    [SerializeField] private float cycleDuration = 2f;
    [Range(0f, 1f)]
    [SerializeField] private float cycleOffset = 0f;
    [SerializeField] private float cycleSnapping = 0f;
    [SerializeField] private bool loop = true;

    [Header("Movement")]
    [SerializeField] private Vector3 positionDestination = Vector3.zero;
    [SerializeField] private Vector3 rotationDestination = Vector3.zero;

    private float transitionTime = 0f; // Tracks time for non-cycling mode
    private Vector3 startPosition;
    private Quaternion startRotation;
    private PowerConnector _powerConnector;

    private void Start()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
        _powerConnector = GetComponentInChildren<PowerConnector>();
    }

    private void Update()
    {
        
        if (HitstopManager.Singleton.IsHitstopActive()) return;
        
        float t;
        
        if (_powerConnector is null)
        {
            if (cycleDuration <= 0f) return;

            float time = Time.time + cycleOffset * cycleDuration;
            float normalizedTime = (time % cycleDuration) / cycleDuration;

            // Sine wave between -1 and 1 → normalized to [0,1]
            float sine = Mathf.Sin(normalizedTime * Mathf.PI * 2f);
            t = (sine * 0.5f) + 0.5f;
        }
        else
        {
            var isForward = _powerConnector.IsPowered();
            // Increment or decrement transitionTime
            transitionTime += (isForward ? 1f : -1f) * Time.deltaTime;
            transitionTime = Mathf.Clamp(transitionTime, 0f, cycleDuration);

            // Normalize transition time
            t = (cycleDuration > 0f) ? transitionTime / cycleDuration : (isForward ? 1f : 0f);
        }

        // Apply sigmoid shaping
        float shapedT = SharpSymmetric(t, cycleSnapping);

        // Lerp position and rotation
        Vector3 targetPos = Vector3.Lerp(Vector3.zero, positionDestination, shapedT);
        Quaternion targetRot = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(rotationDestination), shapedT);

        transform.localPosition = startPosition + targetPos;
        transform.localRotation = startRotation * targetRot;
    }

    private float SharpSymmetric(float t, float snapping)
    {
        float k = Mathf.Lerp(0.1f, 60f, snapping);
        float x = (t - 0.5f) * 2f;
        float y = 1f / (1f + Mathf.Exp(-k * x));

        float min = 1f / (1f + Mathf.Exp(k));
        float max = 1f / (1f + Mathf.Exp(-k));

        return Mathf.InverseLerp(min, max, y);
    }

}