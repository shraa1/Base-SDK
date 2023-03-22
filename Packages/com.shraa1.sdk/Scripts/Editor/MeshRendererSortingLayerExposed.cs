//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BaseSDK.EditorScripts {
	/// <summary>
	/// Does not work for Renderer class, probably need to use Reflection rather than serializedObject.FindProperty, but that seems expensive, rather having multiple editor scripts for types like SkinnedMeshRenderer, etc. will be easier
	/// </summary>
	[CustomEditor(typeof(MeshRenderer)), CanEditMultipleObjects]
	public class MeshRendererSortingLayerExposed : Editor {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			SerializedProperty sortingLayerID = serializedObject.FindProperty("m_SortingLayerID");
			SerializedProperty sortingOrder = serializedObject.FindProperty("m_SortingOrder");

			//Runs on every draw call, but no other alternative for now ffs
			string[] layerNames = GetSortingLayerNames();
			int[] layerID = GetSortingLayerUniqueIDs();

			int selected = -1;
			//What is selected?
			int sortID = sortingLayerID.intValue;

			for (int i = 0; i < layerID.Length; i++)
				if (sortID == layerID[i]) {
					selected = i;
					break;
				}


			//Select Default.
			if (selected == -1)
				for (int i = 0; i < layerID.Length; i++)
					if (layerID[i] == 0) {
						selected = i;
						break;
					}

			//Actually where the rendering in the inspector happens for Sorting Layer
			selected = EditorGUILayout.Popup("Sorting Layer", selected, layerNames);

			sortingLayerID.intValue = layerID[selected];


			//Actually where the rendering in the inspector happens for Sorting Order
			EditorGUILayout.PropertyField(sortingOrder, new GUIContent("Order in layer"));

			serializedObject.ApplyModifiedProperties();
		}

		public string[] GetSortingLayerNames() {
			Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			return (string[])sortingLayersProperty.GetValue(null, null);
		}

		public int[] GetSortingLayerUniqueIDs() {
			Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
			return (int[])sortingLayerUniqueIDsProperty.GetValue(null, null);
		}
	}
}
#endif