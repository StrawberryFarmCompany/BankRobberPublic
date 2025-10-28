using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HPBar
{
    private RectTransform slotParent;
    private Image[] hpSlots;
    private static Canvas hpCanvas;

    public void Init(float maxHP,float currHP,Transform parent)
    {
        if (hpCanvas == null || hpCanvas.Equals(null))
        {
            hpCanvas = new GameObject("hpCanvas").AddComponent<Canvas>();
            hpCanvas.renderMode = RenderMode.WorldSpace;
            CanvasScaler scaler = hpCanvas.gameObject.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 1;
            scaler.referencePixelsPerUnit = 100;

            GraphicRaycaster gr= hpCanvas.gameObject.AddComponent<GraphicRaycaster>();
            gr.ignoreReversedGraphics = false;
            gr.blockingObjects = GraphicRaycaster.BlockingObjects.None;
            gr.blockingMask = 0;
        }

        hpSlots = new Image[Mathf.CeilToInt(maxHP)];

        slotParent = (RectTransform)GameObject.Instantiate((GameObject)ResourceManager.GetInstance.GetPreLoad["HPParent"]).transform;
        
        slotParent.parent = hpCanvas.transform;
        GameObject prefab = (GameObject)ResourceManager.GetInstance.GetPreLoad["HPSlot"];
        float parentTotal = hpSlots.Length*slotParent.sizeDelta.x;
        float childXPos = parentTotal / -2f;
        float margine = parentTotal / hpSlots.Length;
        childXPos += (margine / 2f);
        for (int i = 0; i < hpSlots.Length; i++)
        {
            hpSlots[i] = GameObject.Instantiate(prefab, slotParent).GetComponent<Image>();
            hpSlots[i].rectTransform.rotation = Quaternion.identity;
            hpSlots[i].rectTransform.localPosition = new Vector3(childXPos, 0);
            childXPos += margine;
        }
        slotParent.sizeDelta = new Vector2(parentTotal, slotParent.sizeDelta.y);
        SetCurrHP(currHP);
        slotParent.gameObject.AddComponent<BillBoard>();
    }
    public void SetCurrHP(float currHP)
    {
        int curHp = Mathf.CeilToInt(currHP);
        for (int i = 0; i < hpSlots.Length; i++)
        {
            if (curHp > i )
            {
                hpSlots[i].color = Color.red;

            }
            else
            {
                hpSlots[i].color = Color.black;

            }
        }
    }
    public void SetPosition(Vector3 pos)
    {
        slotParent.position = pos;
        Debug.Log($"피통 위치{pos}");
    }
    public void Destroy(bool OnSceneChange = false)
    {
        GameObject.Destroy(hpCanvas);
        slotParent = null;
        hpSlots = null;
    }
}

public class BillBoard : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
