using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionTooltip : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI text;

    public void Show(string desc, RectTransform target)
    {
        text.text = desc;
        panel.SetActive(true);

        Vector3 offset = new Vector3(0, 150f, 0);
        panel.transform.position = target.position + offset;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
