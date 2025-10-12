using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFsmTightropeCollider : MonoBehaviour
{
    private GravityFsm _owner;
    private Collider _collider;

    public void SetOwner(GravityFsm owner)
    {
        _owner = owner; 
    }
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("Cube").TryGetComponent(out _collider);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        var mask = LayerMask.GetMask("TightropeTrigger");
        var neighbors = Physics.OverlapSphere(_owner.transform.position, 6.5f, mask, QueryTriggerInteraction.Collide);
        foreach (var neighbor in neighbors)
        {
            transform.position = Physics.ClosestPoint(_owner.transform.position, neighbor, neighbor.transform.position, neighbor.transform.rotation);
            neighbor.transform.parent.TryGetComponent(out TightropeController controller);
            transform.rotation = controller.GetAlignmentRotation();
            _collider.enabled = true;
            return;
        }

        transform.rotation = _owner.transform.rotation;
        transform.position = _owner.transform.position;
        _collider.enabled = false;
    }
}
