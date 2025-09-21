using UnityEngine;

public class ManagerIdleState : NeutralState
{
    public ManagerNPC managerNPC;

    public ManagerIdleState(ManagerNPC managerNPC)
    {
        this.managerNPC = managerNPC;
    }

    public override void Enter()
    {
        Debug.Log("Manager 대기 상태");
    }

    // 플레이어 발견시 상태 변경
    public override void Execute()
    {
        managerNPC.OnPlayerDetected();
    }

    public override void Exit()
    {
        Debug.Log("Manager 대기 상태 종료");
    }
}
