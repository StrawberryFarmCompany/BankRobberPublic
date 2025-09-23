using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoSingleTon<TaskManager>
{
    Coroutine coroutine;
    public Queue<TurnTask> task = new Queue<TurnTask>();
    public void AddTurnBehaviour(TurnTask add)
    {
        task.Enqueue(add);
    }
    public void StartTask()
    {
        if (coroutine != null && task.Count == 0) StopCoroutine(coroutine);
        coroutine = StartCoroutine(LoopTask());
    }
    public IEnumerator LoopTask()
    {
        while (task.Count > 0)
        {
            TurnTask currTask = task.Dequeue();
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