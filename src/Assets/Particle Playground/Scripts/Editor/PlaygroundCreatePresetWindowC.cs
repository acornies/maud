using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using ParticlePlayground;
using ParticlePlaygroundLanguage;

class PlaygroundCreatePresetWindowC : EditorWindow {

	public static GameObject particleSystemObject;
	public static Texture2D particleSystemIcon;
	public static string particleSystemName;
	public static bool childConnected = false;
	
	public static EditorWindow window;
	public static Vector2 scrollPosition;
	
	public int presetOrPublish = 0;
	public int selectedPreset = 0;
	public bool createPackage = true;
	
	public bool showError1 = false;

	public static PlaygroundLanguageC playgroundLanguage;
	
	public static void ShowWindow () {
		playgroundLanguage = PlaygroundSettingsC.GetLanguage();
		window = EditorWindow.GetWindow<PlaygroundCreatePresetWindowC>(true, playgroundLanguage.presetWizard);
        window.Show();
	}
	
	void OnEnable () {
		Initialize();
	}
	
	public void Initialize () {
		if (Selection.activeGameObject!=null) {
			particleSystemObject = Selection.activeGameObject;
			particleSystemName = Selection.activeGameObject.name;
		}
	}
	
	void OnGUI () {
		EditorGUILayout.BeginVertical();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField(playgroundLanguage.playgroundPresetWizard, EditorStyles.largeLabel, GUILayout.Height(20));
		EditorGUILayout.Separator();
		
		GUILayout.BeginVertical("box");
		int tmpPresetOrPublish = presetOrPublish;
		presetOrPublish = GUILayout.Toolbar (presetOrPublish, new string[]{playgroundLanguage.preset,playgroundLanguage.publish}, EditorStyles.toolbarButton);
		if (presetOrPublish==0)
			EditorGUILayout.HelpBox(playgroundLanguage.presetText, MessageType.Info);
		else
			EditorGUILayout.HelpBox(playgroundLanguage.publishText, MessageType.Info);
		if (tmpPresetOrPublish!=presetOrPublish && presetOrPublish==1)
			RefreshFromPresetList();
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
		
		if (presetOrPublish==0) {
			EditorGUILayout.PrefixLabel(playgroundLanguage.particleSystem);
		
			// Particle System to become a preset
			GameObject selectedObj = particleSystemObject;
			particleSystemObject = EditorGUILayout.ObjectField(particleSystemObject, typeof(GameObject), true) as GameObject;
			if (particleSystemObject!=selectedObj) {
				
				// Check if this is a Particle Playground System
				if (particleSystemObject!=null) {
				
					// Set new name if user hasn't specified one
					if (particleSystemName=="")
						particleSystemName = particleSystemObject.name;
						
					showError1 = false;
				} else {
					showError1 = true;
				}
			}
		} else {
			EditorGUILayout.PrefixLabel(playgroundLanguage.preset);
		
			// Popup of presets
			int previousSelectedObj = selectedPreset;
			selectedPreset = EditorGUILayout.Popup(selectedPreset, PlaygroundParticleWindowC.presetNames);
			if (previousSelectedObj!=selectedPreset) {
				RefreshFromPresetList();
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(playgroundLanguage.icon);
		particleSystemIcon = EditorGUILayout.ObjectField(particleSystemIcon, typeof(Texture2D), false) as Texture2D;
		GUILayout.EndHorizontal();
		particleSystemName = EditorGUILayout.TextField(playgroundLanguage.nameText, particleSystemName);
		if (presetOrPublish==0)
			childConnected = EditorGUILayout.Toggle (playgroundLanguage.childConnected, childConnected);
		
		EditorGUILayout.Separator();
		
		if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
			particleSystemName = particleSystemName.Trim();
			if (presetOrPublish==0 && particleSystemObject!=null && particleSystemName!="") {
				string tmpName = particleSystemObject.name;
				particleSystemObject.name = particleSystemName;
				if (AssetDatabase.LoadAssetAtPath("Assets/"+PlaygroundParticleSystemInspectorC.playgroundSettings.playgroundPath+PlaygroundParticleSystemInspectorC.playgroundSettings.presetPath+particleSystemName+".prefab", typeof(GameObject))) {
					if (EditorUtility.DisplayDialog(playgroundLanguage.presetWithSameNameFound, 
						particleSystemName+" "+playgroundLanguage.presetWithSameNameFoundText, 
                		playgroundLanguage.yes, 
                        playgroundLanguage.no))
							CreatePreset();
				} else CreatePreset();
				particleSystemObject.name = tmpName;
			} else
			if (presetOrPublish==1 && particleSystemName!="") {
				if (AssetDatabase.LoadAssetAtPath("Assets/"+PlaygroundParticleWindowC.presetNames[selectedPreset]+"/Resources/Presets/"+PlaygroundParticleWindowC.presetNames[selectedPreset]+".prefab", typeof(GameObject))) {
					if (EditorUtility.DisplayDialog(playgroundLanguage.presetWithSameNameFound, 
						PlaygroundParticleWindowC.presetNames[selectedPreset]+" "+playgroundLanguage.presetWithSameNameFoundText, 
                        playgroundLanguage.yes, 
                        playgroundLanguage.no))
							CreatePublicPreset(true);
				} else CreatePublicPreset(false);
			}
		}
		GUILayout.EndVertical();
		
		// Space for error messages
		if (showError1 && particleSystemObject!=null)
			EditorGUILayout.HelpBox(playgroundLanguage.gameObjectIsNotPlayground, MessageType.Error);
		
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
	}
	
	public void CreatePreset () {

		if (childConnected) {

			// Try to child all connected objects to the particle system
			PlaygroundParticlesC[] ppScript = particleSystemObject.GetComponentsInChildren<PlaygroundParticlesC>();

			int i=0;
			for (int x = 0; x<ppScript.Length; x++) {
				for (; i<ppScript[x].manipulators.Count; i++)
					if (ppScript[x].manipulators[i].transform.available)
						ppScript[x].manipulators[i].transform.transform.parent = particleSystemObject.transform;
				for (i = 0; i<ppScript[x].paint.paintPositions.Count; i++)
					if (ppScript[x].paint.paintPositions[i].parent)
						ppScript[x].paint.paintPositions[i].parent.parent = particleSystemObject.transform;
				for (i = 0; i<ppScript[x].states.Count; i++)
					if (ppScript[x].states[i].stateTransform)
						ppScript[x].states[i].stateTransform.parent = particleSystemObject.transform;
				if (ppScript[x].sourceTransform)
					ppScript[x].sourceTransform.parent = particleSystemObject.transform;
				if (ppScript[x].worldObject.transform)
					ppScript[x].worldObject.transform.parent = particleSystemObject.transform;
				if (ppScript[x].skinnedWorldObject.transform)
					ppScript[x].skinnedWorldObject.transform.parent = particleSystemObject.transform;
			}
		}

		// Save it as prefab in presetPath and import
		GameObject particleSystemPrefab = PrefabUtility.CreatePrefab("Assets/"+PlaygroundParticleSystemInspectorC.playgroundSettings.playgroundPath+PlaygroundParticleSystemInspectorC.playgroundSettings.presetPath+particleSystemObject.name+".prefab", particleSystemObject, ReplacePrefabOptions.ReplaceNameBased);
		AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(particleSystemIcon as UnityEngine.Object), "Assets/"+PlaygroundParticleSystemInspectorC.playgroundSettings.playgroundPath+PlaygroundParticleSystemInspectorC.playgroundSettings.iconPath+particleSystemPrefab.name+".png");
		AssetDatabase.ImportAsset("Assets/"+PlaygroundParticleSystemInspectorC.playgroundSettings.playgroundPath+PlaygroundParticleSystemInspectorC.playgroundSettings.iconPath+particleSystemPrefab.name+".png");
		
		// Close window
		window.Close();
	}
	
