using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSetter : MonoBehaviour
{
    public TrapType trapType;
    [SerializeField] Vector3Int targetPos;

    [Header("LateTeleportTrap 전용")]
    [Tooltip("몇 턴 후에 텔레포트 시킬지 설정")]
    public int turnWait; // LateTeleportTrap용


    private void Start()
    {
        switch(trapType)
        {
            case TrapType.TeleportTrap:
                SetTeleportTrap();
                break;
            case TrapType.LateTeleportTrap:
                SetLateTeleportTrap();
                break;
        }
    }

    private void SetTeleportTrap()
    {
        Vector3Int pos;
        pos = GameManager.GetInstance.GetVecInt(transform.position);

        TeleportTrap trap = new TeleportTrap(pos, targetPos);
        trap.Init();

        Destroy(this);
    }

    private void SetLateTeleportTrap()
    {
        Vector3Int pos;
        pos = GameManager.GetInstance.GetVecInt(transform.position);

        LateTeleportTrap trap = new LateTeleportTrap(pos, targetPos, turnWait);
        trap.Init();

        Destroy(this);
    }
}



public enum TrapType
{
    TeleportTrap, LateTeleportTrap
}