using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IStateMachine;
using System;

public class MouseStateMachine
{
    MouseState[] states;
    MouseState curr;
    
    public MouseStateMachine()
    {
        states = new MouseState[Enum.GetValues(typeof(MouseType)).Length];
        for (int i = 0; i < states.Length; i++)
        {
            states[i] = MouseState.Factory((MouseType)i);
        }
    }
    public void ChangeState(MouseType next)
    {
        curr?.Exit();
        curr = states[(int)next];
        curr.Enter();
    }
    public void Execute(Vector3Int pos)
    {
        curr?.Execute(pos);
    }
}
public abstract class MouseState 
{
    public abstract void Enter();
    public abstract void Execute(Vector3Int pos);
    public abstract void Exit();
    public static MouseState Factory(MouseType type)
    {
        switch (type)
        {
            case MouseType.move:
                return new MouseMoveState();
            case MouseType.attack:
                return new MouseAttackState();
            default:
                break;
        }
        return null;
    }
}
public class MouseMoveState : MouseState
{
    public override void Enter()
    {
        MoveRangeHighlighter.normalHighlighter.GoalPreviewOnOff(true);
    }

    public override void Execute(Vector3Int pos)
    {
        if (MoveRangeHighlighter.normalHighlighter.IsPosCludeInBound(pos))
        {
            MoveRangeHighlighter.normalHighlighter.SetGoalPos(pos);
            List<Vector3Int> list = new List<Vector3Int>();
            NodePlayerController ctrl = NodePlayerManager.GetInstance.GetCurrentPlayer();
            EntityStats playerStat = ctrl.playerStats;
            list.Add(ctrl.playerStats.currNode.GetCenter);
            list.AddRange(ctrl.GenerateChebyshevPath(playerStat.currNode.GetCenter, pos));
            MoveRangeHighlighter.normalHighlighter.SetPathLine(list.ToArray());
        }

    }

    public override void Exit()
    {
        MoveRangeHighlighter.normalHighlighter.GoalPreviewOnOff(false);
    }
}
public class MouseAttackState : MouseState
{
    public override void Enter()
    {
        MoveRangeHighlighter.normalHighlighter.TargetPreviewOnOff(true);
    }

    public override void Execute(Vector3Int pos)
    {
        if (NodePlayerManager.GetInstance.GetCurrentPlayer().CheckObstacleOnShotPath(pos))
        {
            MoveRangeHighlighter.normalHighlighter.TargetPreviewOnOff(true);
            MoveRangeHighlighter.normalHighlighter.SetPosTargetPreview(pos);
        }
        else
        {
            MoveRangeHighlighter.normalHighlighter.TargetPreviewOnOff(false);
        }
    }

    public override void Exit()
    {
        MoveRangeHighlighter.normalHighlighter.TargetPreviewOnOff(false);
    }
}
public enum MouseType { move,attack}