using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(ISDSettings))]
public class ISDSettingsEditor : Editor
{

	
	GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is the Plugin version.  If you have problems or compliments please include this so that we know exactly which version to look out for.");
	GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical questions, feel free to drop us an e-mail.");
	



	public override void OnInspectorGUI () {


		GUI.changed = false;


		EditorGUILayout.LabelField("iOS Deploy Settings", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		Frameworks();
		EditorGUILayout.Space();
		LinkerFlags();
		EditorGUILayout.Space();
		CompilerFlags();
		EditorGUILayout.Space();
		AboutGUI();

		if(GUI.changed) {
			DirtyEditor();
		}

	}


	private string _newFramework = string.Empty;
	private void Frameworks() {


        ISDSettings.Instance.IsFrameworksSettingOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsFrameworksSettingOpen, "Frameworks");

		if(ISDSettings.Instance.IsFrameworksSettingOpen) {
			if (ISDSettings.Instance.frameworks.Count == 0) {

				EditorGUILayout.HelpBox("No Frameworks added", MessageType.None);
			}


			foreach(string framework in ISDSettings.Instance.frameworks) {
				

				EditorGUILayout.BeginVertical (GUI.skin.box);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(framework, GUILayout.Height(18));
				EditorGUILayout.Space();

				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.frameworks.Remove(framework);
					return;
				}

				EditorGUILayout.EndHorizontal();


				EditorGUILayout.EndVertical ();
				
			}
				
		
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField("Add New Framework");
            _newFramework = EditorGUILayout.TextField(_newFramework, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();





			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
                if (!ISDSettings.Instance.frameworks.Contains(_newFramework) && _newFramework.Length > 0) {
                    ISDSettings.Instance.frameworks.Add(_newFramework);
                    _newFramework = string.Empty;
				}
				
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	private string NewLinkerFlag = string.Empty;
	private void LinkerFlags() {
		
		
		ISDSettings.Instance.IsLinkerSettingOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsLinkerSettingOpen, "Linker Flags");
		
		if(ISDSettings.Instance.IsLinkerSettingOpen) {
			if (ISDSettings.Instance.frameworks.Count == 0) {
				
				EditorGUILayout.HelpBox("No Linker Flags added", MessageType.None);
			}
			

			foreach(string flasg in ISDSettings.Instance.linkFlags) {
				
				
				EditorGUILayout.BeginVertical (GUI.skin.box);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(flasg, GUILayout.Height(18));
				EditorGUILayout.Space();
				
				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.linkFlags.Remove(flasg);
					return;
				}
				
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.EndVertical ();
				
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.LabelField("Add New Flag");
			NewLinkerFlag = EditorGUILayout.TextField(NewLinkerFlag, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();
			
			
			
			
			
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.linkFlags.Contains(NewLinkerFlag) && NewLinkerFlag.Length > 0) {
					ISDSettings.Instance.linkFlags.Add(NewLinkerFlag);
					NewLinkerFlag = string.Empty;
				}
				
			}
			EditorGUILayout.EndHorizontal();
		}
	}


	private string NewCompilerFlag = string.Empty;
	private void CompilerFlags() {
		
		
		ISDSettings.Instance.IsCompilerSettingsOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsCompilerSettingsOpen, "Compiler Flags");
		
		if(ISDSettings.Instance.IsCompilerSettingsOpen) {
			if (ISDSettings.Instance.frameworks.Count == 0) {
				EditorGUILayout.HelpBox("No Linker Flags added", MessageType.None);
			}
			
			

			foreach(string flasg in ISDSettings.Instance.compileFlags) {
				
				
				EditorGUILayout.BeginVertical (GUI.skin.box);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(flasg, GUILayout.Height(18));
				
				EditorGUILayout.Space();
				
				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.compileFlags.Remove(flasg);
					return;
				}
				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical ();
				
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.LabelField("Add New Flag");
			NewCompilerFlag = EditorGUILayout.TextField(NewCompilerFlag, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();

			
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.compileFlags.Contains(NewCompilerFlag) && NewCompilerFlag.Length > 0) {
					ISDSettings.Instance.compileFlags.Add(NewCompilerFlag);
					NewCompilerFlag = string.Empty;
				}
				
			}
			EditorGUILayout.EndHorizontal();
		}
	}



	private void AboutGUI() {
		EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
		EditorGUILayout.Space();
		
		SelectableLabelField(SdkVersion, ISDSettings.VERSION_NUMBER);
		SelectableLabelField(SupportEmail, "stans.assets@gmail.com");
	}
	
	private void SelectableLabelField(GUIContent label, string value) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(label, GUILayout.Width(180), GUILayout.Height(16));
		EditorGUILayout.SelectableLabel(value, GUILayout.Height(16));
		EditorGUILayout.EndHorizontal();
	}


	



	private static void DirtyEditor() {
			#if UNITY_EDITOR
		EditorUtility.SetDirty(ISDSettings.Instance);
			#endif
	}
}
