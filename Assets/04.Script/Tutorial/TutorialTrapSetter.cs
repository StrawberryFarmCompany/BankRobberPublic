using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrapSetter : MonoBehaviour
{
    //자기 위치에 해당하는 Node에 NodePlayerManager.GetInstance.GetAllPlayer()의 요소가 있다면 해당 요소의 gameObject.transform.position을 특정 위치로
    [SerializeField] Vector3Int targetPos;


    private void Awake()
    {
        Vector3Int pos;
            pos = GameManager.GetInstance.GetVecInt(transform.position);

        TutorialTrap trap = new TutorialTrap(pos, targetPos);

        Destroy(this);
    }
}