	public void CreatePublicPreset (bool isOverwrite) {
		
		// Create folders
		if (!isOverwrite && !createPackage) {
			AssetDatabase.CreateFolder("Assets", "Playground Preset - "+particleSystemName);
			AssetDatabase.CreateFolder("Assets/Playground Preset - "+particleSystemName, "Resources");
			AssetDatabase.CreateFolder("Assets/Playground Preset - "+particleSystemName+"/Resources", "Presets");
		}
		
		// Path to the new resources/presets folder
		string publicPresetPath = "Assets/Playground Preset - "+particleSystemName+"/Resources/Presets/";
				
		
		
		// Get dependencies
		List<string> presetDependencies = new List<string>();
		string[] tmpPresetDependencies = AssetDatabase.GetDependencies(new string[]{AssetDatabase.GetAssetPath(PlaygroundParticleWindowC.presetObjects[selectedPreset].presetObject as UnityEngine.Object)});
		
		// Copy the icon file
		if (!createPackage)
			AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(particleSystemIcon as UnityEngine.Object), publicPresetPath+particleSystemName+".png");
		else
			presetDependencies.Add(AssetDatabase.GetAssetPath(particleSystemIcon as UnityEngine.Object));
		for (int i = 0; i<tmpPresetDependencies.Length; i++) {
			
			
			// Check that the operation won't disturb any of the unnecessary files from the framework
			if (!tmpPresetDependencies[i].Contains("PlaygroundBrushPresetInspectorC.cs") && 
			    !tmpPresetDependencies[i].Contains("PlaygroundCreateBrushWindowC.cs") && 
			    !tmpPresetDependencies[i].Contains("PlaygroundCreatePresetWindowC.cs") && 
			    !tmpPresetDependencies[i].Contains("PlaygroundInspectorC.cs") && 
			    !tmpPresetDependencies[i].Contains("PlaygroundParticleSystemInspectorC.cs") && 
			    !tmpPresetDependencies[i].Contains("PlaygroundParticleWindowC.cs") && 
			    !tmpPresetDependencies[i].Contains("PlaygroundBrushPresetC.cs")
			) {
				
				// It's the preset, rename if user specified another name
				if (Path.GetFileName(tmpPresetDependencies[i]) == PlaygroundParticleWindowC.presetNames[selectedPreset]+".prefab" && particleSystemName!=Path.GetFileName(tmpPresetDependencies[i])) {
					AssetDatabase.Refresh();
					AssetDatabase.RenameAsset(publicPresetPath+Path.GetFileName(tmpPresetDependencies[i]), particleSystemName);
					tmpPresetDependencies[i] = Path.GetDirectoryName(tmpPresetDependencies[i])+"/"+particleSystemName+".prefab";
				}
				
				// Add to preset dependencies list
				presetDependencies.Add(tmpPresetDependencies[i]);
			}
		}
		
