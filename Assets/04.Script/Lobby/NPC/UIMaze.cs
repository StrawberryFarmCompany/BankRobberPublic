//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class UIMaze : MonoBehaviour
//{
//    [Header("UI 요소")]
//    public TextMeshProUGUI timeText; // 시간 표시 텍스트

//    [Header("초기 시간")]
//    public float initialTime = 60f; // 초기 시간 설정 (초 단위)

//    private float currentTime; // 현재 시간

//    [Header("NPC 설정")]
//    public ENPC npcKey; // NPC 키

//    [Header("실패 시 NPC 분기 인덱스 변화")]
//    public int branch; // NPC 분기
//    public int index; // NPC 인덱스

//    private Dictionary<ENPC, (int branch, int index)> npcDictionary; // NPC 키와 NPC 객체를 매핑하는 딕셔너리
//    private bool isGameSet = false; // 게임 설정 여부



//    void Start()
//    {
//        // 초기 시간 설정
//        currentTime = initialTime;

//        npcDictionary = NPCDialogueDataManager.Instance.LoadNpcStates(); // NPC 딕셔너리 가져오기

//    }

//    private void Update()
//    {
//        currentTime -= Time.deltaTime; // 매 프레임마다 시간 감소
//        timeText.text = "Time: " + currentTime.ToString("F2"); // 현재 시간을 초 단위로 표시

//        //승리로직
//        if (QuestManager.Instance.GetQuestStatus("0") == QuestStatus.CanComplete)
//        {
//            Debug.Log("퀘스트 완료: MazeQuest");
//            gameObject.SetActive(false); // UI 비활성화
//            return; // 더 이상 시간 감소를 하지 않음
//        }

//        if (currentTime <= 0 && !isGameSet)
//        {
//            currentTime = 0; // 시간이 0 이하로 떨어지면 0으로 설정
//            isGameSet = true; // 게임 설정 완료
//            OnTimeUp(); // 시간 초과 시 처리
//            gameObject.SetActive(false); // UI 비활성화
//        }
//    }

//    void OnTimeUp()
//    {

//        Debug.Log("Time is up!");
//        // NPC 딕셔너리에서 현재 NPC 키에 해당하는 분기와 인덱스를 가져옴
//        if (npcDictionary.TryGetValue(npcKey, out var npcState))
//        {
//            npcDictionary[npcKey] = (branch, index); // NPC 상태 업데이트)
//            // NPC의 분기와 인덱스를 업데이트
//            npcState.branch = branch;
//            npcState.index = index;
//            // NPC 상태를 저장
//            NPCDialogueDataManager.Instance.SaveNpcStates(npcDictionary);
//            Debug.Log($"NPC {npcKey} 상태 업데이트 → Branch: {branch}, Index: {index}");
//        }
//        else
//        {
//            Debug.LogWarning("NPC key not found in dictionary: " + npcKey);
//        }
//    }
//}
