using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Tilemaps;
using UnityEditor.Build.Pipeline;
public class DiceManager : MonoSingleTon<DiceManager>
{
    sbyte[] diceValues = new sbyte[3];
    sbyte diceMaxValue = 6;
    Coroutine coroutine;
    bool isCoroutineRun = false;
    /// <summary>
    /// 주사위 굴리기
    /// </summary>
    /// <param name="factor">보정치</param>
    private void Roll()
    {
        for (sbyte i = 0; i < diceValues.Length; i++)
        {
            diceValues[i] = (sbyte)(UnityEngine.Random.Range(0, diceMaxValue));
        }
    }


    public void DelayedRoll(int factor,Action<int> action)
    {
        coroutine = StartCoroutine(RollCallback(factor, (r) =>
        {
            action.Invoke(r);
        }));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action">콜백 실행 로직을 여기에 두면됨</param>
    /// <returns></returns>
    private IEnumerator RollCallback(int factor,Action<int> action)
    {
        yield return new WaitUntil(() => isCoroutineRun);
        StopCoroutine(coroutine);
        isCoroutineRun = true;
        float timer = 0;
        Roll();
        int result = 0;
        if (diceValues.All(x => diceValues[0] == x))
        {
            result = 77;
        }
        for (int i = 0; i < diceValues.Length; i++)
        {
            result += diceValues[i];
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
