﻿using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using ParticlePlayground;
using ParticlePlaygroundLanguage;

class PlaygroundParticleWindowC : EditorWindow {
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PlaygroundParticleWindow variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	// Update check
	WWW versionRequest;
	string onlineVersion;

	// Presets
	public static List<PresetObjectC> presetObjects;
	public static int presetListStyle = 0;
	public static int presetExampleUser = 0;
	public static string[] presetNames;

	// Settings
	public static PlaygroundSettingsC playgroundSettings;
	public static PlaygroundLanguageC playgroundLanguage;

	// Editor Window specific
	public static Vector2 scrollPosition;
	public static GUIStyle presetButtonStyle;
	public static string searchString = "";
	public static GUIStyle toolbarSearchSkin;
	public static GUIStyle toolbarSearchButtonSkin;
	public static GUIStyle boxStyle;
	bool didSendVersionCheck = false;
	bool updateAvailable = false;
	bool assetsFound = true;
	
	[MenuItem ("Window/Particle Playground")]
	public static void ShowWindow () {
		PlaygroundParticleWindowC window = GetWindow<PlaygroundParticleWindowC>();
		window.title = "Playground";
        window.Show();
	}
	
	public void OnEnable () {
		Initialize();
	}
	
	public void OnProjectChange () {
		Initialize();
	}
	
	public void OnFocus () {
		Initialize();
	}
	
