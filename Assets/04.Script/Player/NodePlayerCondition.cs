using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NodePlayerCondition : MonoBehaviour
{
    public NodePlayerController nodePlayerController;
    public PlayerStats playerStats;


    [SerializeField] private EntityData playerData; // 플레이어 데이터 (인스펙터에 할당)

    public int moveRange;        //이동 범위

    private void Awake()
    {
        playerStats = new PlayerStats(playerData);

        moveRange = 2; // 기본 이동 범위 설정
    }


    public bool ConsumeActionPoint(int amount)
    {
        if (playerStats.curActionPoint >= amount)
        {
            playerStats.curActionPoint -= amount;
            return true; // 행동 성공
        }
        return false; // 행동 실패, 행동력이 부족함
    }

    public bool ConsumeMovement(int amount)
    {
        if (playerStats.movement >= amount)
        {
            playerStats.movement -= amount;
            return true; // 이동 성공
        }
        return false; // 이동 실패, 이동력이 부족함
    }

    public void ActiveRun()
    {
        if(ConsumeActionPoint(1)) playerStats.movement += playerStats.movementSpeed; // 달리기 활성화 시 이동력 증가
    }

    public void Damaged(int damage)
    {
        playerStats.curHp -= damage;
        if (playerStats.curHp <= 0)
        {
            playerStats.curHp = 0;
            Dead();
        }
    }

    private void Dead()
    {
        //GameManager.GetInstance.사망으로 인해 발생할 게임내 상황을 정의
        Destroy(this.gameObject);
    }

    public void ResetForNewTurn()
    {
        playerStats.curActionPoint = playerStats.actionPoint;
        playerStats.movement = playerStats.movementSpeed;
    }

}
