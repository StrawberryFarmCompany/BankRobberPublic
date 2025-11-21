using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterStatusElement : MonoBehaviour
{
    [SerializeField] Image portrait;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI actionPointText;
    [SerializeField] TextMeshProUGUI movementPointText;
    public NodePlayerController player;

    public void SetTargetCharacter(NodePlayerController player)
    {
        this.player = player;
        portrait.sprite = player.playerStats.portrait;
    }

    public void UpdateUI()
    {
        if (player == null) return;
        healthText.text = $"체력\n{player.playerStats.CurHp}/{player.playerStats.maxHp}";
        actionPointText.text = $"행동력\n{player.playerStats.curActionPoint}/{player.playerStats.actionPoint}";
        movementPointText.text = $"이동력\n{player.playerStats.movement}/{player.playerStats.movementSpeed}";
    }
}
