using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SetUI : MonoBehaviour, INPCInteractable
{
    [SerializeField] private GameObject uiToShow;
    public string promptMessage;
    

    void Start()
    {
        uiToShow.SetActive(false);
        
    }

    public string GetInteractPrompt()
    {
        return promptMessage;
    }

    public void OnInteract()
    {
        uiToShow.SetActive(true);
        CharacterManager.Instance.player.controller.crosshair.SetActive(false);
        CharacterManager.Instance.player.controller.UnlockCursor();
        CharacterManager.Instance.player.controller.DisableActions();
        CharacterManager.Instance.player.curUIPanel = uiToShow;
    }

    public void CloseUI()
    {
        uiToShow.SetActive(false);
        CharacterManager.Instance.player.controller.crosshair.SetActive(true);
        CharacterManager.Instance.player.controller.LockCursor();
        CharacterManager.Instance.player.controller.EnableActions();
        if (CharacterManager.Instance.player.curUIPanel == uiToShow)
        {
            CharacterManager.Instance.player.curUIPanel = null;
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnCloseUI(InputAction.CallbackContext context)
    {
        if (uiToShow.activeInHierarchy && context.started)
        {
            CloseUI();
        }
    }
}
