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
    private PlayerInput playerInput;

    void Start()
    {
        uiToShow.SetActive(false);
        playerInput = CharacterManager.Instance.player.playerInput;
    }

    public string GetInteractPrompt()
    {
        return promptMessage;
    }

    public void OnInteract()
    {
        uiToShow.SetActive(true);
        CharacterManager.Instance.player.controller.crosshair.SetActive(false);
        UnlockCursor();
        DisableActions();
        CharacterManager.Instance.player.curUIPanel = uiToShow;
    }

    public void CloseUI()
    {
        uiToShow.SetActive(false);
        CharacterManager.Instance.player.controller.crosshair.SetActive(true);
        LockCursor();
        EnableActions();
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

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DisableActions()
    {
        foreach (var action in playerInput.actions)
        {
            if (action.name == "Interaction")
                continue; // Interact액션은 제외
            if (action.name == "CloseUI")
                continue; // CloseUI액션은 제외
            action.Disable();
        }
    }
    public void EnableActions()
    {
        foreach (var action in playerInput.actions)
        {
            if (action.name == "Interaction")
                continue; // Interact액션은 제외
            if (action.name == "CloseUI")
                continue; // CloseUI액션은 제외
            action.Enable();
        }
    }
}
