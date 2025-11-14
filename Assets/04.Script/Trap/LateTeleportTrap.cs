using NodeDefines;
using UnityEngine;

public class LateTeleportTrap : MonoBehaviour
{
    public Vector3Int tile;
    public Vector3Int targetPos;
    public int turnWait;

    public bool requireStayOnTile = false; // 옵션

    private int reservedActionId = -1;

    public LateTeleportTrap(Vector3Int tile, Vector3Int pos, int turnWait)
    {
        this.tile = tile;
        targetPos = pos;
        this.turnWait = turnWait;
    }

    public void Init()
    {
        // trap이 적용될 실제 노드
        Node node = GameManager.GetInstance.GetNode(tile);

        // 들어올 때
        node.AddEvent(OnStanding);

        // 떠날 때
        node.AddExitEvent(OnExitTile);
    }

    public void OnStanding(EntityStats stat)
    {
        if (stat.thisGameObject == null)
            return;

        // 이미 예약되어 있으면 중복 예약 방지
        if (reservedActionId != -1)
            return;

        requireStayOnTile = true;

        reservedActionId = GameManager.GetInstance.NoneBattleTurn.InvokeAfterTurnsCancellable(turnWait, () =>
            {
                // 조건: 노드 위에 계속 있어야 함
                if (requireStayOnTile)
                {
                    if (stat.currNode != null && stat.currNode.GetCenter == tile)
                    {
                        Teleport(stat);
                    }
                }

                reservedActionId = -1; // 실행 후 초기화
            });
    }

    /// <summary>
    /// 타일에서 벗어나면 취소
    /// </summary>
    public void OnExitTile(EntityStats stat)
    {
        if (!requireStayOnTile)
            return;

        requireStayOnTile = false;

        if (reservedActionId != -1)
        {
            GameManager.GetInstance.NoneBattleTurn.CancelInvoke(reservedActionId);
            reservedActionId = -1;
        }
    }

    private void Teleport(EntityStats stat)
    {
        if (stat.thisGameObject != null)
        {
            stat.thisGameObject.transform.position = targetPos;
            stat.NodeUpdates(targetPos);
            if(stat.characterType != CharacterType.None)
                stat.thisGameObject.GetComponent<NodePlayerController>().TurnOnHighlighter();
        }
    }
}
