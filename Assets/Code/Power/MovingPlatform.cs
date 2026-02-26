using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Cycle Settings")]
    [SerializeField] private float cycleDuration = 2f;
    [Range(0f, 1f)]
    [SerializeField] private float cycleOffset = 0f;
    [SerializeField] private float cycleSnapping = 0f;
    [SerializeField] private bool loop = true;

    [Header("Movement (Per Cycle Delta)")]
    [SerializeField] private Vector3 positionPerCycle = Vector3.zero;
    [SerializeField] private Vector3 rotationPerCycle = Vector3.zero;

    private float transitionTime = 0f;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private PowerConnector _powerConnector;


    public void JumpToEnd()
    {
        transitionTime = cycleDuration;
    }
    
    private void Start()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
        _powerConnector = GetComponentInChildren<PowerConnector>();
    }

    private void Update()
    {
        if (HitstopManager.Singleton.IsHitstopActive()) return;
        if (cycleDuration <= 0f) return;

        if (_powerConnector is null)
        {
            if (loop)
            {
                // Continuous linear motion (no snapping)
                float time = Time.time + cycleOffset * cycleDuration;
                float cycles = time / cycleDuration;

                ApplyContinuousMotion(cycles);
            }
            else
            {
                // Ping-pong
                float time = Time.time + cycleOffset * cycleDuration;
                float normalizedTime = (time % cycleDuration) / cycleDuration;
                float sine = Mathf.Sin(normalizedTime * Mathf.PI * 2f);
                float t = (sine * 0.5f) + 0.5f;

                ApplyLerpedMotion(t);
            }
        }
        else
        {
            var isForward = _powerConnector.IsPowered();

            transitionTime += (isForward ? 1f : -1f) * Time.deltaTime;
            transitionTime = Mathf.Clamp(transitionTime, 0f, cycleDuration);

            float t = transitionTime / cycleDuration;
            ApplyLerpedMotion(t);
        }
    }

    private void ApplyContinuousMotion(float cycles)
    {
        Vector3 targetPos = positionPerCycle * cycles;
        Quaternion targetRot = Quaternion.Euler(rotationPerCycle * cycles);

        transform.localPosition = startPosition + targetPos;
        transform.localRotation = startRotation * targetRot;
    }

    private void ApplyLerpedMotion(float t)
    {
        float shapedT = SharpSymmetric(t, cycleSnapping);

        Vector3 targetPos = Vector3.Lerp(Vector3.zero, positionPerCycle, shapedT);
        Quaternion targetRot = Quaternion.Lerp(
            Quaternion.identity,
            Quaternion.Euler(rotationPerCycle),
            shapedT
        );

        transform.localPosition = startPosition + targetPos;
        transform.localRotation = startRotation * targetRot;
    }

    private float SharpSymmetric(float t, float snapping)
    {
        if (snapping <= 0f) return t;

        float k = Mathf.Lerp(0.1f, 60f, snapping);
        float x = (t - 0.5f) * 2f;
        float y = 1f / (1f + Mathf.Exp(-k * x));

        float min = 1f / (1f + Mathf.Exp(k));
        float max = 1f / (1f + Mathf.Exp(-k));

        return Mathf.InverseLerp(min, max, y);
    }
}
