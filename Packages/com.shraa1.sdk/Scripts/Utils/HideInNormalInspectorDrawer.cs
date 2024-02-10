#if UNITY_EDITOR
namespace BaseSDK.EditorScripts {
    [UnityEditor.CustomPropertyDrawer(typeof(HideInNormalInspectorAttribute))]
    class HideInNormalInspectorDrawer : UnityEditor.PropertyDrawer {
        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) => 0f;
        public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) { }
    }
}
#endif