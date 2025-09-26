using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoSingleTon<TaskManager>
{
    Coroutine coroutine;
    public Queue<TurnTask> task = new Queue<TurnTask>();
    protected override void Init() 
    {
        StartTask();
    }
    public void AddTurnBehaviour(TurnTask add)
    {
        task.Enqueue(add);
    }
    public void RemoveTurnBehaviour(TurnTask remove)
    {
        TurnTask[] tasks = task.ToArray().Where(x => x.Action.Method.Name != remove.Action.Method.Name).ToArray();
        task = new Queue<TurnTask>(tasks);
    }
    public void StartTask()
    {
        if (coroutine != null) return;
        else
        {
            coroutine = StartCoroutine(LoopTask());
        }
    }
    public IEnumerator LoopTask()
    {
        Debug.Log("테스크 루프 실행");
        while (true)
        {
            if (task.Count <= 0) yield return new WaitUntil(()=> task.Count > 0);
            TurnTask currTask = task.Dequeue();
            Debug.Log($"실행된 액션 명 {currTask.Action?.Method.Name}");
            currTask.Action?.Invoke();
            yield return new WaitForSeconds(currTask.time);
        }
    }
}
public class TurnTask
{
    public TurnBehaviour Action;
    public float time = 0;

    public TurnTask(TurnBehaviour action, float time) 
    {
        Action = action;
        this.time = time;
    }
}