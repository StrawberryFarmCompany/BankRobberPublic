using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextAsset csvFile;          // Unity에 연결할 CSV 파일
    private TextAsset currCsv;
    public TMP_Text dialogueText;      // 대사 출력할 UI 텍스트
    public string fullText;          // 전체 대사 텍스트 (타이핑 효과용)
    public TMP_Text nameText;          // NPC 이름 출력할 UI 텍스트
    public Transform choiceContainer;  // 선택지 버튼들을 담을 부모 오브젝트
    public Button choiceButtonPrefab;  // 선택지 버튼 프리팹

    public EventSystem eventSystem; // 이벤트 시스템 (선택지 버튼 클릭 시 사용)

    public Camera npcCamera; // NPC 대화용 카메라
    public Vector3 npcCameraOriginPosition; // NPC 대화용 카메라 원래 위치
    public Vector3 npcCameraZoomPosition; // NPC 대화용 카메라 확대 위치
    private bool isZoom = false; // 카메라 확대 여부

    public bool isDialogue = false;        // 현재 대화 중인지 여부
    public bool isLastLine = false;         // 현재 대화의 마지막 라인인지 여부
    public bool isTyping = false; // 현재 타이핑 중인지 여부
    public Coroutine typingCoroutine; // 타이핑 코루틴 참조 (중단 시 사용)

    public List<DialogueLine> dialogueData = new List<DialogueLine>(); // CSV에서 읽은 전체 대화 데이터 저장
    public int currentBranch = 1;     // 현재 분기 번호
    public int currentIndex = 1;      // 현재 인덱스 번호 (한 대화 묶음 내 순서)
    private bool isEqualBranchIndex = false; // 현재 분기와 인덱스가 같은지 여부
    private List<DialogueLine> currentLines = new List<DialogueLine>();
    private int currentLineIndex = 0;

    public LobbyInteraction interaction;
    public Animator npcAnimator; // NPC 애니메이터 (필요시 사용)

    public Dictionary<ENPC, (int branch, int index)> npcDialogueStates = new Dictionary<ENPC, (int, int)>();
    public ENPC npcDialogueStateKey;

    public NPC npc; // NPC 참조 (필요시 사용)

    public NPCLoadScene npcLoadScene; // NPC 로드 씬 스크립트 참조 (필요시 사용)
    private string sceneName;
    private bool isSceneLoaded = false; // 씬 로드 여부

    public DialogueLine transLine; // 위치 변경 시 사용할 대화 라인

    [HideInInspector]
    public bool isTransPos = false; // NPC 위치 변경 여부

    private PlayerController playerController;

    public SaveNPC saveNPC; // NPC 저장 스크립트 참조 (필요시 사용)
    public SceneType sceneType; // 씬 타입 (메인 씬, 퍼즐 씬 등)

    void Start()
    {
        gameObject.SetActive(false); // 초기에는 대화 UI 비활성화
        CharacterManager.Instance.player.dialogueManager = this;
        playerController = CharacterManager.Instance.player.controller;
        // 게임 시작 시 JSON 로드
        npcDialogueStates = NPCDialogueDataManager.Instance.LoadNpcStates();
        saveNPC.Load(sceneType);
    }
    void Update()
    {
        if (isZoom)
        {
            ZoomNpcCamera(); // 카메라 확대 중이면 확대 함수 호출
        }
    }

    public void LoadCSV()
    {
        if (csvFile == currCsv) return; // 이미 로드된 CSV 파일이면 중복 로드 방지
        currCsv = csvFile; // 현재 CSV 파일로 설정

        string[] lines = csvFile.text.Split('\n'); // CSV 파일을 줄 단위로 분리

        dialogueData.Clear();
        for (int i = 1; i < lines.Length; i++) // 첫 줄은 헤더라서 i=1부터 시작
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue; // 빈 줄은 건너뜀

            string[] cols = lines[i].Split(','); // 한 줄을 콤마로 분리해서 컬럼 배열로 저장

            // 폰트 크기 처리 (없으면 50)
            int fontSize = 50;
            if (cols.Length > 18 && !string.IsNullOrWhiteSpace(cols[18]))
                int.TryParse(cols[18], out fontSize);

            // 폰트 색상 처리 (없으면 흰색 255/255/255/255)
            Color fontColor = new Color32(255, 255, 255, 255);
            if (cols.Length > 19 && !string.IsNullOrWhiteSpace(cols[19]))
            {
                string[] rgba = cols[19].Split('/');
                if (rgba.Length == 4)
                {
                    byte r = byte.Parse(rgba[0]);
                    byte g = byte.Parse(rgba[1]);
                    byte b = byte.Parse(rgba[2]);
                    byte a = byte.Parse(rgba[3]);
                    fontColor = new Color32(r, g, b, a);
                }
            }

            DialogueLine line = new DialogueLine // CSV 한 줄을 DialogueLine 객체로 변환
            {
                branch = SafeParseInt(cols[0]),                                   // 분기 번호
                index = SafeParseInt(cols[1]),                               // 인덱스 번호
                npcName = cols.Length > 2 ? cols[2] : "",                      // NPC 이름
                text = cols.Length > 3 ? cols[3] : "",                            // 대사
                actions = string.IsNullOrWhiteSpace(cols[4]) ? null : cols[4].Split('/'), // 선택지 (없으면 null)
                nextBranches = string.IsNullOrWhiteSpace(cols[5]) ? null : System.Array.ConvertAll(cols[5].Split('/'), int.Parse), // 선택지별 다음 분기
                nextIndex = string.IsNullOrWhiteSpace(cols[6]) ? null : System.Array.ConvertAll(cols[6].Split('/'), int.Parse), // 선택지별 다음 인덱스
                branchTransition = SafeParseInt(cols.Length > 7 ? cols[7] : "0"), // 분기 전환
                indextransition = SafeParseInt(cols.Length > 8 ? cols[8] : "0"),       // 인덱스 전환
                questID = cols.Length > 9 ? cols[9] : "",                          // 퀘스트 ID (없으면 빈 문자열)
                questState = SafeParseInt(cols.Length > 10 ? cols[10] : ""),       // 퀘스트 상태 (0: 시작, 1: 완료, 2: 실패)
                rewardRandom = SafeParseInt(cols.Length > 11 ? cols[11] : "0"),
                rewardCategories = (cols.Length > 12 && !string.IsNullOrWhiteSpace(cols[12]))
              ? System.Array.ConvertAll(cols[12].Split('/'), x => SafeParseInt(x)) : null,
                rewardCounts = (cols.Length > 13 && !string.IsNullOrWhiteSpace(cols[13]))
              ? System.Array.ConvertAll(cols[13].Split('/'), x => SafeParseInt(x)) : null,
                shake = cols[14] == "1",                                        // 화면 진동 여부
                animation = SafeParseInt(cols[15]),                                    // 애니메이션 실행 여부
                zoom = cols[16] == "1",                                         // 카메라 확대 여부
                typingSpeed = float.Parse(cols[17]),                            // 타이핑 속도
                fontSize = fontSize,                                           // 폰트 크기
                fontColor = fontColor,                                          // 폰트 색상
                sceneName = cols.Length > 20 ? cols[20] : "",// 씬 이름 (추가된 컬럼)
                transPosObjects = (cols.Length > 21 && !string.IsNullOrWhiteSpace(cols[21]))
              ? System.Array.ConvertAll(cols[21].Split('/'), x => SafeParseInt(x)) : null,
                transPos = (cols.Length > 22 && !string.IsNullOrWhiteSpace(cols[22]))
              ? System.Array.ConvertAll(cols[22].Split('/'), x => SafeParseInt(x)) : null,
                destroyObjects = (cols.Length > 23 && !string.IsNullOrWhiteSpace(cols[23]))
              ? System.Array.ConvertAll(cols[23].Split('/'), x => SafeParseInt(x)) : null,
                permanentDestroy = cols[24] == "1",
                spawnObjects = (cols.Length > 25 && !string.IsNullOrWhiteSpace(cols[25]))
              ? System.Array.ConvertAll(cols[25].Split('/'), x => SafeParseInt(x)) : null,
                permanentSpawn = (cols.Length > 26 && !string.IsNullOrWhiteSpace(cols[26]))
              ? System.Array.ConvertAll(cols[26].Split('/'), x => (x == "1")) : null,
                spawnPos = (cols.Length > 27 && !string.IsNullOrWhiteSpace(cols[27]))
              ? System.Array.ConvertAll(cols[27].Split('/'), x => SafeParseInt(x)) : null
            };
            dialogueData.Add(line); // 완성된 DialogueLine을 리스트에 추가
        }
    }
    int SafeParseInt(string s, int defaultValue = 0)
    {
        int result;
        if (int.TryParse(s.Trim(), out result)) // Trim()으로 개행/공백 제거
            return result;
        return defaultValue;
    }

    public void ShowDialogue()
    {
        if (npcDialogueStates.ContainsKey(npcDialogueStateKey))
        {
            // NPC의 대화 상태가 이미 존재하면 해당 상태로 초기화
            currentBranch = npcDialogueStates[npcDialogueStateKey].branch;
            currentIndex = npcDialogueStates[npcDialogueStateKey].index;
        }
        else
        {
            // NPC의 대화 상태가 없으면 기본값으로 초기화
            npcDialogueStates[npcDialogueStateKey] = (1, 1);
            currentBranch = 1;
            currentIndex = 1;
        }
        int branch = npcDialogueStates[npcDialogueStateKey].branch; // NPC의 현재 분기
        int index = npcDialogueStates[npcDialogueStateKey].index; // NPC의 현재 인덱스
        // 현재 분기와 인덱스에 해당하는 모든 대사 라인 찾기
        currentLines = dialogueData.FindAll(d => d.branch == branch && d.index == index);
        currentLineIndex = 0;

        // 기존에 생성된 선택지 버튼들 삭제
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }

        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (currentLineIndex < currentLines.Count)
        {
            DialogueLine line = currentLines[currentLineIndex];
            nameText.text = line.npcName;
            fullText = line.text;
            dialogueText.fontSize = line.fontSize;
            dialogueText.color = line.fontColor;
            if (line.zoom)
            {
                isZoom = true; // 카메라 확대 활성화
            }
            else
            {
                isZoom = false; // 카메라 확대 비활성화
                npcCamera.transform.localPosition = npcCameraOriginPosition; // 원래 위치로 복원
            }
            if (line.shake)
            {
                StartCoroutine(ScreenShake()); // 화면 진동 효과
            }
            npcAnimator.SetInteger("actionValue", line.animation); // 애니메이션 상태 설정
            typingCoroutine = StartCoroutine(ShowTypingEffect(line.typingSpeed)); // 타이핑 효과로 대사 출력
            if (line.rewardCategories != null)
            {
                GiveRewards(line, npc); // 보상 지급
            }
            sceneName = line.sceneName.Trim(); // 씬 이름 저장
            if (npcLoadScene != null && sceneName != "")
            {
                isSceneLoaded = true; // 씬 로드 상태 설정
            }


            // 선택지가 있다면 버튼 생성
            foreach (Transform child in choiceContainer)
            {
                Destroy(child.gameObject);
            }
            if (line.actions != null && line.actions.Length > 0)
            {
                playerController.ToggleCursor();
                for (int i = 0; i < line.actions.Length; i++)
                {
                    int nextBranch = line.nextBranches[i];
                    int nextIndex = line.nextIndex[i];
                    Button btn = Instantiate(choiceButtonPrefab, choiceContainer);
                    if (i == 0) eventSystem.SetSelectedGameObject(btn.gameObject); // 첫 번째 버튼 선택
                    StartCoroutine(BtnInteractableDelay(btn)); // 버튼 활성화 딜레이
                    btn.GetComponentInChildren<TMP_Text>().text = line.actions[i];
                    btn.onClick.AddListener(() => OnChoiceSelected(nextBranch, nextIndex));
                }
            }

            HandleQuest(line); // 퀘스트 처리
            QuestManager.GetInstance.UpdateQuestProgressNPC(npcDialogueStateKey, line.branch, line.index); // 퀘스트 완료 체크
            DestroyNPCObjects(line); // NPC 오브젝트 파괴 처리
            SpawnObjects(line); // NPC 오브젝트 생성 처리
            if (line.transPosObjects != null || line.transPos != null)
            {
                isTransPos = true; // 위치 변경 활성화
                transLine = line;
            }


            currentLineIndex++;
        }
        if (currentLineIndex == currentLines.Count)
        {
            if (currentLines[currentLineIndex - 1].actions == null)
            {
                isLastLine = true;
            }
            if (currentLines[currentLines.Count - 1].branchTransition != 0)
            {
                currentBranch = currentLines[currentLines.Count - 1].branchTransition; // 분기 전환
            }
            if (currentLines[currentLines.Count - 1].indextransition != 0)
            {
                currentIndex = currentLines[currentLines.Count - 1].indextransition; // 인덱스 전환
            }
        }
        UpdateNpcState(); // NPC 상태 업데이트
                          // 모든 대사 출력 후 추가 처리 필요시 여기에 작성

    }

    public void OnChoiceSelected(int nextBranch, int nextIndex)
    {
        playerController.ToggleCursor(); // 커서 토글
        if (currentBranch == nextBranch && currentIndex == nextIndex)
        {
            // 현재 분기와 인덱스가 같으면 대화 종료 즉, 더 생각한다는 선택지
            ResetDialogueUI();
            return;
        }
        currentBranch = nextBranch;  // 다음 분기로 변경
        currentIndex = nextIndex;            // 새로운 분기 시작 시 인덱스 초기화
        UpdateNpcState();
        ShowDialogue(); // 새로운 분기 대사 출력
    }

    IEnumerator ScreenShake()
    {
        Vector3 originalPosition = npcCamera.transform.position; // 원래 위치 저장
        float shakeDuration = 0.5f; // 진동 지속 시간
        float shakeMagnitude = 0.1f; // 진동 강도
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            npcCamera.transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }
        npcCamera.transform.position = originalPosition; // 원래 위치로 복원

    }

    IEnumerator ShowTypingEffect(float typingSpeed)
    {
        isTyping = true; // 타이핑 시작
        dialogueText.text = ""; // 처음엔 빈 문자열
        foreach (char c in fullText)
        {
            if (!isTyping) yield break; // 타이핑 중지 시 코루틴 종료
            dialogueText.text += c;  // 한 글자씩 추가
            yield return new WaitForSeconds(0.02f / typingSpeed); // 출력 속도 조절
        }
        isTyping = false; // 타이핑 완료
    }
    IEnumerator BtnInteractableDelay(Button btn)
    {
        yield return new WaitForSeconds(0.5f); // 딜레이 시간 대기
        btn.interactable = true; // 버튼 활성화
    }
    void ZoomNpcCamera()
    {
        Vector3 currentPos = npcCamera.transform.localPosition;
        Vector3 targetPos = npcCameraZoomPosition;
        npcCamera.transform.localPosition = Vector3.Lerp(currentPos, npcCameraZoomPosition, 5 * Time.deltaTime);
    }
    public void ResetDialogueUI()
    {
        gameObject.SetActive(false);
        isLastLine = false;
        isDialogue = false;
        npcCamera.gameObject.SetActive(false);
        npcAnimator.SetInteger("actionValue", 0);
        interaction.EnableActions();
        currentLines.Clear();
        currentLineIndex = 0;
        csvFile = null;
        currentBranch = 1;
        currentIndex = 1;
        dialogueText.text = "";
        nameText.text = "";
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine); // 타이핑 코루틴 중지
            typingCoroutine = null;
        }
        isTyping = false;
    }
    private void UpdateNpcState()
    {
        npcDialogueStates[npcDialogueStateKey] = (currentBranch, currentIndex);
        NPCDialogueDataManager.Instance.SaveNpcStates(npcDialogueStates); // 상태 업데이트 시 저장
    }
    // 보상 지급 함수
    public void GiveRewards(DialogueLine line, NPC npc)
    {
        if (line.rewardCategories == null || line.rewardCounts == null) return;

        List<int> selectedCategories = new List<int>();

        if (line.rewardRandom == 0)
        {
            // 모든 카테고리 선택
            selectedCategories.AddRange(line.rewardCategories);
        }
        else
        {
            // 랜덤으로 rewardRandom개 선택
            List<int> pool = new List<int>(line.rewardCategories);
            for (int i = 0; i < line.rewardRandom && pool.Count > 0; i++)
            {
                int idx = Random.Range(0, pool.Count);
                selectedCategories.Add(pool[idx]);
                pool.RemoveAt(idx);
            }
        }

        // 실제 보상 지급
        for (int i = 0; i < line.rewardCategories.Length; i++)
        {
            int category = line.rewardCategories[i];
            int count = line.rewardCounts[i];

            if (selectedCategories.Contains(category))
            {
                if (category >= 0 && category < npc.npcRewards.Length)
                {
                    GameObject rewardPrefab = npc.npcRewards[category];
                    for (int j = 0; j < count; j++)
                    {
                        GameObject.Instantiate(rewardPrefab, npc.transform.position + Vector3.up, Quaternion.identity);
                    }
                }
            }
        }
    }
    private void OnDisable()
    {
        if (isSceneLoaded)
        {
            Debug.Log($"씬 로딩: {sceneName}");
            isSceneLoaded = false; // 대화 UI가 비활성화되면 씬 로드 상태 초기화
            npcLoadScene.LoadScene(sceneName); // NPC 로드 씬 스크립트로 씬 로드
        }
    }

    private void HandleQuest(DialogueLine line)
    {
        if (string.IsNullOrEmpty(line.questID)) return;

        QuestData quest = Resources.Load<QuestData>("QuestData/" + "Quest" + line.questID);
        if (quest == null)
        {
            Debug.LogWarning($"퀘스트 데이터가 없습니다: Quest{line.questID}");
            return;
        }

        switch (line.questState)
        {
            case 0:
                QuestManager.GetInstance.AcceptQuest(quest);
                break;
            case 1:
                QuestManager.GetInstance.CompleteQuest(quest);
                break;
            case 2:
                QuestManager.GetInstance.FailQuest(quest);
                break;
        }
    }

    public void TransPosNPC(DialogueLine line)
    {
        if (line.transPosObjects == null || line.transPos == null || line.transPosObjects.Length != line.transPos.Length)
        {
            Debug.LogWarning("대화 라인에 위치 변경 정보가 올바르지 않습니다.");
            return;
        }
        for (int i = 0; i < line.transPosObjects.Length; i++)
        {
            int objectIndex = line.transPosObjects[i];
            int newPositionIndex = line.transPos[i];
            GameObject transPosObj = npc.gameObjects[objectIndex];
            Vector3 transPos = npc.npcPos[newPositionIndex];
            transPosObj.transform.position = transPos;
        }
    }

    public void DestroyNPCObjects(DialogueLine line)
    {
        if (line.destroyObjects == null || line.destroyObjects.Length == 0) return;
        foreach (int index in line.destroyObjects)
        {
            if (index >= 0 && index < npc.gameObjects.Length)
            {
                if (line.permanentDestroy)
                {
                    saveNPC.Remove(sceneType, npc.gameObjects[index]);
                }
                else Destroy(npc.gameObjects[index]);
            }
            else
            {
                Debug.LogWarning($"파괴할 오브젝트 인덱스가 범위를 벗어났습니다: {index}");
            }
        }
    }

    public void SpawnObjects(DialogueLine line)
    {
        if (line.spawnObjects == null || line.spawnPos == null || line.spawnObjects.Length != line.spawnPos.Length)
        {
            Debug.LogWarning("대화 라인에 생성 정보가 올바르지 않습니다.");
            return;
        }
        for (int i = 0; i < line.spawnObjects.Length; i++)
        {
            int objectIndex = line.spawnObjects[i];
            int spawnPositionIndex = line.spawnPos[i];
            GameObject spawnObj = npc.gameObjects[objectIndex];
            Vector3 spawnPos = npc.npcPos[spawnPositionIndex];
            GameObject spawnNPC = Instantiate(spawnObj, spawnPos, Quaternion.identity);
            Debug.Log(line.permanentSpawn[i]);
            if (line.permanentSpawn[i])
            {
                saveNPC.Save(spawnNPC, sceneType); // 영구적으로 저장
            }
        }
    }
}
