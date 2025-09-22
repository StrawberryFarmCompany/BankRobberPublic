using NodeDefines;
using UnityEngine;

public class LobbyNPC : MonoBehaviour,IInteractable
{
    public Vector3Int tile { get; set; }

    public void OnInteraction()
    {
        Debug.Log("레스터와 대화 시작");
        // UI 열기, 대화창 띄우기 등
    }
    public void UnInteraction()
    {
        Debug.Log("레스터와 대화 종료");
        // UI 닫기, 대화창 닫기 등
    }
    public void RegistInteraction(Interaction interaction)
    {

    }

    public void ReleaseInteraction(Interaction interaction)
    {

    }
}
