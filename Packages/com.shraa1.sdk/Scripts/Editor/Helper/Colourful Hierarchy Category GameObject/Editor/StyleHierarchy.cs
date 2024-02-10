using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MStudio {
	[InitializeOnLoad]
	public class StyleHierarchy {
		static string[] dataArray;//Find ColorPalette GUID
		static List<string> paths = new List<string>();//Get ColorPalette(ScriptableObject) path
		static List<ColorPalette> colorPalettes = new List<ColorPalette>();

		static StyleHierarchy () {
			dataArray = AssetDatabase.FindAssets("t:ColorPalette");

			if (dataArray.Length >= 1) {
				paths.Clear();
				colorPalettes.Clear();
				for (var i = 0; i < dataArray.Length; i++) {
					//We have only one color palette, so we use dataArray[0] to get the path of the file
					paths.Add(AssetDatabase.GUIDToAssetPath(dataArray[i]));
					colorPalettes.Add(AssetDatabase.LoadAssetAtPath<ColorPalette>(paths[i]));
				}

				EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindow;
			}
		}

		private static void OnHierarchyWindow (int instanceID, Rect selectionRect) {
			//To make sure there is no error on the first time the tool imported in project
			if (dataArray.Length == 0) return;

			Object instance = EditorUtility.InstanceIDToObject(instanceID);

			if (instance != null) {
				for (var i = 0; i < colorPalettes.Count; i++) {
					for (int j = 0; j < colorPalettes[i].colorDesigns.Count; j++) {
						var design = colorPalettes[i].colorDesigns[j];

						//Check if the name of each gameObject is begin with keyChar in colorDesigns list.
						if (instance.name.StartsWith(design.keyChar)) {
							//Remove the symbol(keyChar) from the name.
							string newName = instance.name.Substring(design.keyChar.Length);
							//Draw a rectangle as a background, and set the color.
							EditorGUI.DrawRect(selectionRect, design.backgroundColor);

							//Create a new GUIStyle to match the desing in colorDesigns list.
							GUIStyle newStyle = new GUIStyle {
								alignment = design.textAlignment,
								fontStyle = design.fontStyle,
								normal = new GUIStyleState() {
									textColor = design.textColor,
								}
							};

							//Draw a label to show the name in upper letters and newStyle.
							//If you don't like all capital latter, you can remove ".ToUpper()".
							EditorGUI.LabelField(selectionRect, design.allUppercase ? newName.ToUpper() : newName, newStyle);
						}
					}
				}
			}
		}
	}
}