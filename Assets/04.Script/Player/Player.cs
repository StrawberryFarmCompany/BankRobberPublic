using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private CharacterType character;
    public PlayerController controller;
    public DialogueManager dialogueManager;
    public PlayerInput playerInput;

    private void Awake()
    {
        CharacterManager.Instance.player = this;
        controller = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
    }
}
