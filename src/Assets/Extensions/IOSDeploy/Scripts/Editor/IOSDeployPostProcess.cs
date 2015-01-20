using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Diagnostics;

public class IOSDeployPostProcess  {

	#if UNITY_IPHONE
	[PostProcessBuild(100)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {

		#if UNITY_IPHONE &&  UNITY_EDITOR_WIN
		UnityEngine.Debug.LogWarning("ISD Postprocess is not available for Win");
		#endif
		
		
		#if UNITY_IPHONE && UNITY_EDITOR_OSX
		
		Process myCustomProcess = new Process();		
		myCustomProcess.StartInfo.FileName = "python";
		
		string frameworks 		= string.Join(" ", ISDSettings.Instance.frameworks.ToArray());
		string linkFlags 		= string.Join(" ", ISDSettings.Instance.linkFlags.ToArray());
		string compileFlags 	= string.Join(" ", ISDSettings.Instance.compileFlags.ToArray());
		
		
		myCustomProcess.StartInfo.Arguments = string.Format("Assets/Extensions/IOSDeploy/Scripts/Editor/post_process.py \"{0}\" \"{1}\" \"{2}\" \"{3}\"", new object[] { pathToBuiltProject, frameworks, compileFlags, linkFlags });
		myCustomProcess.StartInfo.UseShellExecute = false;
		myCustomProcess.StartInfo.RedirectStandardOutput = true;
		myCustomProcess.Start(); 
		myCustomProcess.WaitForExit();
		
		
		UnityEngine.Debug.Log("ISD Executing post process done.");
		
		#endif

	}
	#endif
}
