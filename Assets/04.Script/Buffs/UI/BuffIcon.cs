using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class BuffIcon : MonoBehaviour, IPointerClickHandler
{
    private Image image;
    private int index;
    public int Index { get { return index; } }
    private BuffPannel Pannel { get { return UIManager.GetInstance.BuffPannel; } }
    public void Init(int index)
    {
        if (image == null) image = GetComponent<Image>();
        this.index = index;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("우클릭 감지됨");
            Pannel.SetDescription(index);
        }
    }
    public void SetImage(Sprite sprite)
    {
        image.sprite = null;
        image.sprite = sprite;
    }
    public void Release()
    {
        image.sprite = null;
    }
}