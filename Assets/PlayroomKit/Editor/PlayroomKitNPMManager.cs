using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
public class PlayroomKitSetupWindow : EditorWindow
{
    private const string defaultNodePath = @"C:\Program Files\nodejs";

    [MenuItem("PlayroomKit/Run Setup")]
    public static void ShowWindow()
    {
        var window = GetWindow<PlayroomKitSetupWindow>();
        window.titleContent = new GUIContent("PlayroomKit Setup");
        window.minSize = new Vector2(450, 300);
    }

    public void CreateGUI()
    {
        // Load UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Packages/com.playroomkit.sdk/Editor/PlayroomKitNPMManager.uxml"
        );

        if (visualTree == null)
        {
            Debug.LogError("Could not find UXML file. Check the path.");
            return;
        }

        VisualElement root = visualTree.CloneTree();
        rootVisualElement.Add(root);

        // Reference UI elements
        Toggle npmCheck = root.Q<Toggle>("npmCheck");
        Toggle modulesCheck = root.Q<Toggle>("modulesCheck");
        TextField pathField = root.Q<TextField>("pathField");
        Button inputPathButton = root.Q<Button>("inputPath");
        Button downloadButton = root.Q<Button>("downloadButton");
        Button refreshButton = root.Q<Button>("refreshButton");
        Button installButton = root.Q<Button>("installButton");


        // Set initial display values
        npmCheck.SetEnabled(false);
        modulesCheck.SetEnabled(false);
        npmCheck.value = CheckIfNpmExists();
        modulesCheck.value = CheckIfNodeModulesExist();
        installButton.SetEnabled(false);
        installButton.visible = false;

        bool modulesExist = CheckIfNodeModulesExist();
        installButton.SetEnabled(!modulesExist);
        installButton.visible = !modulesExist;

        if (string.IsNullOrWhiteSpace(pathField.value))
        {
            pathField.value = defaultNodePath;
        }

        // Let user select custom Node.js path
        inputPathButton.clicked += () =>
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Node.js directory", pathField.value, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                pathField.value = selectedPath;
                Debug.Log("Custom Node path set to: " + selectedPath);
            }
        };

        installButton.clicked += () =>
{
    string npmPath = FindNpmInGlobalPath();
    if (string.IsNullOrEmpty(npmPath) || !File.Exists(npmPath))
    {
        Debug.LogError("Cannot run npm install. npm not found.");
        return;
    }

    // Locate PlayroomKit package in PackageCache
    string packagePrefix = "com.playroomkit.sdk@";
    string packageCacheRoot = "Library/PackageCache";
    string editorCachePath = null;

    try
    {
        var matchingDir = Directory.GetDirectories(packageCacheRoot, $"{packagePrefix}*")
                                   .FirstOrDefault(d => Directory.Exists(Path.Combine(d, "Editor")));

        if (matchingDir == null)
        {
            Debug.LogError("PlayroomKit package not found in PackageCache.");
            return;
        }

        editorCachePath = Path.Combine(matchingDir, "Editor");
    }
    catch (System.Exception ex)
    {
        Debug.LogException(ex);
        return;
    }

    // Run `npm install` in that editor path
    var process = new System.Diagnostics.Process();
    process.StartInfo = new System.Diagnostics.ProcessStartInfo
    {
        FileName = npmPath,
        Arguments = "install",
        WorkingDirectory = editorCachePath,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    process.OutputDataReceived += (sender, e) =>
    {
        if (!string.IsNullOrEmpty(e.Data))
            Debug.Log("[npm] " + e.Data);
    };

    process.ErrorDataReceived += (sender, e) =>
    {
        if (!string.IsNullOrEmpty(e.Data))
            Debug.LogWarning("[npm error] " + e.Data);
    };

    try
    {
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        Debug.Log("npm install finished with exit code: " + process.ExitCode);

        if (process.ExitCode == 0)
        {
            string sourcePath = Path.Combine(editorCachePath, "node_modules");
            string targetRoot = Path.Combine("Packages", "PlayroomKit");
            string targetEditorPath = Path.Combine(targetRoot, "Editor");
            string targetNodeModules = Path.Combine(targetEditorPath, "node_modules");

            // Create destination folders if missing
            if (!Directory.Exists(targetEditorPath))
            {
                Directory.CreateDirectory(targetEditorPath);
                Debug.Log("Created directory: " + targetEditorPath);
            }

            // Remove old node_modules if already there
            if (Directory.Exists(targetNodeModules))
            {
                Directory.Delete(targetNodeModules, true);
                Debug.Log("Old node_modules deleted in target.");
            }

            // Move node_modules to the embedded package
            Directory.Move(sourcePath, targetNodeModules);
            Debug.Log("node_modules successfully moved to: " + targetNodeModules);
        }
        else
        {
            Debug.LogWarning("npm install failed.");
        }
    }
    catch (System.Exception ex)
    {
        Debug.LogError("Failed during npm install or moving node_modules: " + ex.Message);
    }
};



        // Open Node.js download page
        downloadButton.clicked += () =>
        {
            Application.OpenURL("https://nodejs.org/");
        };

        // Refresh environment check
        refreshButton.clicked += () =>
        {
            string userNodePath = pathField.value.Trim();

            bool npmInstalled = CheckIfNpmExists(userNodePath);
            bool modulesInstalled = CheckIfNodeModulesExist();

            npmCheck.value = npmInstalled;
            modulesCheck.value = modulesInstalled;

            Debug.Log("Environment refreshed.");
        };

        // Optional: validate path on change
        pathField.RegisterValueChangedCallback(evt =>
        {
            string testPath = Path.Combine(evt.newValue.Trim(), "npm.cmd");
            if (!File.Exists(testPath))
            {
                Debug.LogWarning("npm.cmd not found in: " + evt.newValue);
            }
        }
        );
    }

    private bool CheckIfNpmExists(string userDefinedPath = null)
    {
        string npmPath = !string.IsNullOrWhiteSpace(userDefinedPath)
            ? Path.Combine(userDefinedPath.Trim(), Application.platform == RuntimePlatform.WindowsEditor ? "npm.cmd" : "npm")
            : FindNpmInGlobalPath();

        if (string.IsNullOrEmpty(npmPath) || !File.Exists(npmPath))
        {
            Debug.LogWarning("npm not found.");
            return false;
        }

        try
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = npmPath,
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
                Debug.LogWarning("npm error: " + error);

            Debug.Log("npm version: " + output.Trim());
            return !string.IsNullOrEmpty(output);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Failed to run npm: " + ex.Message);
            return false;
        }
    }

    private string FindNpmInGlobalPath()
    {
        string envPath = System.Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(envPath))
            return null;

        string[] pathDirs = envPath.Split(Path.PathSeparator);
        string npmFile = Application.platform == RuntimePlatform.WindowsEditor ? "npm.cmd" : "npm";

        foreach (string dir in pathDirs)
        {
            string fullPath = Path.Combine(dir.Trim(), npmFile);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        return null;
    }


    private bool CheckIfNodeModulesExist()
    {
        string path = "Packages/PlayroomKit/Editor/node_modules";
        return Directory.Exists(path);
    }
}
#endif // UNITY_EDITOR  