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
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;

    [Header("옵션 UI 버튼")]
    [SerializeField] private Button backButton;

    private void Start()
    {
        mainUI.SetActive(true);
        optionUI.SetActive(false);

        startButton.onClick.AddListener(OnClickStart);
        optionButton.onClick.AddListener(OnClickOption);
        exitButton.onClick.AddListener(OnClickExit);
        backButton.onClick.AddListener(OnClickBack);
    }

    private void OnClickStart()
    {
        Debug.Log("게임 시작 버튼 클릭됨");
        //SceneManager.LoadScene("d"); 나중에 게임씬 이름 넣기
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
