using Cinemachine;
using Code.TriggerParams;
using UnityEngine;

public partial class PlayerFsm
{
    private void SurgeStartupOnUpdate()
    {
        var transposer = _surgeStartupCamera.GetCinemachineComponent<CinemachineTransposer>();
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, transposer.m_FollowOffset.normalized * 14f,  (Time.deltaTime * 2f));
        _surgeStartupCamera.m_Lens.FieldOfView =
            Mathf.Lerp(_surgeStartupCamera.m_Lens.FieldOfView, 110f, Time.deltaTime * 2f);
        
        if (!_playerInput.actions["Interact"].IsPressed() && TimeInCurrentState() < 0.5f) Machine.Jump(PlayerFsmState.Idle);
        
        _currentSurgePedestalMaterial.SetFloat("_Weight", Mathf.InverseLerp(0, 1.5f, TimeInCurrentState()));
    }
    
    private void SurgeDashOnUpdate()
    {
        var movementMofifier = Mathf.Lerp(2f, 1f, Mathf.InverseLerp(0, 0.1f, TimeInCurrentState()));
        HandleCollisionMove(movementMofifier);
    }
    
    
    private void SurgeConfigure()
    {
        Machine.Configure(PlayerFsmState.SurgeStartup)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.SurgeDash)
            .OnExit(_ =>
            {
                _currentSurgePedestalMaterial.SetFloat("_Weight", 0);
                _surgeStartupCamera.Priority = -20;
            })
            .OnExitFrom(FsmTrigger.Timeout, _ =>
            {
                StartSurge();
            })
            .OnEntry(@params =>
            {

                if (@params is MaterialParam materialParam)
                {
                    _currentSurgePedestalMaterial = materialParam.Material;
                }
                
                var state = PlayerCinemachineFreeLook.Singleton.GetFreeLook().State;
                var playerPos = PlayerFsm.Singleton.transform.position;

                // 2. Calculate the offset vector from player to camera
                // This is the "stick" length and direction
                Vector3 offset = state.RawPosition - playerPos;

                // 3. Apply to Transposer
                var transposer = _surgeStartupCamera.GetCinemachineComponent<CinemachineTransposer>();
                transposer.m_FollowOffset = offset;

                // 4. Match the rotation and FOV
                _surgeStartupCamera.transform.rotation = state.RawOrientation;
                _surgeStartupCamera.m_Lens.FieldOfView = state.Lens.FieldOfView;
                
                _surgeStartupCamera.Priority = 20;
                
                Animator.SetLayerWeight(1, 0);
            });
        
        Machine.Configure(PlayerFsmState.SurgeDash)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnExit(_ =>
            {
            })
            .OnEntry(_ =>
            {
            });
    }
}