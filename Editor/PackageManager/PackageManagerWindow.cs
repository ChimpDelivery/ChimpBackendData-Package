using System.Collections.Generic;

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using UnityEngine;

namespace TalusBackendData.Editor.PackageManager
{
    public class PackageManagerWindow : EditorWindow
    {
        private static Dictionary<string, PackageStatus> s_BackendPackages = new Dictionary<string, PackageStatus>();

        private static ListRequest s_ListPackageRequest;
        private static AddRequest s_AddPackageRequest;
        private static RemoveRequest s_RemovePackageRequest;

        private static PackageManagerWindow s_Instance;

        private static int s_InstalledPackageCount = 0;

        [MenuItem("TalusKit/Backend/Package Manager", false, 10000)]
        private static void Init()
        {
            PreparePackageData();

            s_Instance = GetWindow<PackageManagerWindow>();
            s_Instance.titleContent = new GUIContent("Talus Backend");
            s_Instance.Show();
        }

        private void OnFocus()
        {
            if (s_ListPackageRequest == null)
            {
                PreparePackageData();
            }
        }

        private void OnGUI()
        {
            if (s_ListPackageRequest == null || !s_ListPackageRequest.IsCompleted)
            {
                GUILayout.Space(8);
                GUILayout.Label("Fetching...", EditorStyles.boldLabel);

                return;
            }

            GUILayout.BeginVertical();

            if (s_ListPackageRequest.IsCompleted)
            {
                GUILayout.Space(8);
                GUILayout.Label("Backend Packages:", EditorStyles.boldLabel);

                foreach (var package in s_BackendPackages)
                {
                    bool isPackageInstalled = package.Value.Exist;
                    bool isUpdateExist = package.Value.UpdateExist;

                    Debug.Log($"{package.Key} {isPackageInstalled} {isUpdateExist}");

                    GUI.backgroundColor = (isPackageInstalled) ?
                        ((isUpdateExist) ? Color.yellow : Color.green)
                        : Color.red;

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

            if (s_InstalledPackageCount == s_BackendPackages.Count)
            {
                GUI.backgroundColor = Color.green;
                GUILayout.Label("All backend packages installed!", EditorStyles.boldLabel);

                GUILayout.Space(8);

#if ENABLE_BACKEND
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Backend Define Symbol exists."))
                {
                    RemoveBackendSymbol();
                }
#else
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Backend Define Symbol doesn't exist!"))
                {
                    AddBackendSymbol();
                }
#endif
            }

            GUILayout.Space(8);

            GUILayout.Label("Backend Integration Steps:", EditorStyles.boldLabel);
            GUILayout.Label("1. Install/Update all Backend Packages");
            GUILayout.Label("2. Add Backend Define Symbol");
            GUILayout.Label("3. Populate Edit/Preferences/Talus/Backend Settings");
            GUILayout.Label("4. TalusKit/Backend/Fetch App Info");
            GUILayout.Label("5. Populate RuntimeDataManager scriptable object");

            GUILayout.EndVertical();
        }

        private static void RemoveBackendPackage(string packageId)
        {
            Debug.Log("Remove package: " + packageId);

            s_RemovePackageRequest = Client.Remove(packageId);
            EditorApplication.update += RemoveProgress;
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

        private static void PreparePackageData()
        {
            s_InstalledPackageCount = 0;
            s_BackendPackages.Clear();

            foreach (string packageId in BackendDefinitions.BackendPackageList)
            {
                s_BackendPackages[packageId] = new PackageStatus(false, "", false);
            }

            s_ListPackageRequest = Client.List();
            EditorApplication.update += ListProgress;
        }

        private static bool IsUpdateExist(string packageId, string packageHash)
        {
            bool response = false;

            string apiUrl = EditorPrefs.GetString(BackendDefinitions.BackendApiUrlPref);
            string apiToken = EditorPrefs.GetString(BackendDefinitions.BackendApiTokenPref);
            BackendApi api = new BackendApi(apiUrl, apiToken);
            api.GetPackageInfo(packageId, package => response = !packageHash.Equals(package.hash));

            return response;
        }

        private static void ListProgress()
        {
            if (!s_ListPackageRequest.IsCompleted) { return; }

            if (s_ListPackageRequest.Status == StatusCode.Success)
            {
                foreach (var package in s_ListPackageRequest.Result)
                {
                    if (!s_BackendPackages.ContainsKey(package.name)) { continue; }

                    bool isGitPackage = package.source == PackageSource.Git;

                    string gitHash = isGitPackage ? package.git.hash : "";
                    bool gitUpdateExist = isGitPackage && IsUpdateExist(package.name, package.git.hash);

                    s_BackendPackages[package.name] = new PackageStatus(true, gitHash, gitUpdateExist);

                    ++s_InstalledPackageCount;
                }
            }
            else
            {
                Debug.Log(s_ListPackageRequest.Error.message);
            }

            EditorApplication.update -= ListProgress;
            RepaintManagerWindow();
        }

        private static void AddProgress()
        {
            if (!s_AddPackageRequest.IsCompleted) { return; }

            Debug.Log(s_AddPackageRequest.Status == StatusCode.Success ?
                s_AddPackageRequest.Result.packageId + " added successfully!" :
                s_AddPackageRequest.Error.message);

            EditorApplication.update -= AddProgress;
            RepaintManagerWindow();
        }

        private static void RemoveProgress()
        {
            if (!s_RemovePackageRequest.IsCompleted) { return; }

            Debug.Log(s_RemovePackageRequest.Status == StatusCode.Success ?
                s_RemovePackageRequest.PackageIdOrName + " removed successfully!" :
                s_AddPackageRequest.Error.message);

            EditorApplication.update -= RemoveProgress;
            RepaintManagerWindow();
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

        private static void RepaintManagerWindow()
        {
            if (s_Instance != null)
            {
                s_Instance.Repaint();
            }
        }
    }
}