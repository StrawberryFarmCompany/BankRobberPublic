using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public interface ILock
{
    public bool IsLock();
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
    public bool IsLock() => true;
}
public class LockPick : ILock
{
    int unlockMin;
    bool isLocked;
    public bool IsLock()
    {
        if (isLocked)
        {
            //TODO : 현재 선택된 캐릭터의 락핏 스텟
            
            DiceManager.GetInstance.DelayedRoll(0, (result) =>
            {
                if (unlockMin > result)
                {

                }

            });
        }
        return isLocked;
    }
    public void ActivateWarning()
    {

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
    public bool IsLock()
    {
        return GameManager.GetInstance.isPlayerGeyKeyCard[cardKeyIndex];
    }
    public KeyCardLock(int cardKeyIndex)
    {
        this.cardKeyIndex = cardKeyIndex;
    }
}