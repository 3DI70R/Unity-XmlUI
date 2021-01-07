using UnityEditor;

namespace ThreeDISevenZeroR.XmlUI
{
    [CustomEditor(typeof(XmlLayoutElement))]
    public class LayoutElementInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var element = (XmlLayoutElement) target;

            var visibility = element.Visibility;
            var newVisibility = (Visibility) EditorGUILayout.EnumPopup(visibility);

            if (visibility != newVisibility)
                element.Visibility = newVisibility;

            EditorGUILayout.Space(8);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(element.GetYogaInfo(), EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();
        }
    }
}