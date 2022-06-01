using System.Collections.Generic;

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using UnityEngine;

namespace TalusBackendData.Editor
{
    public class PackageManagerWindow : EditorWindow
    {
        private static Dictionary<string, bool> s_BackendPackages = new Dictionary<string, bool>();

        private static ListRequest s_ListPackageRequest;
        private static AddRequest s_AddPackageRequest;
        private static RemoveRequest s_RemovePackageRequest;

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
            int installedPackageCount = 0;

            GUILayout.BeginVertical();

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
                    GUILayout.Label("Backend Packages:", EditorStyles.boldLabel);

                    foreach (KeyValuePair<string, bool> package in s_BackendPackages)
                    {
                        bool isPackageInstalled = package.Value;

                        if (isPackageInstalled)
                        {
                            ++installedPackageCount;
                        }

                        GUI.backgroundColor = (isPackageInstalled) ? Color.green : Color.red;

                        if (GUILayout.Button(package.Key))
                        {
                            if (isPackageInstalled)
                            {
                                RemoveBackendPackage(package.Key);
                            }
                            else
                            {
                                AddBackendPackage(package.Key);
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

            if (installedPackageCount == s_BackendPackages.Count)
            {
                GUI.backgroundColor = Color.green;
                GUILayout.Space(8);
                GUILayout.Label("All backend packages installed!", EditorStyles.boldLabel);

#if ENABLE_BACKEND
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Backend Define Symbol exists."))
                {
                    RemoveBackendSymbol();
                }

                GUILayout.Space(8);
                GUILayout.Label("Backend Integration Steps:", EditorStyles.boldLabel);
                GUILayout.Label("1. Install/Update all Backend Packages");
                GUILayout.Label("2. Add Backend Define Symbol");
                GUILayout.Label("3. Populate Edit/Preferences/Talus/Backend Settings");
                GUILayout.Label("4. TalusKit/Backend/Fetch App Info");
                GUILayout.Label("5. Populate RuntimeDataManager scriptable object");
#else
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Backend Define Symbol doesn't exist!"))
                {
                    AddBackendSymbol();
                }
#endif
            }

            GUILayout.EndVertical();
        }

        private static void RemoveBackendPackage(string packageId)
        {
            Debug.Log("Remove package: " + packageId);

            s_RemovePackageRequest = Client.Remove(packageId);
        }

        private static void AddBackendPackage(string packageId)
        {
            Debug.Log("Add package: " + packageId);

            string apiUrl = EditorPrefs.GetString(BackendDefinitions.BackendApiUrlPref);
            string apiToken = EditorPrefs.GetString(BackendDefinitions.BackendApiTokenPref);
            BackendApi api = new BackendApi(apiUrl, apiToken);
            api.GetPackageInfo(packageId, package => {
                s_AddPackageRequest = Client.Add(package.url);
                EditorApplication.update += AddProgress;
            });
        }

        private static void PreparePackageList()
        {
            s_BackendPackages.Clear();

            foreach (string packageId in BackendDefinitions.BackendPackageList)
            {
                s_BackendPackages.Add(packageId, false);
            }
        }

        private static void ListProgress()
        {
            if (!s_ListPackageRequest.IsCompleted) { return; }

            if (s_ListPackageRequest.Status == StatusCode.Success)
            {
                foreach (var package in s_ListPackageRequest.Result)
                {
                    if (package.source != PackageSource.Git) { continue; }
                    if (!s_BackendPackages.ContainsKey(package.name)) { continue; }

                    s_BackendPackages[package.name] = true;
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
            if (!s_AddPackageRequest.IsCompleted) { return; }

            Debug.Log(s_AddPackageRequest.Status == StatusCode.Success ?
                s_AddPackageRequest.Result.packageId + " added successfully!" :
                s_AddPackageRequest.Error.message);

            EditorApplication.update -= AddProgress;
        }

        private static void RemoveBackendSymbol()
        {
            if (!DefineSymbols.Contains(BackendDefinitions.BackendSymbol)) { return; }

            DefineSymbols.Remove(BackendDefinitions.BackendSymbol);
            Debug.Log(BackendDefinitions.BackendSymbol + " define symbol removing...");
        }

        private static void AddBackendSymbol()
        {
            if (DefineSymbols.Contains(BackendDefinitions.BackendSymbol)) { return; }

            DefineSymbols.Add(BackendDefinitions.BackendSymbol);
            Debug.Log(BackendDefinitions.BackendSymbol + " define symbol adding...");
        }
    }
}