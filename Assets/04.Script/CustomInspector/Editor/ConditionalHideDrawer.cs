using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHideDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute cond = (ConditionalHideAttribute)attribute;
        SerializedProperty conditionProp = property.serializedObject.FindProperty(cond.conditionField);

        if (conditionProp == null)
        {
            EditorGUI.PropertyField(position, property, label, true);
            Debug.LogWarning($"ConditionalHide: '{cond.conditionField}' 필드를 찾을 수 없음.");
            return;
        }

        bool shouldShow = false;

        if (cond.isBool)
        {
            if (conditionProp.propertyType == SerializedPropertyType.Boolean)
                shouldShow = conditionProp.boolValue == cond.expectedBool;
        }
        else
        {
            int val = 0;
            switch (conditionProp.propertyType)
            {
                case SerializedPropertyType.Enum:
                    val = conditionProp.enumValueIndex;
                    break;
                case SerializedPropertyType.Integer:
                    val = conditionProp.intValue;
                    break;
                default:
                    Debug.LogWarning($"ConditionalHide: 지원되지 않는 타입({conditionProp.propertyType})");
                    shouldShow = true;
                    break;
            }

            if (cond.expectedValues != null)
            {
                foreach (int expected in cond.expectedValues)
                {
                    if (val == expected)
                    {
                        shouldShow = true;
                        break;
                    }
                }
            }
        }

        if (shouldShow)
            EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute cond = (ConditionalHideAttribute)attribute;
        SerializedProperty conditionProp = property.serializedObject.FindProperty(cond.conditionField);

        if (conditionProp == null)
            return EditorGUI.GetPropertyHeight(property, label, true);

        bool shouldShow = false;

        if (cond.isBool)
        {
            if (conditionProp.propertyType == SerializedPropertyType.Boolean)
                shouldShow = conditionProp.boolValue == cond.expectedBool;
        }
        else
        {
            int val = 0;
            switch (conditionProp.propertyType)
            {
                case SerializedPropertyType.Enum:
                    val = conditionProp.enumValueIndex;
                    break;
                case SerializedPropertyType.Integer:
                    val = conditionProp.intValue;
                    break;
                default:
                    shouldShow = true;
                    break;
            }

            if (cond.expectedValues != null)
            {
                foreach (int expected in cond.expectedValues)
                {
                    if (val == expected)
                    {
                        shouldShow = true;
                        break;
                    }
                }
            }
        }

        return shouldShow ? EditorGUI.GetPropertyHeight(property, label, true) : 0f;
    }
}
