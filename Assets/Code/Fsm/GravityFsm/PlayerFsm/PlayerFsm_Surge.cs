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
            (transposer.m_FollowOffset.normalized * 14f),  (Time.deltaTime * 0.75f));
        
        var composer = _surgeStartupCamera.GetCinemachineComponent<CinemachineComposer>();
        composer.m_TrackedObjectOffset =
            Vector3.Lerp(composer.m_TrackedObjectOffset, forwardOffset, Time.deltaTime * 0.75f);
        
        _surgeStartupCamera.m_Lens.FieldOfView =
            Mathf.Lerp(_surgeStartupCamera.m_Lens.FieldOfView, 95f, Time.deltaTime * 1.25f);

        if (TimeInCurrentState() > 1.25f)
        {
            InteractionCanvas.Singleton.SetPsuedoInteractable("Surge");
            if (_inputBuffer.IsBuffered("Interact")) Machine.Jump(PlayerFsmState.SurgeDashStartup);
        }
        // if (!_playerInput.actions["Interact"].IsPressed() && TimeInCurrentState() < 0.5f) Machine.Jump(PlayerFsmState.Idle);
        
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
            // .Permit(FsmTrigger.Timeout, PlayerFsmState.SurgeDash)
            .OnExit(_ =>
            {
                InteractionCanvas.Singleton.ClearPsuedoInteractable();
                // _surgeStartupCamera.Priority = -20;
                _surgeStartupCamera.m_Follow = null;
                _surgeStartupCamera.m_LookAt = null;
            })
            .OnExitFrom(FsmTrigger.Timeout, _ =>
            {
            })
            .OnEntry(@params =>
            {
                EndSurge();
                if (@params is SurgePedestalParam surgePedestalParam)
                {
                    _currentSurgePedestal = surgePedestalParam.SurgePedestal;
                    _currentSurgePedestal.StartChannel();
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
                
                // _surgeStartupCamera.Priority = 20;
                
                Animator.SetLayerWeight(1, 0);
            });
        
        Machine.Configure(PlayerFsmState.SurgeDashStartup)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.SurgeDash)
            .OnExit(_ =>
            {
                StartSurge();
                _currentSurgePedestal.EndChannel();

            })
            .OnEntry(_ =>
            {
            });
        
        Machine.Configure(PlayerFsmState.SurgeDash)
            .SubstateOf(GravityFsmState.Aerial)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnExit(_ =>
            {
            })
            .OnEntry(_ =>
            {
                _inputBuffer.ConsumeBuffer("Interact");
            });
    }
}