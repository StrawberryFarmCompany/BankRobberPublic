using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private static readonly int isRifle = Animator.StringToHash("isRifle");
    private static readonly int equip = Animator.StringToHash("Equip");
    public static readonly int unEquip = Animator.StringToHash("UnEquip");

    [SerializeField] Animator animator;
    private NodePlayerController playerController;
    private Gun gun;

    private AnimationStateMachine stateMachine;

    // 상태들
    private AimingState aimingState;
    private AimRangedAttackState aimRangedAttackState;
    private DamagedState damagedState;
    private DeadState deadState;
    private HideState hideState;
    private HipRangedAttackState hipRangedAttackState;
    private IdleState idleState;
    private InteractionState interactionState;
    private MoveState moveState;
    private ReloadState reloadState;
    private RunState runState;  
    private SneakState sneakAttackState;
    private ThrowState throwState;


    void Start()
    {
        Init();

        // 처음 상태는 Idle
        stateMachine.ForceSet(idleState);
    }

    void Update()
    {
        stateMachine.Update();
        if(playerController != null) // 플레이어 애니메이션 부분
        {
            if (playerController.isMoving)
                 stateMachine.ChangeState(moveState);
            else
                stateMachine.ChangeState(idleState);

        }
        else                        // NPC 애니메이션 부분
        {

        }
    }

    void Init()
    {
        animator.applyRootMotion = false;
        playerController = GetComponentInParent<NodePlayerController>();
        gun = GetComponentInParent<Gun>();
        // 상태 머신 초기화
        stateMachine = new AnimationStateMachine();

        if (gun != null)
            animator.SetBool(isRifle, gun.type == GunType.HandGun ? false : true);

        // 상태 인스턴스 생성
        aimingState = new AimingState(animator);
        aimRangedAttackState = new AimRangedAttackState(animator, gun);
        damagedState = new DamagedState(animator);
        deadState = new DeadState(animator);
        hideState = new HideState(animator);
        hipRangedAttackState = new HipRangedAttackState(animator);
        idleState = new IdleState(animator);
        interactionState = new InteractionState(animator);
        moveState = new MoveState(animator);
        reloadState = new ReloadState(animator);
        runState = new RunState(animator);
        sneakAttackState = new SneakState(animator);
        throwState = new ThrowState(animator);
    }

    public void OnEquip()
    {
        animator.Play(equip);
    }
}
