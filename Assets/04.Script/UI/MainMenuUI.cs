using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI 오브젝트")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject optionUI;

    [Header("메인 UI 버튼")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;

    [Header("옵션 UI 버튼")]
    [SerializeField] private Button backButton;

    [Header("종료 확인 UI")]
    [SerializeField] private GameObject qutiConfirmUI;
    [SerializeField] private Button quitYesButton;
    [SerializeField] private Button quitNoButton;

    private void Start()
    {
        mainUI.SetActive(true);
        optionUI.SetActive(false);
        qutiConfirmUI.SetActive(false);

        newGameButton.onClick.AddListener(OnClickNewGame);
        continueButton.onClick.AddListener(OnClickContinue);
        optionButton.onClick.AddListener(OnClickOption);
        exitButton.onClick.AddListener(OnClickExit);
        backButton.onClick.AddListener(OnClickBack);

        quitYesButton.onClick.AddListener(DoQuit);
        quitNoButton.onClick.AddListener(CloseExitConfirm);
    }

    private void OnClickNewGame()
    {
        SceneManager.LoadScene("LobbyPlayerChange");
    }

    private void OnClickContinue()
    {
        //저장 정보 가져오기
        SceneManager.LoadScene("LobbyPlayerChange");
    }

    private void OnClickOption()
    {
        mainUI.SetActive(false);
        optionUI.SetActive(true);
    }

    private void OnClickBack()
    {
        optionUI.SetActive(false);
        mainUI.SetActive(true);
    }

    private void OnClickExit()
    {
        //종료 확인창 오픈
        qutiConfirmUI.SetActive(true);
    }

    private void CloseExitConfirm()
    {
        if (qutiConfirmUI) qutiConfirmUI.SetActive(false);
    }

    private void DoQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
