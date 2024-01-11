using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SPG.InspectorButton.Editor
{
    [CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
    public class InspectorButtonDrawer : PropertyDrawer
    {
        private const BindingFlags MethodFlags 
            = BindingFlags.Instance 
            | BindingFlags.NonPublic 
            | BindingFlags.Public;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var buttonArea = position;
            buttonArea.height = EditorGUIUtility.singleLineHeight;

            var certainAttribute = (InspectorButtonAttribute) attribute;

            foreach (var methodName in certainAttribute.MethodNames)
            {
                if (GUI.Button(buttonArea, methodName))
                {
                    var targetObject = property.serializedObject.targetObject;
                    
                    var method = targetObject
                        .GetType()
                        .GetMethod(methodName, MethodFlags);
                    
                    if (method != null)
                    {
                        method.Invoke(targetObject, null);
                    }
                    else
                    {
                        Debug.LogError("Method not found: " + methodName);
                    }
                }
                buttonArea.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            position.y += buttonArea.y;
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var certainAttribute = (InspectorButtonAttribute) attribute;
            var fieldHeight = base.GetPropertyHeight(property, label);
            var buttonsHeight = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * certainAttribute.MethodNames.Length;
            
            return fieldHeight + buttonsHeight;
        }
    }
}