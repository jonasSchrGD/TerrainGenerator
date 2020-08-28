using UnityEngine;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;

[CustomPropertyDrawer(typeof(FilterData))]
public class FilterDataDrawer : PropertyDrawer
{
    const float rectHeight = 16;
    const float offset = 2;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return rectHeight;

        var filterType = property.FindPropertyRelative("FilterType");
        int variableCount = 2;

        switch ((FilterType)filterType.intValue)
        {
            case FilterType.basefilter:
                variableCount += 7;
                break;
            case FilterType.landfilter:
                variableCount += 9;
                break;
            case FilterType.terracefilter:
                variableCount += 1;
                break;
            case FilterType.heightoffsetfilter:
                variableCount += 1;
                break;
            default:
                break;
        }
        return rectHeight * variableCount + offset * (variableCount - 1);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = rectHeight;

        var filterType = property.FindPropertyRelative("FilterType");
        label.text = ((FilterType)filterType.intValue).ToString();
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);

        if (!property.isExpanded)
            return;

        EditorGUI.indentLevel += 1;
        position = EditorGUI.IndentedRect(position);

        position.y += rectHeight + offset;
        EditorGUI.PropertyField(position, filterType);

        switch ((FilterType)filterType.intValue)
        {
            case FilterType.basefilter:
                position.y += rectHeight + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("scale"));
                position.y += position.height + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("octaves"));
                position.y += position.height + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("persistence"));
                position.y += position.height + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("lacunarity"));
                position.y += position.height + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("strength"));
                position.y += position.height + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("xOffset"));
                position.y += position.height + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("yOffset"));
                break;
            case FilterType.landfilter:
                position.y += rectHeight + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("scale"));
                position.y += position.height + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("octaves"));
                position.y += position.height + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("persistence"));
                position.y += position.height + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("lacunarity"));
                position.y += position.height + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("strength"));
                position.y += position.height + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("xOffset"));
                position.y += position.height + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("yOffset"));
                position.y += rectHeight + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("landMin"));
                position.y += position.height + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("landMax"));
                break;
            case FilterType.terracefilter:
                position.y += rectHeight + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("terraceCount"));
                break;
            case FilterType.heightoffsetfilter:
                position.y += rectHeight + offset;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("levelOffset"));
                break;
            default:
                break;
        }
    }
}
