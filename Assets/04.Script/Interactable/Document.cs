using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Document : IInteractable
{
    public Vector3Int tile { get; set; }
    private GameObject target;      //사라지게 할 건지는 아직 미정
    private int docsValue;

    public void Init(Vector3Int tile, /*GameObject target, */ int docsValue)
    {
        this.tile = tile;
        this.docsValue = docsValue;
        RegistInteraction(OnInteraction);
    }

    public void OnInteraction(EntityStats stat)
    {
        UIManager.GetInstance.SetDocumentUI(docsValue);
    }
    public void UnInteraction(EntityStats stat)
    {

    }

    public void RegistInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].AddInteraction(OnInteraction, InteractionType.Document.ToString());
        }
    }
    public void ReleaseInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].RemoveInteraction(OnInteraction, InteractionType.Document.ToString());
        }
    }

}
