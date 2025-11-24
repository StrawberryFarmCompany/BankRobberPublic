using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatusUI : MonoBehaviour
{
    public GameObject characterStatusUIPrefab;
    public Transform prefabParent;
    public List<CharacterStatusElement> characterStatusElements = new List<CharacterStatusElement>();

    private void Start()
    {
        if(UIManager.GetInstance != null)
        {
            UIManager.GetInstance.characterStatusUI = this;
        }

        NodePlayerManager.GetInstance.GetAllPlayers().ForEach(player =>
        {
            CreateCharacterStatusUI(player);
        });
    }

     private void CreateCharacterStatusUI(NodePlayerController player)
    {

        GameObject obj = Instantiate(characterStatusUIPrefab, prefabParent);
        CharacterStatusElement element = obj.GetComponent<CharacterStatusElement>();
        element.SetTargetCharacter(player);
        characterStatusElements.Add(element);
    }

    public CharacterStatusElement GetCharacterStatusElement(NodePlayerController player)
    {
        foreach (var element in characterStatusElements)
        {
            if (element != null && element.player.playerStats.characterName == player.playerStats.characterName)
            {
                return element;
            }
        }
        return null;
    }

    public void UpdateCharacterStatusElement(CharacterType playerType)
    {
        foreach (var element in characterStatusElements)
        {
            if (element != null && element.player.playerStats.characterType == playerType)
            {
                element.UpdateUI();
            }
        }
    }
}
