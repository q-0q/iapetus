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
    private static readonly Vector3 PositionOffset = new(0f, 1f, 0f);


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
        var blocked = Physics.Raycast(_camera.transform.position, toPlayer, toPlayer.magnitude - 5f, ~0, QueryTriggerInteraction.Ignore);
        Shader.SetGlobalFloat("_CutoutRadius", Mathf.Lerp(currentRadius, blocked ? MaxRadius : 0f, Time.deltaTime * RadiusLerpStrength));
        
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
    
    public static Vector2 ScreenToCustomSpace(Vector2 screenPosition)
    {
        var width = PixelRawImage.Singleton == null ? Screen.width : PixelRawImage.Singleton.RawImage.texture.width;
        var height = PixelRawImage.Singleton == null ? Screen.height : PixelRawImage.Singleton.RawImage.texture.height;
        float x = (screenPosition.x / width) * 0.1f;
        float y = (screenPosition.y / height) * 0.1f;
        return new Vector2(x, y);
    }




}
