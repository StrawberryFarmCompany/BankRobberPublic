using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HighlightTEST : MonoBehaviour
{
    [SerializeField] private MoveRangeHighlighter highlighter;
    [SerializeField] private Transform player;  
    void Start()
    {
        Vector3Int vector3Int = GameManager.GetInstance.GetVecInt(player.position);
        highlighter.ShowMoveRange(vector3Int);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            Vector3Int vector3Int = GameManager.GetInstance.GetVecInt(player.position);
            highlighter.ShowMoveRange(vector3Int);
        }
        if(Input.GetKeyDown(KeyCode.J))
        {
            highlighter.ClearHighlights();
        }
    }
}
