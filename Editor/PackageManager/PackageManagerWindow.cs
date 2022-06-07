using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using CallbackFunction = UnityEditor.EditorApplication.CallbackFunction;

using TalusBackendData.Editor.User;
using TalusBackendData.Editor.Utility;

namespace TalusBackendData.Editor.PackageManager
{
    /// <summary>
    ///     List/Add/Remove Talus Packages
    /// </summary>
    internal class PackageManagerWindow : EditorWindow
    {
        // api keys
        private static string s_ApiUrl => EditorPrefs.GetString(BackendDefinitions.BackendApiUrlPref);
        private static string s_ApiToken => EditorPrefs.GetString(BackendDefinitions.BackendApiTokenPref);

        private static PackageManagerWindow s_Instance;

        private static Dictionary<string, PackageStatus> s_BackendPackages = new Dictionary<string, PackageStatus>();

        private static ListRequest s_ListPackageRequest;
        private static AddRequest s_AddPackageRequest;
        private static RemoveRequest s_RemovePackageRequest;

        private static int s_InstalledPackageCount = 0;
        private static int s_UpdatablePackageCount = 0;

        public delegate void RequestDelegate();

        [MenuItem("TalusKit/Backend/Package Manager", false, 10000)]
        private static void Init()
        {
            if (string.IsNullOrEmpty(BackendSettings.ApiUrl))
            {
                InfoBox.Create(
                    "Error :(",
                    $"'Api URL' can not be empty!\n\n(Edit/Project Settings/{BackendSettings.Path})",
                    "Open Settings",
                    "Close",
                    () => SettingsService.OpenProjectSettings(BackendSettings.Path)
                );
            }
            else if (string.IsNullOrEmpty(BackendSettings.ApiToken))
            {
                InfoBox.Create(
                    "Error :(",
                    $"'Api Token' can not be empty!\n\n(Edit/Project Settings/{BackendSettings.Path})",
                    "Open Settings",
                    "Close",
                    () => SettingsService.OpenProjectSettings(BackendSettings.Path)
                );
            }
            else
            {
                PreparePackageData();

                s_Instance = GetWindow<PackageManagerWindow>();
                s_Instance.minSize = new Vector2(500, 400);
                s_Instance.titleContent = new GUIContent("Talus Package Manager");
                s_Instance.Show();
            }
        }

        private void OnFocus()
        {
            if (s_ListPackageRequest != null) { return; }

            PreparePackageData();
        }

        private void OnGUI()
        {
            if (s_ListPackageRequest == null || !s_ListPackageRequest.IsCompleted)
            {
                GUI.backgroundColor = Color.yellow;
                GUILayout.Space(8);
                GUILayout.Label("Preparing package list...", EditorStyles.foldoutHeader);

                return;
            }

            if ((s_AddPackageRequest != null && !s_AddPackageRequest.IsCompleted) ||
                (s_RemovePackageRequest != null && !s_RemovePackageRequest.IsCompleted) ||
                (EditorApplication.isCompiling || EditorApplication.isUpdating))
            {
                GUI.backgroundColor = Color.yellow;
                GUILayout.Space(8);
                GUILayout.Label("Wait for editor reloading...", EditorStyles.foldoutHeader);

                return;
            }

            GUILayout.BeginVertical();

            GUILayout.Space(8);
            GUILayout.Label($"Packages ({s_BackendPackages.Count}):", EditorStyles.boldLabel);

            foreach (var package in s_BackendPackages)
            {
                bool isPackageInstalled = package.Value.Exist;
                bool isUpdateExist = package.Value.UpdateExist;

                GUI.backgroundColor = (isPackageInstalled) ? ((isUpdateExist) ? Color.yellow : Color.green) : Color.red;
                if (GUILayout.Button(package.Key))
                {
                    if (!isPackageInstalled || isUpdateExist)
                    {
                        AddBackendPackage(package.Key);
                    }
                    else
                    {
                        InfoBox.Create("Are you sure ?",
                            $"You are about to remove the '{package.Key}' package!",
                            "Yes, I know",
                            "Cancel",
                            () => RemoveBackendPackage(package.Key));
                    }
                }
            }

            bool symbolCheck = DefineSymbols.Contains(BackendDefinitions.BackendSymbol);

            if (s_InstalledPackageCount == s_BackendPackages.Count)
            {
                GUILayout.Space(8);
                GUILayout.Label($"Backend Define Symbol ({BackendDefinitions.BackendSymbol}):", EditorStyles.boldLabel);

                GUI.backgroundColor = (symbolCheck) ? Color.green : Color.red;
                if (symbolCheck)
                {
                    if (GUILayout.Button("Backend Define Symbol exists."))
                    {
                        InfoBox.Create("Are you sure ?",
                            "You are about to remove the 'Backend Define Symbol' definition!",
                            "Yes, I know",
                            "Cancel",
                            () => BackendDefinitions.RemoveBackendSymbol());
                    }
                }
                else
                {
                    if (GUILayout.Button("Backend Define Symbol doesn't exist!"))
                    {
                        BackendDefinitions.AddBackendSymbol();
                    }
                }
            }

            GUI.backgroundColor = default;

            GUILayout.Space(8);
            GUILayout.Label("Backend Integration Steps:", EditorStyles.boldLabel);

            bool packageCheck = (s_InstalledPackageCount == s_BackendPackages.Count) && s_UpdatablePackageCount == 0;
            GUI.backgroundColor = packageCheck ? Color.green : Color.red;
            GUILayout.Toggle(packageCheck, "Install & Update all packages");

            GUI.backgroundColor = symbolCheck ? Color.green : Color.red;
            GUILayout.Toggle(symbolCheck, "Add Backend Define Symbol");

            bool dataCheck = !(PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS).Equals("com.Talus.TalusTemplateURP"));
            GUI.backgroundColor = dataCheck ? Color.green : Color.red;
            GUILayout.Toggle(dataCheck, "Populate 'TalusKit/Backend/App Settings' and click 'Update Settings' button");

