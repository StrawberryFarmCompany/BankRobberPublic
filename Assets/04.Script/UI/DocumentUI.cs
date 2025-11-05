using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DocumentUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI documentText;
    public int index = 0;
    public bool isFirstTwoDigit;
    public DocumentType type;

    private void Awake()
    {
        UIManager.GetInstance.documentUI = this;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        switch (type)
        {
            case DocumentType.fake:
                documentText.text = $"별다른 단서를 찾기 힘들었다.";
                break;
            case DocumentType.button:
                documentText.text = $"금주의 금속탐지기 버튼 : {GameManager.GetInstance.GetButtonDoorNumber(index)}번";
                break;
            case DocumentType.password:
                int fullPassword = GameManager.GetInstance.passwordDoorPair[index];

                if (isFirstTwoDigit)
                {
                    int firstTwo = fullPassword / 100;
                    documentText.text = $"문의 비밀번호 앞 두 자리 : {firstTwo.ToString("D2")}번";
                }
                else
                {
                    int lastTwo = fullPassword % 100;
                    documentText.text = $"문의 비밀번호 뒤 두 자리 : {lastTwo.ToString("D2")}번";
                }
                break;
            default:
                break;
        }
        
    }
}
