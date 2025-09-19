using UnityEngine;

public class ManagerDeadState : NeutralState
{
    public override void Enter()
    {
        Debug.Log("Manager 죽음");
        // 애니메이션, 파티클, Disable 처리 등
    }

    public override void Execute()
    {
        // 죽은 상태에서는 아무 것도 안 함
    }

    public override void Exit()
    {
        // 필요하다면 나갈 때 처리
    }
}
