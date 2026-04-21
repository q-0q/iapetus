using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Wasp;

public class StateMap<T>
{
    private Dictionary<int, List<Binding<T>>> _dictionary;
    private T _default;
    
    public StateMap(T @default)
    {
        _dictionary = new Dictionary<int, List<Binding<T>>>();
        _default = @default;
    }
    
    public T Get(Fsm fsm)
    {
        Binding<T> bestBinding = null;
        int maxWeight = int.MinValue;
        bool tieDetected = false;

        // Use a direct foreach to avoid LINQ overhead
        foreach (var kvp in _dictionary)
        {
            // Check if the FSM is in this state
            if (fsm.Machine.IsInState(kvp.Key))
            {
                List<Binding<T>> bindings = kvp.Value;
                for (int i = 0; i < bindings.Count; i++)
                {
                    Binding<T> current = bindings[i];
                    int currentWeight = current.Weight();

                    if (bestBinding == null || currentWeight > maxWeight)
                    {
                        maxWeight = currentWeight;
                        bestBinding = current;
                        tieDetected = false;
                    }
                    else if (currentWeight == maxWeight)
                    {
                        tieDetected = true;
                    }
                }
            }
        }

        if (bestBinding == null)
            return _default;

        if (tieDetected)
            Debug.LogError($"Tie detected for weight {maxWeight}");

        return bestBinding.Value();
    }

    public void Add(int state, T value, int weight = 0)
    {
        if (!_dictionary.ContainsKey(state)) _dictionary[state] = new List<Binding<T>>();
        _dictionary[state].Add(new Binding<T>(value, weight));
    }
    
    public T GetStrict(Fsm fsm)
    {
        var eligibleBindings = _dictionary
            .Where(kv => fsm.Machine.State() == kv.Key)
            .SelectMany(kv => kv.Value)
            .ToHashSet();

        if (eligibleBindings.Count == 0)
            return _default;

        var maxWeight = eligibleBindings.Max(b => b.Weight());

        var topBindings = eligibleBindings
            .Where(b => b.Weight() == maxWeight)
            .ToHashSet();

        if (topBindings.Count > 1)
            Debug.LogError($"Tie detected: {topBindings.Count} bindings with weight {maxWeight}");

        return topBindings.First().Value();
    }
    
}

public class Binding<T>
{
    private readonly T _value;
    private readonly int _weight;
    public Binding(T value, int weight = 0)
    {
        _value = value;
        _weight = weight;
    }

    public int Weight()
    {
        return _weight;
    }

    public T Value()
    {
        return _value;
    }
}