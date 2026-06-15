using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public partial class InventoryMenuFsm
{
    
    private PlayerInput _playerInput;
    private CanvasGroup _canvasGroup;
    private CanvasGroup _listCanvasGroup;
    private CanvasGroup _useConfirmationCanvasGroup;
    private Image _closeImage;

    public static InventoryMenuFsm Singleton;
    

    private GameObject _listSelection;
    private TextMeshProUGUI _listSelectionName;
    private TextMeshProUGUI _listSelectionDescription;
    private TextMeshProUGUI _listSelectionUseDescription;
    private int _listSelectionListItemId;
    
    
    private TextMeshProUGUI _useConfirmationItemName;
    private TextMeshProUGUI _useConfirmation;
    
    private KeyItemRegistration _confirmationData;
    
    public Button navBagButton;
    public Button navMovelistButton;
    public Button navTestButton;
    public GameObject listContentParent;

    private List<Button> navButtons;

    public void OnQuitClicked()
    {
        Application.Quit();
    }

    public void OnNewGameConfirmClicked()
    {
        SceneLoader.Singleton.LoadScene(SaveSystem.LoadCachedSaveData().scene);
    }

    private bool NeedToSelect(GameObject currentObject)
    {
        foreach (var selectable in GetComponentsInChildren<Selectable>())
        {
            if (EventSystem.current.currentSelectedGameObject == selectable.gameObject) return false;
        }

        return _playerInput.actions["Navigate"].ReadValue<Vector2>().magnitude > 0.1f;
    }

    private void RefreshInventoryData(SaveSystem.SaveData obj)
    {
        var data = new List<InventoryListItem.InventoryListItemData>();
        foreach (var item in obj.items)
        {
            var keyItemRegistration = KeyItemRegistry.KeyItemRegistrations[item];
            data.Add(new InventoryListItem.InventoryListItemData()
            {
                description = keyItemRegistration.description,
                displayName = keyItemRegistration.displayName,
                id = item,
                subText = keyItemRegistration.GetUseDescription(),
            });
        }

        PopulateListView(data);
    }

    private void OnGameMenuOpened()
    {
        if (Machine.IsInState(InventoryMenuFsmState.Closed)) return;
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }

    private void OnGameMenuClosed()
    {
        if (Machine.IsInState(InventoryMenuFsmState.Closed)) return;
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
    }

    private void PopulateListView(List<InventoryListItem.InventoryListItemData> data)
    {
        for (int i = listContentParent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(listContentParent.transform.GetChild(i).gameObject);
        }
        var prefab = Resources.Load("Prefab/InventoryListItem") as GameObject;
        var buttons = new List<Button>();
        for (int i = 0; i < data.Count; i++)
        {
            var obj = Instantiate(prefab, listContentParent.transform);
            buttons.Add(obj.GetComponentInChildren<Button>());
            obj.GetComponent<InventoryListItem>().SetItemData(data[i]);
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            var prevId = i == 0 ? buttons.Count - 1 : i - 1;
            var nextId = i == buttons.Count - 1 ? 0 : i + 1;
            var navigation = buttons[i].navigation;
            navigation.selectOnUp = buttons[prevId];
            navigation.selectOnDown = buttons[nextId];
            buttons[i].navigation = navigation;
        }
    }

    
}