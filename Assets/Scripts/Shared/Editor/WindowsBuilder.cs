using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Builds the Windows player from the scenes enabled in the Build Settings into Builds/Windows (git-ignored) and logs
// the result and the size, so that a texture imported without compression shows up as a jump in size.
public static class WindowsBuilder
{
    const string OutputPath = "Builds/Windows/KingBar.exe";
    const long BytesPerMegabyte = 1024 * 1024;

    [MenuItem("Tools/Build Windows")]
    public static void BuildWindows()
    {
        string[] scenes = EditorBuildSettings
            .scenes.Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();
        BuildReport report = BuildPipeline.BuildPlayer(
            new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = OutputPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None,
            }
        );

        // The Input System adds its project-wide actions to the preloaded assets and saves the Player Settings when the
        // build starts, then removes them in memory only when it ends: save again so the file on disk stays clean.
        AssetDatabase.SaveAssets();

        BuildSummary summary = report.summary;
        string message =
            $"Windows build {summary.result}: {summary.totalSize / BytesPerMegabyte} MB, "
            + $"{summary.totalErrors} errors, {summary.totalWarnings} warnings, "
            + $"{(int)summary.totalTime.TotalSeconds} s, {scenes.Length} scenes, in {OutputPath}.";

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log(message);
        }
        else
        {
            Debug.LogError(message);
        }
    }
}
