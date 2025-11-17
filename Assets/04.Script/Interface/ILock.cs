using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public interface ILock
{
    public bool IsLock(EntityStats stat);
    public string GetErrorMessege();
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
            case DoorLockType.button:
                return new ButtonLock(value);
            case DoorLockType.password:
                return new PasswordLock(value);
            default:
                return null;
        }
    }
}
public class NoneLock : ILock
{
    public bool IsLock(EntityStats stat) => true;

    public string GetErrorMessege()
    {
        return null;
    }
}
public class LockPick : ILock
{
    int unlockMin;
    bool released;
    public bool IsLock(EntityStats stat)
    {
        if (!released)
        {
            //TODO : 추후 연출 추가 시 delayedRoll로 처리

            released = unlockMin > DiceManager.GetInstance.DirrectRoll(stat.sabotage, 6, 3);
/*            DiceManager.GetInstance.DelayedRoll(stat.sabotage,6,3, (result) =>
            {
                released = unlockMin > result;
                if (!released)
                {
                    Debug.Log("해제 실패, 경고발동");
                    NoiseManager.AddNoise(stat.currNode.GetCenter, NoiseType.Disarm, false);
                    ActivateWarning();
                }
                else
                {
                    Debug.Log("해제 성공");
                    NoiseManager.AddNoise(stat.currNode.GetCenter, NoiseType.Disarm);
                }

            });*/
        }
        return released;
    }

    public string GetErrorMessege()
    {
        return "문따기 실패!";
    }
    public void ActivateWarning()
    {
        //TODO : 여기에 넣어야됨
    }
    public LockPick(int unlockMin)
    {
        this.unlockMin = unlockMin;
        released = false;
    }
}
public class KeyCardLock : ILock
{
    int cardKeyIndex;
    public bool released = false;
    public bool IsLock(EntityStats stat)
    {
        if (released == true) return released;
        released = GameManager.GetInstance.isPlayerGetKeyCard[cardKeyIndex];
        if (released == false) Debug.Log("해당 키카드가 없습니다 키카드 넘버 : "+cardKeyIndex);
        return GameManager.GetInstance.isPlayerGetKeyCard[cardKeyIndex];
    }

    public string GetErrorMessege()
    {
        return "올바른 키카드가 없습니다.";
    }
    public KeyCardLock(int cardKeyIndex)
    {
        this.cardKeyIndex = cardKeyIndex;
        if (GameManager.GetInstance.isPlayerGetKeyCard == null) GameManager.GetInstance.isPlayerGetKeyCard = new List<bool>();
        while (GameManager.GetInstance.isPlayerGetKeyCard.Count <= cardKeyIndex)
        {
            GameManager.GetInstance.isPlayerGetKeyCard.Add(false);
        }
    }
}

public class ButtonLock : ILock
{
    int buttonIndex;
    public bool released = true;
    public bool IsLock(EntityStats stat)
    {
        return released;
    }

    public string GetErrorMessege()
    {
        return "버튼으로 열 수 있을 것 같습니다.";
    }
    public ButtonLock(int index)
    {
        buttonIndex = index;
        //GameManager.GetInstance.RegisterButtonDoor();
    }
}

public class PasswordLock : ILock
{
    int index;
    [Range(0, 9999)]int password;
    public bool released = false;
    public bool IsLock(EntityStats stat)
    {
        if (released == true) return released;
        released = GameManager.GetInstance.isOpenPasswordDoor[index];
        if (released == false) Debug.Log("패스워드 입력 필요 : " + index);
        return GameManager.GetInstance.isOpenPasswordDoor[index];
    }

    public string GetErrorMessege()
    {
        return "비밀번호가 올바르지 않습니다.";
    }
    public PasswordLock(int index)
    {
        this.index = index;
        password = Random.Range(0, 10000);
        GameManager.GetInstance.RegisterPasswordDoor(index, password);
    }
}
