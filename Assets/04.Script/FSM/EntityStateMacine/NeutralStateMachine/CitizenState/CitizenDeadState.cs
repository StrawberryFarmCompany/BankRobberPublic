using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenDeadState : NeutralState
{
    public NeutralNPC citizen;

    public CitizenDeadState(NeutralNPC citizen)
    {
        this.citizen = citizen;
    }

    public override void Enter()
    {
        //Destroy 쓰면 바로 삭제 되니 애니메이션 먼저 실행되게 해주기
        GameObject.Destroy(citizen.gameObject);
    }
}
