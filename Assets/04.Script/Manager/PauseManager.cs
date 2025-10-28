using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("정지UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionUI;

    [SerializeField] private Button resumeButton;   //계속하기
    [SerializeField] private Button optionButton;   //설정
    [SerializeField] private Button quitButton;     //나가기

    [Header("옵션 UI")]
    [SerializeField] private Button backButton;     //옵션ui에서 뒤로가기

    [Header("나가기 확인 UI")]
    [SerializeField] private GameObject quitConfirmUI;
    [SerializeField] private TMP_Text quitText;
    [SerializeField] private Button quitYesButton;
    [SerializeField] private Button quitNoButton;

    public static bool isPaused { get; private set; } = false;

    private const string LOBBY_SCENE = "LobbyScene";
    private const string TITLE_SCENE = "MainTitleScene";

    void Start()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (optionUI) optionUI.SetActive(false);
        if (quitConfirmUI) quitConfirmUI.SetActive(false);

        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (optionButton != null) optionButton.onClick.AddListener(OpenOption);
        if (quitButton != null) quitButton.onClick.AddListener(OpenQuitConfirm);
        if (backButton != null) backButton.onClick.AddListener(CloseOption);
        if (quitYesButton) quitYesButton.onClick.AddListener(QuitGame);
        if (quitNoButton) quitNoButton.onClick.AddListener(CloseQuitConfirm);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (quitConfirmUI && quitConfirmUI.activeSelf) { CloseQuitConfirm(); return; }
            if (optionUI && optionUI.activeSelf)
            {
                CloseOption();
                return;
            }
            if (CharacterManager.Instance.player.curUIPanel != null) return;
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
        quitConfirmUI.SetActive(false);

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

    private void OpenQuitConfirm()
    {
        if (!quitConfirmUI) return;

        string cur = SceneManager.GetActiveScene().name;
        string msg = (cur == LOBBY_SCENE) ? "메인메뉴로 돌아가겠습니까?" : "로비로 돌아가겠습니까?";
        if (quitText) quitText.text = msg;

        quitConfirmUI.SetActive(true);
        EventSystem.current?.SetSelectedGameObject(quitYesButton ? quitYesButton.gameObject : null);
    }

    private void CloseQuitConfirm()
    {
        if (quitConfirmUI) quitConfirmUI.SetActive(false);
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pausePanel) pausePanel.SetActive(false);
        if (optionUI) optionUI.SetActive(false);
        if (quitConfirmUI) quitConfirmUI.SetActive(false);

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
