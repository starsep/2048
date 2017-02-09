using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public static class BuildTargetExtension {
	public static string Name (this BuildTarget target) {
		switch (target) {
		case BuildTarget.StandaloneLinux:
			return "Linux32";
		case BuildTarget.StandaloneLinux64:
			return "Linux64";
		case BuildTarget.StandaloneWindows:
			return "Win32";
		case BuildTarget.StandaloneWindows64:
			return "Win64";
		case BuildTarget.Android:
			return "Android";
		case BuildTarget.WebGL:
			return "WebGL";
		default:
			return "Unknown";
		}
	}

	public static string Extension (this BuildTarget target) {
		switch (target) {
		case BuildTarget.StandaloneWindows:
		case BuildTarget.StandaloneWindows64:
			return ".exe";
		case BuildTarget.Android:
			return ".apk";
		default:
			return "";
		}
	}
}

class BuildSettings : MonoBehaviour {
	public const string VERSION = "v0.2.0";
	public const string PROJECT_NAME = "2048";
	public const string DIRECTORY = "Build";
	private const string GITHUB_PAGES_DIRECTORY = "docs";

	private static string[] levels = new string[] {
		"Assets/Scenes/Game.unity"
	};

	private static string DirectoryName (string platform) {
		return PROJECT_NAME + "-" + platform + "-" + VERSION;
	}

	private static string DirectoryName (BuildTarget target) {
		return DirectoryName (target.Name ());
	}

	private static void MakeAction (ActionAfterBuild action, BuildTarget target, string extension = "") {
		string dir = DirectoryName (target);
		switch (action) {
		case ActionAfterBuild.Tar:
			Archiver.CreateTar (dir, dir);
			return;
		case ActionAfterBuild.Zip:
			Archiver.CreateZip (dir, dir);
			return;
		case ActionAfterBuild.MoveApk:
			MoveApk (dir);
			return;
		case ActionAfterBuild.MoveDirDeployAndZip:
			MoveDir (dir);
			DeployGithubPages (dir);
			Archiver.CreateZip (dir, dir);
			return;
		default:
			return;
		}
	}

	private static void Build (BuildTarget target, ActionAfterBuild action) {
		UnityEditor.PlayerSettings.runInBackground = false;
		char separator = Path.DirectorySeparatorChar;
		string platform = target.Name ();
		string directory = DirectoryName (platform);
		string path = DIRECTORY + separator + directory + separator + PROJECT_NAME + target.Extension ();
		string message = BuildPipeline.BuildPlayer (levels, path, target, BuildOptions.None);
		if (string.IsNullOrEmpty (message)) {
			UnityEngine.Debug.Log (platform + " build complete");
			MakeAction (action, target);
		} else {
			UnityEngine.Debug.LogError ("Error building " + platform + ":\n" + message);
		}
	}

	private enum ActionAfterBuild {
		Zip,
		Tar,
		MoveApk,
		MoveDirDeployAndZip,
		None
	}

	private static void MoveApk (string dir) {
		Directory.SetCurrentDirectory (BuildSettings.DIRECTORY);
		string target = dir + ".apk";
		if (File.Exists (target)) {
			File.Delete (target);
		}
		File.Move (dir + Path.DirectorySeparatorChar + PROJECT_NAME + ".apk", target);
		Directory.Delete (dir, true);
		Directory.SetCurrentDirectory ("..");
	}

	private static void MoveDir (string dir) {
		string startDirectory = Directory.GetCurrentDirectory ();
		Directory.SetCurrentDirectory (BuildSettings.DIRECTORY + Path.DirectorySeparatorChar +
		dir + Path.DirectorySeparatorChar + PROJECT_NAME);
		string[] files = Directory.GetFiles (".");
		foreach (string filename in files) {
			string target = ".." + Path.DirectorySeparatorChar + filename;
			if (File.Exists (target)) {
				File.Delete (target);
			}
			File.Move (filename, target);
		}
		string[] directories = Directory.GetDirectories (".");
		foreach (string directory in directories) {
			string target = ".." + Path.DirectorySeparatorChar + directory;
			if (Directory.Exists (target)) {
				Directory.Delete (target, true);
			}
			Directory.Move (directory, target);
		}
		Directory.SetCurrentDirectory ("..");
		Directory.Delete (PROJECT_NAME, true);
		Directory.SetCurrentDirectory (startDirectory);
	}

	private static void DeployGithubPages (string dir) {
		if (Directory.Exists (GITHUB_PAGES_DIRECTORY)) {
			Directory.Delete (GITHUB_PAGES_DIRECTORY, true);
			Directory.CreateDirectory (GITHUB_PAGES_DIRECTORY);
		}
		string startDirectory = Directory.GetCurrentDirectory ();
		Directory.SetCurrentDirectory (BuildSettings.DIRECTORY + Path.DirectorySeparatorChar + dir);
		string[] files = Directory.GetFiles (".");
		string up = ".." + Path.DirectorySeparatorChar;
		foreach (string filename in files) {
			File.Copy (filename, up + up + GITHUB_PAGES_DIRECTORY + Path.DirectorySeparatorChar + filename);
		}
		string[] directories = Directory.GetDirectories (".");
		foreach (string directory in directories) {
			DirectoryCopy (directory, up + up + GITHUB_PAGES_DIRECTORY + Path.DirectorySeparatorChar + directory);
		}
		Directory.SetCurrentDirectory (startDirectory);
	}

	private static void DirectoryCopy (string sourceDirName, string destDirName) {
		DirectoryInfo dir = new DirectoryInfo (sourceDirName);

		DirectoryInfo[] dirs = dir.GetDirectories ();
		// If the destination directory doesn't exist, create it.
		if (!Directory.Exists (destDirName)) {
			Directory.CreateDirectory (destDirName);
		}

		// Get the files in the directory and copy them to the new location.
		FileInfo[] files = dir.GetFiles ();
		foreach (FileInfo file in files) {
			string temppath = Path.Combine (destDirName, file.Name);
			file.CopyTo (temppath, false);
		}


		foreach (DirectoryInfo subdir in dirs) {
			string temppath = Path.Combine (destDirName, subdir.Name);
			DirectoryCopy (subdir.FullName, temppath);
		}
	}

	[MenuItem ("Build/Linux32")]
	public static void BuildLinux32 () {
		Build (BuildTarget.StandaloneLinux, ActionAfterBuild.Tar);
	}

	[MenuItem ("Build/Linux64")]
	public static void BuildLinux64 () {
		Build (BuildTarget.StandaloneLinux64, ActionAfterBuild.Tar);
	}

	[MenuItem ("Build/Win32")]
	public static void BuildWin32 () {
		Build (BuildTarget.StandaloneWindows, ActionAfterBuild.Zip);
	}

	[MenuItem ("Build/Win64")]
	public static void BuildWin64 () {
		Build (BuildTarget.StandaloneWindows64, ActionAfterBuild.Zip);
	}

	[MenuItem ("Build/Android")]
	public static void BuildAndroid () {
		Build (BuildTarget.Android, ActionAfterBuild.MoveApk);
	}

	[MenuItem ("Build/WebGL")]
	public static void BuildWebGL () {
		Build (BuildTarget.WebGL, ActionAfterBuild.MoveDirDeployAndZip);
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
