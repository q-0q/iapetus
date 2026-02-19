using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerConnector : MonoBehaviour
{

    public bool Source = false;
    
    [SerializeField]
    private List<PowerConnector> outputs;
    
    private HashSet<PowerConnector> _spatialInputs;
    private HashSet<PowerConnector> _inputs;
    private PowerLamp _powerLamp;

    public bool Disabled = false;
    public bool RequireAllInputs = false;
    private bool _powered;
    public string PersistentEvent = "";
    private bool _persistentEventCached;
    
    void Awake()
    {
        _inputs = new HashSet<PowerConnector>();
        _spatialInputs = new HashSet<PowerConnector>();
        _powerLamp = GetComponentInChildren<PowerLamp>();
    }

    private void Start()
    {
        var data = SaveSystem.LoadSaveData(0);
        _persistentEventCached = SaveSystem.GetPersistentEventCompleted(PersistentEvent);
        foreach (var powerConnector in outputs)
        {
            powerConnector.AddInput(this);
        }
    }

    void Update()
    {
        _spatialInputs = FindSpatialInputs();
        if (_powerLamp is not null)
        {
            _powerLamp.SetLamp(IsPowered());
        }
        
        // var color = IsPowered() ? Color.yellow : Color.red;
        // var mat = GetComponent<MeshRenderer>().material;
        // mat.color = color;
    }

    public void AddInput(PowerConnector input)
    {
        _inputs.Add(input);
    }
    
    public void RemoveInput(PowerConnector input)
    {
        _inputs.Remove(input);
    }

    public bool IsPowered()
    {
        if (_persistentEventCached) return true;
        if (RequireAllInputs)
        {
            foreach (var input in _inputs)
            {
                if (!input.IsPowered()) return false;
            }
        }
        HashSet<PowerConnector> visited = new HashSet<PowerConnector>();
        var isPoweredRecursive = IsPoweredRecursive(this, visited);
        if (!_persistentEventCached && isPoweredRecursive && PersistentEvent != "")
        {
            SaveSystem.WritePersistentEvent(PersistentEvent, 0);
            _persistentEventCached = true;
        }
        return isPoweredRecursive;
    }

    private bool IsPoweredRecursive(PowerConnector current, HashSet<PowerConnector> visited)
    {
        // Avoid infinite loops (cycles)
        if (!visited.Add(current))
            return false;

        // Base case: if current connector is a source
        if (current.Source && !current.Disabled)
            return true;

        // Combine all inputs
        HashSet<PowerConnector> combined = new HashSet<PowerConnector>(current._spatialInputs);
        combined.UnionWith(current._inputs);

        // DFS: check each connected input recursively
        foreach (PowerConnector input in combined)
        {
            if (IsPoweredRecursive(input, visited))
                return true;
        }

        // No path leads to a powered source
        return false;
    }

    private HashSet<PowerConnector> FindSpatialInputs()
    {
        return Physics.OverlapSphere(transform.position, 0.75f, LayerMask.GetMask("PowerConnectors"))
            .Select(hit => hit.GetComponent<PowerConnector>())
            .Where(pc => pc != null && pc != this)
            .ToHashSet();
    }
    
}
