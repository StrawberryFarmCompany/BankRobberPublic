using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public interface ILock
{
    public bool IsLock();
    public bool TryUnLock(EntityStats stat);
    public string GetErrorMessege();
    public static ILock Factory(DoorLockType types, int value, string name,Vector3Int center,byte boundary,float damage,Transform installObjParent)
    {
        switch (types)
        {
            case DoorLockType.none:
                return new NoneLock(true);
            case DoorLockType.lockPick:
                return new LockPick(value);
            case DoorLockType.keyCard:
                return new KeyCardLock(value);
            case DoorLockType.button:
                return new ButtonLock(value);
            case DoorLockType.password:
                return new PasswordLock(value);
            case DoorLockType.drill:
                return new DrillLock(value, name, installObjParent);
            case DoorLockType.bomb:
                return new BombLock(value, name,center,boundary,damage,installObjParent);
            default:
                return null;
        }
    }
}
public class NoneLock : ILock
{
    private bool defaultValue = true;

    public bool IsLock()
    {
        return defaultValue;
    }
    public NoneLock(bool dv)
    {
        defaultValue = dv;
    }
    public bool TryUnLock(EntityStats stat) => defaultValue;

    public string GetErrorMessege()
    {
        return "";
    }
}
public class LockPick : ILock
{
    int unlockMin;
    bool released;
    public bool IsLock()
    {
        return released;
    }
    public bool TryUnLock(EntityStats stat)
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

    public bool IsLock()
    {
        return released;
    }
    public bool TryUnLock(EntityStats stat)
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
    public bool IsLock()
    {
        return released;
    }
    public bool TryUnLock(EntityStats stat)
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
    public bool IsLock()
    {
        return released;
    }
    public bool TryUnLock(EntityStats stat)
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
public class DrillLock : ILock
{
    private bool released = false;
    private bool isActivated = false;
    private string targetName;
    private byte leftTurn = 255;
    Transform installParent;
    GameObject installedOBJ;
    public bool IsLock()
    {
        return released;
    }
    public bool TryUnLock(EntityStats stat)
    {
        if(stat != null)OnInstallDrill();
        return released;
    }

    private void OnInstallDrill()
    {
        if (isActivated && !released)
        {
            UIManager.GetInstance.SetWarningMessege(GetErrorMessege());
            return;
        }
        else if(!isActivated && !released)
        {
            isActivated = true;
            GameManager.GetInstance.NoneBattleTurn.BuffCount += OnTurnCounting;
            UIManager.GetInstance.SetWarningMessege($"드릴이 설치 되엇습니다. 잠금 해제까지 : {leftTurn}턴");
            installedOBJ = GameObject.Instantiate((GameObject)ResourceManager.GetInstance.GetPreLoad["AttachDrill"]);
            installedOBJ.transform.parent = installParent;
            installedOBJ.transform.localEulerAngles = Vector3.zero;
            installedOBJ.transform.localPosition = Vector3.zero;
        }
    }
    public void OnTurnCounting()
    {
        --leftTurn;
        if (leftTurn == 0)
        {
            released = true;
            GameManager.GetInstance.NoneBattleTurn.BuffCount -= OnTurnCounting;
            GameObject.Destroy(installedOBJ);
        }
        UIManager.GetInstance.SetWarningMessege(GetErrorMessege());
    }
    public string GetErrorMessege()
    {
        return released? $"{targetName} 잠금이 해제되엇습니다.":$"{targetName} 잠금 해제까지 : {leftTurn}턴";
    }
    public DrillLock(int leftTurn,string targetName,Transform installObjParent)
    {
        this.leftTurn = (byte)leftTurn;
        this.targetName = targetName;
        this.installParent = installObjParent;
    }
}
public class BombLock : ILock
{
    AttachBoomb explosion;
    private bool released = false;
    private bool isActivated = false;
    private string targetName;
    private byte leftTurn = 255;


    public bool IsLock()
    {
        return released;
    }
    public bool TryUnLock(EntityStats stat)
    {
        if(stat != null)InstallBomb();
        return released;
    }

    private void InstallBomb()
    {
        if (isActivated && !released)
        {
            UIManager.GetInstance.SetWarningMessege(GetErrorMessege());
            return;
        }
        else if(!isActivated && !released)
        {
            isActivated = true;
            GameManager.GetInstance.NoneBattleTurn.BuffCount += OnTurnCounting;
            explosion.InstallBomb();
            UIManager.GetInstance.SetWarningMessege($"폭탄이 설치 되엇습니다. 폭발까지 : {leftTurn}턴");
        }
    }
    public void OnTurnCounting()
    {
        --leftTurn;
        if (leftTurn == 0)
        {
            released = true;

            GameManager.GetInstance.NoneBattleTurn.BuffCount -= OnTurnCounting;
            explosion.Explosion();
        }
        UIManager.GetInstance.SetWarningMessege(GetErrorMessege());
    }
    public string GetErrorMessege()
    {
        return released? $"{targetName}이 폭발했습니다.":$"{targetName} 폭발까지 : {leftTurn}턴";
    }
    public BombLock(int leftTurn,string targetName,Vector3Int pos,ushort boundary,float damage,Transform installObjParent)
    {
        this.leftTurn = (byte)leftTurn;
        this.targetName = targetName;
        explosion = new AttachBoomb(boundary, damage, pos,installObjParent);
    }
}
