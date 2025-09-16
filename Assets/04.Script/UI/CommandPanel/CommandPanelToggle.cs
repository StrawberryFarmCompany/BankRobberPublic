using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandPanelToggle : MonoBehaviour
{
    public GameObject expandedPanel;
    public GameObject minimizedPanel;

    public Button minimizeButton;
    public Button restoreButton;

    public KeyCode toggleKey = KeyCode.U;

    bool isMinimized = false;

    void Awake()
    {
        if (minimizeButton) minimizeButton.onClick.AddListener(Minimize);
        if (restoreButton) restoreButton.onClick.AddListener(Expand);
        ApplyState();
    }

    void OnDestroy()
    {
        if (minimizeButton) minimizeButton.onClick.RemoveListener(Minimize);
        if (restoreButton) restoreButton.onClick.RemoveListener(Expand);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) Toggle();
    }

    public void Toggle() { if (isMinimized) Expand(); else Minimize(); }
    public void Minimize() { isMinimized = true; ApplyState(); }
    public void Expand() { isMinimized = false; ApplyState(); }

    void ApplyState()
    {
        if (expandedPanel) expandedPanel.SetActive(!isMinimized);
        if (minimizedPanel) minimizedPanel.SetActive(isMinimized);
    }
}