		// Check that necessary files are in
		if (!presetDependencies.Contains("PlaygroundC.cs"))
			presetDependencies.Add("Assets/"+PlaygroundParticleSystemInspectorC.playgroundSettings.playgroundPath+PlaygroundParticleSystemInspectorC.playgroundSettings.scriptPath+"PlaygroundC.cs");
		if (!presetDependencies.Contains("PlaygroundParticlesC.cs"))
			presetDependencies.Add("Assets/"+PlaygroundParticleSystemInspectorC.playgroundSettings.playgroundPath+PlaygroundParticleSystemInspectorC.playgroundSettings.scriptPath+"PlaygroundParticlesC.cs");
		if (!presetDependencies.Contains("Playground Manager"))
			presetDependencies.Add("Assets/"+PlaygroundParticleSystemInspectorC.playgroundSettings.playgroundPath+"Resources/Playground Manager.prefab");

		// Refresh the project
		AssetDatabase.Refresh();
		
		// User wants to create a package of all dependency assets
		if (createPackage) {
			string assetPackagePath = EditorUtility.SaveFilePanel ("Save Preset", "", "Playground Preset - "+particleSystemName+".unitypackage", "unitypackage");
			if (assetPackagePath.Length!=0)
				AssetDatabase.ExportPackage(presetDependencies.ToArray(), assetPackagePath, ExportPackageOptions.Interactive);

			// Refresh the project
			AssetDatabase.Refresh();
			
			// Close window
			if (assetPackagePath.Length!=0)
				window.Close();

		// All dependency assets will be moved instead (deprecated)	
		} else {
			Undo.RecordObjects(EditorUtility.CollectDeepHierarchy (new Object[]{PlaygroundParticleWindowC.presetObjects[selectedPreset].presetObject as UnityEngine.Object}) as UnityEngine.Object[], "Move all dependencies of preset");
			for (int i = 0; i<presetDependencies.Count; i++) {
				AssetDatabase.MoveAsset(presetDependencies[i], publicPresetPath+Path.GetFileName(presetDependencies[i]));
			}
			
			// Select the prefab
			EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(publicPresetPath+PlaygroundParticleWindowC.presetNames[selectedPreset]+".prefab", typeof(UnityEngine.Object)) as UnityEngine.Object);
		
		}

	}
	
	public void RefreshFromPresetList () {
		if (PlaygroundParticleWindowC.presetNames.Length==0) return;
		particleSystemIcon = Resources.LoadAssetAtPath(PlaygroundParticleSystemInspectorC.playgroundSettings.playgroundPath+PlaygroundParticleSystemInspectorC.playgroundSettings.iconPath+PlaygroundParticleWindowC.presetNames[selectedPreset]+".png", typeof(Texture2D)) as Texture2D;
		particleSystemName = PlaygroundParticleWindowC.presetNames[selectedPreset];
	}
}