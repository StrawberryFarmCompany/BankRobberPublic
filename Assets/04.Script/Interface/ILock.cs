using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public interface ILock
{
    public bool IsLock(EntityStats stat);
    public static ILock Factory(DoorLockType types,int value)
    {
        switch (types)
        {
            case DoorLockType.none:
                return new NoneLock();
            case DoorLockType.lockPick:
                return new LockPick(value);
            case DoorLockType.keyCard:
                return new KeyCardLock(value);
            default:
                return null;
        }
    }
}
public class NoneLock : ILock
{
    public bool IsLock(EntityStats stat) => true;
}
public class LockPick : ILock
{
    int unlockMin;
    bool isLocked;
    public bool IsLock(EntityStats stat)
    {
        if (isLocked)
        {
            //TODO : 현재 선택된 캐릭터의 락핏 스텟
            
            DiceManager.GetInstance.DelayedRoll(stat.sabotage,6,3, (result) =>
            {
                isLocked = unlockMin > result;
                if (!isLocked)
                {
                    Debug.Log("해제 실패, 경고발동");
                    ActivateWarning();
                }
                else
                {
                    Debug.Log("해제 성공");
                }

            });
        }
        return isLocked;
    }
    public void ActivateWarning()
    {
        //TODO : 여기에 넣어야됨
    }
    public LockPick(int unlockMin)
    {
        this.unlockMin = unlockMin;
        isLocked = true;
    }
}
public class KeyCardLock : ILock
{
    int cardKeyIndex;
    public bool IsLock(EntityStats stat)
    {
        return GameManager.GetInstance.isPlayerGetKeyCard[cardKeyIndex];
    }
    public KeyCardLock(int cardKeyIndex)
    {
        this.cardKeyIndex = cardKeyIndex;
        if (GameManager.GetInstance.isPlayerGetKeyCard == null) GameManager.GetInstance.isPlayerGetKeyCard = new List<bool>();
        while (GameManager.GetInstance.isPlayerGetKeyCard.Count >= cardKeyIndex)
        {
            GameManager.GetInstance.isPlayerGetKeyCard.Add(false);
        }
    }
}