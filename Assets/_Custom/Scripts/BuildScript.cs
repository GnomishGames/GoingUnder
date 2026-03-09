using UnityEditor;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Linq;

public class BuildScript
{
    static string GameName = "MyGame";

    [MenuItem("Build/Build All Platforms")]
    public static void BuildAllPlatforms()
    {
        string version = PlayerSettings.bundleVersion;

        string root = Path.Combine("Builds", version);

        Debug.Log("Starting full build pipeline...");
        Debug.Log("Version: " + version);

        string[] scenes = GetScenes();

        BuildWindows(root, scenes);
        BuildMac(root, scenes);
        BuildLinux(root, scenes);

        Debug.Log("All builds completed.");
    }

    static string[] GetScenes()
    {
        return EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
    }

    static void BuildWindows(string root, string[] scenes)
    {
        Debug.Log("Switching to Windows build target...");

        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone,
            BuildTarget.StandaloneWindows64);

        string buildFolder = Path.Combine(root, "Windows");
        Directory.CreateDirectory(buildFolder);

        string buildPath = Path.Combine(buildFolder, GameName + ".exe");

        Debug.Log("Building Windows player...");

        BuildPipeline.BuildPlayer(
            scenes,
            buildPath,
            BuildTarget.StandaloneWindows64,
            BuildOptions.None);

        Zip(buildFolder, Path.Combine(root, GameName + "_Windows.zip"));
    }

    static void BuildMac(string root, string[] scenes)
    {
        Debug.Log("Switching to Mac build target...");

        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone,
            BuildTarget.StandaloneOSX);

        string buildFolder = Path.Combine(root, "Mac");
        Directory.CreateDirectory(buildFolder);

        string buildPath = Path.Combine(buildFolder, GameName + ".app");

        Debug.Log("Building Mac player...");

        BuildPipeline.BuildPlayer(
            scenes,
            buildPath,
            BuildTarget.StandaloneOSX,
            BuildOptions.None);

        Zip(buildFolder, Path.Combine(root, GameName + "_Mac.zip"));
    }

    static void BuildLinux(string root, string[] scenes)
    {
        Debug.Log("Switching to Linux build target...");

        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone,
            BuildTarget.StandaloneLinux64);

        string buildFolder = Path.Combine(root, "Linux");
        Directory.CreateDirectory(buildFolder);

        string buildPath = Path.Combine(buildFolder, GameName + ".x86_64");

        Debug.Log("Building Linux player...");

        BuildPipeline.BuildPlayer(
            scenes,
            buildPath,
            BuildTarget.StandaloneLinux64,
            BuildOptions.None);

        Zip(buildFolder, Path.Combine(root, GameName + "_Linux.zip"));
    }

    static void Zip(string sourceFolder, string zipFile)
    {
        if (File.Exists(zipFile))
            File.Delete(zipFile);

        Debug.Log("Creating zip: " + zipFile);

        ZipFile.CreateFromDirectory(sourceFolder, zipFile);
    }
}