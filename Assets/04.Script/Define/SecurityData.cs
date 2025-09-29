using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BuffDefine;
public class SecurityData
{
    private int secLevel;
    public int GetSecLevel { get { return secLevel; } }
    private static IBuff battleSec;
    IBuff currSec;

    public IBuff GetSecBuff { get {return battleSec != null ? battleSec : currSec ; }}

    public static void Reset()
    {
        battleSec = null;
    }
    /// <summary>
    /// 시큐리티레벨 설정하는 함수
    /// </summary>
    /// <param name="defaultLevel">1~3 사이의 값을 넣어야합니다.</param>
    public SecurityData(ushort defaultLevel = 1)
    {
        secLevel = defaultLevel;
    }
    /// <summary>
    /// 시큐리티레벨 설정하는 함수
    /// </summary>
    /// <param name="level">0~2 사이의 값을 넣어야합니다.</param>
    public void SetSecLevel(ushort level)
    {
        if (level >= 3 || level == secLevel) return;
        ushort key = (ushort)(6000 + level);
        ResourceManager.GetInstance.GetBuffData.TryGetValue(key, out BuffData data);
        currSec = IBuff.Factory(data, null, BuffType.securityLevel);
    }
}