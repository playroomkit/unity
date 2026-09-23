#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Playroom
{
    public static class PlayroomWebGLTools
    {
        private const string TemplateName = "DiscordTemplate";
        private const string InstallMenuPath = "PlayroomKit/WebGL/Install Discord Template";
        private const string SettingsMenuPath =
            "PlayroomKit/WebGL/Apply WebGL Settings (Run In Background, Discord Template, Compression None)";

        [MenuItem(InstallMenuPath)]
        public static void InstallTemplate()
        {
            var sourcePath = GetPackageTemplatePath();
            if (!Directory.Exists(sourcePath))
            {
                EditorUtility.DisplayDialog(
                    "PlayroomKit",
                    "Could not find DiscordTemplate in the package.",
                    "OK");
                return;
            }

            var templatesRoot = "Assets/WebGLTemplates";
            var destinationPath = Path.Combine("Assets", "WebGLTemplates", TemplateName);
            var sourceAssetPath = sourcePath.Replace('\\', '/');
            var destinationAssetPath = destinationPath.Replace('\\', '/');

            if (Directory.Exists(destinationPath))
            {
                var overwrite = EditorUtility.DisplayDialog(
                    "PlayroomKit",
                    $"Template already exists at {destinationPath}. Overwrite?",
                    "Overwrite",
                    "Cancel");
                if (!overwrite)
                {
                    return;
                }

                FileUtil.DeleteFileOrDirectory(destinationPath);
                var metaPath = destinationPath + ".meta";
                if (File.Exists(metaPath))
                {
                    FileUtil.DeleteFileOrDirectory(metaPath);
                }
            }

            AssetDatabase.DisallowAutoRefresh();
            try
            {
                AssetDatabase.Refresh();

                if (AssetDatabase.IsValidFolder(destinationAssetPath))
                {
                    AssetDatabase.DeleteAsset(destinationAssetPath);
                }

                EnsureFolder(templatesRoot);

                var success = CopyAssetFolder(sourceAssetPath, destinationAssetPath);
                if (!success)
                {
                    EditorUtility.DisplayDialog(
                        "PlayroomKit",
                        "Failed to copy DiscordTemplate into Assets/WebGLTemplates.",
                        "OK");
                    return;
                }
            }
            finally
            {
                AssetDatabase.AllowAutoRefresh();
                AssetDatabase.Refresh();
            }

            EditorUtility.DisplayDialog(
                "PlayroomKit",
                $"Installed WebGL template to {destinationPath}.",
                "OK");
        }

        [MenuItem(SettingsMenuPath)]
        public static void ApplyRecommendedSettings()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            {
                EditorUtility.DisplayDialog(
                    "PlayroomKit",
                    "WebGL is not the active build target. Switch to WebGL in Build Settings first.",
                    "OK");
                return;
            }

            var destinationPath = Path.Combine("Assets", "WebGLTemplates", TemplateName);
            if (!Directory.Exists(destinationPath))
            {
                var install = EditorUtility.DisplayDialog(
                    "PlayroomKit",
                    "DiscordTemplate is not installed. Install it now?",
                    "Install",
                    "Cancel");
                if (!install)
                {
                    return;
                }

                InstallTemplate();
            }

            PlayerSettings.runInBackground = true;
            PlayerSettings.WebGL.template = $"PROJECT:{TemplateName}";
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog(
                "PlayroomKit",
                "Applied WebGL settings (run in background, Discord template, compression none).",
                "OK");
        }

        private static string GetPackageTemplatePath()
        {
            return Path.Combine(
                "Packages",
                "com.playroomkit.sdk",
                "Assets",
                "WebGLTemplates",
                TemplateName);
        }

        private static bool CopyAssetFolder(string sourceAssetPath, string destinationAssetPath)
        {
            if (!AssetDatabase.IsValidFolder(sourceAssetPath))
            {
                return false;
            }

            if (!AssetDatabase.IsValidFolder(destinationAssetPath))
            {
                var parent = Path.GetDirectoryName(destinationAssetPath)?.Replace('\\', '/');
                var name = Path.GetFileName(destinationAssetPath);
                if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(name))
                {
                    return false;
                }

                var created = AssetDatabase.CreateFolder(parent, name);
                if (string.IsNullOrEmpty(created))
                {
                    return false;
                }
            }

            foreach (var guid in AssetDatabase.FindAssets(string.Empty, new[] { sourceAssetPath }))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (assetPath == sourceAssetPath)
                {
                    continue;
                }

                var relative = assetPath.Substring(sourceAssetPath.Length).TrimStart('/');
                var targetPath = $"{destinationAssetPath}/{relative}";
                if (AssetDatabase.IsValidFolder(assetPath))
                {
                    if (!AssetDatabase.IsValidFolder(targetPath))
                    {
                        var parent = Path.GetDirectoryName(targetPath)?.Replace('\\', '/');
                        var name = Path.GetFileName(targetPath);
                        if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(name))
                        {
                            return false;
                        }

                        var created = AssetDatabase.CreateFolder(parent, name);
                        if (string.IsNullOrEmpty(created))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (!AssetDatabase.CopyAsset(assetPath, targetPath))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            var parent = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            var name = Path.GetFileName(folderPath);
            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(name))
            {
                return;
            }

            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
#endif
