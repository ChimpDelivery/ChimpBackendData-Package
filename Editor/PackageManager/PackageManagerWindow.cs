using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using TalusBackendData.Editor.Utility;
using TalusBackendData.Editor.PackageManager.Requests;

namespace TalusBackendData.Editor.PackageManager
{
    /// <summary>
    ///     List/Add/Update/Remove Talus Packages
    /// </summary>
    internal class PackageManagerWindow : EditorWindow
    {
        private static PackageManagerWindow s_Instance;

        private readonly Dictionary<string, Models.PackageStatus> _Packages = new Dictionary<string, Models.PackageStatus>();

        private int _InstalledPackageCount = 0;
        private int _UpdatablePackageCount = 0;

        private RequestHandler<ListRequest> _ListPackages;
        private RequestHandler<AddRequest> _AddPackage;
        private RequestHandler<RemoveRequest> _RemovePackage;

        private void RefreshPackages()
        {
            _InstalledPackageCount = 0;
            _UpdatablePackageCount = 0;
            _Packages.Clear();

            foreach (var package in BackendDefinitions.Packages)
            {
                _Packages[package.Value] = new Models.PackageStatus(false, "", false);
            }

            ListPackages();
        }

        [MenuItem("TalusKit/Backend/Package Manager", false, 10000)]
        private static void Init()
        {
            if (string.IsNullOrEmpty(BackendDefinitions.ApiUrl))
            {
                InfoBox.Create(
                    "Error :(",
                    $"'Api URL' can not be empty!\n\n(Edit/Project Settings/{BackendDefinitions.ProviderPath})",
                    "Open Settings",
                    "Close",
                    () => SettingsService.OpenProjectSettings(BackendDefinitions.ProviderPath)
                );
            }
            else if (string.IsNullOrEmpty(BackendDefinitions.ApiToken))
            {
                InfoBox.Create(
                    "Error :(",
                    $"'Api Token' can not be empty!\n\n(Edit/Project Settings/{BackendDefinitions.ProviderPath})",
                    "Open Settings",
                    "Close",
                    () => SettingsService.OpenProjectSettings(BackendDefinitions.ProviderPath)
                );
            }
            else
            {
                s_Instance = GetWindow<PackageManagerWindow>();
                s_Instance.minSize = new Vector2(500, 400);
                s_Instance.titleContent = new GUIContent("Talus Package Manager");
                s_Instance.Show();
            }
        }

        private void OnEnable()
        {
            if (_ListPackages != null) { return; }

            RefreshPackages();
        }

        private void OnFocus()
        {
            if (_ListPackages != null) { return; }

            RefreshPackages();
        }

        private void OnGUI()
        {
            if (_ListPackages == null || !_ListPackages.Request.IsCompleted)
            {
                GUI.backgroundColor = Color.yellow;
                GUILayout.Space(8);
                GUILayout.Label("Preparing package list...", EditorStyles.foldoutHeader);

                return;
            }

            if ((_AddPackage != null && !_AddPackage.Request.IsCompleted) ||
                (_RemovePackage != null && !_RemovePackage.Request.IsCompleted) ||
                (EditorApplication.isCompiling || EditorApplication.isUpdating))
            {
                GUI.backgroundColor = Color.yellow;
                GUILayout.Space(8);
                GUILayout.Label("Wait for editor reloading...", EditorStyles.foldoutHeader);

                return;
            }

            GUILayout.BeginVertical();

            GUILayout.Space(8);
            GUILayout.Label($"Packages ({_Packages.Count}):", EditorStyles.boldLabel);

            foreach (var package in _Packages)
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

            if (_InstalledPackageCount == _Packages.Count)
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

            bool packageCheck = (_InstalledPackageCount == _Packages.Count) && _UpdatablePackageCount == 0;
            GUI.backgroundColor = packageCheck ? Color.green : Color.red;
            GUILayout.Toggle(packageCheck, "Install & Update all packages");

            GUI.backgroundColor = symbolCheck ? Color.green : Color.red;
            GUILayout.Toggle(symbolCheck, "Add Backend Define Symbol");

            bool dataCheck = !(PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS).Equals("com.Talus.TalusTemplateURP"));
            GUI.backgroundColor = dataCheck ? Color.green : Color.red;
            GUILayout.Toggle(dataCheck, "Populate 'TalusKit/Backend/App Settings' and click 'Update Settings' button");

            GUILayout.EndVertical();
        }

        private void ListPackages()
        {
            if (_ListPackages != null && !_AddPackage.Request.IsCompleted) { return; }

            _ListPackages = new RequestHandler<ListRequest>(Client.List(), (statusCode) =>
            {
                if (statusCode == StatusCode.Success)
                {
                    foreach (var package in _ListPackages.Request.Result)
                    {
                        if (!_Packages.ContainsKey(package.name)) { continue; }

                        bool isGitPackage = (package.source == PackageSource.Git);
                        string gitHash = (isGitPackage) ? package.git.hash : "";

                        _Packages[package.name] = new Models.PackageStatus(true, gitHash, false);

                        if (isGitPackage)
                        {
                            CheckPackageVersion(package.name, gitHash);
                        }

                        ++_InstalledPackageCount;
                    }
                }
                else
                {
                    InfoBox.Create("Error :(", _ListPackages.Request.Error.message, "OK");
                }

                RefreshWindow();
            });
        }

        private void RemoveBackendPackage(string packageId)
        {
            if (_RemovePackage != null && !_RemovePackage.Request.IsCompleted) { return; }

            _RemovePackage = new RequestHandler<RemoveRequest>(Client.Remove(packageId), (statusCode) =>
            {
                string message = (statusCode == StatusCode.Success) ?
                _RemovePackage.Request.PackageIdOrName + " removed successfully!" :
                _RemovePackage.Request.Error.message;

                InfoBox.Create($"{statusCode} !", message, "OK");

                RefreshWindow();
            });
        }

        private void AddBackendPackage(string packageId)
        {
            if (_AddPackage != null && !_AddPackage.Request.IsCompleted) { return; }

            BackendApi api = new BackendApi(BackendDefinitions.ApiUrl, BackendDefinitions.ApiToken);
            api.GetPackageInfo(packageId, package => {
                _AddPackage = new RequestHandler<AddRequest>(Client.Add(package.url), (statusCode) =>
                {
                    string message = (statusCode == StatusCode.Success) ?
                    _AddPackage.Request.Result.packageId + " added successfully!" :
                    _AddPackage.Request.Error.message;

                    InfoBox.Create($"{statusCode} !", message, "OK");

                    RefreshWindow();
                });
            });
        }

        private void CheckPackageVersion(string packageId, string packageHash)
        {
            BackendApi api = new BackendApi(BackendDefinitions.ApiUrl, BackendDefinitions.ApiToken);
            api.GetPackageInfo(packageId, package =>
            {
                bool updateExist = !packageHash.Equals(package.hash);
                _Packages[packageId].UpdateExist = updateExist;

                if (updateExist)
                {
                    ++_UpdatablePackageCount;
                }
            });
        }

        private void RepaintManagerWindow()
        {
            if (s_Instance == null) { return; }

            s_Instance.Repaint();
        }

        private void RefreshWindow(bool saveAssets = true)
        {
            if (saveAssets)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            RepaintManagerWindow();
        }
    }
}