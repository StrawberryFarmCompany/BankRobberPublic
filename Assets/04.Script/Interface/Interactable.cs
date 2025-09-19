using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public Vector3Int[] tiles { get; set; }
    void OnInteraction();
    void UnInteraction();
    void RegistInteraction();
    void ReleaseInteraction();
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
