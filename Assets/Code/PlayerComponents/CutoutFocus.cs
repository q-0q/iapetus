using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class CutoutFocus : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        _timeSpentTriggered = 0f;
    }

    private const float PlayerCameraDistanceOffset = -4f;
    private const float PlayerCameraDistanceFlatOffset = -1.25f;
    
    private const float MaxRadius = 0.0225f;
    
    private const float RadiusGrowLerpStrength = 30f;
    private const float RadiusShrinkLerpStrength = 5f;
    private const float TimeToTrigger = 0.05f;
    
    private static readonly Vector3 PositionOffset = new(0f, 1f, 0f);
    private Camera _camera1;
    private float _timeSpentTriggered;

    
    void Update()
    {
        Vector2 playerScreenPosition = _camera.WorldToScreenPoint(transform.position + PositionOffset);
        
        
        Shader.SetGlobalVector("_PlayerScreenPosition", ScreenToCustomSpace(playerScreenPosition));


        float playerCameraDistance = Vector3.Distance(transform.position, _camera.transform.position) + PlayerCameraDistanceOffset;
        float playerCameraFlatDistance = Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(_camera.transform.position.x, 0f, _camera.transform.position.z)) + PlayerCameraDistanceFlatOffset;

        Shader.SetGlobalFloat("_PlayerCameraDistance", playerCameraDistance);
        Shader.SetGlobalFloat("_PlayerCameraFlatDistance", playerCameraFlatDistance);

        var currentRadius = Shader.GetGlobalFloat("_CutoutRadius");
        var toPlayer = transform.position + PositionOffset - _camera.transform.position;
        Debug.DrawRay(_camera.transform.position, toPlayer);
        var blocked = Physics.Raycast(_camera.transform.position, toPlayer, toPlayer.magnitude - 5f, Fsm.GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore);
        _timeSpentTriggered = blocked ? _timeSpentTriggered + Time.deltaTime : 0f;
        var triggered = (_timeSpentTriggered >= TimeToTrigger) && (!PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.TrialTeleport)) && !CutsceneManager.Singleton.IsCutsceneActive();
        Shader.SetGlobalFloat("_CutoutRadius", Mathf.Lerp(currentRadius, triggered ? MaxRadius : 0f, Time.deltaTime *
            (triggered ? RadiusGrowLerpStrength : RadiusShrinkLerpStrength)));
        
    }
    
    public static Vector2 ScreenToCustomSpaceOnRawImage(Vector2 screenPos, RectTransform rawImageRectTransform)
    {
        // Get world corners of the RawImage
        Vector3[] corners = new Vector3[4];
        rawImageRectTransform.GetWorldCorners(corners);

        Vector2 bottomLeft = corners[0]; // bottom-left corner
        Vector2 topRight = corners[2];   // top-right corner

        // Size of the RawImage in screen space
        Vector2 rawImageSize = topRight - bottomLeft;

        // Convert screen position to RawImage-local position
        Vector2 localPos = screenPos - bottomLeft;
        Vector2 centered = localPos - (rawImageSize / 2f);

        // Scale X so right edge is 0.1
        float scale = 0.1f / (rawImageSize.x / 2f);
        Vector2 custom = centered * scale;
        
        return custom;
    }
    
    public Vector2 ScreenToCustomSpace(Vector2 screenPosition)
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 v2 = screenPosition - screenCenter;
        float x = (v2.x / Screen.width) * 0.2f;
        float y = (v2.y / Screen.width) * 0.2f;
        var output = new Vector2(y, x);
        return output;
    }




}
