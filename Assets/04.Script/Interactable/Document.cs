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

    }
    public void UnInteraction(EntityStats stat)
    {
        throw new System.NotImplementedException();
    }

    public void RegistInteraction(Interaction interaction)
    {
        throw new System.NotImplementedException();
    }

    public void ReleaseInteraction(Interaction interaction)
    {
        throw new System.NotImplementedException();
    }

}
