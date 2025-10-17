using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyInteraction : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;

    public GameObject curInteractGameObject;
    private INPCInteractable curInteractable;
    private PlayerInput playerInput;

    public TextMeshProUGUI promptText;
    private Camera camera;
    public GameObject npcObject;

    private DialogueManager dialogueManager;

    void Start()
    {
        camera = Camera.main;
        dialogueManager = CharacterManager.Instance.player.dialogueManager;
        playerInput = CharacterManager.Instance.player.playerInput;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<INPCInteractable>();
                    SetPromptText();
                }
            }
            else
            {
                curInteractGameObject = null;
                curInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = curInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract();
            curInteractGameObject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);

        }
        if (context.phase == InputActionPhase.Started && dialogueManager.gameObject.activeSelf)
        {
            DisableActions();
            if (dialogueManager.isDialogue)
            {
                if (dialogueManager.isTyping)
                {
                    dialogueManager.isTyping = false; // 타이핑 중지
                    dialogueManager.dialogueText.text = dialogueManager.fullText; // 현재 대사 전체 표시
                    return;
                }
                if (dialogueManager.isLastLine)
                {
                    Debug.Log("대화 종료");
                    dialogueManager.gameObject.SetActive(false);
                    dialogueManager.isLastLine = false;
                    dialogueManager.isDialogue = false;
                    if (dialogueManager.npcCamera != null) dialogueManager.npcCamera.gameObject.SetActive(false); // NPC 대화용 카메라 비활성화
                    if (dialogueManager.npcAnimator != null) dialogueManager.npcAnimator.SetInteger("actionValue", 0); // NPC 애니메이터 대화 상태 해제
                    dialogueManager.csvFile = null; // CSV 파일 초기화
                    dialogueManager.currentBranch = 1; // 현재 브랜치 초기화
                    dialogueManager.currentIndex = 1; // 현재 인덱스 초기화
                    dialogueManager.dialogueText.text = "";
                    dialogueManager.nameText.text = "";
                    if (dialogueManager.isTransPos)
                    {
                        dialogueManager.isTransPos = false;
                        dialogueManager.TransPosNPC(dialogueManager.transLine);
                    }
                    EnableActions();
                    return;
                }
                dialogueManager.ShowNextLine();
                return;
            }
            dialogueManager.ShowDialogue();
            dialogueManager.isDialogue = true;
        }
        
    }
    public void DisableActions()
    {
        foreach (var action in playerInput.actions)
        {
            if (action.name == "Interaction")
                continue; // Interact액션은 제외
            action.Disable();
        }
    }
    public void EnableActions()
    {
        foreach (var action in playerInput.actions)
        {
            if (action.name == "Interaction")
                continue; // Interact액션은 제외
            action.Enable();
        }
    }
}