	public void Initialize () {
		presetButtonStyle = new GUIStyle();
		presetButtonStyle.stretchWidth = true;
		presetButtonStyle.stretchHeight = true;

		// Load settings
		playgroundSettings = PlaygroundSettingsC.SetReference();
		PlaygroundInspectorC.playgroundSettings = playgroundSettings;
		PlaygroundParticleSystemInspectorC.playgroundSettings = playgroundSettings;

		// Load language
		playgroundLanguage = PlaygroundSettingsC.GetLanguage();
		PlaygroundInspectorC.playgroundLanguage = playgroundLanguage;
		PlaygroundParticleSystemInspectorC.playgroundLanguage = playgroundLanguage;

		List<Object> particlePrefabs = new List<Object>();

		// Get all user presets (bound to Resources)
		int userPrefabCount = 0;
		Object[] resourcePrefabs = (Object[])Resources.LoadAll ("Presets", typeof(GameObject));
		foreach (Object thisResourcePrefab in resourcePrefabs) {
			particlePrefabs.Add (thisResourcePrefab);
			userPrefabCount++;
		}

		// Get all example presets
		string assetsDataPath = Application.dataPath;
		string editorPresetPath = assetsDataPath+"/"+playgroundSettings.playgroundPath+playgroundSettings.examplePresetPath;
		string[] editorPresetPaths = new string[0];

		if (Directory.Exists (assetsDataPath+"/"+playgroundSettings.playgroundPath)) {
			if (Directory.Exists (editorPresetPath))
				editorPresetPaths = Directory.GetFiles (editorPresetPath);
			assetsFound = true;
		} else {
			assetsFound = false;
			playgroundSettings.settingsFoldout = true;
			playgroundSettings.settingsPathFoldout = true;
			return;
		}

		if (editorPresetPaths!=null) {
			foreach (string thisPresetPath in editorPresetPaths) {
				string convertedPresetPath = thisPresetPath.Substring(assetsDataPath.Length-6);
				Object presetPathObject = (Object)AssetDatabase.LoadAssetAtPath(convertedPresetPath, typeof(Object));
				if (presetPathObject!=null && (presetPathObject.GetType().Name)=="GameObject") {
					particlePrefabs.Add (presetPathObject);
				}
			}
		}

		Texture2D particleImageDefault = Resources.LoadAssetAtPath("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.iconPath+"Default.png", typeof(Texture2D)) as Texture2D;
		Texture2D particleImage;
		
		presetObjects = new List<PresetObjectC>();
		int i = 0;
		for (; i<particlePrefabs.Count; i++) {
			presetObjects.Add(new PresetObjectC());
			presetObjects[i].presetObject = particlePrefabs[i];
			presetObjects[i].example = (i>=userPrefabCount);
			particleImage = Resources.LoadAssetAtPath("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.iconPath+presetObjects[i].presetObject.name+".png", typeof(Texture2D)) as Texture2D;
			
			// Try the asset location if we didn't find it in regular editor folder
			if (particleImage==null) {
				particleImage = Resources.LoadAssetAtPath(Path.GetDirectoryName(AssetDatabase.GetAssetPath(presetObjects[i].presetObject as UnityEngine.Object))+"/"+presetObjects[i].presetObject.name+".png", typeof(Texture2D)) as Texture2D;
			}
			
			// Finally use the specified icon (or the default)
			if (particleImage!=null)
				presetObjects[i].presetImage = particleImage;
			else if (particleImageDefault!=null)
				presetObjects[i].presetImage = particleImageDefault;
		}
		presetNames = new string[presetObjects.Count];
		for (i = 0; i<presetNames.Length; i++) {
			presetNames[i] = presetObjects[i].presetObject.name;
			
			// Filter on previous search
			presetObjects[i].unfiltered = (searchString==""?true:presetNames[i].ToLower().Contains(searchString.ToLower()));
		}

		if (playgroundSettings.checkForUpdates && !didSendVersionCheck)
			versionRequest = new WWW(playgroundSettings.versionUrl);
	}

	void CheckUpdate () {

		// Check if an update is available
		if (!didSendVersionCheck) {
			if (versionRequest==null)
				versionRequest = new WWW(playgroundSettings.versionUrl);
			if (versionRequest.isDone) {
				if (versionRequest.error==null) {
					onlineVersion = versionRequest.text;
					updateAvailable = (onlineVersion!="" && float.Parse (onlineVersion)>float.Parse (PlaygroundC.version));
				}
				didSendVersionCheck = true;
			}
		}
	}
	
	void OnGUI () {
		if (boxStyle==null)
			boxStyle = GUI.skin.FindStyle("box");
		if (toolbarSearchSkin==null) {
			toolbarSearchSkin = GUI.skin.FindStyle("ToolbarSeachTextField");
			if (toolbarSearchButtonSkin==null)
				toolbarSearchButtonSkin = GUI.skin.FindStyle("ToolbarSeachCancelButton");
		}
		EditorGUILayout.BeginVertical();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
		EditorGUILayout.BeginVertical(boxStyle);
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField(playgroundLanguage.playgroundName+" "+PlaygroundC.version+PlaygroundC.specialVersion, EditorStyles.largeLabel, GUILayout.Height(20));
		EditorGUILayout.Separator();

		// New version message
		if (playgroundSettings.checkForUpdates && !didSendVersionCheck)
			CheckUpdate();
		if (playgroundSettings.checkForUpdates && updateAvailable) {
			EditorGUILayout.BeginVertical(boxStyle);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(playgroundLanguage.updateAvailable);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("x", EditorStyles.miniButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(18)})){
				updateAvailable = false;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.LabelField(onlineVersion+" "+playgroundLanguage.updateAvailableText, EditorStyles.wordWrappedMiniLabel);
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(playgroundLanguage.unityAssetStore, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
				Application.OpenURL("com.unity3d.kharma:content/13325");
			}

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();
		}
		EditorGUILayout.BeginVertical(boxStyle);
		
		// Create New-buttons
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button(playgroundLanguage.newParticlePlaygroundSystem, EditorStyles.toolbarButton)){
			if (PlaygroundC.reference==null)
				CreateManager();
			PlaygroundParticlesC newParticlePlayground = PlaygroundC.Particle();
			Selection.activeGameObject = newParticlePlayground.particleSystemGameObject;
		}
		GUI.enabled = PlaygroundC.reference==null;
		if(GUILayout.Button(playgroundLanguage.playgroundManager, EditorStyles.toolbarButton)){
			PlaygroundC.ResourceInstantiate("Playground Manager");
		}
		GUI.enabled = true;
		if(GUILayout.Button(playgroundLanguage.presetWizard, EditorStyles.toolbarButton)){
			PlaygroundCreatePresetWindowC.ShowWindow();
		}
		/*if(GUILayout.Button(playgroundLanguage.copyWizard, EditorStyles.toolbarButton)){
			PlaygroundCopyWindowC.ShowWindow();
		}*/
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndVertical();

		if (assetsFound) {

			// Presets
			EditorGUILayout.BeginVertical(boxStyle);
			playgroundSettings.presetsFoldout = GUILayout.Toggle(playgroundSettings.presetsFoldout, playgroundLanguage.presets, EditorStyles.foldout);
			if (playgroundSettings.presetsFoldout) {
				EditorGUILayout.BeginHorizontal("Toolbar");
				
				// Search
				string prevSearchString = searchString;
				searchString = GUILayout.TextField(searchString, toolbarSearchSkin, new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Width(Mathf.FloorToInt(Screen.width)-120), GUILayout.MinWidth(100)});
				if (GUILayout.Button("", toolbarSearchButtonSkin)) {
					searchString = "";
					GUI.FocusControl(null);
				}

				if (prevSearchString!=searchString) {
					for (int p = 0; p<presetNames.Length; p++)
						presetObjects[p].unfiltered = (searchString==""?true:presetNames[p].ToLower().Contains(searchString.ToLower()));
				}
				
				EditorGUILayout.Separator();
				presetExampleUser = GUILayout.Toolbar (presetExampleUser, new string[]{playgroundLanguage.all,playgroundLanguage.user,playgroundLanguage.examples}, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
				GUILayout.Label ("", EditorStyles.toolbarButton);
				presetListStyle = GUILayout.Toolbar (presetListStyle, new string[]{playgroundLanguage.icons,playgroundLanguage.list}, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginVertical(boxStyle);
				
				if (presetObjects.Count>0) {
					if (presetListStyle==0) EditorGUILayout.BeginHorizontal();
					int rows = 1;
					int iconwidths = 0;
					int skippedPresets = 0;
					for (int i = 0; i<presetObjects.Count; i++) {
						
						// Filter out by search
						if (!presetObjects[i].unfiltered || presetExampleUser==2 && !presetObjects[i].example || presetExampleUser==1 && presetObjects[i].example) {
							skippedPresets++;
							continue;
						}
						// Preset Object were destroyed
						if (presetObjects[i].presetObject==null) {
							Initialize();
							return;
						}
						
						// List
						if (presetListStyle==1) {
							EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(24));
							EditorGUILayout.BeginHorizontal();
							GUILayout.Label(i.ToString(), EditorStyles.miniLabel, new GUILayoutOption[]{GUILayout.Width(18)});
							EditorGUILayout.LabelField(presetObjects[i].example?"("+playgroundLanguage.example+")":"("+playgroundLanguage.user+")", EditorStyles.miniLabel, new GUILayoutOption[]{GUILayout.MaxWidth(70)});
							if (GUILayout.Button (presetObjects[i].presetObject.name, EditorStyles.label)) {
								CreatePresetObject(i);
							}
							EditorGUILayout.Separator();
							if(GUILayout.Button(presetObjects[i].example?playgroundLanguage.convertTo+" "+playgroundLanguage.user:playgroundLanguage.convertTo+" "+playgroundLanguage.example, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Height(16)})){
								if (presetObjects[i].example) {
									AssetDatabase.MoveAsset (AssetDatabase.GetAssetPath(presetObjects[i].presetObject), "Assets/"+playgroundSettings.playgroundPath+playgroundSettings.presetPath+presetObjects[i].presetObject.name+".prefab");
								} else {
									AssetDatabase.MoveAsset (AssetDatabase.GetAssetPath(presetObjects[i].presetObject), "Assets/"+playgroundSettings.playgroundPath+playgroundSettings.examplePresetPath+presetObjects[i].presetObject.name+".prefab");
								}
							}
							if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								RemovePreset(presetObjects[i].presetObject);
							}
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.EndVertical();
						}
						
						
						if (presetListStyle==0) {
						
							// Break row for icons
							rows = Mathf.FloorToInt(Screen.width/322);
							iconwidths+=322;
							if (iconwidths>Screen.width && i>0) {
								iconwidths=322;
								EditorGUILayout.EndHorizontal();
								EditorGUILayout.BeginHorizontal();
							}
							
							if (Screen.width>=644) {
								EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth (Mathf.CeilToInt(Screen.width/rows)-(44/rows)));
							} else
								EditorGUILayout.BeginVertical(boxStyle);
							EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(46));
							if(GUILayout.Button(presetObjects[i].presetImage, EditorStyles.miniButton, new GUILayoutOption[]{GUILayout.Width(44), GUILayout.Height(44)})){
								CreatePresetObject(i);
							}
							EditorGUILayout.BeginVertical();

							if (GUILayout.Button(presetObjects[i].presetObject.name, EditorStyles.label, GUILayout.Height(18)))
								CreatePresetObject(i);
							EditorGUILayout.LabelField(presetObjects[i].example?playgroundLanguage.example:playgroundLanguage.user, EditorStyles.miniLabel);
							EditorGUILayout.EndVertical();
							GUILayout.FlexibleSpace();
							EditorGUILayout.BeginVertical();

							if(GUILayout.Button("x", EditorStyles.miniButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(18)})){
								RemovePreset(presetObjects[i].presetObject);
							}
							EditorGUILayout.EndVertical();
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.EndVertical();

						}
					}
					if (skippedPresets==presetObjects.Count) {
						if (searchString!="") {
							EditorGUILayout.HelpBox(playgroundLanguage.searchNoPresetFound+" \""+searchString+"\".", MessageType.Info);
						} else {
							if (presetExampleUser==0)
								EditorGUILayout.HelpBox(playgroundLanguage.noPresetsFound, MessageType.Info);
							else if (presetExampleUser==1)
								EditorGUILayout.HelpBox(playgroundLanguage.noUserPresetsFound, MessageType.Info);
							else if (presetExampleUser==2)
								EditorGUILayout.HelpBox(playgroundLanguage.noExamplePresetsFound+" \""+"Assets/"+playgroundSettings.playgroundPath+playgroundSettings.examplePresetPath+"\"", MessageType.Info);
						}
					}
					if (presetListStyle==0) EditorGUILayout.EndHorizontal();
				} else {
					EditorGUILayout.HelpBox(playgroundLanguage.noPresetsFoundInProject+" \""+"Assets/"+playgroundSettings.playgroundPath+playgroundSettings.examplePresetPath+"\"", MessageType.Info);
				}
				
				if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
					PlaygroundCreatePresetWindowC.ShowWindow();
				}
				
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndVertical();
		} else {
			EditorGUILayout.HelpBox (playgroundLanguage.noAssetsFoundMessage, MessageType.Warning);
		}

		PlaygroundInspectorC.RenderPlaygroundSettings();

		// Playground Settings
		EditorGUILayout.BeginVertical(boxStyle);
		playgroundSettings.settingsFoldout = GUILayout.Toggle(playgroundSettings.settingsFoldout, playgroundLanguage.settings, EditorStyles.foldout);
		if (playgroundSettings.settingsFoldout) {
			EditorGUILayout.BeginVertical(boxStyle);
			if (playgroundSettings==null || playgroundSettings.IsInstance()) {
				EditorGUILayout.HelpBox(playgroundLanguage.noSettingsFile+" \""+PlaygroundSettingsC.settingsPath+"\".", MessageType.Warning);
				if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
					PlaygroundSettingsC.New();
					Initialize();
					return;
				}
			} else {
				playgroundSettings.checkForUpdates = EditorGUILayout.Toggle(playgroundLanguage.checkForUpdates, playgroundSettings.checkForUpdates);
				playgroundSettings.presetsHasPrefabConnection = EditorGUILayout.Toggle (playgroundLanguage.prefabConnection, playgroundSettings.presetsHasPrefabConnection);
				EditorGUILayout.Separator();

				EditorGUILayout.BeginVertical(boxStyle);
				playgroundSettings.settingsLanguageFoldout = GUILayout.Toggle(playgroundSettings.settingsLanguageFoldout, playgroundLanguage.language, EditorStyles.foldout);
				if (playgroundSettings.settingsLanguageFoldout) {
					if (playgroundSettings.languages.Count>0) {


						bool setThisLoadFrom = false;
						int currentLanguageCount = playgroundSettings.languages.Count;
						for (int i = 0; i<playgroundSettings.languages.Count; i++) {
							if (currentLanguageCount!=playgroundSettings.languages.Count) {
								Initialize();
								return;
							}
							setThisLoadFrom = false;
							EditorGUILayout.BeginHorizontal(boxStyle);

							if (playgroundSettings.languages[i]==null) {
								playgroundSettings.languages.RemoveAt (i);
								Initialize();
								return;
							}

							if (playgroundSettings.selectedLanguage == i) {
								EditorGUILayout.Toggle (true, EditorStyles.radioButton, GUILayout.Width(14));
							} else
								setThisLoadFrom = EditorGUILayout.Toggle (setThisLoadFrom, EditorStyles.radioButton, GUILayout.Width(14));
							if (setThisLoadFrom || GUILayout.Button (playgroundSettings.languages[i].languageName+" ("+playgroundSettings.languages[i].languageNameSeenByEnglish+")", EditorStyles.label)) {
								playgroundSettings.selectedLanguage = i;
								playgroundLanguage = PlaygroundSettingsC.GetLanguage();
								PlaygroundInspectorC.playgroundLanguage = playgroundLanguage;
								PlaygroundParticleSystemInspectorC.playgroundLanguage = playgroundLanguage;
							}

							if(GUILayout.Button(playgroundLanguage.edit, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Height(16)}))
								Selection.activeObject = (Object)playgroundSettings.languages[i];
							if(GUILayout.Button(playgroundLanguage.export, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Height(16)})) 
								PlaygroundSettingsC.ExportLanguage(i);
							if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								if (EditorUtility.DisplayDialog(
									playgroundLanguage.remove+" "+playgroundSettings.languages[i].languageName+"?",
									playgroundLanguage.removeLanguage+" "+playgroundSettings.languages[i].languageName+"?", 
									playgroundLanguage.yes, playgroundLanguage.no)) {
									AssetDatabase.MoveAssetToTrash (AssetDatabase.GetAssetPath (playgroundSettings.languages[i]));
									playgroundSettings.languages.RemoveAt (i);
									if (playgroundSettings.selectedLanguage == i)
										playgroundSettings.selectedLanguage = 0;
									Initialize();
									return;
								}
							}

							EditorGUILayout.EndHorizontal();
						}
					} else {
						EditorGUILayout.HelpBox(playgroundLanguage.noLanguageFound, MessageType.Warning);
					}
					EditorGUILayout.BeginHorizontal();
					if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
						playgroundSettings.languages.Add (PlaygroundLanguageC.New());
						EditorUtility.SetDirty(playgroundSettings);
						Initialize();
						return;
					}
					if(GUILayout.Button(playgroundLanguage.install, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
						PlaygroundInstallLanguageWindowC.ShowWindow();
					}
					EditorGUILayout.EndHorizontal();
				}

				EditorGUILayout.EndVertical();

				// Limits
				EditorGUILayout.BeginVertical(boxStyle);
				playgroundSettings.limitsFoldout = GUILayout.Toggle(playgroundSettings.limitsFoldout, playgroundLanguage.editorLimits, EditorStyles.foldout);
				if (playgroundSettings.limitsFoldout) {
					EditorGUILayout.Separator();
					playgroundSettings.maximumAllowedTransitionTime = EditorGUILayout.FloatField(playgroundLanguage.transitionTime, playgroundSettings.maximumAllowedTransitionTime);
					EditorGUILayout.Separator();
					playgroundSettings.maximumAllowedParticles = EditorGUILayout.IntField(playgroundLanguage.particleCount, playgroundSettings.maximumAllowedParticles);
					playgroundSettings.maximumAllowedLifetime = EditorGUILayout.FloatField(playgroundLanguage.particleLifetime, playgroundSettings.maximumAllowedLifetime);
					playgroundSettings.maximumAllowedRotation = EditorGUILayout.FloatField(playgroundLanguage.particleRotation, playgroundSettings.maximumAllowedRotation);
					playgroundSettings.maximumAllowedSize = EditorGUILayout.FloatField(playgroundLanguage.particleSize, playgroundSettings.maximumAllowedSize);
					playgroundSettings.maximumAllowedScale = EditorGUILayout.FloatField(playgroundLanguage.particleScale, playgroundSettings.maximumAllowedScale);
					playgroundSettings.maximumAllowedSourceScatter = EditorGUILayout.FloatField(playgroundLanguage.sourceScatter, playgroundSettings.maximumAllowedSourceScatter);
					EditorGUILayout.Separator();
					playgroundSettings.maximumAllowedDeltaMovementStrength = EditorGUILayout.FloatField(playgroundLanguage.deltaMovementStrength, playgroundSettings.maximumAllowedDeltaMovementStrength);
					playgroundSettings.maximumAllowedDamping = EditorGUILayout.FloatField(playgroundLanguage.damping, playgroundSettings.maximumAllowedDamping);
					playgroundSettings.maximumAllowedInitialVelocity = EditorGUILayout.FloatField(playgroundLanguage.initialVelocity, playgroundSettings.maximumAllowedInitialVelocity);
					playgroundSettings.maximumAllowedVelocity = EditorGUILayout.FloatField(playgroundLanguage.velocity, playgroundSettings.maximumAllowedVelocity);
					playgroundSettings.maximumAllowedStretchSpeed = EditorGUILayout.FloatField(playgroundLanguage.stretchSpeed, playgroundSettings.maximumAllowedStretchSpeed);
					EditorGUILayout.Separator();
					playgroundSettings.maximumAllowedCollisionRadius = EditorGUILayout.FloatField(playgroundLanguage.collisionRadius, playgroundSettings.maximumAllowedCollisionRadius);
					playgroundSettings.maximumAllowedMass = EditorGUILayout.FloatField(playgroundLanguage.mass, playgroundSettings.maximumAllowedMass);
					playgroundSettings.maximumAllowedBounciness = EditorGUILayout.FloatField(playgroundLanguage.bounciness, playgroundSettings.maximumAllowedBounciness);
					playgroundSettings.maximumAllowedDepth = EditorGUILayout.FloatField(playgroundLanguage.depth2D, playgroundSettings.maximumAllowedDepth);
					EditorGUILayout.Separator();
					playgroundSettings.minimumAllowedUpdateRate = EditorGUILayout.IntField(playgroundLanguage.updateRate, playgroundSettings.minimumAllowedUpdateRate);
					playgroundSettings.maximumRenderSliders = EditorGUILayout.FloatField(playgroundLanguage.renderSliders, playgroundSettings.maximumRenderSliders);
					EditorGUILayout.Separator();
					playgroundSettings.maximumAllowedPaintPositions = EditorGUILayout.IntField(playgroundLanguage.paintPositions, playgroundSettings.maximumAllowedPaintPositions);
					playgroundSettings.minimumAllowedBrushScale = EditorGUILayout.FloatField(playgroundLanguage.brushSizeMin, playgroundSettings.minimumAllowedBrushScale);
					playgroundSettings.maximumAllowedBrushScale = EditorGUILayout.FloatField(playgroundLanguage.brushSizeMax, playgroundSettings.maximumAllowedBrushScale);
					playgroundSettings.minimumEraserRadius = EditorGUILayout.FloatField(playgroundLanguage.eraserSizeMin, playgroundSettings.minimumEraserRadius);
					playgroundSettings.maximumEraserRadius = EditorGUILayout.FloatField(playgroundLanguage.eraserSizeMax, playgroundSettings.maximumEraserRadius);
					playgroundSettings.maximumAllowedPaintSpacing = EditorGUILayout.FloatField(playgroundLanguage.paintSpacing, playgroundSettings.maximumAllowedPaintSpacing);
					EditorGUILayout.Separator();
					playgroundSettings.maximumAllowedManipulatorSize = EditorGUILayout.FloatField(playgroundLanguage.manipulatorSize, playgroundSettings.maximumAllowedManipulatorSize);
					playgroundSettings.maximumAllowedManipulatorStrength = EditorGUILayout.FloatField(playgroundLanguage.manipulatorStrength, playgroundSettings.maximumAllowedManipulatorStrength);
					playgroundSettings.maximumAllowedManipulatorStrengthEffectors = EditorGUILayout.FloatField (playgroundLanguage.manipulatorStrengthEffectors, playgroundSettings.maximumAllowedManipulatorStrengthEffectors);
					playgroundSettings.maximumAllowedManipulatorZeroVelocity = EditorGUILayout.FloatField(playgroundLanguage.manipulatorZeroVelocityStrength, playgroundSettings.maximumAllowedManipulatorZeroVelocity);
					EditorGUILayout.Separator();
				}
				EditorGUILayout.EndVertical();

				// Paths
				EditorGUILayout.BeginVertical(boxStyle);
				playgroundSettings.settingsPathFoldout = GUILayout.Toggle(playgroundSettings.settingsPathFoldout, playgroundLanguage.paths, EditorStyles.foldout);
				if (playgroundSettings.settingsPathFoldout) {
					string currentPlaygroundPath = playgroundSettings.playgroundPath;
					string currentExamplePresetPath = playgroundSettings.examplePresetPath;
					string currentPresetPath = playgroundSettings.presetPath;

					playgroundSettings.playgroundPath = EditorGUILayout.TextField(playgroundLanguage.playgroundPath, playgroundSettings.playgroundPath);
					playgroundSettings.languagePath = EditorGUILayout.TextField(playgroundLanguage.languagesPath, playgroundSettings.languagePath);
					playgroundSettings.presetPath = EditorGUILayout.TextField(playgroundLanguage.userPresetPath, playgroundSettings.presetPath);
					playgroundSettings.examplePresetPath = EditorGUILayout.TextField(playgroundLanguage.examplePresetPath, playgroundSettings.examplePresetPath);
					playgroundSettings.iconPath = EditorGUILayout.TextField(playgroundLanguage.presetIconPath, playgroundSettings.iconPath);
					playgroundSettings.brushPath = EditorGUILayout.TextField(playgroundLanguage.brushPath, playgroundSettings.brushPath);
					playgroundSettings.scriptPath = EditorGUILayout.TextField(playgroundLanguage.scriptPath, playgroundSettings.scriptPath);
					playgroundSettings.versionUrl = EditorGUILayout.TextField(playgroundLanguage.updateUrl, playgroundSettings.versionUrl);

					if (currentPlaygroundPath!=playgroundSettings.playgroundPath || currentExamplePresetPath!=playgroundSettings.examplePresetPath || currentPresetPath!=playgroundSettings.presetPath)
						Initialize();
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndVertical();
		
		GUILayout.FlexibleSpace();
		
		EditorGUILayout.BeginVertical(boxStyle);
		EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(playgroundLanguage.officialSite, EditorStyles.toolbarButton)) {
				Application.OpenURL ("http://playground.polyfied.com/");
			}
			if (GUILayout.Button(playgroundLanguage.assetStore, EditorStyles.toolbarButton)) {
				Application.OpenURL("com.unity3d.kharma:content/13325");
			}
			if (GUILayout.Button(playgroundLanguage.manual, EditorStyles.toolbarButton)) {
				Application.OpenURL ("http://www.polyfied.com/products/Particle-Playground-2-Next-Manual.pdf");
			}
			if (GUILayout.Button(playgroundLanguage.supportForum, EditorStyles.toolbarButton)) {
				Application.OpenURL ("http://forum.unity3d.com/threads/215154-Particle-Playground");
			}
			if (GUILayout.Button(playgroundLanguage.mailSupport, EditorStyles.toolbarButton)) {
				Application.OpenURL ("mailto:support@polyfied.com");
			}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		
		GUILayout.EndScrollView();
		EditorGUILayout.EndVertical();

		if (GUI.changed)
			EditorUtility.SetDirty(playgroundSettings);
	}

	public void RemovePreset (Object presetObject) {
		if (EditorUtility.DisplayDialog(playgroundLanguage.removePreset, 
		                                presetObject.name+" "+playgroundLanguage.removePresetText, 
		                                playgroundLanguage.yes, 
		                                playgroundLanguage.no)) {
			AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(presetObject));
		}
	}

	// User created preset
	public void CreatePresetObject (int i) {
		PlaygroundParticlesC instantiatedPreset = InstantiateEditorPreset(presetObjects[i].presetObject);
		if (instantiatedPreset!=null)
			instantiatedPreset.EditorYieldSelect();
	}
	
	// Instantiate a preset by name reference
	public static PlaygroundParticlesC InstantiateEditorPreset (Object presetObject) {
		GameObject presetGo;
		if (playgroundSettings.presetsHasPrefabConnection) {
			presetGo = (GameObject)PrefabUtility.InstantiatePrefab(presetObject);
		} else {
			presetGo = (GameObject)Instantiate(presetObject);
		}
		PlaygroundParticlesC presetParticles = presetGo.GetComponent<PlaygroundParticlesC>();
		if (presetParticles!=null) {
			if (PlaygroundC.reference==null)
				PlaygroundC.ResourceInstantiate("Playground Manager");
			if (PlaygroundC.reference) {
				if (PlaygroundC.reference.autoGroup && presetParticles.particleSystemTransform.parent==null)
					presetParticles.particleSystemTransform.parent = PlaygroundC.referenceTransform;
				PlaygroundC.particlesQuantity++;
				presetParticles.particleSystemId = PlaygroundC.particlesQuantity;
			}
			presetGo.name = presetObject.name;
			return presetParticles;
		} else return null;
	}
	
	public void CreateManager () {
		PlaygroundC.ResourceInstantiate("Playground Manager");
	}
}

public class PresetObjectC {
	public Object presetObject;
	public Texture2D presetImage;
	public bool unfiltered = true;
	public bool example = false;
}