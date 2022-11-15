using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ActionKey))]
public class ActionKeyDrawer : PropertyDrawer
{
    SerializedProperty _keys;
    SerializedProperty _name;
    SerializedProperty _isKeysDown;
    SerializedProperty _attemptAction;
    int keyCount = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect foldOut = new Rect(position.x, position.y, position.size.x, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldOut, property.isExpanded, label);

        _keys = property.FindPropertyRelative("keyList");
        keyCount = _keys == null? 0 : _keys.arraySize;
        _name = property.FindPropertyRelative("actionName");
        _isKeysDown = property.FindPropertyRelative("isKeysDown");
        _attemptAction = property.FindPropertyRelative("attemptAction");


        if (property.isExpanded)
        {
            DrawNameProperty(position);
            DrawKeysProperty(position, property);
            //DrawKeysDownProperty(position);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int totalLines = 1;
        if (property.isExpanded)
        {
            totalLines += 2;
            totalLines += keyCount;
        }
        


        return EditorGUIUtility.singleLineHeight * totalLines;
    }

    private void DrawNameProperty(Rect position)
    {
        EditorGUIUtility.labelWidth = 80f;

        float xPos = position.min.x;
        float yPos = position.min.y + EditorGUIUtility.singleLineHeight;
        float width = position.size.x;
        float height = EditorGUIUtility.singleLineHeight;

        Rect drawArea = new Rect(xPos, yPos, width*.4f, height);
        EditorGUI.PropertyField(drawArea, _name, new GUIContent("Action Name"), true);
    }
    private void DrawKeysProperty(Rect position, SerializedProperty property)
    {
        float xPos = position.min.x;
        float yPos = position.min.y + EditorGUIUtility.singleLineHeight * 2;
        float width = position.size.x;
        float height = EditorGUIUtility.singleLineHeight;

        Rect keyLabelRect = new Rect(xPos, yPos, width * .4f, height);
        EditorGUI.LabelField(keyLabelRect, new GUIContent("Keys"));


        Rect drawArea = new Rect(xPos, yPos, width * .4f, height + (EditorGUIUtility.singleLineHeight * (keyCount + 2)));
        EditorGUIUtility.labelWidth = 40f;
        if (GUI.Button(new Rect(xPos + EditorGUIUtility.labelWidth, yPos, 20, 20), "+"))
        {
            _keys.InsertArrayElementAtIndex(keyCount);
            _isKeysDown.InsertArrayElementAtIndex(keyCount);
        }
        if (GUI.Button(new Rect(xPos + EditorGUIUtility.labelWidth + 20, yPos, 20, 20), "-"))
        {
            _keys.DeleteArrayElementAtIndex(keyCount - 1);
            _isKeysDown.DeleteArrayElementAtIndex(keyCount - 1);
        }
        keyCount = _keys == null ? 0 : _keys.arraySize;
        for (int i = 0; i < keyCount; i++)
        {
            Color color = Color.black;
            EditorGUI.PropertyField(new Rect(xPos, yPos + (1 + i) * EditorGUIUtility.singleLineHeight, width * .4f, EditorGUIUtility.singleLineHeight), _keys.GetArrayElementAtIndex(i), true);
            if (_isKeysDown.GetArrayElementAtIndex(i).boolValue)
                color = Color.red;
            EditorGUI.DrawRect(new Rect(xPos + width * .4f, yPos + (1 + i) * EditorGUIUtility.singleLineHeight, width * .1f, EditorGUIUtility.singleLineHeight), color);
        }
    }
}
