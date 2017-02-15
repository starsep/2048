using UnityEditor;
using UnityEngine;

namespace Editor {
    public class Versioning : MonoBehaviour {
        private const char VersionSeparator = '.';
        private const string VersioningMenuPrefix = "Versioning/";

        public static string Version() {
            return PlayerSettings.bundleVersion;
        }

        private static void SetVersion(int major, int minor, int fix) {
            PlayerSettings.bundleVersion = major.ToString() + VersionSeparator + minor + VersionSeparator + fix;
            PlayerSettings.Android.bundleVersionCode++;
        }

        private static int Major() {
            return int.Parse(Version().Split(VersionSeparator)[0]);
        }

        private static int Minor() {
            return int.Parse(Version().Split(VersionSeparator)[1]);
        }

        private static int Fix() {
            return int.Parse(Version().Split(VersionSeparator)[2]);
        }

        private static void DebugBumpVersion(string old) {
            Debug.Log("Version bumped from " + old + " to " + Version());
        }

        [MenuItem(VersioningMenuPrefix + "Bump Major")]
        public static void BumpMajor() {
            var oldVersion = Version();
            SetVersion(Major() + 1, 0, 0);
            DebugBumpVersion(oldVersion);
        }

        [MenuItem(VersioningMenuPrefix + "Bump Minor")]
        public static void BumpMinor() {
            var oldVersion = Version();
            SetVersion(Major(), Minor() + 1, 0);
            DebugBumpVersion(oldVersion);
        }

        [MenuItem(VersioningMenuPrefix + "Bump Fix")]
        public static void BumpFix() {
            var oldVersion = Version();
            SetVersion(Major(), Minor(), Fix() + 1);
            DebugBumpVersion(oldVersion);
        }
    }
}
