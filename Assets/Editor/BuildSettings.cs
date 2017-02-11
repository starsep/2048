using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor {
    public static class BuildTargetExtension {
        public static string Name(this BuildTarget target) {
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

        public static string Extension(this BuildTarget target) {
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

    public class BuildSettings : MonoBehaviour {
        private const string BuildMenuPrefix = "Build/";
        private const string ProjectName = "2048";
        public const string Directory = "Build";
        private const string GithubPagesDirectory = "docs";

        private static readonly string[] Levels = {
            "Assets/Scenes/Game.unity"
        };

        private static string DirectoryName(string platform) {
            return ProjectName + "-" + platform + "-" + Versioning.Version();
        }

        private static string DirectoryName(BuildTarget target) {
            return DirectoryName(target.Name());
        }

        private static void MakeAction(ActionAfterBuild action, BuildTarget target) {
            var dir = DirectoryName(target);
            switch (action) {
                case ActionAfterBuild.Tar:
                    Archiver.CreateTar(dir, dir);
                    return;
                case ActionAfterBuild.Zip:
                    Archiver.CreateZip(dir, dir);
                    return;
                case ActionAfterBuild.MoveApk:
                    MoveApk(dir);
                    return;
                case ActionAfterBuild.MoveDirDeployAndZip:
                    MoveDir(dir);
                    DeployGithubPages(dir);
                    Archiver.CreateZip(dir, dir);
                    return;
                default:
                    return;
            }
        }

        private static void Build(BuildTarget target, ActionAfterBuild action) {
            PlayerSettings.runInBackground = false;
            var separator = Path.DirectorySeparatorChar;
            var platform = target.Name();
            var directory = DirectoryName(platform);
            var path = Directory + separator + directory + separator + ProjectName + target.Extension();
            var message = BuildPipeline.BuildPlayer(Levels, path, target, BuildOptions.None);
            if (string.IsNullOrEmpty(message)) {
                Debug.Log(platform + " build complete");
                MakeAction(action, target);
            }
            else {
                Debug.LogError("Error building " + platform + ":\n" + message);
            }
        }

        private enum ActionAfterBuild {
            Zip,
            Tar,
            MoveApk,
            MoveDirDeployAndZip
        }

        private static void MoveApk(string dir) {
            System.IO.Directory.SetCurrentDirectory(Directory);
            var target = dir + ".apk";
            if (File.Exists(target)) {
                File.Delete(target);
            }
            File.Move(dir + Path.DirectorySeparatorChar + ProjectName + ".apk", target);
            System.IO.Directory.Delete(dir, true);
            System.IO.Directory.SetCurrentDirectory("..");
        }

        private static void MoveDir(string dir) {
            var startDirectory = System.IO.Directory.GetCurrentDirectory();
            System.IO.Directory.SetCurrentDirectory(Directory + Path.DirectorySeparatorChar +
                                                    dir + Path.DirectorySeparatorChar + ProjectName);
            var files = System.IO.Directory.GetFiles(".");
            foreach (var filename in files) {
                var target = ".." + Path.DirectorySeparatorChar + filename;
                if (File.Exists(target)) {
                    File.Delete(target);
                }
                File.Move(filename, target);
            }
            var directories = System.IO.Directory.GetDirectories(".");
            foreach (var directory in directories) {
                var target = ".." + Path.DirectorySeparatorChar + directory;
                if (System.IO.Directory.Exists(target)) {
                    System.IO.Directory.Delete(target, true);
                }
                System.IO.Directory.Move(directory, target);
            }
            System.IO.Directory.SetCurrentDirectory("..");
            System.IO.Directory.Delete(ProjectName, true);
            System.IO.Directory.SetCurrentDirectory(startDirectory);
        }

        private static void DeployGithubPages(string dir) {
            if (System.IO.Directory.Exists(GithubPagesDirectory)) {
                System.IO.Directory.Delete(GithubPagesDirectory, true);
                System.IO.Directory.CreateDirectory(GithubPagesDirectory);
            }
            var startDirectory = System.IO.Directory.GetCurrentDirectory();
            System.IO.Directory.SetCurrentDirectory(Directory + Path.DirectorySeparatorChar + dir);
            var files = System.IO.Directory.GetFiles(".");
            var up = ".." + Path.DirectorySeparatorChar;
            foreach (var filename in files) {
                File.Copy(filename, up + up + GithubPagesDirectory + Path.DirectorySeparatorChar + filename);
            }
            var directories = System.IO.Directory.GetDirectories(".");
            foreach (var directory in directories) {
                DirectoryCopy(directory, up + up + GithubPagesDirectory + Path.DirectorySeparatorChar + directory);
            }
            System.IO.Directory.SetCurrentDirectory(startDirectory);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName) {
            var dir = new DirectoryInfo(sourceDirName);

            var dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!System.IO.Directory.Exists(destDirName)) {
                System.IO.Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files) {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }


            foreach (var subdir in dirs) {
                var temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }

        [MenuItem(BuildMenuPrefix + "Linux32")]
        public static void BuildLinux32() {
            Build(BuildTarget.StandaloneLinux, ActionAfterBuild.Tar);
        }

        [MenuItem(BuildMenuPrefix + "Linux64")]
        public static void BuildLinux64() {
            Build(BuildTarget.StandaloneLinux64, ActionAfterBuild.Tar);
        }

        [MenuItem(BuildMenuPrefix + "Win32")]
        public static void BuildWin32() {
            Build(BuildTarget.StandaloneWindows, ActionAfterBuild.Zip);
        }

        [MenuItem(BuildMenuPrefix + "Win64")]
        public static void BuildWin64() {
            Build(BuildTarget.StandaloneWindows64, ActionAfterBuild.Zip);
        }

        [MenuItem(BuildMenuPrefix + "Android")]
        public static void BuildAndroid() {
            Build(BuildTarget.Android, ActionAfterBuild.MoveApk);
        }

        [MenuItem(BuildMenuPrefix + "WebGL")]
        public static void BuildWebGl() {
            Build(BuildTarget.WebGL, ActionAfterBuild.MoveDirDeployAndZip);
        }

        [MenuItem(BuildMenuPrefix + "All")]
        public static void BuildAll() {
            BuildLinux32();
            BuildLinux64();
            BuildWin32();
            BuildWin64();
            BuildAndroid();
            BuildWebGl();
        }
    }
}
