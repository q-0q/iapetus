using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManaManager : MonoBehaviour
{

    public static PlayerManaManager Singleton;

    private int _maxMana;
    private int _currentMana;
    private Transform _flipHolder;
    private List<PlayerManaIndicator> _manaIndicators;

    private const float ReplenishCooldown = 1.5f;
    private float _replenishTimer = 0f;

    public static event Action OnPlayerMaxMana;
    public static event Action OnPlayerConsumedMana;

    private void Awake()
    {
        Singleton = this;
        _flipHolder = transform.Find("FlipHolder");
    }

    public int GetCurrentAvailableMana()
    {
        return _currentMana;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _maxMana = 2;
        _currentMana = _maxMana;
        SetUpIndicators(_maxMana);
        transform.SetParent(null);
    }

    private void SetUpIndicators(int count)
    {
        _maxMana = count;
        var manaPrefab = Resources.Load("Prefab/PlayerManaIndicator") as GameObject;
        for (int i = _flipHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(_flipHolder.GetChild(i));
        }

        _manaIndicators = new List<PlayerManaIndicator>();
        float spacing = 0.65f;
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(manaPrefab, _flipHolder);
            obj.transform.localPosition = new Vector3(-5.5f, (spacing * i) - (spacing * count * 0.5f) + 1f, 0);
            _manaIndicators.Add(obj.GetComponent<PlayerManaIndicator>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        var angle = Vector3.SignedAngle(PlayerFsm.Singleton.transform.forward, Camera.main.transform.forward, Vector3.up);
        _flipHolder.transform.localScale = new Vector3(angle > 0 ? 1f : -1f, 1f, 1f);
        if (_currentMana < _maxMana) _replenishTimer += Time.deltaTime;
        
        if (_replenishTimer >= ReplenishCooldown)
        {
            _currentMana++;
            _manaIndicators[_currentMana - 1].Replenish();
            _replenishTimer = 0;
            
            if (_currentMana == _maxMana) OnPlayerMaxMana?.Invoke();
        }

        var xzLerp = 30f;
        var yLerp = 8f;

        var playerPos = PlayerFsm.Singleton.transform.position;
        var pos = new Vector3(
            Mathf.Lerp(transform.position.x, playerPos.x, xzLerp * Time.deltaTime),
            Mathf.Lerp(transform.position.y, playerPos.y, yLerp * Time.deltaTime),
            Mathf.Lerp(transform.position.z, playerPos.z, xzLerp * Time.deltaTime));
        transform.position = pos;
    }

    public void Consume()
    {
        if (_currentMana < 1) return;
        OnPlayerConsumedMana?.Invoke();
        _manaIndicators[_currentMana - 1].Consume();
        _replenishTimer = 0.25f;
        _currentMana--;
    }
}
