using System.Collections.Generic;

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using UnityEngine;

using TalusBackendData.Editor.Models;

namespace TalusBackendData.Editor
{
    public class PackageManagerWindow : EditorWindow
    {
        private static Dictionary<string, TalusPackageModel> s_BackendPackages = new Dictionary<string, TalusPackageModel>();

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

                    foreach (KeyValuePair<string, TalusPackageModel> package in s_BackendPackages)
                    {
                        bool isPackageInstalled = package.Value.installed;

                        if (isPackageInstalled)
                        {
                            ++installedPackageCount;
                        }

                        GUI.backgroundColor = (isPackageInstalled) ? Color.green : Color.red;

                        if (GUILayout.Button(package.Key))
                        {
                            if (isPackageInstalled)
                            {
                                s_RemovePackageRequest = Client.Remove(package.Key);
                            }
                            else
                            {
                                s_AddPackageRequest = Client.Add(package.Value.package_url);
                                EditorApplication.update += AddProgress;
                            }

                            Debug.Log(isPackageInstalled ?
                                package.Key + " removing..." :
                                package.Value.package_url + " adding...");
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
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Remove Backend Define Symbol"))
                {
                    RemoveBackendSymbol();
                }

                GUILayout.Space(8);
                GUILayout.Label("Backend Integration Steps:", EditorStyles.boldLabel);
                GUILayout.Label("1. Populate Edit/Preferences/Talus/Backend Settings");
                GUILayout.Label("2. Install all Backend Packages");
                GUILayout.Label("3. Add Backend Define Symbol");
                GUILayout.Label("3. Populate RuntimeDataManager scriptable object");
                GUILayout.Label("4. TalusKit/Backend/Fetch App Info");
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
            if (!s_ListPackageRequest.IsCompleted) { return; }

            if (s_ListPackageRequest.Status == StatusCode.Success)
            {
                foreach (var package in s_ListPackageRequest.Result)
                {
                    if (package.source != PackageSource.Git) { continue; }

                    if (s_BackendPackages.ContainsKey(package.name))
                    {
                        // Debug.Log(package.name + " hash: " + package.git.hash);
                        // Debug.Log(package.name + " revision: " + package.git.revision);

                        s_BackendPackages[package.name] = new TalusPackageModel(s_BackendPackages[package.name].package_url, true);
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
            if (!s_AddPackageRequest.IsCompleted) { return; }

            Debug.Log(s_AddPackageRequest.Status == StatusCode.Success ?
                s_AddPackageRequest.Result.packageId + " added successfully!" :
                s_AddPackageRequest.Error.message);

            EditorApplication.update -= AddProgress;
        }

        private static void PreparePackageList()
        {
            s_BackendPackages.Clear();

            foreach (KeyValuePair<string, string> package in BackendDefinitions.BackendPackages)
            {
                s_BackendPackages.Add(package.Key, new TalusPackageModel(package.Value, false));
            }
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