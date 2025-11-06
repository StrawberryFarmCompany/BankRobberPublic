using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoSingleTon<TaskManager>
{
    Coroutine coroutine;
    private Queue<TurnTask> task = new Queue<TurnTask>();
    private Queue<TurnTask> actionTask = new Queue<TurnTask>();
    private List<Coroutine> coroutines = new List<Coroutine>();
    private bool skipDelay;
    public bool isSceneChanged = false;
    protected override void Init() 
    {
        StartTask();
    }
    public override void OnSceneChange()
    {
        task.Clear();
        actionTask.Clear();
        for (int i = 0; i < coroutines.Count; i++)
        {
            if (coroutines[i] == null) continue;
            StopCoroutine(coroutines[i]);
        }
        coroutines.Clear();
        isSceneChanged = true;
    }
    public void AddTurnBehaviour(TurnTask add)
    {
        task.Enqueue(add);
    }
    /// <summary>
    /// task를 강제로 
    /// </summary>
    /// <param name="target">실행시켜 줄 값</param>
    /// <param name="order">해당 배열에 있는 값을 뒤로 밀어내고 target을 추가</param>
    public void AddActionBehaviour(List<TurnTask> target,int order)
    {
        skipDelay = (order == 0) && task.Count > 0;
        for (int i = 0; i < target.Count; i++)
        {
            actionTask.Enqueue(target[i]);
        }
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
            coroutine = base.StartCoroutine(LoopTask());
        }
    }
    public new Coroutine StartCoroutine(IEnumerator enumerator)
    {
        Coroutine coroutine = base.StartCoroutine(enumerator);
        coroutines.Add(coroutine);
        return coroutine;
    }
    public IEnumerator LoopTask()
    {
        Debug.Log("테스크 루프 실행");
        while (true)
        {
            if (task.Count <= 0&& actionTask.Count <= 0) yield return new WaitUntil(()=> task.Count > 0 || actionTask.Count > 0);
            TurnTask currTask;
            if (actionTask.Count > 0)
            {
                currTask = actionTask.Dequeue();
                skipDelay = false;
            }
            else
            {
                currTask = task.Dequeue();
            }
            Debug.Log($"실행된 액션 명 {currTask.Action?.Method.Name}");
            if (!isSceneChanged)
            {
                currTask.Action?.Invoke();

                float currTime = 0f;
                if (currTask.time > 0f)
                {
                    while (currTask.time > currTime)
                    {
                        if (skipDelay)
                        {
                            skipDelay = false;
                            break;
                        }
                        currTime += Time.deltaTime;
                        yield return null;
                    }
                }
                currTask.Action = null;
            }
            
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