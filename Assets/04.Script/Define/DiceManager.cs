using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Tilemaps;
using UnityEditor.Build.Pipeline;
public class DiceManager : MonoSingleTon<DiceManager>
{
    sbyte diceMaxValue = 6;
    Coroutine coroutine;
    bool isCoroutineRun = false;
    /// <summary>
    /// 주사위 굴리기
    /// </summary>
    /// <param name="factor">보정치</param>
    private sbyte[] Roll(int diceCount,sbyte max)
    {
        sbyte[] diceValues = new sbyte[diceCount];
        for (sbyte i = 0; i < diceValues.Length; i++)
        {
            diceValues[i] = (sbyte)(UnityEngine.Random.Range(0, max+1));
        }
        return diceValues;
    }


    public void DelayedRoll(int factor,sbyte max,int diceCount,Action<int> action)
    {
        coroutine = StartCoroutine(RollCallback(factor, max,diceCount, (r) =>
        {
            action.Invoke(r);
        }));
    }
    public int DirrectRoll(int factor,sbyte max,int diceCount)
    {
        sbyte[] diceValues = Roll(diceCount, max);
        int result = 0;
        if (diceValues.All(x => diceValues[0] == x))
        {
            result = 7777;
        }
        else
        {
            for (int i = 0; i < diceValues.Length; i++)
            {
                result += diceValues[i];
            }
        }
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action">콜백 실행 로직을 여기에 두면됨</param>
    /// <returns></returns>
    private IEnumerator RollCallback(int factor,sbyte max,int diceCount,Action<int> action)
    {
        yield return new WaitUntil(() => isCoroutineRun);
        StopCoroutine(coroutine);
        isCoroutineRun = true;
        float timer = 0;
        sbyte[] diceValues = Roll(diceCount, max);
        int result = 0;
        if (diceValues.All(x => diceValues[0] == x))
        {
            result = 7777;
        }
        else
        {
            for (int i = 0; i < diceValues.Length; i++)
            {
                result += diceValues[i];
            }
        }
        while (timer < 5)
        {
            timer += Time.deltaTime;
            yield return null;

        }
        action.Invoke(result);
        isCoroutineRun = false;
    }

}
