using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    [SerializeField] Animator animator;

    private AnimationStateMachine stateMachine;

    // 상태들
    private IdleState idleState;
    private WalkState walkState;
    private RunState runState;  
    private MeleeState attackState;
    private DamagedState damagedState;
    private DeadState deadState;
    private InteractionState InteractionState;
    private RangeAttackState rangeAttackState;


    void Start()
    {
        stateMachine = new AnimationStateMachine();

        // 상태 인스턴스 생성
        idleState = new IdleState(animator);
        walkState = new WalkState(animator);

        // 처음 상태는 Idle
        stateMachine.ForceSet(idleState);
    }

    void Update()
    {
        stateMachine.Update();

        // 입력 조건에 따라 상태 전환
        float moveInput = Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical"));

        if (moveInput > 0)
            stateMachine.ChangeState(walkState);
        else
            stateMachine.ChangeState(idleState);
    }
}
