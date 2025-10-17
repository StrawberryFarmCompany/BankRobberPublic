using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    public static bool isPaused { get; private set; } = false;

    private const string LOBBY_SCENE = "LobbyPlayerChange";
    private const string TITLE_SCENE = "MainTitleUIScene";

    void Start()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (optionUI) optionUI.SetActive(false);

        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (optionButton != null) optionButton.onClick.AddListener(OpenOption);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
        if (backButton != null) backButton.onClick.AddListener(CloseOption);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionUI && optionUI.activeSelf)
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        optionUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
    }

    private void OpenOption()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (optionUI) optionUI.SetActive(true);
    }

    private void CloseOption()
    {
        if (pausePanel) pausePanel.SetActive(true);
        if (optionUI) optionUI.SetActive(false);
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pausePanel) pausePanel.SetActive(false);
        if (optionUI) optionUI.SetActive(false);

        string cur = SceneManager.GetActiveScene().name;

        //로비에서 메인타이틀
        if (cur == LOBBY_SCENE)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(TITLE_SCENE);
        }
        //인게임에서 로비
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SceneManager.LoadScene(LOBBY_SCENE);
        }
    }
}
