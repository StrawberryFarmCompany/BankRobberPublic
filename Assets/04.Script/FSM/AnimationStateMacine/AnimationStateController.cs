using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private static readonly int isRifle = Animator.StringToHash("isRifle");
    private static readonly int equip = Animator.StringToHash("Equip");
    public static readonly int unEquip = Animator.StringToHash("UnEquip");
    public static readonly int isIdle = Animator.StringToHash("isIdle");

    [Header("애니메이터")]
    [SerializeField] Animator animator;

    [Header("총기류")]
    public GameObject rifle;
    public GameObject sniperRifle;
    public GameObject shotGun;
    public GameObject subMachineGun;
    public GameObject handGun;

    [Header("총기 부착 위치")]
    public Transform gunHoldPosition;

    public GameObject currentGun;
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
    private StrafeState strafeState;
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
        hipRangedAttackState = new HipRangedAttackState(animator, gun);
        idleState = new IdleState(animator);
        interactionState = new InteractionState(animator);
        if(playerController != null)
            moveState = new MoveState(animator, playerController);
        else
            moveState = new MoveState(animator);
        reloadState = new ReloadState(animator);
        runState = new RunState(animator);
        sneakAttackState = new SneakState(animator);
        strafeState = new StrafeState(animator);
        throwState = new ThrowState(animator);

        if (playerController != null)
            playerController.animationController = this;

        if (gun != null)
        {
            switch (gun.type)
            {
                case GunType.HandGun:
                    currentGun = Instantiate(handGun, gunHoldPosition);
                    break;
                case GunType.AssaultRifle:
                    currentGun = Instantiate(rifle, gunHoldPosition);
                    break;
                case GunType.SniperRifle:
                    currentGun = Instantiate(sniperRifle, gunHoldPosition);
                    break;
                case GunType.ShotGun:
                    currentGun = Instantiate(shotGun, gunHoldPosition);
                    break;
                case GunType.SubMachineGun:
                    currentGun = Instantiate(subMachineGun, gunHoldPosition);
                    break;
                default:
                    currentGun = null;
                    break;
            }
        }
            
    }

    public void OnEquip()
    {
        animator.Play(equip);
    }
    public void OnEquipGun()
    {
        currentGun.SetActive(true);
    }
    public void OnUnEquipGun()
    {
        currentGun.SetActive(false);
    }

    public void AimingState()
    {
        stateMachine.ChangeState(aimingState);
    }
    public void AimRangedAttackState(Vector3 target)
    {
        RotateTowards(target);
        stateMachine.ChangeState(aimRangedAttackState);
    }
    public void DamagedState()
    {
        stateMachine.ChangeState(damagedState);
    }
    public void DeadState()
    {
        stateMachine.ChangeState(deadState);
    }
    public void HideState()
    {
        stateMachine.ChangeState(hideState);
    }
    public void HipRangedAttackState(Vector3 target)
    {
        RotateTowards(target);
        stateMachine.ChangeState(hipRangedAttackState);
    }
    public void IdleState()
    {
        stateMachine.ChangeState(idleState);
    }
    public void InteractionState()
    {
        stateMachine.ChangeState(interactionState);
    }
    public void MoveState()
    {
        stateMachine.ChangeState(moveState);
    }
    public void ReloadState()
    {
        stateMachine.ChangeState(reloadState);
    }
    public void RunState()
    {
        currentGun.SetActive(false);
        stateMachine.ChangeState(runState);
    }
    public void SneakAttackState()
    {
        stateMachine.ChangeState(sneakAttackState);
    }
    public void StrafeState()
    {
        stateMachine.ChangeState(strafeState);
    }
    public void ThrowState()
    {
        stateMachine.ChangeState(throwState);
    }

    private void RotateTowards(Vector3 target)
    {
        Transform body = playerController != null ? playerController.transform : transform;
        Vector3 direction = target - body.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            body.rotation = targetRotation;
        }
    }


}
