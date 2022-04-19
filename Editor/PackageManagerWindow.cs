using System.Collections.Generic;

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using UnityEngine;

namespace TalusBackendData.Editor
{
    public class PackageManagerWindow : EditorWindow
    {
        private static Dictionary<string, TalusPackage> s_Backend_Packages = new Dictionary<string, TalusPackage>();

        private static AddRequest s_AddPackageRequest;
        private static RemoveRequest s_RemovePackageRequest;
        private static ListRequest s_ListPackageRequest;

        [MenuItem("TalusKit/Backend/Package Manager", false, 10000)]
        private static void Init()
        {
            PreparePackageList();

            s_ListPackageRequest = Client.List();
            EditorApplication.update += ListProgress;

            var window = GetWindow<PackageManagerWindow>();
            window.titleContent = new GUIContent("Talus Backend");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            int installedPackageCount = 0;

            if (s_ListPackageRequest == null)
            {
                PreparePackageList();

                s_ListPackageRequest = Client.List();
                EditorApplication.update += ListProgress;
            }

            if (s_ListPackageRequest != null)
            {
                if (s_ListPackageRequest.IsCompleted)
                {
                    GUILayout.Space(8);
                    GUILayout.Label("Installed Talus Packages:", EditorStyles.boldLabel);

                    foreach (KeyValuePair<string, TalusPackage> package in s_Backend_Packages)
                    {
                        if (package.Value.Installed) { ++installedPackageCount; }

                        GUI.backgroundColor = (package.Value.Installed) ? Color.green : Color.red;

                        if (GUILayout.Button(package.Key))
                        {
                            if (package.Value.Installed)
                            {
                                s_RemovePackageRequest = Client.Remove(package.Key);
                                Debug.Log(package.Key + " removing...");
                            }
                            else
                            {
                                s_AddPackageRequest = Client.Add(package.Value.PackageUrl);
                                EditorApplication.update += AddProgress;
                                Debug.Log(package.Value.PackageUrl + " adding...");
                            }
                        }
                    }
                }
                else
                {
                    GUILayout.Space(8);
                    GUILayout.Label("Fetching...", EditorStyles.boldLabel);
                }
            }

            if (installedPackageCount == s_Backend_Packages.Count)
            {
                GUI.backgroundColor = Color.green;
                GUILayout.Space(8);
                GUILayout.Label("All backend packages installed!", EditorStyles.boldLabel);

#if ENABLE_BACKEND
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Remove Backend Define Symbol"))
                {
                    RemoveBackendSymbol();
                }
#else
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Add Backend Define Symbol"))
                {
                    AddBackendSymbol();
                }
#endif
            }

            GUILayout.EndVertical();
        }

        private static void ListProgress()
        {
            if (!s_ListPackageRequest.IsCompleted)
            {
                return;
            }

            if (s_ListPackageRequest.Status == StatusCode.Success)
            {
                foreach (var package in s_ListPackageRequest.Result)
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
                Debug.Log(s_ListPackageRequest.Error.message);
            }

            EditorApplication.update -= ListProgress;
        }

        private static void AddProgress()
        {
            if (!s_AddPackageRequest.IsCompleted)
            {
                return;
            }

            if (s_AddPackageRequest.Status == StatusCode.Success)
            {
                Debug.Log(s_AddPackageRequest.Result.packageId + " added successfully!");
            }
            else
            {
                Debug.Log(s_AddPackageRequest.Error.message);
            }

            EditorApplication.update -= AddProgress;
        }

        private static void PreparePackageList()
        {
            s_Backend_Packages.Clear();

            foreach (KeyValuePair<string, string> package in BackendDefinitions.BackendPackages)
            {
                s_Backend_Packages.Add(package.Key, new TalusPackage(package.Value, false));
            }
        }

        private static void RemoveBackendSymbol()
        {
            if (DefineSymbols.Contains(BackendDefinitions.BackendSymbol))
            {
                DefineSymbols.Remove(BackendDefinitions.BackendSymbol);
                Debug.Log(BackendDefinitions.BackendSymbol + " define symbol removing...");
            }
        }

        private static void AddBackendSymbol()
        {
            if (!DefineSymbols.Contains(BackendDefinitions.BackendSymbol))
            {
                DefineSymbols.Add(BackendDefinitions.BackendSymbol);
                Debug.Log(BackendDefinitions.BackendSymbol + " define symbol adding...");
            }
        }
    }
}
