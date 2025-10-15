using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("정지UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionUI;

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button quitButton;

    [Header("옵션 UI")]
    [SerializeField] private Button backButton;

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (optionUI) optionUI.SetActive(false);

        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (optionButton != null) optionButton.onClick.AddListener(OpenOption);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
        if (backButton) backButton.onClick.AddListener(CloseOption);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionUI.activeSelf)
            {
                CloseOption();
                return;
            }

            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        optionUI.SetActive(false);
        isPaused = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        optionUI.SetActive(false);
        isPaused = false;
    }

    private void OpenOption()
    {
        Debug.Log("옵션 메뉴 열기");
        pausePanel.SetActive(false);
        optionUI.SetActive(true);
    }

    private void CloseOption()
    {
        Debug.Log("옵션 메뉴 닫기");
        pausePanel.SetActive(true);
        optionUI.SetActive(false);
    }

    private void QuitGame()
    {
        Debug.Log("메인메뉴 혹은 로비 이동");
    }
}
