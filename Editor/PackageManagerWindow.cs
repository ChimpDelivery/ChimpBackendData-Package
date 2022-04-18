using System.Collections.Generic;

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using UnityEngine;

namespace TalusBackendData.Editor
{
    public class PackageManagerWindow : EditorWindow
    {
        private const string TALUS_BACKEND_KEYWORD = "ENABLE_BACKEND";
        private const string ELEPHANT_SCENE_PATH = "Assets/Scenes/Template_Persistent/elephant_scene.unity";

        private static Dictionary<string, TalusPackage> s_Backend_Packages = new Dictionary<string, TalusPackage>
        {
            { "com.talus.talusplayservicesresolver", new TalusPackage("https://github.com/TalusStudio/TalusPlayServicesResolver-Package.git", false) },
            { "com.talus.talusfacebook", new TalusPackage("https://github.com/TalusStudio/TalusFacebook-Package.git", false) },
            { "com.talus.taluselephant", new TalusPackage("https://github.com/TalusStudio/TalusElephant-Package.git", false) }
        };

        private static AddRequest s_AddRequest;
        private static RemoveRequest s_RemoveRequest;
        private static ListRequest s_ListRequest;

        [MenuItem("TalusKit/Backend/Package Manager", priority = -999)]
        private static void Init()
        {
            s_ListRequest = Client.List();
            EditorApplication.update += ListProgress;

            var window = GetWindow<PackageManagerWindow>();
            window.titleContent = new GUIContent("Talus Backend");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Backend Status", EditorStyles.boldLabel);

#if ENABLE_BACKEND
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Disable Backend"))
            {
                DisableBackend();
            }
#else
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Enable Backend"))
            {
                EnableBackend();
            }
#endif

            if (s_ListRequest == null)
            {
                s_ListRequest = Client.List();
                EditorApplication.update += ListProgress;
            }

            if (s_ListRequest != null)
            {
                int installedPackageCount = 0;

                if (s_ListRequest.IsCompleted)
                {
                    GUILayout.Space(4);
                    GUILayout.Label("Installed Talus Packages", EditorStyles.boldLabel);

                    foreach (var package in s_Backend_Packages)
                    {
                        if (package.Value.Installed) { ++installedPackageCount; }

                        GUI.backgroundColor = (package.Value.Installed) ? Color.green : Color.red;

                        if (GUILayout.Button(package.Key))
                        {
                            if (package.Value.Installed)
                            {
                                s_RemoveRequest = Client.Remove(package.Key);
                                Debug.Log(package.Key + " removing...");
                            }
                            else
                            {
                                s_AddRequest = Client.Add(package.Value.PackageUrl);
                                Debug.Log(package.Value.PackageUrl + " adding...");
                            }
                        }
                    }

                    GUILayout.Space(4);
                    GUILayout.Label($"Installed package count: {installedPackageCount}", EditorStyles.boldLabel);
                }
                else
                {
                    GUILayout.Space(4);
                    GUILayout.Label("Fetching...", EditorStyles.boldLabel);
                }
            }

            GUILayout.EndVertical();
        }

        private static void ListProgress()
        {
            if (!s_ListRequest.IsCompleted)
            {
                return;
            }

            if (s_ListRequest.Status == StatusCode.Success)
            {
                foreach (var package in s_ListRequest.Result)
                {
                    // Only retrieve packages that are currently installed in the
                    // project (and are neither Built-In nor already Embedded)
                    if (package.isDirectDependency &&
                        package.source != PackageSource.BuiltIn &&
                        package.source != PackageSource.Embedded)
                    {
                        if (s_Backend_Packages.ContainsKey(package.name))
                        {
                            s_Backend_Packages[package.name] = new TalusPackage(s_Backend_Packages[package.name].PackageUrl, true);
                        }
                    }
                }
            }
            else
            {
                Debug.Log(s_ListRequest.Error.message);
            }

            EditorApplication.update -= ListProgress;
        }

        private static void DisableBackend()
        {
            if (DefineSymbols.Contains(TALUS_BACKEND_KEYWORD))
            {
                DefineSymbols.Remove(TALUS_BACKEND_KEYWORD);
                Debug.Log(TALUS_BACKEND_KEYWORD + " define symbol removing...");
            }

            //
            var editorBuildSettingsScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            editorBuildSettingsScenes.Remove(editorBuildSettingsScenes.Find(val => val.path.Contains("elephant")));
            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

            Debug.Log("elephant_scene removed from build settings...");
        }

        private static void EnableBackend()
        {
            if (!DefineSymbols.Contains(TALUS_BACKEND_KEYWORD))
            {
                DefineSymbols.Add(TALUS_BACKEND_KEYWORD);
                Debug.Log(TALUS_BACKEND_KEYWORD + " define symbol adding...");
            }

            //
            var editorBuildSettingsScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (editorBuildSettingsScenes.Count > 0 && !editorBuildSettingsScenes[0].path.Contains("elephant"))
            {
                var elephantScene = new EditorBuildSettingsScene(ELEPHANT_SCENE_PATH, true);
                editorBuildSettingsScenes.Insert(0, elephantScene);
                EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

                Debug.Log("elephant_scene added to build settings...");
            }
        }
    }
}
