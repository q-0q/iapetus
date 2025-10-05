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
    }

    private const float PlayerCameraDistanceOffset = -4f;
    private const float PlayerCameraDistanceFlatOffset = -1.5f;
    
    private const float MaxRadius = 0.01f;
    
    private const float RadiusLerpStrength = 30f;
    private static readonly Vector3 PositionOffset = new(0f, 0.5f, 0f);


    void Update()
    {
        Vector2 playerScreenPosition = _camera.WorldToScreenPoint(transform.position + PositionOffset);
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 playerScreenPositionNormalized = (playerScreenPosition - screenCenter);
        var scale = (Screen.width * 5f);
        playerScreenPositionNormalized =
            new Vector2(playerScreenPositionNormalized.x / scale, playerScreenPositionNormalized.y /
                                                                  scale);

        Shader.SetGlobalVector("_PlayerScreenPosition", playerScreenPositionNormalized);


        float playerCameraDistance = Vector3.Distance(transform.position, _camera.transform.position) + PlayerCameraDistanceOffset;
        float playerCameraFlatDistance = Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(_camera.transform.position.x, 0f, _camera.transform.position.z)) + PlayerCameraDistanceFlatOffset;
        print(playerCameraFlatDistance);

        Shader.SetGlobalFloat("_PlayerCameraDistance", playerCameraDistance);
        Shader.SetGlobalFloat("_PlayerCameraFlatDistance", playerCameraFlatDistance);

        var currentRadius = Shader.GetGlobalFloat("_CutoutRadius");
        var toPlayer = transform.position + PositionOffset - _camera.transform.position;
        Debug.DrawRay(_camera.transform.position, toPlayer);
        var blocked = Physics.Raycast(_camera.transform.position, toPlayer, toPlayer.magnitude - 5f, ~0, QueryTriggerInteraction.Ignore);
        Shader.SetGlobalFloat("_CutoutRadius", Mathf.Lerp(currentRadius, blocked ? MaxRadius : 0f, Time.deltaTime * RadiusLerpStrength));

        // print("Screen position: " + Shader.GetGlobalVector("_PlayerScreenPosition"));
    }
}
