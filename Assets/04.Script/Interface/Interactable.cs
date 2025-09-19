using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public Vector3Int tile { get; set; }
    void OnInteraction();
    void UnInteraction();
    void RegistInteraction(Interaction interaction);
    void ReleaseInteraction(Interaction interaction);
    static IInteractable Factory(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.AlamBTN:
                return new AlarmButton();
            case InteractionType.BankVault:
                return new BankVault();
            case InteractionType.Door:
                return new Door();
            case InteractionType.GoldBar:
                return new GoldBar();
            case InteractionType.MoneyBag:
                return new MoneyBag();
            default:
                return null;
        }
        
    }
}
public enum InteractionType{AlamBTN,BankVault,Door,GoldBar,MoneyBag}
