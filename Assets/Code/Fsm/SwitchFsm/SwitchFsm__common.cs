using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public partial class SwitchFsm
{
    [SerializeField]
    private List<PowerConnector> outputs;
    
    private InteractionCollider _interactionCollider;
    private PowerConnector _powerConnector;
}