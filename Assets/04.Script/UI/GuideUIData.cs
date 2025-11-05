using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GuideMissionType
{
    MoveToPosition,     // 특정 위치로 이동
    GetKeyCard,         // 특정 열쇠 획득
    OpenDoorWithButton,  // 올바른 버튼 눌러 문 열기
    OpenDoorWithPassword, // 올바른 패스워드 눌러 문 열기
    OpenDocument,       //문서 확인 시
    LastMission
}

[System.Serializable]
public class GuideListElement
{
    [Header("임무 종류")]
    public GuideMissionType missionType;

    [Header("설명 (UI 표시용)")]
    [TextArea]
    public string description;

    [Header("MoveToPosition 관련")]
    public Vector3 targetPosition;
    public float reachRange = 1.5f;

    [Header("GetKeyCard 관련")]
    public int keyCardIndex; // GameManager.GetInstance.isPlayerGetKeyCard[index] 참조

    [Header("OpenDoorWith 관련")]
    public int doorIndex;

    [Header("OpenDocument 관련")]
    public DocumentType documentType;
    public int documentIndex;
    

    public bool IsMissionComplete()
    {
        switch (missionType)
        {
            case GuideMissionType.MoveToPosition:
                if (NodePlayerManager.GetInstance.GetAllPlayers() == null) return false;
                float distance = Vector3.Distance(NodePlayerManager.GetInstance.GetCurrentPlayer().transform.position, targetPosition);
                return distance <= reachRange;

            case GuideMissionType.GetKeyCard:
                if (keyCardIndex < 0 || keyCardIndex >= GameManager.GetInstance.isPlayerGetKeyCard.Count)
                    return false;
                return GameManager.GetInstance.isPlayerGetKeyCard[keyCardIndex];

            case GuideMissionType.OpenDoorWithButton:
                var door = GameManager.GetInstance.alarmDoor[doorIndex];
                if (door == null) return false;
                return door.isOpen; // 문이 열렸는지 여부 (isOpen은 예시)
            case GuideMissionType.OpenDoorWithPassword:
                var isOpen = GameManager.GetInstance.isOpenPasswordDoor[doorIndex];
                if (!isOpen) return false;
                return isOpen;
            case GuideMissionType.OpenDocument:
                bool isSaw = true;
                if (documentType == DocumentType.button) isSaw = GameManager.GetInstance.buttonDocuments[documentIndex].isSaw;
                if (documentType == DocumentType.password) 
                    foreach (Document document in GameManager.GetInstance.passwordDocuments[documentIndex])
                        isSaw = isSaw && document.isSaw;
                return isSaw;

        }
        return false;
    }
}

[CreateAssetMenu(fileName = "GuideUIData", menuName = "GameData/GuideUIData")]
public class GuideUIData : ScriptableObject
{
    public List<GuideListElement> Elements = new List<GuideListElement>();
}
