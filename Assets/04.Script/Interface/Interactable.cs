using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public Vector3Int tile { get; set; }
    void OnInteraction(EntityStats stat);
    void UnInteraction(EntityStats stat);
    void RegistInteraction(Interaction interaction);
    void ReleaseInteraction(Interaction interaction);
    static IInteractable Factory(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.AlamBTN:
                return new AlarmButton();
            case InteractionType.Window:
                return new Window();
            case InteractionType.Door:
                return new Door();
            case InteractionType.GoldBar:
                return new GoldBar();
            case InteractionType.MoneyBag:
                return new MoneyBag();
            case InteractionType.KeyCard:
                return new KeyCard();
            default:
                return null;
        }

    }
}
public enum InteractionType{AlamBTN,Door,GoldBar,MoneyBag,KeyCard,Window}
