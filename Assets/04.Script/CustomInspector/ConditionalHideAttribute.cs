using UnityEngine;

/// <summary>
/// 특정 필드 값이 하나 또는 여러 기대값 중 하나일 때만 인스펙터에 표시
/// </summary>
public class ConditionalHideAttribute : PropertyAttribute
{
    public string conditionField;  // 비교할 필드 이름
    public int[] expectedValues;   // int/enum 비교 시 여러 값 지원
    public bool expectedBool;      // bool 비교
    public bool isBool;

    // int/enum용 (단일 값도 params로 처리 가능)
    public ConditionalHideAttribute(string conditionField, params int[] expectedValues)
    {
        this.conditionField = conditionField;
        this.expectedValues = expectedValues;
        this.isBool = false;
    }

    // bool용
    public ConditionalHideAttribute(string conditionField, bool expectedBool)
    {
        this.conditionField = conditionField;
        this.expectedBool = expectedBool;
        this.isBool = true;
    }
}
