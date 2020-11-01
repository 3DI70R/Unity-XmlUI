using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace ThreeDISevenZeroR.XmlUI
{
    [CustomEditor(typeof(LayoutInflater))]
    public class LayoutInflaterEditor : Editor
    {
        private readonly HashSet<string> expandedGroups = new HashSet<string>();
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var inflater = (LayoutInflater) target;

            EditorGUILayout.Space(32);
            EditorGUILayout.LabelField("Element attributes", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.BeginVertical();

            foreach (var element in inflater.RegisteredElements)
            {
                var wasExpanded = expandedGroups.Contains(element.Name);
                var isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(wasExpanded, element.Name);

                if (isExpanded != wasExpanded)
                {
                    if (isExpanded)
                    {
                        expandedGroups.Add(element.Name);
                    }
                    else
                    {
                        expandedGroups.Remove(element.Name);
                    }
                }

                if (isExpanded)
                    DrawAttributes(element);
                
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawAttributes(IXmlElementInfo element)
        {
            foreach (var groups in element.Attributes.GroupBy(e => e.TargetType))
            {
                EditorGUILayout.LabelField(groups.Key.Name, EditorStyles.miniLabel);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                foreach (var attr in groups)
                {
                    EditorGUILayout.LabelField($"{attr.Name}:", attr.FormatHint, 
                        EditorStyles.wordWrappedMiniLabel);
                }
                
                EditorGUILayout.EndVertical();
            }
        }
    }
}