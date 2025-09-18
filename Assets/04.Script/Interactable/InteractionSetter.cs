using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSetter : MonoBehaviour
{
    [SerializeField]InteractionType type;
    IInteractable interaction;
    void Start()
    {
        interaction = IInteractable.Factory(type);
        interaction.tiles = GameManager.GetInstance.GetNearNodes(transform.position).ToArray();
        GameManager.GetInstance.RegistEvent(interaction.tiles, interaction.OnInteraction , type.ToString());
    }
}
