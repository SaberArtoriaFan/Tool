using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChineseNameAttribute : PropertyAttribute
{
    public string ChineseName;

    public ChineseNameAttribute(string chineseName)
    {
        ChineseName = chineseName;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ChineseNameAttribute))]
public class ChineseNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty serializedProperty, GUIContent label)
    {
        string valueName = "";
        switch (serializedProperty.propertyType)
        {
            case SerializedPropertyType.Integer:
                valueName=serializedProperty.intValue.ToString();
                //Debug.Log("Integer: " + serializedProperty.intValue);
                break;
            case SerializedPropertyType.Boolean:
                valueName = serializedProperty.boolValue.ToString();
                // Debug.Log("Boolean: " + serializedProperty.boolValue);
                break;
            case SerializedPropertyType.Float:
                valueName = serializedProperty.floatValue.ToString();
                //Debug.Log("Float: " + serializedProperty.floatValue);
                break;
            case SerializedPropertyType.String:
                valueName = serializedProperty.stringValue;
                //Debug.Log("String: " + serializedProperty.stringValue);
                break;
                // 其他类型...
        }
        EditorGUI.LabelField(position, ((ChineseNameAttribute)attribute).ChineseName, valueName);

        //EditorGUI.PropertyField(position, property, label);
    }
}
#endif