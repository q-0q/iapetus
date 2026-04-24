using System;
using UnityEngine;
using UnityEngine.UI;

public class CultTrialManager : MonoBehaviour
{
    public static CultTrialManager Singleton;
    
        
    public DialogueController dialogueNoItem;
    public DialogueController dialogueItem;
    public DialogueController dialogueFirstTimeUse1;
    public DialogueController dialogueFirstTimeUse2;

    private const string FirstTimeUsePersistentEvent = "CultTrialUsed";
    
    private Material _curseHaloMaterial;
    private Transform _curseHalo;
    private Image _curseCanvasImage;

    private CanvasGroup _markHudCanvasGroup;
    public bool isCurseEnabled;

    private void Awake()
    {
        Singleton = this;
        _curseHalo = transform.Find("CurseHalo");
        _curseHaloMaterial = _curseHalo.GetComponent<Renderer>().material;
        _curseCanvasImage = transform.Find("CurseCanvas").GetComponentInChildren<Image>();
        _markHudCanvasGroup = transform.Find("Canvas").Find("MarkHud").GetComponent<CanvasGroup>();
        DisableCurse();
    }
    
    public void SetCurseEffects(float timeInCurrentState)
    {
        _curseHalo.position = PlayerFsm.Singleton.transform.position + Vector3.up * 2f;
        _curseCanvasImage.transform.parent.position = _curseHalo.position;
        
        var strength = 2f;
        var freeLook = PlayerCinemachineFreeLook.Singleton.GetFreeLook();
        freeLook.m_Lens.FieldOfView =
            Mathf.Lerp(freeLook.m_Lens.FieldOfView, 65f, Time.deltaTime * strength);

        var offset = freeLook.transform.GetComponent<CinemachineCameraOffset>();
        offset.m_Offset = Vector3.Lerp(offset.m_Offset,
            new Vector3(0, 0, -6f), Time.deltaTime * strength);
        
        _curseHalo.localScale = Vector3.one * Mathf.Lerp(10f, 2f, Mathf.InverseLerp(0, 2.5f, timeInCurrentState));
        var w = Mathf.InverseLerp(0, 2f, timeInCurrentState);
        _curseHaloMaterial.SetFloat("_Multiply", w * Mathf.InverseLerp(2.5f, 2f, timeInCurrentState));
        _curseHaloMaterial.SetFloat("_Radius", Mathf.Lerp(0.1f, 0.6f, w));

        _curseCanvasImage.transform.localScale = Vector3.one * Mathf.Lerp(1.5f, 0.5f, Mathf.InverseLerp(1.5f, 3.5f, timeInCurrentState));
        _curseCanvasImage.transform.Rotate(new Vector3(0, 0, Time.deltaTime * 100f));
        var c = _curseCanvasImage.color;
        c.a = Mathf.InverseLerp(0f, 1.5f, timeInCurrentState) * Mathf.InverseLerp(2.25f, 1.5f, timeInCurrentState);
        _curseCanvasImage.color = c;
    }

    public void EnableCurse()
    {
        _markHudCanvasGroup.alpha = 1f;
        isCurseEnabled = true;
    }
    
    public void DisableCurse()
    {
        _markHudCanvasGroup.alpha = 0f;
        isCurseEnabled = false;
    }
}
