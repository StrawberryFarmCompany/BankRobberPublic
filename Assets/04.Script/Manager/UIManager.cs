using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager GetInstance;
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private GameObject cancelPanel;

    public AutoPipBar pip;

    private void Awake()
    {
        GetInstance = this;
    }

    public void ShowActionPanel(bool show)
    {
        actionPanel.SetActive(show);
        cancelPanel.SetActive(!show);
    }
}
