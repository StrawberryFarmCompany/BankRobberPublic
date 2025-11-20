using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
namespace Defines
{
    class WarningMessage
    {
        private TextMeshProUGUI warningMessege;
        private Image messegePannel;
        Color defaultMessageColor;
        public void SetErrorMessege(string messege)
        {
            warningMessege.DOComplete(false);
            messegePannel.DOComplete(false);

            messegePannel.color = defaultMessageColor;
            warningMessege.alpha = 1f;
            warningMessege.color = Color.red;


            warningMessege.text = messege;

            warningMessege.DOColor(Color.clear, 5f);
            messegePannel.DOColor(Color.clear, 5f);
        }

        public WarningMessage(Transform MessesgeRT)
        {
            warningMessege = new GameObject("ErrorMessege").AddComponent<TextMeshProUGUI>();
            MessesgeRT = MessesgeRT.transform.parent.parent.parent.parent;

            GameObject obj = GameObject.Instantiate((GameObject)(ResourceManager.GetInstance.GetPreLoad["WarningTextOutLine"]));
            messegePannel = obj.GetComponent<Image>();
            defaultMessageColor = messegePannel.color;
            messegePannel.rectTransform.parent = MessesgeRT;
            messegePannel.rectTransform.anchorMin = Vector2.one * 0.5f;
            messegePannel.rectTransform.anchorMax = Vector2.one * 0.5f;
            messegePannel.rectTransform.anchoredPosition = Vector2.up * 250f;
            messegePannel.rectTransform.sizeDelta = new Vector2(500f, 100f);
            messegePannel.raycastTarget = false;
            messegePannel.color = Color.clear;

            warningMessege.rectTransform.parent = obj.transform;
            warningMessege.alignment = TextAlignmentOptions.Center;
            warningMessege.rectTransform.anchorMin = Vector2.one * 0.5f;
            warningMessege.rectTransform.anchorMax = Vector2.one * 0.5f;
            warningMessege.rectTransform.anchoredPosition = Vector2.zero;
            warningMessege.rectTransform.sizeDelta = new Vector2(500f, 100f);
            warningMessege.raycastTarget = false;
            warningMessege.font = (TMP_FontAsset)ResourceManager.GetInstance.GetPreLoad["DoHyeon-Regular SDF"];
        }
    }
}
