#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.IO;
using BaseSDK.Extension;

public class AssetBundlesEditor : EditorWindow {
	#region MenuItems
	//Haven't tested this in a long time, but might still work. Just need to uncomment the MenuItem. But better to use Addressables anyways ;)
	//[MenuItem("Shraavan/Asset Bundles Creation")]
	[Obsolete("Use Addressables Instead")]
	public static void AssetBundleCreation() {
		var ew = EditorWindow.GetWindow<AssetBundlesEditor>(false, "AssetBundles", false);
		ew.minSize = new Vector2(500f, 600f);
		ew.Show();
	}
#endregion

	private BuildPlatform buildPlatform, switchToPlatform;
	private BuildQuality buildQuality, switchToQuality;
	private string backendPath, frontendPath;
	private bool editSelected = false, selectedSwitchAssetBundle = false, editSelectedFrontend = false;
	private Dictionary<BuildPlatform, string> buildPlatformDict = new Dictionary<BuildPlatform, string>();
	private Dictionary<BuildQuality, string> buildQualityDict = new Dictionary<BuildQuality, string>();

	public void OnEnable(){
		buildPlatform = (BuildPlatform)EditorPrefs.GetInt("buildPlatform", 0);
		buildQuality = (BuildQuality)EditorPrefs.GetInt("buildQuality", 0);
		backendPath = EditorPrefs.GetString("backendPath", @"C:\xampp\htdocs");
		frontendPath = EditorPrefs.GetString("frontendPath", Application.dataPath);
		selectedSwitchAssetBundle = false;
		switchToPlatform = (BuildPlatform)0;
		switchToQuality = (BuildQuality)0;
		if (buildPlatformDict.Count == 0)
			//ADD NEW HERE
			buildPlatformDict = new Dictionary<BuildPlatform, string>() {
			{ BuildPlatform.Android, "droid" },
			{ BuildPlatform.iOS ,"iOS" },
			{ BuildPlatform.Tizen, "tizen" },
			{ BuildPlatform.StandaloneWindows64, "win" }
		};
		if (buildQualityDict.Count == 0)
			//ADD NEW HERE
			buildQualityDict = new Dictionary<BuildQuality, string>() {
			{ BuildQuality.HD, "hd" },
			{ BuildQuality.SD,"sd" }
		};
	}

