using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class DamageProjector
{
    Queue<TextMeshPro> textQueue = new Queue<TextMeshPro>();
    Transform dtParent;

    private void EnQueue(TextMeshPro tmp)
    {
        if (tmp == null)
        {
            OnCreateOBJ();
        }
        else
        {
            textQueue.Enqueue(tmp);
            tmp.gameObject.SetActive(false);
        }
    }
    public void DeQueue(float v,Vector3 pos)
    {
        TextMeshPro temp;
        if (textQueue.Count > 0)
        {
            temp = textQueue.Dequeue();
        }
        else
        {
            OnCreateOBJ();
            temp = textQueue.Dequeue();
        }
        temp.gameObject.SetActive(true);
        temp.text = $"- {v.ToString("n2")}";
        temp.transform.position = pos;

        temp.transform.DOMoveY(pos.y+1.8f, 1f).OnComplete(()=> 
        {
            EnQueue(temp);
        });
    }

    private void OnCreateOBJ()
    {
        if (dtParent == null)
        {
            dtParent = new GameObject("dtParent").transform;
            dtParent.position = Vector3.zero;
            dtParent.rotation = Quaternion.identity;
        }

        GameObject obj = GameObject.Instantiate((GameObject)(ResourceManager.GetInstance.GetPreLoad["DamageText"]), Vector3.up * 9999, Quaternion.identity,dtParent);
        TextMeshPro tmp = obj.GetComponent<TextMeshPro>();
        textQueue.Enqueue(tmp);
        obj.SetActive(false);
    }

    public void OnReset()
    {
        textQueue.Clear();
        dtParent = null;
    }
}
