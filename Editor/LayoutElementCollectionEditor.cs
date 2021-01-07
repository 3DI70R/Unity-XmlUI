using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    [CustomEditor(typeof(LayoutElementCollection))]
    public class LayoutElementCollectionEditor : Editor
    {
        private ReorderableList reorderableList;

        public override void OnInspectorGUI()
        {
            if (reorderableList == null)
            {
                var property = serializedObject.FindProperty(nameof(LayoutElementCollection.individualElements));

                reorderableList = new ReorderableList(serializedObject, property, true, false, true, true)
                {
                    drawHeaderCallback = (rect) =>
                    {
                        EditorGUI.LabelField(rect, "Individual elements");
                    },
                    onAddCallback = list =>
                    {
                        property.InsertArrayElementAtIndex(property.arraySize);
                    },
                    drawElementCallback = (rect, index, active, focused) =>
                    {
                        var element = property.GetArrayElementAtIndex(index);
                        var layoutProp = element.FindPropertyRelative(nameof(LayoutElementCollection.Entry.layoutXml));
                        var nameProp = element.FindPropertyRelative(nameof(LayoutElementCollection.Entry.name));
                        var labelSpacing = 4;
                        var labelWidth = rect.width / 3f - labelSpacing;

                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, labelWidth, rect.height), 
                            nameProp, GUIContent.none);
                        
                        EditorGUI.PropertyField(new Rect(rect.x + labelWidth + labelSpacing, rect.y, rect.width - labelWidth, rect.height), 
                            layoutProp, GUIContent.none);
                    }
                };
            }
    
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LayoutElementCollection.collections)));
            EditorGUILayout.Space();
            reorderableList.DoLayoutList();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}