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
    private List<BuffDefine.BuffData> datas = new List<BuffDefine.BuffData>();      //버프 데이터 담는 리스트
    [SerializeField] private List<BuffIcon> icons = new List <BuffIcon>();                           //아이콘 객체를 담는 리스트
    [SerializeField] private BuffDescription description;                                            //버프 설명 UI
    private readonly string[] formats = new string[]
    {
        $"[durationRemain]"
    };
    private string GetBuffVariableText(FormatType formatType,int dataIndex)
    {
        switch (formatType)
        {
            case FormatType:
                return datas[dataIndex].Duration.ToString();
                break;
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
    public void UpdateBuffList(NodeDefines.Node node)
    {
        if (node.standing.Count <= 0) return;
        stat = node.standing[0];
        characterName.text = stat.characterName;
        ReleaseBuffList();
        datas.Add(stat.secData.GetSecBuff.Data);

        for (int i = 0; i < icons.Count; i++) icons[i].gameObject.SetActive(false);

        for (int i = 0; i < stat.Buffs.Count; i++)
        {
            datas.Add(stat.Buffs[i].Data);
            if (icons.Count <= i)//icons를 넘을때,즉 생성 시
            {

                GameObject obj = GameObject.Instantiate((GameObject)(ResourceManager.GetInstance.GetPreLoad["BuffIcon"]), transform.Find("BuffSlot"));
                if (obj.TryGetComponent<BuffIcon>(out BuffIcon result))
                {
                    icons.Add(result);
                }
                else
                {
                    result = obj.AddComponent<BuffIcon>();
                }

                icons.Add(result);
                result.Init(i);
                icons[i].SetImage((Sprite)(ResourceManager.GetInstance.GetPreLoad[datas[i].Key.ToString()]));
            }
            else
            {
                icons[i].gameObject.SetActive(true);
                icons[i].Init(i);
                icons[i].SetImage((Sprite)(ResourceManager.GetInstance.GetPreLoad[datas[i].Key.ToString()]));
            }
        }
    }
    public void LoadDescription()
    {
        if (description == null)
        {
            GameObject obj = GameObject.Instantiate((GameObject)(ResourceManager.GetInstance.GetPreLoad["BuffDescription"]), transform.parent);
            description = new BuffDescription(obj);
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
        string descriptionResult = datas[index].DescKor;
        for (int i = 0; i < formats.Length; i++)
        {
            GetBuffVariableText((FormatType)i, index);
        }
        description.SetDescription(new string[] {datas[index].DisStatusNameKor,datas[index].Duration.ToString(),descriptionResult }) ;
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
        buffName = tr.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="str">[0]버프이름,[1]남은 턴,[2]설명</param>
    public void SetDescription(string[] str)
    {
        buffName.text = str[0];
        leftTurn.text = str[1];
        description.text = string.Format(str[2]);
    }
}