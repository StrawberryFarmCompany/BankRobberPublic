using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordUI : MonoBehaviour
{
    [HideInInspector] public int index;

    [Header("버튼 목록")]
    [Tooltip("(0~9 순서 지켜서 배열 삽입 필수)")]
    [SerializeField] Button[] numberButton;
    [SerializeField] Button clearButton;
    [SerializeField] Button enterButton;

    [Header("불빛 이미지 목록")]
    [SerializeField] Image deniedImage;
    [SerializeField] Sprite deniedSprite;
    [SerializeField] Image progressImage;
    [SerializeField] Sprite progressSprite;
    [SerializeField] Image acceptImage;
    [SerializeField] Sprite acceptSprite;
    [SerializeField] Sprite noneSprite;

    [Header("입력숫자 텍스트 목록")]
    [SerializeField] TextMeshProUGUI firstNumText;
    [SerializeField] TextMeshProUGUI secondNumText;
    [SerializeField] TextMeshProUGUI thirdNumText;
    [SerializeField] TextMeshProUGUI forthNumText;

    private List<int> inputNumbers = new List<int>();

    private void Awake()
    {
        UIManager.GetInstance.passwordUI = this;
        Init();
        Clear();
        gameObject.SetActive(false);
    }

    private void Init() //버튼 연결
    {
        for (int i = 0; i < numberButton.Length; i++)
        {
            int num = i; // 클로저 문제 방지
            numberButton[i].onClick.AddListener(() => InputNumber(num));
        }

        clearButton.onClick.AddListener(Clear);
        enterButton.onClick.AddListener(Enter);
    }


    public void Clear()
    {
        deniedImage.sprite = noneSprite;
        progressImage.sprite = progressSprite;
        acceptImage.sprite = noneSprite;
        firstNumText.text = "";
        secondNumText.text = "";
        thirdNumText.text = "";
        forthNumText.text = "";
        inputNumbers.Clear();
    }

    public void InputNumber(int num)
    {
        if (inputNumbers.Count == 0) // 첫 번째 숫자
        {
            inputNumbers.Add(num);
            firstNumText.text = num.ToString();
        }
        else if (inputNumbers.Count == 1) // 두 번째 숫자
        {
            inputNumbers.Add(num);
            secondNumText.text = num.ToString();
        }
        else if (inputNumbers.Count == 2) // 세 번째 숫자
        {
            inputNumbers.Add(num);
            thirdNumText.text = num.ToString();
        }
        else if (inputNumbers.Count == 3) // 네 번째 숫자
        {
            inputNumbers.Add(num);
            forthNumText.text= num.ToString();
        }
    }

    public void Enter()     //게임매니저에서 도어인덱스에 해당하는 패스워드를 비교해보기
    {
        int answer = inputNumbers[0]*1000 + inputNumbers[1]*100 + inputNumbers[2]*10 + inputNumbers[3];
        if (answer == GameManager.GetInstance.passwordDoorPair[index])  //비밀번호 정답
        {
            // 성공 시
            progressImage.sprite = noneSprite;
            acceptImage.sprite = acceptSprite;
            GameManager.GetInstance.isOpenPasswordDoor[index] = true;
        }
        else
        {
            // 실패 시 
            progressImage.sprite = noneSprite;
            deniedImage.sprite = deniedSprite;
        }
    }

}