	public void OnGUI(){
		if (EditorApplication.isCompiling)
			return;

		Space(15);

#region Build Platform
		buildPlatform = (BuildPlatform)EditorGUILayout.EnumFlagsField("Build Platform", buildPlatform);
		EditorPrefs.SetInt("buildPlatform", (int)buildPlatform);

		if((int)buildPlatform == 0){
			EditorGUILayout.HelpBox("You can not build bundles without defining which platform to build it for", MessageType.Error);
			return;
		}

		if((int)buildPlatform<0){
			int bits = 0;
			foreach (var platform in Enum.GetValues(typeof(BuildPlatform)))
				if (((int)buildPlatform & (int)platform) != 0)
					bits |= (int)platform;
			buildPlatform = (BuildPlatform)bits;
		}
#endregion

		Space(10);

#region Build Quality
		buildQuality = (BuildQuality)EditorGUILayout.EnumFlagsField("Build Quality", buildQuality);
		EditorPrefs.SetInt("buildQuality", (int)buildQuality);
		Space(5);

		if((int)buildQuality==0){
			EditorGUILayout.HelpBox("You must choose a valid Graphics Quality setting to build for", MessageType.Error);
			return;
		}

		if((int)buildQuality<0){
			int bits = 0;
			foreach (var quality in Enum.GetValues(typeof(BuildQuality)))
				if (((int)buildQuality & (int)quality) != 0)
					bits |= (int)quality;
			buildQuality = (BuildQuality)bits;
		}
#endregion

#region Warning Messages
		var all = buildQuality.GetIndividualFlags().ToList();
		var allPlatform = buildPlatform.GetIndividualFlags().ToList();
		if(all.Count > 1 && allPlatform.Count > 1)
			EditorGUILayout.HelpBox(string.Format("You have selected {0} platform settings and {1} quality settings." +
				"If you choose to build assetbundles now, it will build assetbundles for each quality for each platform one of the other",
				allPlatform.Print(), all.Print()), MessageType.Warning);
		else if (allPlatform.Count > 1 || all.Count > 1)
			EditorGUILayout.HelpBox(string.Format("You have selected {0} settings. It will build all the selected ones one after the other."+
				"It may take a while, depending on the files marked with asset bundles and the platforms and quality types selected",
				all.Count > 1 ? all.Print() : allPlatform.Print()), MessageType.Warning);
#endregion

		Space(15);

#region Edit Paths
		if (editSelected){
			GUILayout.BeginHorizontal();
			GUILayout.Label("Backend Path", GUILayout.Width(100));
			backendPath = EditorGUILayout.TextField(backendPath);
			if(Directory.Exists(backendPath))
				EditorPrefs.SetString("backendPath", backendPath);
			GUI.color = Color.green;
			if(GUILayout.Button("Select Path", GUILayout.Width(100))){
				var split = backendPath.Split(new char[]{ '\\', '/' });
				backendPath = EditorUtility.OpenFolderPanel("Backend Path", Directory.Exists(backendPath) ? backendPath : @"C:\xampp\htdocs", split[split.Length - 1]);
				EditorPrefs.SetString("backendPath", backendPath);
			}
			GUI.color = Color.red;
			if (GUILayout.Button("X", GUILayout.Width(20)))
				editSelected = false;
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
			if(!Directory.Exists(backendPath))
				EditorGUILayout.HelpBox("Invalid directory, it does not exist", MessageType.Error);
		}
		else if (GUILayout.Button("Edit Backend Path"))
			editSelected = true;

		if (editSelectedFrontend){
			GUILayout.BeginHorizontal();
			GUILayout.Label("Bundle path in project", GUILayout.Width(140));
			frontendPath = EditorGUILayout.TextField(frontendPath);
			if (Directory.Exists(frontendPath))
				EditorPrefs.SetString("frontendPath", frontendPath);
			GUI.color = Color.green;
			if(GUILayout.Button("Select Path", GUILayout.Width(100))){
				var split = frontendPath.Split(new char[]{ '\\', '/' });
				frontendPath = EditorUtility.OpenFolderPanel("Frontend Path", Directory.Exists(frontendPath) ? frontendPath : Application.dataPath, split[split.Length - 1]);
				EditorPrefs.SetString("frontendPath", frontendPath);
			}
			GUI.color = Color.red;
			if (GUILayout.Button("X", GUILayout.Width(20)))
				editSelectedFrontend = false;
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
			if (!Directory.Exists(frontendPath))
				EditorGUILayout.HelpBox("Invalid directory, it does not exist", MessageType.Error);
		}
		else if (GUILayout.Button("Edit Path Of Bundles Folder in Project"))
			editSelectedFrontend = true;
#endregion

#region Switch Asset Bundle
		if(selectedSwitchAssetBundle){
			Space(10);
			GUILayout.Label("Select settings you want to switch to");
			EditorGUILayout.BeginHorizontal();
			switchToPlatform = (BuildPlatform)EditorGUILayout.EnumPopup(switchToPlatform);
			switchToQuality = (BuildQuality)EditorGUILayout.EnumPopup(switchToQuality);
			GUI.color = Color.red;
			if(GUILayout.Button("X", GUILayout.Width(20))){
				switchToPlatform = (BuildPlatform)0;
				switchToQuality = (BuildQuality)0;
				selectedSwitchAssetBundle = false;
			}
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal();
			Space(5);
			if (!((int)switchToPlatform == 0) && !((int)switchToQuality == 0)){
				if(GUILayout.Button("Switch Platform"))
					SwitchAssetBundle(string.Format("{0}-{1}", buildPlatformDict[switchToPlatform], buildQualityDict[switchToQuality]),
						switchToPlatform.ToString(), true);
			}
			else
				EditorGUILayout.HelpBox("You need to select both build platform and quality to switch to",MessageType.Error);
		}
		else if(GUILayout.Button("Switch Asset Bundle settings in project"))
			selectedSwitchAssetBundle = true;
#endregion

		Space(20);

#region Build Bundles Buttons and other warnings
		EditorGUILayout.HelpBox("Building Asset Bundles may take a while", MessageType.Warning);
		var a = Directory.Exists(backendPath);
		var b = Directory.Exists(frontendPath);
		if(b && frontendPath.StartsWith(Application.dataPath)){
			if(GUILayout.Button("Build Selected Bundles"))
				BuildBundles();
			if(GUILayout.Button("Build All Bundles"))
				BuildBundles(false,true);
		}
		else{
			EditorGUILayout.HelpBox("You need a valid path in the project where you have built all the bundles", MessageType.Error);
			return;
		}

		if (a)
		{
			if (GUILayout.Button("Build Selected Bundles and Copy Bundles to backend"))
				BuildBundles(true);
			if(GUILayout.Button("Build All Bundles and Copy Bundles to backend"))
				BuildBundles(true, true);
		}
		else{
			EditorGUILayout.HelpBox("You need a valid backend path to copy your bundles to", MessageType.Error);
			return;
		}

		if(a && b){
			if(GUILayout.Button("Copy Bundles to backend"))
				CopyBundles();
		}
		else
			EditorGUILayout.HelpBox("Either the backend or bundles directory path in the project is wrong. Please check that again", MessageType.Warning);

		EditorGUILayout.HelpBox("Building all bundles together for all qualities and all platforms will take a long time. " +
			"You might want to consider building single bundles at a time", MessageType.Warning);
#endregion
	}

