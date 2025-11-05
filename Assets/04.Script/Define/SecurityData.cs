using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BuffDefine;
public class SecurityData
{
    private ushort secLevel;
    public ushort GetSecLevel { get { return secLevel; } }

    private static IBuff sharedSec;
    IBuff currSec;
    public IBuff GetSecBuff { get { return isBattlePhase ? sharedSec : currSec; } }

    private EntityStats stat;
    public static Action OnBattlePhase;

    public bool isBattlePhase { get { return sharedSec != null; } }
    
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
        SetSecLevel(defaultLevel); 
        this.stat = stat;
    }

    /// <summary>
    /// 시큐리티레벨 설정하는 함수
    /// </summary>
    /// <param name="level">0~2 사이의 값을 넣어야합니다.</param>
    public void SetSecLevel(ushort level)
    {
        if (level >= 3 || level+1 == secLevel) return;
        ushort key = (ushort)(6000 + level);
        this.secLevel = level++;
        ResourceManager.GetInstance.GetBuffData.TryGetValue(key, out BuffData data);

        if (this.secLevel == 3)
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