            GUILayout.EndVertical();
        }

        private static void RemoveBackendPackage(string packageId)
        {
            if (s_RemovePackageRequest != null && !s_RemovePackageRequest.IsCompleted) { return; }

            s_RemovePackageRequest = Client.Remove(packageId);
            EditorApplication.update += RemoveProgress;
        }

        private static void AddBackendPackage(string packageId)
        {
            if (s_AddPackageRequest != null && !s_AddPackageRequest.IsCompleted) { return; }

            BackendApi api = new BackendApi(s_ApiUrl, s_ApiToken);
            api.GetPackageInfo(packageId, package => {
                s_AddPackageRequest = Client.Add(package.url);
                EditorApplication.update += AddProgress;
            });
        }

        private static void PreparePackageData()
        {
            s_InstalledPackageCount = 0;
            s_UpdatablePackageCount = 0;
            s_BackendPackages.Clear();

            foreach (var package in BackendDefinitions.Packages)
            {
                s_BackendPackages[package.Value] = new PackageStatus(false, "", false);
            }

            s_ListPackageRequest = Client.List();
            EditorApplication.update += ListProgress;
        }

        private static void CheckVersion(string packageId, string packageHash)
        {
            BackendApi api = new BackendApi(s_ApiUrl, s_ApiToken);
            api.GetPackageInfo(packageId, package =>
            {
                bool updateExist = !packageHash.Equals(package.hash);
                s_BackendPackages[packageId].UpdateExist = updateExist;

                if (updateExist)
                {
                    ++s_UpdatablePackageCount;
                }

                RepaintManagerWindow();
            });
        }

        private static void ListProgress()
        {
            HandleRequest(s_ListPackageRequest, ListProgress, () =>
            {
                if (s_ListPackageRequest.Status == StatusCode.Success)
                {
                    foreach (PackageInfo package in s_ListPackageRequest.Result)
                    {
                        if (!s_BackendPackages.ContainsKey(package.name)) { continue; }

                        bool isGitPackage = (package.source == PackageSource.Git);
                        string gitHash = (isGitPackage) ? package.git.hash : "";

                        s_BackendPackages[package.name] = new PackageStatus(true, gitHash, false);

                        if (isGitPackage)
                        {
                            CheckVersion(package.name, gitHash);
                        }

                        ++s_InstalledPackageCount;
                    }

                    return;
                }

                InfoBox.Create("Error :(", s_ListPackageRequest.Error.message, "OK");
            });
        }

        private static void AddProgress()
        {
            HandleRequest(s_AddPackageRequest, AddProgress, () =>
            {
                StatusCode addPackageStatus = s_AddPackageRequest.Status;
                string message = (addPackageStatus == StatusCode.Success) ?
                    s_AddPackageRequest.Result.packageId + " added successfully!" :
                    s_AddPackageRequest.Error.message;

                InfoBox.Create($"{addPackageStatus} !", message, "OK");
            }, true);
        }

        private static void RemoveProgress()
        {
            HandleRequest(s_RemovePackageRequest, RemoveProgress, () =>
            {
                StatusCode removePackageStatus = s_RemovePackageRequest.Status;
                string message = (removePackageStatus == StatusCode.Success) ?
                    s_RemovePackageRequest.PackageIdOrName + " removed successfully!" :
                    s_RemovePackageRequest.Error.message;

                InfoBox.Create($"{removePackageStatus} !", message, "OK");
            }, true);
        }

        private static void RepaintManagerWindow()
        {
            if (s_Instance == null) { return; }

            s_Instance.Repaint();
        }

        private static void HandleRequest(Request request, CallbackFunction caller, System.Action onComplete, bool saveAssets = false)
        {
            if (!request.IsCompleted) { return; }

            onComplete?.Invoke();

            EditorApplication.update -= caller;
            RepaintManagerWindow();

            if (saveAssets)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}