using System.Collections.Generic;
using Code.Fsm.TrialCollectibleFSM;
using Code.TriggerParams;
using UnityEngine;
using UnityEngine.Serialization;

public partial class TrialCollectibleFsm
{
    [SerializeField] private List<TrialCollectibleKeyframe> _keyframes;
    [SerializeField] private string _id;
    private int _currentKeyframeIndex;
    private float _timeOnCurrentKeyframe;

    private Transform _marker;
}