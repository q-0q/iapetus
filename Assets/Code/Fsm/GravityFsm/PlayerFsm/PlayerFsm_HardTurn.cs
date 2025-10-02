using UnityEngine;

public partial class PlayerFsm
{
    private void HardTurnOnUpdate()
    {
        _momentum = Mathf.Max(0, _momentum - MomentumLossRate * Time.deltaTime * HardTurnMomentumLossModifier);
        Animator.SetLayerWeight(2, 0);
    }
}