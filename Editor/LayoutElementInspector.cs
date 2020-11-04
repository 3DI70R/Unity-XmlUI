using UnityEditor;

namespace ThreeDISevenZeroR.XmlUI
{
    [CustomEditor(typeof(LayoutElement))]
    public class LayoutElementInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var element = (LayoutElement) target;

            EditorGUILayout.Space(8);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(element.GetYogaInfo(), EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();
        }
    }
}