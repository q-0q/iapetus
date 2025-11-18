using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GravityFsmSpringCollider : MonoBehaviour
{
    public GravityFsm owner;
    private Collider _collider;
    public TightropeController tightropeController;
    private Rigidbody _rigidBody;
    public static float Sag = 1f;

    public void SetOwner(GravityFsm owner)
    {
        this.owner = owner; 
    }
    // Start is called before the first frame update
    void Start()
    {
        transform.TryGetComponent(out _collider);
        transform.TryGetComponent(out _rigidBody);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTransform();

    }

    public Vector3 anchorPoint; // The point where the spring is anchored
    public float springConstant = 500f; // The spring constant
    public float damping = 20f; // Damping factor
    
    void FixedUpdate()
    {
        
        // return;
        // Calculate the displacement vector between the anchor point and the current position
        Vector3 displacement = anchorPoint - transform.localPosition;

        // Calculate the spring force using Hooke's Law
        Vector3 springForce = springConstant * displacement;

        // Calculate the damping force
        Vector3 dampingForce = damping * _rigidBody.velocity;

        // Calculate the total force
        Vector3 totalForce = springForce - dampingForce;

        // Apply the force to the Rigidbody
        _rigidBody.AddForce(totalForce * (Time.deltaTime * 40f), ForceMode.Force);
        transform.localPosition = new Vector3(0, transform.localPosition.y, 0);

    }
    
    private void UpdateTransform()
    {
        var mask = LayerMask.GetMask("TightropeTrigger");
        var neighbors = Physics.OverlapSphere(owner.transform.position, 6.5f, mask, QueryTriggerInteraction.Collide);
        if (owner.StateMapConfig.LockSpringCollider.Get(owner)) return;
        var target = transform.parent;
        var closestPosition = Vector3.zero;
        Collider closestNeighbor = null;
        foreach (var neighbor in neighbors)
        {
            var pos = Physics.ClosestPoint(owner.transform.position, neighbor, neighbor.transform.position, neighbor.transform.rotation) - Vector3.up * Sag;
            if (Vector3.Distance(pos, owner.transform.position) <
                Vector3.Distance(closestPosition, owner.transform.position))
            {
                closestNeighbor = neighbor;
                closestPosition = pos;
            }
        }

        if (tightropeController != null) tightropeController.currentSpringCollider = null;
        
        if (closestNeighbor != null && closestPosition.y < owner.transform.position.y + 2f)
        {
            target.position = closestPosition;
            closestNeighbor.transform.parent.TryGetComponent(out TightropeController controller);
            tightropeController = controller;
            controller.currentSpringCollider = this;
            target.rotation = controller.GetAlignmentRotation();
            _collider.enabled = true;
            return;
        }

        
        target.rotation = owner.transform.rotation;
        target.position = owner.transform.position;
        _collider.enabled = false;
    }

    private void OnPlayerParentTransformChanged(Transform t, float momentum, float yVelocity)
    {
        
        if (t != transform) return;

        
        var forward = PlayerFsm.Singleton.transform.forward * (momentum * 0.1f);
        var down = PlayerFsm.Singleton.transform.up * (yVelocity * 0.5f);;
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.AddForce(down, ForceMode.Impulse);
    }

    private void OnEnable()
    {
        PlayerFsm.OnPlayerParentTransformChanged += OnPlayerParentTransformChanged;
    }

    private void OnDestroy()
    {
        PlayerFsm.OnPlayerParentTransformChanged -= OnPlayerParentTransformChanged;
    }
    
    
    
}
