using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurnMergeTest : MonoBehaviour
{
    public List<BattleTurnState> turnStates;
    void Start()
    {
        turnStates = new List<BattleTurnState>();
        for (int i = 0; i < 100; i++)
        {
            turnStates.Add(new BattleTurnState());
            turnStates[i].Init(false);
            turnStates[i].OnStart += OnEvent;
            turnStates[i].OnEnd += OnEndEvent;
        }
        BattleTurnState temp = new BattleTurnState();
        turnStates.Insert(50, temp);
        temp.Init(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Merge();
            Debug.Log($"Merge{turnStates.Count}");
            turnStates[0].OnStart();
            turnStates[0].OnEnd();
        }
    }
    void Merge()
    {
        TurnBehaviour start;
        TurnBehaviour end;
        int lastAlly = -1;
        for (int i = 0; i < turnStates.Count; i++)
        {

            if (!turnStates[i].isEnemy)
            {
                if (lastAlly != -1 && i - 1 == lastAlly)
                {
                    turnStates[lastAlly].OnStart += turnStates[i].OnStart;
                    turnStates[lastAlly].OnEnd += turnStates[i].OnEnd;
                    turnStates.RemoveAt(i);
                    i--;
                }
                lastAlly = i;
            }
        }
    }
    void OnEvent()
    {
        Debug.Log("스타트");
    }
    void OnEndEvent()
    {
        Debug.Log("엔드");
    }
}
