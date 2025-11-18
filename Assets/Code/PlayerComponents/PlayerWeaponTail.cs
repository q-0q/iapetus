using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponTail : MonoBehaviour
{

    private const int SegmentCount = 4;
    private const float SegmentDistance = 0.35f;
    private const float SegmentScale = 0.15f;
    private List<GameObject> _segments;
    private LineRenderer _lineRenderer;

    private GameObject _segmentPrefab;
    public static Rigidbody FinalSegmentRigidbody;
    
    // Start is called before the first frame update
    void Start()
    {
        _segments = new List<GameObject>();
        _segmentPrefab = Resources.Load("Prefab/PlayerWeaponTailSegment") as GameObject;
        var connectedObject = gameObject;
        for (int i = 0; i < SegmentCount; i++)
        {
            var newSegmentPosition = transform.position - transform.forward * i * SegmentDistance;
            var newSegmentObject = GameObject.Instantiate(_segmentPrefab, newSegmentPosition, transform.rotation);
            newSegmentObject.transform.localScale = new Vector3(SegmentScale, SegmentScale, SegmentScale);

            newSegmentObject.TryGetComponent(out ConfigurableJoint joint);
            if (connectedObject.TryGetComponent(out Rigidbody rb)) joint.connectedBody = rb;
            connectedObject = newSegmentObject;

            if (i == SegmentCount - 1) newSegmentObject.TryGetComponent(out FinalSegmentRigidbody);

            _segments.Add(newSegmentObject);
        }

        TryGetComponent(out _lineRenderer);
        _lineRenderer.positionCount = SegmentCount;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < SegmentCount; i++)
        {
            _lineRenderer.SetPosition(i, _segments[i].transform.position);
        }
    }
}
