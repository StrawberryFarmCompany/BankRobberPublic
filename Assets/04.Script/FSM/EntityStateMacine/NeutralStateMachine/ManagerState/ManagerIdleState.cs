using UnityEngine;

public class ManagerIdleState : NeutralState
{

    public override void Enter()
    {
        Debug.Log("Manager 대기 상태");
    }

    public override void Execute()
    {
        // 플레이어 감지 로직 넣을 수도 있음
    }

    public override void Exit()
    {
        Debug.Log("Manager 대기 상태 종료");
    }


}
