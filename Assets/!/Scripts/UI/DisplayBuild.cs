using System.Diagnostics;
using TMPro;
using UnityEngine;

/// <summary>
/// Shows the build version on screen and generate a build version name for the editor
/// </summary>
public class DisplayBuild : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    static DisplayBuild instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
#if UNITY_EDITOR
        text.text = GenerateBuildVersion();
#else
        TextAsset buildInfo = Resources.Load<TextAsset>("build");
        if (buildInfo != null )
            text.text = buildInfo.text;
#endif
    }

    string GenerateBuildVersion()
    {
        string gitHash = GetGitCommitHash();
        string version = Application.version;
        return $"EDITOR v{version} ({gitHash})";
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