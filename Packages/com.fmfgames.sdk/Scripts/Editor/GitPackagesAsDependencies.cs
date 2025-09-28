using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace BaseSDK.EditorScripts {
	public class GitPackagesAsDependencies : AssetPostprocessor {
		public string[] m_PackagesToImport;
		private const string DependencyFileName = "GitDependencies.txt";

		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
			foreach (var asset in importedAssets)
				if (Path.GetFileName(asset).Equals(DependencyFileName))
					ImportDependencies(asset);
		}

		private static void ImportDependencies(string assetPath) {
			var fullPath = Path.GetFullPath(assetPath);
			var lines = File.ReadAllLines(fullPath)
				.Select(l => l.Trim())
				.Where(l => !string.IsNullOrEmpty(l) && !l.StartsWith("#"))
				.ToList();

			foreach (var line in lines) {
				Debug.Log($"[GitDependencies] Adding package: {line}");
				Client.Add(line);
			}
		}
	}
}