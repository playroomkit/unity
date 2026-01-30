#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Playroom
{
    public static class PlayroomDevMenu
    {
        private const string MenuPath = "PlayroomKit/Dev/Add Playroom Mock Manager To Scene";
        private const string PrefabPath = "Packages/com.playroomkit.sdk/Runtime/Prefabs/PlayroomMockManager.prefab";

        [MenuItem(MenuPath)]
        public static void AddPlayroomMockManagerToScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid() || !activeScene.isLoaded)
            {
                EditorUtility.DisplayDialog(
                    "PlayroomKit",
                    "No active scene is loaded. Open a scene before adding the Playroom Mock Manager.",
                    "OK");
                return;
            }

            if (Object.FindObjectsOfType<PlayroomkitDevManager>(true).Length > 0)
            {
                EditorUtility.DisplayDialog(
                    "PlayroomKit",
                    "A Playroom Mock Manager already exists in the scene.",
                    "OK");
                return;
            }

            var prefab = LoadPlayroomMockManagerPrefab();
            if (prefab == null)
            {
                EditorUtility.DisplayDialog(
                    "PlayroomKit",
                    "Could not find PlayroomMockManager.prefab in the package.",
                    "OK");
                return;
            }

            var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instance == null)
            {
                EditorUtility.DisplayDialog(
                    "PlayroomKit",
                    "Failed to instantiate PlayroomMockManager.prefab.",
                    "OK");
                return;
            }

            SceneManager.MoveGameObjectToScene(instance, activeScene);
            Undo.RegisterCreatedObjectUndo(instance, "Add Playroom Mock Manager");
            Selection.activeObject = instance;
            EditorSceneManager.MarkSceneDirty(activeScene);
        }

        private static GameObject LoadPlayroomMockManagerPrefab()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (prefab != null)
            {
                return prefab;
            }

            var guids = AssetDatabase.FindAssets(
                "PlayroomMockManager t:Prefab",
                new[] { "Packages/com.playroomkit.sdk" });
            if (guids.Length == 0)
            {
                return null;
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        }
    }
}
#endif
