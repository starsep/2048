using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

class BuildSettings : MonoBehaviour {
	private const string VERSION = "v0.02";

	private static string[] levels = new string[] {
		"Assets/Scenes/Game.unity"
	};

	private static string DirectoryName(string platform) {
		return "2048-" + platform + "-" + VERSION;
	}

	private static void Build (string platform, BuildTarget target, string extension = "", string filename = "2048") {
		UnityEditor.PlayerSettings.runInBackground = false;
		string directory = DirectoryName (platform);
		char separator = Path.DirectorySeparatorChar;
		string path = "." + separator + "Build" + separator + directory + separator + filename + extension;
		string message = BuildPipeline.BuildPlayer (levels, path, target, BuildOptions.None);
		if (string.IsNullOrEmpty (message)) {
			UnityEngine.Debug.Log (platform + " build complete");
		} else {
			UnityEngine.Debug.LogError ("Error building " + platform + ":\n" + message);
		}
	}

	[MenuItem ("Build/Linux32")]
	public static void BuildLinux32 () {
		Build ("Linux32", BuildTarget.StandaloneLinux);
	}

	[MenuItem ("Build/Linux64")]
	public static void BuildLinux64 () {
		Build ("Linux64", BuildTarget.StandaloneLinux64);
	}

	[MenuItem ("Build/Win32")]
	public static void BuildWin32 () {
		Build ("Win32", BuildTarget.StandaloneWindows, ".exe");
	}

	[MenuItem ("Build/Win64")]
	public static void BuildWin64 () {
		Build ("Win64", BuildTarget.StandaloneWindows64, ".exe");
	}

	[MenuItem ("Build/Android")]
	public static void BuildAndroid () {
		Build ("Android", BuildTarget.Android, ".apk", DirectoryName("Android"));
	}

	[MenuItem ("Build/WebGL")]
	public static void BuildWebGL () {
		Build ("WebGL", BuildTarget.WebGL);
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