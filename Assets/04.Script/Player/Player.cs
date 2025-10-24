using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerController controller;
    public DialogueManager dialogueManager;
    public PlayerInput playerInput;
    public GameObject curUIPanel;

    private void Awake()
    {
        CharacterManager.Instance.player = this;
        controller = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
    }
}
