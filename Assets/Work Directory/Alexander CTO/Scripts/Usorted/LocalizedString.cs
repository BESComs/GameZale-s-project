using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct LocalizedString
{
    public string value;
        
    public static implicit operator string(LocalizedString ls)
    {
        return ls.value;
    }
 
    public static implicit operator LocalizedString(string value)
    {
        return new LocalizedString { value = value};
    }
}


#if UNITY_EDITOR

[CustomPropertyDrawer (typeof (LocalizedString))]
public class LocalizedStringDrawer : PropertyDrawer {
    
    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty (position, label, property);
        position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);
        EditorGUI.PropertyField (position, property.FindPropertyRelative("value"), GUIContent.none);
        EditorGUI.EndProperty ();
    }
}

#endif