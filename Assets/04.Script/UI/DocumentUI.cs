using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DocumentUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI documentText;
    public int index = 0;

    private void Awake()
    {
        UIManager.GetInstance.documentUI = this;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        documentText.text = $"금주의 금속탐지기 번호 : {GameManager.GetInstance.GetButtonDoorNumber(index)}번";
    }
}
