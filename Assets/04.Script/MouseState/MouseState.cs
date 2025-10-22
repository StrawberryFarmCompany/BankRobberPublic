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
    public void Execute(Vector2 pos)
    {
        curr?.Execute(pos);
    }
}
public abstract class MouseState 
{
    public abstract void Enter();
    public abstract void Execute(Vector2 pos);
    public abstract void Exit();
    public static MouseState Factory(MouseType type)
    {
        switch (type)
        {
            case MouseType.none:
                return new MouseNoneState();
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

    public override void Execute(Vector2 pos)
    {
        Vector3Int selectedNode = NodePlayerManager.GetInstance.GetCurrentPlayer().GetNodeVector3ByRay(pos, ~(1 << 8));
        if (MoveRangeHighlighter.normalHighlighter.IsPosCludeInBound(selectedNode))
        {
            MoveRangeHighlighter.normalHighlighter.SetGoalPos(selectedNode);
            List<Vector3Int> list = new List<Vector3Int>();
            NodePlayerController ctrl = NodePlayerManager.GetInstance.GetCurrentPlayer();
            EntityStats playerStat = ctrl.playerStats;
            list.Add(ctrl.playerStats.currNode.GetCenter);
            list.AddRange(ctrl.GenerateChebyshevPath(playerStat.currNode.GetCenter, selectedNode));
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

    public override void Execute(Vector2 pos)
    {
        Vector3Int selectedNode = NodePlayerManager.GetInstance.GetCurrentPlayer().GetNodeVector3ByRay(pos, 1 << 8, true);
        if (NodePlayerManager.GetInstance.GetCurrentPlayer().CheckObstacleOnShotPath(selectedNode))
        {
            MoveRangeHighlighter.normalHighlighter.TargetPreviewOnOff(true);
            MoveRangeHighlighter.normalHighlighter.SetPosTargetPreview(selectedNode);
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
public class MouseNoneState : MouseState
{
    public override void Enter()
    {
    }

    public override void Execute(Vector2 pos)
    {
    }

    public override void Exit()
    {
    }
}
public enum MouseType { none,move,attack}