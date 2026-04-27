using System;
using System.Collections;
using Code.Misc;
using DG.Tweening;
using TMPro;
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
    private Material _curseHudTextMaterial;

    private CanvasGroup _markHudCanvasGroup;
    public bool isCurseEnabled;

    public bool isCurseTicking;
    private float _curseDuration;
    private CustomFogController _activeFogController;
    private TextMeshProUGUI _markHudTmp;
    public DialogueController dialogueFirstTimeUse3;
    private const float CurseMaximumDuration = 6f;

    private Color _activeFogControllerBaseColor;
    private float _activeFogControllerDepthMin;
    private float _activeFogControllerDepthMax;

    public static event Action<CultTrialFsm> OnTrialActive;

    private Transform _activeHalo;
    private Material _activeHaloMaterial;

    private void OnEnable()
    {
        PlayerFsm.OnPlayerCultTrialDeath += OnPlayerCultTrialDeath;
    }
    
    private void OnDisable()
    {
        PlayerFsm.OnPlayerCultTrialDeath -= OnPlayerCultTrialDeath;
    }

    private void Awake()
    {
        Singleton = this;
        _curseHalo = transform.Find("CurseHalo");
        _curseHaloMaterial = _curseHalo.GetComponent<Renderer>().material;
        _curseCanvasImage = transform.Find("CurseCanvas").GetComponentInChildren<Image>();
        _markHudCanvasGroup = transform.Find("Canvas").Find("MarkHud").GetComponent<CanvasGroup>();
        _markHudTmp = _markHudCanvasGroup.GetComponentInChildren<TextMeshProUGUI>();
        _curseHudTextMaterial = _markHudTmp.fontMaterial;
        _activeFogController = transform.Find("ActiveFogController")
            .GetComponent<CustomFogController>();
        _activeFogControllerBaseColor = _activeFogController.Color;
        _activeFogControllerDepthMin = _activeFogController.DepthMin;
        _activeFogControllerDepthMax = _activeFogController.DepthMax;

        _activeHalo = transform.Find("ActiveHalo");
        _activeHaloMaterial = _activeHalo.GetComponent<Renderer>().material;
        
        DisableCurse();
    }

    private void Update()
    {
        _activeHalo.position = Camera.main.transform.position;
        _activeFogController.transform.position = PlayerFsm.Singleton.transform.position;
        _curseHudTextMaterial.SetFloat("_BarGlowMultiply",Mathf.Lerp(_curseHudTextMaterial.GetFloat("_BarGlowMultiply"), isCurseTicking ? 2.5f : 0f, Time.deltaTime * 4f));
        _activeFogController.LerpStrengthMultiplier = isCurseTicking ? 1f : 100f;
        if (!isCurseTicking)
        {
            _activeFogController.LerpStrengthMultiplier = 1f;
            _activeFogController.Color = _activeFogControllerBaseColor;
            _activeFogController.DepthMin = _activeFogControllerDepthMin;
            _activeFogController.DepthMax = _activeFogControllerDepthMax;
            return;
        };
        
        _curseDuration -= Time.deltaTime;
        var w = _curseDuration / CurseMaximumDuration;
        _curseHudTextMaterial.SetFloat("_BarAmount", w);
        _activeFogController.LerpStrengthMultiplier = 20f;
        _activeFogController.Color = Color.Lerp(_activeFogControllerBaseColor, Color.black, (1f - w) * 1.3f);

        var depthOffset = Mathf.Lerp(0, -50f, 1f - w);
        // _activeFogController.DepthMin = _activeFogControllerDepthMin;
        _activeFogController.DepthMax = _activeFogControllerDepthMax + depthOffset;
        _activeHaloMaterial.SetFloat("_Alpha", Mathf.Lerp(0f,1f,1f-w));

        
        
        if (w < 0) OnCurseExpire();
    }

    private void OnCurseExpire()
    {
        PlayerFsm.Singleton.InvokePlayerDeath();
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
        _curseHaloMaterial.SetFloat("_Radius", Mathf.Lerp(0.1f, 1f, w));

        _curseCanvasImage.transform.localScale = Vector3.one * Mathf.Lerp(1.5f, 0.5f, Mathf.InverseLerp(1.5f, 3.5f, timeInCurrentState));
        _curseCanvasImage.transform.Rotate(new Vector3(0, 0, Time.deltaTime * 100f));
        var c = _curseCanvasImage.color;
        c.a = Mathf.InverseLerp(0f, 1.5f, timeInCurrentState) * Mathf.InverseLerp(2.25f, 1.5f, timeInCurrentState);
        _curseCanvasImage.color = c;
    }

    public void ReplenishCurseDuration(bool sphereEffect = true)
    {
        _curseDuration = CurseMaximumDuration;
        _curseHudTextMaterial.SetFloat("_BarAmount", 1f);
        if (sphereEffect) Util.InvokeSphereEffect(PlayerFsm.Singleton.transform.position + Vector3.up, Vector3.one * 5f, 1.5f, 1f, 0 );
        HudTmpPunchPosition();
    }

    private void HudTmpPunchPosition()
    {
        _markHudTmp.transform.parent.DOComplete();
        _markHudTmp.transform.parent.DOPunchPosition(Vector3.right * 10f, 0.5f, 10, 1f);

        IEnumerator PanelGlowCoroutine()
        {
            var t = 0f;
            var d = 0.15f;
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                t += Time.deltaTime;
                
                yield return null;
            }
        }
    }

    public void EnableActiveFog()
    {
        _activeFogController.Priority = 10;
    }
    
    public void DisableActiveFog()
    {
        _activeFogController.Priority = -10;
    }

    public void EnableCurse()
    {
        _curseHudTextMaterial.SetFloat("_BarGlowMultiply", 0f);
        _markHudCanvasGroup.alpha = 1f;
        _curseDuration = CurseMaximumDuration;
        isCurseEnabled = true;
        HudTmpPunchPosition();
    }
    
    public void DisableCurse()
    {
        _markHudCanvasGroup.alpha = 0f;
        isCurseEnabled = false;
    }

    public void StartCurseTicking(CultTrialFsm fsm)
    {
        isCurseTicking = true;
        OnTrialActive?.Invoke(fsm);
        HudTmpPunchPosition();
        // StartCoroutine(ActiveHaloMaterialCoroutine());
        //
        // IEnumerator ActiveHaloMaterialCoroutine()
        // {
        //     var t = 0f;
        //     var d = 0.15f;
        //     while (t < d)
        //     {
        //         var w = Util.SmoothLerp01(t / d);
        //         t += Time.deltaTime;
        //         _activeHaloMaterial.SetFloat("_Alpha", w);
        //         yield return null;
        //     }
        // }
    }

    private void OnPlayerCultTrialDeath()
    {
        _activeHaloMaterial.SetFloat("_Alpha", 0);

    }
}
