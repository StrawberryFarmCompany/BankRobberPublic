using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENPC
{
    Amy,
    Knight,
    Seonghan
}

public class NPC : MonoBehaviour, INPCInteractable
{
    [Header("NPC 대화 CSV")]
    public TextAsset csvDialogueFile; // CSV 파일을 연결할 변수
    public string npcName; // NPC 이름
    public string npcDescription; // NPC 설명
    public Camera npcCamera; // NPC 대화용 카메라
    public Vector3 npcCameraOriginPosition; // NPC 대화용 카메라 위치
    public Vector3 npcCameraZoomPosition; // NPC 대화용 카메라 확대 위치
    public Animator npcAnimator; // NPC 애니메이터 (필요시 사용)
    public GameObject[] npcRewards; // NPC 보상 오브젝트 (필요시 사용)
    public ENPC npcKey;

    [Header("NPC 상태 변화")]
    public GameObject[] gameObjects; // NPC 상태 변화에 따른 게임 오브젝트 배열
    public Vector3[] npcPos;

    private DialogueManager dialogueManager; // DialogueManager 참조

    private void Start()
    {
        dialogueManager = CharacterManager.Instance.player.dialogueManager;
        npcCamera.gameObject.SetActive(false); // NPC 대화용 카메라 비활성화
    }

    public string GetInteractPrompt()
    {
        string str = $"{npcDescription}\n{npcName}";
        return str;
    }

    public void OnInteract()
    {
        dialogueManager.gameObject.SetActive(true); // 대화 UI 활성화
        dialogueManager.csvFile = csvDialogueFile; // DialogueManager에 CSV 파일 설정Z
        dialogueManager.LoadCSV(); // CSV 파일 로드
        npcCamera.gameObject.SetActive(true); // NPC 대화용 카메라 활성화
        dialogueManager.npcCamera = npcCamera; // NPC 대화용 카메라 설정
        dialogueManager.npcAnimator = npcAnimator; // NPC 애니메이터 설정
        dialogueManager.npcCameraOriginPosition = npcCameraOriginPosition; // NPC 카메라 원래 위치 설정
        dialogueManager.npcCameraZoomPosition = npcCameraZoomPosition; // NPC 카메라 확대 위치 설정
        dialogueManager.npcDialogueStateKey = npcKey; // NPC 대화 상태 키 설정
        dialogueManager.npc = this; // NPC 설정                                                           그냥 npc.~~~ 로 다 리팩터링 해버리자
    }

}
