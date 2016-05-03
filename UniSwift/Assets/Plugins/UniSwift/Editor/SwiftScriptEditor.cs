using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Diagnostics;

[CustomEditor(typeof(SwiftScript))]
public class SwiftScriptEditor : Editor {

	private SwiftScript targetScript;

	void OnEnable () {
		targetScript = target as SwiftScript;
	}

	public override void OnInspectorGUI() {
		targetScript.TargetClassName = EditorGUILayout.TextField ("Target class name", targetScript.TargetClassName);

		if (targetScript.TargetClassName == "") {
			targetScript.TargetClassName = "SwiftClass";
		}
			
		EditorGUILayout.SelectableLabel(targetScript.SwiftCode, GUILayout.MaxHeight(75));

		if (GUILayout.Button("Edit")) {
			EditScript ();
		}
	}

	#region

	private string tempSwiftFile;
	private FileSystemWatcher watcher;

	private void EditScript() {
		if (!TempFileExist()) {
			CreateTempFile ();
			CreateFileWatcher ();
		}

		Process.Start(tempSwiftFile);
	}

	private bool TempFileExist() {
		return tempSwiftFile != null && File.Exists (tempSwiftFile);
	}

	private void CreateTempFile() {
		tempSwiftFile = string.Format("{0}.swift", FileUtil.GetUniqueTempPathInProject ());

		File.WriteAllText(tempSwiftFile, targetScript.SwiftCode);
	}

	private void CreateFileWatcher() {
		watcher = new FileSystemWatcher (Path.GetDirectoryName(tempSwiftFile), "*.swift");
		watcher.Changed += (object sender, FileSystemEventArgs e) => {
			UnityEngine.Debug.Log ("Changed");
			targetScript.SwiftCode = File.ReadAllText(tempSwiftFile);
		};
	}

	#endregion
}
