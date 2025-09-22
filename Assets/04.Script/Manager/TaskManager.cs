using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoSingleTon<TaskManager>
{
    public Queue<TurnTask> task = new Queue<TurnTask>();
    public void AddTurnBehaviour()
    {
        
    }
}
public class TurnTask
{
    public TurnBehaviour Action;
    public float time = 0;
}