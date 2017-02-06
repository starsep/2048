using UnityEngine;
using UnityEditor;
using System.Collections;

class BuildSettings : MonoBehaviour {
	private static string[] levels = new string[] {
		"Assets/Scenes/Game.unity"
	};

	private static void Build (string name, string path, BuildTarget target) {
		UnityEditor.PlayerSettings.runInBackground = false;
		string message = BuildPipeline.BuildPlayer (
			                 levels,
			                 path,
			                 target,
			                 BuildOptions.None);

		if (string.IsNullOrEmpty (message)) {
			UnityEngine.Debug.Log (name + " build complete");
		} else {
			UnityEngine.Debug.LogError ("Error building " + name + ":\n" + message);
		}
	}

	[MenuItem ("Build/Linux32")]
	public static void BuildLinux32 () {
		Build ("Linux32", "./Build/Linux32/2048", BuildTarget.StandaloneLinux);
	}

	[MenuItem ("Build/Linux64")]
	public static void BuildLinux64 () {
		Build ("Linux64", "./Build/Linux64/2048", BuildTarget.StandaloneLinux64);
	}

	[MenuItem ("Build/Win32")]
	public static void BuildWin32 () {
		Build ("Win32", "./Build/Win32/2048.exe", BuildTarget.StandaloneWindows);
	}

	[MenuItem ("Build/Win64")]
	public static void BuildWin64 () {
		Build ("Win64", "./Build/Win64/2048.exe", BuildTarget.StandaloneWindows64);
	}

	[MenuItem ("Build/Android")]
	public static void BuildAndroid () {
		Build ("Android", "./Build/Android/2048", BuildTarget.Android);
	}

	[MenuItem ("Build/WebGL")]
	public static void BuildWebGL () {
		Build ("WebGL", "./Build/WebGL/2048", BuildTarget.WebGL);
	}

	[MenuItem ("Build/All")]
	public static void BuildAll () {
		BuildLinux32 ();
		BuildLinux64 ();
		BuildWin32 ();
		BuildWin64 ();
		BuildAndroid ();
		BuildWebGL ();
	}
}