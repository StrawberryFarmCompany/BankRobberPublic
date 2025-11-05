using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BuffDefine;
public class SecurityData
{
    public ushort GetSecLevel { get { return GetSecBuff == null ? (ushort)0: (ushort)GetSecBuff.Data.StatusValue; } }

    private static IBuff sharedSec;
    IBuff currSec;
    public IBuff GetSecBuff { get { return sharedSec != null ? sharedSec : currSec; } }

    private EntityStats stat;
    public static Action OnBattlePhase;
    
    public static void Reset()
    {
        sharedSec = null;
        OnBattlePhase = null;
    }

    /// <summary>
    /// 시큐리티레벨 설정하는 함수
    /// </summary>
    /// <param name="defaultLevel">1~3 사이의 값을 넣어야합니다.</param>
    public SecurityData(EntityStats stat,ushort defaultLevel = 0)
    {
        this.stat = stat;
        SetSecLevel(defaultLevel);
    }

    /// <summary>
    /// 시큐리티레벨 설정하는 함수
    /// </summary>
    /// <param name="level">0~2 사이의 값을 넣어야합니다.</param>
    public void SetSecLevel(ushort level)
    {
        if (level >= 3 || level+1 == this.GetSecLevel) return;
        ushort key = (ushort)(6000 + level);
        ResourceManager.GetInstance.GetBuffData.TryGetValue(key, out BuffData data);

        if (level == 2)
        {
            sharedSec = IBuff.Factory(data, stat, BuffType.securityLevel);
            OnBattlePhase?.Invoke();
        }
        else
        {
            currSec = IBuff.Factory(data, stat, BuffType.securityLevel);
        }
    }
}