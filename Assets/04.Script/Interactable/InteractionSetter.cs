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
        GameManager.GetInstance.RegistEvent(transform.position,interaction.OnInteraction , type.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
