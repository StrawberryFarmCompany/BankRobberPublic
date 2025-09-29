using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BuffDefine;
public class SecurityData
{
    private int secLevel;
    public int GetSecLevel { get { return secLevel; } }
    static IBuff battleSec;
    IBuff currSec;

    public IBuff GetSecBuff { get {return battleSec != null ? battleSec : currSec ; }}

    /// <summary>
    /// 시큐리티레벨 설정하는 함수
    /// </summary>
    /// <param name="defaultLevel">1~3 사이의 값을 넣어야합니다.</param>
    public SecurityData(ushort defaultLevel = 1)
    {
        secLevel = defaultLevel;
        SetSecLevel(defaultLevel);
    }
    /// <summary>
    /// 시큐리티레벨 설정하는 함수
    /// </summary>
    /// <param name="level">1~3 사이의 값을 넣어야합니다.</param>
    private void SetSecLevel(ushort level)
    {
        if (level == 0 || level >= 3 || level == secLevel) return;
        ushort key = (ushort)(6000 + level - 1);
        secLevel = level;
        ResourceManager.GetInstance.GetBuffData.TryGetValue(key, out BuffData data);

        if (secLevel != 3)
        {
            currSec = IBuff.Factory(data, null, BuffType.securityLevel);
        }
        else
        {
            battleSec = IBuff.Factory(data, null, BuffType.securityLevel);
        }

            
    }
}