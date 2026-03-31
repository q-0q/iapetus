using Cinemachine;
using Code.TriggerParams;
using UnityEngine;

public partial class PlayerFsm
{
    private void SurgeStartupOnUpdate()
    {
        var transposer = _surgeStartupCamera.GetCinemachineComponent<CinemachineTransposer>();

        var forwardOffset = transform.forward * Mathf.Lerp(0, 4f, Mathf.InverseLerp(0.15f, 0.25f, TimeInCurrentState()));
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset,
            (transposer.m_FollowOffset.normalized * 14f) + forwardOffset,  (Time.deltaTime * 0.75f));
        
        var composer = _surgeStartupCamera.GetCinemachineComponent<CinemachineComposer>();
        composer.m_TrackedObjectOffset =
            Vector3.Lerp(composer.m_TrackedObjectOffset, forwardOffset, Time.deltaTime * 0.75f);
        
        _surgeStartupCamera.m_Lens.FieldOfView =
            Mathf.Lerp(_surgeStartupCamera.m_Lens.FieldOfView, 95f, Time.deltaTime * 1.25f);
        
        if (!_playerInput.actions["Interact"].IsPressed() && TimeInCurrentState() < 0.5f) Machine.Jump(PlayerFsmState.Idle);
        
        _currentSurgePedestalMaterial.SetFloat("_Weight", Mathf.InverseLerp(0, 1.25f, TimeInCurrentState()));
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
                _surgeStartupCamera.m_Follow = null;
                _surgeStartupCamera.m_LookAt = null;
            })
            .OnEntry(@params =>
            {
                
                EndSurge();
                if (@params is MaterialParam materialParam)
                {
                    _currentSurgePedestalMaterial = materialParam.Material;
                }
                
                _surgeStartupCamera.m_Follow = PlayerCinemachineFreeLook.Singleton.GetFreeLook().m_Follow;
                _surgeStartupCamera.m_LookAt = _surgeStartupCamera.m_Follow;
                
                var state = PlayerCinemachineFreeLook.Singleton.GetFreeLook().State;
                var playerPos = PlayerFsm.Singleton.transform.position;
                
                Vector3 offset = state.RawPosition - playerPos;
                var transposer = _surgeStartupCamera.GetCinemachineComponent<CinemachineTransposer>();
                transposer.m_FollowOffset = offset;
                _surgeStartupCamera.transform.rotation = state.RawOrientation;
                _surgeStartupCamera.m_Lens.FieldOfView = state.Lens.FieldOfView;

                _surgeStartupCamera.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = Vector3.zero;
                
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