using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Diagnostics;
using System.IO;


/// <summary>
/// Generates a build verision name at build time
/// </summary>
public class BuildVersionGenerator : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string gitHash = GetGitCommitHash();
        string version = PlayerSettings.bundleVersion;
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        string finalVersion = $"v{version} ({gitHash}) - {timestamp}";

        string path = "Assets/Resources/build.txt";

        File.WriteAllText(path, finalVersion);

        UnityEngine.Debug.Log("Build version generated: " + finalVersion);

        AssetDatabase.Refresh();
    }

    private string GetGitCommitHash()
    {
        Process process = new Process();
        process.StartInfo.FileName = "git";
        process.StartInfo.Arguments = "rev-parse --short HEAD";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
        string result = process.StandardOutput.ReadToEnd().Trim();
        process.WaitForExit();

        return result;
    }
}