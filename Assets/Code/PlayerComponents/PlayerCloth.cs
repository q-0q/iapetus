using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCloth : MonoBehaviour
{

    private const int SegmentCount = 4;
    private const float SegmentDistance = 0.125f;
    private const float SegmentScale = 1f;
    private List<GameObject> _segments;
    private LineRenderer _lineRenderer;

    private GameObject _segmentPrefab;
    public Transform root;
    
    // Start is called before the first frame update
    void Start()
    {
        // return;
        transform.position = transform.parent.position;
        _segments = new List<GameObject>();
        _segmentPrefab = Resources.Load("Prefab/PlayerClothSegment") as GameObject;
        var connectedObject = gameObject;
        for (int i = 0; i < SegmentCount; i++)
        {
            var newSegmentPosition = transform.position - transform.forward * i * SegmentDistance;
            var newSegmentObject = GameObject.Instantiate(_segmentPrefab, newSegmentPosition, transform.rotation);
            newSegmentObject.transform.localScale = new Vector3(SegmentScale, SegmentScale, SegmentScale);

            newSegmentObject.TryGetComponent(out ConfigurableJoint joint);
            if (connectedObject.TryGetComponent(out Rigidbody rb)) joint.connectedBody = rb;

            if (i == 0)
            {
                joint.angularXMotion = ConfigurableJointMotion.Locked;
                joint.angularYMotion = ConfigurableJointMotion.Locked;
                joint.angularZMotion = ConfigurableJointMotion.Locked;
            }
            connectedObject = newSegmentObject;
            
            _segments.Add(newSegmentObject);
        }

        TryGetComponent(out _lineRenderer);
        _lineRenderer.positionCount = SegmentCount + 1;
    }

    // Update is called once per frame
    void Update()
    {
        // return;
        for (int i = 0; i < SegmentCount + 1; i++)
        {
            var pos = i == 0 ? root.position : _segments[i - 1].transform.position;
            _lineRenderer.SetPosition(i, pos);
        }
    }

    private void FixedUpdate()
    {
        // return;
        
        for (int i = 0; i < SegmentCount; i++)
        {
            _segments[i].TryGetComponent(out Rigidbody rigidbody);
            rigidbody.AddForce(Vector3.down * (3000f * Time.fixedDeltaTime));
        }
    }

    private int GetEnvironmentalLayerMask()
    {
        return ~LayerMask.NameToLayer("PlayerClothCollider");
    }
}
