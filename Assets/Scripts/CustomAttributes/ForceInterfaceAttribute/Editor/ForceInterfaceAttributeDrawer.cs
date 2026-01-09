using UnityEditor;
using UnityEngine;

namespace CustomAttributes
{
    [CustomPropertyDrawer(typeof(ForceInterfaceAttribute))]
    public class ForceInterfaceAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, "Use Force Interface property only with values of Object type");
                return;
            }
            ForceInterfaceAttribute forceInterface = (ForceInterfaceAttribute) attribute;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            Object obj = EditorGUI.ObjectField(position, label + $" ({forceInterface.InterfaceType})",
                property.objectReferenceValue, typeof(Object),
                !EditorUtility.IsPersistent(property.serializedObject.targetObject));
            if (EditorGUI.EndChangeCheck())
            {
                if (obj == null)
                {
                    property.objectReferenceValue = null;
                }
                else if (forceInterface.InterfaceType.IsAssignableFrom(obj.GetType()))
                {
                    property.objectReferenceValue = obj;
                }
                else if (obj is GameObject gameObject)
                {
                    property.objectReferenceValue = gameObject.GetComponent(forceInterface.InterfaceType);
                }
            }
            EditorGUI.EndProperty();
        }
    }
}