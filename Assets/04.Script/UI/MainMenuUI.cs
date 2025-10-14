using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private void Start()
    {
        mainUI.SetActive(true);
        optionUI.SetActive(false);

        newGameButton.onClick.AddListener(OnClickNewGame);
        continueButton.onClick.AddListener(OnClickContinue);
        optionButton.onClick.AddListener(OnClickOption);
        exitButton.onClick.AddListener(OnClickExit);
        backButton.onClick.AddListener(OnClickBack);
    }

    private void OnClickNewGame()
    {
        Debug.Log("새로하기 버튼 클릭됨");
        //SceneManager.LoadScene("");
    }

    private void OnClickContinue()
    {
        Debug.Log("이어하기 버튼 클릭됨");
        //SceneManager.LoadScene("");
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
        Debug.Log("게임 종료");
    }
}
