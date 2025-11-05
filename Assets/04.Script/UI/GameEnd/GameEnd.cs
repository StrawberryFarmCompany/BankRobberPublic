using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform resultParent;
    [SerializeField] private GameObject resultPrefab;
    [SerializeField] private GameObject failImage;
    [SerializeField] private GameObject successImage;
    [SerializeField] private GameObject perfectImage;
    [SerializeField] private Button LobbyButton;

    [SerializeField] private TextMeshProUGUI revenueText;
    [SerializeField] private TextMeshProUGUI projectCostText;
    [SerializeField] private TextMeshProUGUI arrestCostText;
    [SerializeField] private TextMeshProUGUI benefitText;


    //public List<CharacterResult> characterResults = new List<CharacterResult>();

    private CharacterResult bishopResult;
    private CharacterResult rookResult;
    private CharacterResult knightResult;

    private void Awake()
    {
        LobbyButton.onClick.AddListener(LoadLobby);
    }

    private void Start()
    {
        failImage.SetActive(false);
        successImage.SetActive(false);
        perfectImage.SetActive(false);
    }

    public void TurnOffPanel()
    {
        panel.SetActive(false);
    }

    public void TurnOnPanel()
    {
        SetBenefitResult();
        panel.SetActive(true);
    }

    /// <summary>
    /// 캐릭터 결과창 생성
    /// </summary>
    /// <param name="stats"></param>
    public void CreateCharacterUI(EntityStats stats)
    {
        GameObject characterResult = Instantiate(resultPrefab, resultParent);
        CharacterResult cResult = characterResult.GetComponent<CharacterResult>();

        switch (stats.characterType)
        {
            case CharacterType.Bishop:
                cResult.typeText.text = "비숍";
                bishopResult = cResult;
                break;
            case CharacterType.Rook:
                cResult.typeText.text = "룩";
                rookResult = cResult;
                break;
            case CharacterType.Knight:
                cResult.typeText.text = "나이트";
                knightResult = cResult;
                break;
            default:
                break;
        }

        cResult.portrait.sprite = stats.portrait;
        cResult.characterType = stats.characterType;
        cResult.resultText.text = "이벤트 발생 전";
    }

    public void SetDeadCharacter(EntityStats stats)
    {
        Color color = Color.red;
        switch (stats.characterType) 
        {
            case CharacterType.Bishop:
                bishopResult.resultText.text = "체포됨";
                bishopResult.resultText.color = color;
                break;
            case CharacterType.Rook:
                rookResult.resultText.text = "체포됨";
                rookResult.resultText.color = color;
                break;
            case CharacterType.Knight:
                knightResult.resultText.text = "체포됨";
                knightResult.resultText.color = color;
                break;
            default:
                break;
        }
    }

    public void SetEscapeCharacter(EntityStats stats) 
    {
        switch (stats.characterType)
        {
            case CharacterType.Bishop:
                bishopResult.resultText.text = "탈출 성공";
                break;
            case CharacterType.Rook:
                rookResult.resultText.text = "탈출 성공";
                break;
            case CharacterType.Knight:
                knightResult.resultText.text = "탈출 성공";
                break;
            default:
                break;
        }
    }

    public void SetFail()
    {
        failImage.SetActive(true);
    }

    public void SetSuccess()
    {
        successImage.SetActive(true);
    }

    public void SetPerfect()
    {
        perfectImage.SetActive(true);
    }

    private void LoadLobby()
    {
        GameManager.GetInstance.Reset();
        LoadSceneManager.GetInstance.SceneLoad(SceneType.LobbyScene);
    }

    private void SetBenefitResult()
    {
        int totalBenefit = GameManager.GetInstance.GetBenefitResult();

        revenueText.text = $"$ {GameManager.GetInstance.GatheredGold}";
        projectCostText.text = $"$ -{GameManager.GetInstance.GetProjectCost()}";
        arrestCostText.text = $"$ -{GameManager.GetInstance.GatheredCost}";
        benefitText.text = totalBenefit >= 0 ? $"$ {totalBenefit}" : $"$ {totalBenefit}";
    }
}