	private void SwitchAssetBundle(string newVariantName, string platform, bool showDialog = false){
		if (showDialog)
		if (!EditorUtility.DisplayDialog("Sure?", string.Format("You are about to switch all asset bundles to {0} asset variant. This may take time. Are you sure?", newVariantName), "Yes", "No")){
			selectedSwitchAssetBundle = false;
			switchToPlatform = (BuildPlatform) 0;
			switchToQuality = (BuildQuality) 0;
			return;
		}
		var listOfBundles = AssetDatabase.GetAllAssetBundleNames().ToList();
		for(var i=0;i<listOfBundles.Count;i++)
			if (listOfBundles[i].Contains(newVariantName))
				listOfBundles.Remove(listOfBundles[i]);

		foreach (var name in listOfBundles){
			var assets = AssetDatabase.GetAssetPathsFromAssetBundle(name).ToList();
			for(var i=0;i<assets.Count;i++){
				var importer = AssetImporter.GetAtPath(assets[i]);
				importer.assetBundleVariant = newVariantName;
				if(assets[i].EndsWith(".psd")){
					var texImporter = importer as TextureImporter;
					texImporter.textureType = TextureImporterType.Default;

					var settings = texImporter.GetPlatformTextureSettings(platform);
					if (switchToQuality == BuildQuality.HD){
						texImporter.textureCompression = TextureImporterCompression.Uncompressed;
						settings.overridden = false;
					}
					else{
						texImporter.textureCompression = TextureImporterCompression.Compressed;
						if (platform == BuildPlatform.Android.ToString())
							settings.format = TextureImporterFormat.DXT5Crunched;
						else if (platform == BuildPlatform.Tizen.ToString())
							settings.format = TextureImporterFormat.ETC2_RGBA8;
						else if (platform == BuildPlatform.iOS.ToString())
							settings.format = TextureImporterFormat.PVRTC_RGBA4;
						settings.compressionQuality = 100;
						settings.overridden = true;
					}
					texImporter.SetPlatformTextureSettings(settings);

					texImporter.mipmapEnabled = false;
					texImporter.anisoLevel = 0;
				}
				importer.SaveAndReimport();
			}
		}
		AssetDatabase.Refresh();

		if (showDialog)
			EditorUtility.DisplayDialog("Hooray", "Switched asset bundles successfully", "Cool");
	}

	private void BuildBundles(bool copyToBackend = false, bool buildAllBundlesTogether = false){
		var qualities = new List<string>();
		var platforms = new List<string>();

		if(buildAllBundlesTogether){
			foreach (Enum quality in Enum.GetValues(typeof(BuildQuality)))
				qualities.Add(quality.ToString());
			foreach (Enum platform in Enum.GetValues(typeof(BuildPlatform)))
				platforms.Add(platform.ToString());
		}
		else{
			qualities = buildQuality.GetIndividualFlags().ToList().ConvertAll(x => x.ToString());
			platforms = buildPlatform.GetIndividualFlags().ToList().ConvertAll(x => x.ToString());
		}

		foreach (var quality in qualities){
			foreach (var platform in platforms){
				SwitchAssetBundle(string.Format("{0}-{1}",
					buildPlatformDict[(BuildPlatform)Enum.Parse(typeof(BuildPlatform), platform)],
					buildQualityDict[(BuildQuality)Enum.Parse(typeof(BuildQuality), quality)]
				), platform);
				BuildPipeline.BuildAssetBundles(frontendPath, BuildAssetBundleOptions.None, (BuildTarget)Enum.Parse(typeof(BuildTarget), platform));
			}
		}

		if (copyToBackend)
			CopyBundles();
	}

	private void CopyBundles(){
		var allFiles = Directory.GetFiles(frontendPath, "*.*", SearchOption.AllDirectories).ToList();
		var dict = new Dictionary<string, List<string>>();
		foreach (var bPlatform in buildPlatformDict)
			foreach (var bQuality in buildQualityDict){
				var l = new List<string>();
				foreach(var file in allFiles)
					if (file.EndsWith(bPlatform.Value + "-" + bQuality.Value, true, null))
						l.Add(file);
				dict.Add(bPlatform.Value + "-" + bQuality.Value, l);
			}

		foreach (var d in dict)
			foreach(var f in d.Value){
				var split = f.Split(new char[]{ '\\', '/' });
				var fName = split[split.Length - 1];
				var path = Path.Combine(backendPath, d.Key);
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
				File.Copy(f, Path.Combine(path, fName));
			}
	}

	private void Space(int gap) { GUILayout.Space(gap); }

	/// <summary>
	/// <para>BuildPlatform names should be exactly like they are in the UnityEditor's BuildTarget Enum</para>
	/// <para>This is just a wrapper enum that uses flags and allows you to select multiple build targets at a time</para>
	/// <para></para>
	/// <para>IMPORTANT</para> <para>If you add a platform here, also make changes in the OnEnable's buildPlatformDict for that entry</para>
	/// <para>Look For ADD NEW HERE in this script for easily finding it</para>
	/// </summary>
	[Flags] public enum BuildPlatform { Android = 1, iOS = 2, Tizen = 4, StandaloneWindows64 = 8 }

	/// <summary>
	/// <para>IMPORTANT</para> <para>If you add a quality here, also make changes in the OnEnable's buildQualityDict for that entry</para>
	/// <para>Look For ADD NEW HERE in this script for easily finding it</para>
	/// </summary>
	[Flags] public enum BuildQuality { HD = 1, SD = 2 }
}
#endif