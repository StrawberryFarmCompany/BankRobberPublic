using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StealthTurnRunner : MonoBehaviour
{
    [SerializeField] private AnimationStateController[] enemies;
    public System.Action OnStealthTurnFinished;

    public void BeginStealthTurn()
    {
        if (enemies == null || enemies.Length == 0)
        {
            OnStealthTurnFinished?.Invoke();
            return;
        }

        // 지금은 근접공격 1회로 통일 (추후 다른 액션으로 교체 가능)
        //foreach (var e in enemies)
            //e?.StartMelee();

        //StartCoroutine(WaitAllDone());
    }

    //private IEnumerator WaitAllDone()
    //{
        //yield return new WaitUntil(() =>
            //enemies != null && enemies.All(e => e && e.ActionFinished));

        //OnStealthTurnFinished?.Invoke(); //다음 단계(플레이어 턴 등)로 전환
    //}
}
