using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class BuffPannel : MonoBehaviour
{
    private EntityStats stat;
    [SerializeField] TextMeshProUGUI characterName;
    private List<BuffDefine.IBuff> datas = new List<BuffDefine.IBuff>();      //버프 데이터 담는 리스트
    [SerializeField] private List<BuffIcon> icons = new List <BuffIcon>();                           //아이콘 객체를 담는 리스트
    [SerializeField] private BuffDescription description;                                            //버프 설명 UI
    public BuffDescription Description { get { return description; } }                                            //버프 설명 UI
    private readonly string[] formats = new string[]
    {
        $"[durationRemain]"
    };
    private string GetBuffVariableText(FormatType formatType,int dataIndex)
    {
        switch (formatType)
        {
            case FormatType.duration:
                return datas[dataIndex].Duration.ToString();
            default:
                break;
        }
        Debug.LogError("테이블에 매칭되는 타입이 없습니다.");
        return formats[(int)formatType];
    }
    private enum FormatType { duration = 0 }
    public void Init()
    {
        ReleaseBuffList();
        LoadDescription();
    }
    public void TurnOn(bool turnOn)
    {
        gameObject.SetActive(turnOn);
        if (!turnOn) description.TurnOn(turnOn);
    }
    public void UpdateBuffList(NodeDefines.Node node)
    {
        if (node.Standing  == null|| node.Standing.Count <= 0) return;
        stat = node.Standing[0];
        if (stat.characterType != CharacterType.None) return;

        gameObject.SetActive(true);
        characterName.text = stat.characterName;
        ReleaseBuffList();
        if(stat.secData.GetSecBuff != null) datas.Add(stat.secData.GetSecBuff);

        for (int i = 0; i < icons.Count; i++) icons[i].gameObject.SetActive(false);

        for (int i = 0; i < stat.Buffs.Count; i++){datas.Add(stat.Buffs[i]);}      //등록만

        for (int i = 0; i < datas.Count; i++)
        {
            if (icons.Count <= i)//icons를 넘을때,즉 생성 시
            {
                CreateBuffIcon(i);
            }
            else
            {
                icons[i].gameObject.SetActive(true);
                icons[i].Init(i);
                Sprite sprite = ((UnityEngine.U2D.SpriteAtlas)ResourceManager.GetInstance.GetPreLoad["UIAtlas"]).GetSprite(datas[i].Data.Key.ToString());
                icons[i].SetImage(sprite);
            }
        }
        ((RectTransform)transform).sizeDelta = new Vector2((icons.Count <= 5? datas.Count: 5) * 100, (stat.Buffs.Count / 5)*100)+Vector2.one*100;
    }
    private void CreateBuffIcon(int index)
    {
        GameObject obj = GameObject.Instantiate((GameObject)ResourceManager.GetInstance.GetPreLoad["BuffIcon"], transform.Find("BuffSlot"));
        obj.name = $"BuffIcon{index}";
        if (obj.TryGetComponent<BuffIcon>(out BuffIcon result))
        {
            icons.Add(result);
        }
        else
        {
            result = obj.AddComponent<BuffIcon>();
            icons.Add(result);
        }

        result.Init(index);
        Sprite sprite = ((UnityEngine.U2D.SpriteAtlas)ResourceManager.GetInstance.GetPreLoad["UIAtlas"]).GetSprite(datas[index].Data.Key.ToString());
        icons[index].SetImage(sprite);
    }
    public void LoadDescription()
    {
        if (description == null)
        {
            GameObject obj = GameObject.Instantiate((GameObject)(ResourceManager.GetInstance.GetPreLoad["BuffDescription"]), transform.parent);
            description = new BuffDescription(obj);
            obj.SetActive(false);
        }
    }
    public void ReleaseBuffList()
    {
        datas.Clear();
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].Release();
        }
    }
    public void SetDescription(int index)
    {
        string descriptionResult = datas[index].Data.DescKor;
        description.TurnOn(true);
        for (int i = 0; i < formats.Length; i++)
        {
            descriptionResult = descriptionResult.Replace(formats[i], GetBuffVariableText((FormatType)i, index));
        }
        description.SetDescription(new string[] {datas[index].Data.DisStatusNameKor,datas[index].Duration.ToString(),descriptionResult }) ;
    }
}
public class BuffDescription
{
    Transform tr;
    TextMeshProUGUI buffName;
    TextMeshProUGUI leftTurn;
    TextMeshProUGUI description;
    
    public BuffDescription(GameObject obj)
    {
        tr = obj.transform;
        buffName = tr.Find("BuffNameText").GetComponent<TextMeshProUGUI>();
        leftTurn = tr.Find("LeftTrunText").GetComponent<TextMeshProUGUI>();
        description = tr.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="str">[0]버프이름,[1]남은 턴,[2]설명</param>
    public void SetDescription(string[] str)
    {
        buffName.text = str[0];
        leftTurn.text = $"남은 턴 : {(str[1] == "-1" ? "영구 지속" : str[1])}";
        description.text = str[2];
    }
    public void TurnOn(bool isOn)
    {
        tr.gameObject.SetActive(isOn);
    }
}