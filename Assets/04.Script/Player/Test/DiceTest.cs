using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceTest : MonoBehaviour
{

    public int testint = 0;
    public int DiceResult;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            DiceManager.GetInstance.DelayedRoll(testint, OnDiceResult);
            //Debug.Log($"값: {DiceResult}");
        }
    }

    public void RollDice(int result)
    {
        DiceResult = result;
    }

    private void OnDiceResult(int result)
    {
        Debug.Log("주사위 결과: " + result);

        if (result == 77)
        {
            Debug.Log("🎲 세 주사위가 같음! 특수 이벤트 발생");
        }
        else if (result >= 12)
        {
            Debug.Log("큰 수가 나와서 강력한 액션 가능!");
        }
        else
        {
            Debug.Log("작은 수... 조심해야 함.");
        }
    }
}
