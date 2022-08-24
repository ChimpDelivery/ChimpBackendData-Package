using System.Collections.Generic;

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using UnityEngine;

using TalusBackendData.Editor.PackageManager.Requests;
using TalusBackendData.Editor.Utility;
using TalusBackendData.Editor.Models;

namespace TalusBackendData.Editor.PackageManager
{
    /// <summary>
    ///     <b>Talus Package Manager.</b>
    ///     Package version information is coming from the backend server.
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

        [MenuItem("TalusBackend/Package Manager", false, 10000)]
        private static void Init()
        {
            if (string.IsNullOrEmpty(BackendSettingsHolder.instance.ApiUrl))
            {
                InfoBox.ShowBackendParameterError(nameof(BackendSettingsHolder.instance.ApiUrl));
                return;
            }

            if (string.IsNullOrEmpty(BackendSettingsHolder.instance.ApiToken))
            {
                InfoBox.ShowBackendParameterError(nameof(BackendSettingsHolder.instance.ApiToken));
                return;
            }

            s_Instance = GetWindow<PackageManagerWindow>();
            s_Instance.minSize = new Vector2(450, 500);
            s_Instance.titleContent = new GUIContent("Talus Package Manager");
            s_Instance.Show();
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

            if (IsUnityReloading())
            {
                GUI.backgroundColor = Color.yellow;
                GUILayout.Space(8);
                GUILayout.Label("Wait for editor reloading...", EditorStyles.foldoutHeader);

                return;
            }

            GUILayout.BeginVertical();

            // package manager info
            {
                GUILayout.Space(8);
                GUILayout.BeginHorizontal();

                GUI.backgroundColor = Color.green;
                GUILayout.Button("Installed");

                GUI.backgroundColor = Color.yellow;
                GUILayout.Button("Update available");

                GUI.backgroundColor = Color.red;
                GUILayout.Button("Not installed");

                GUILayout.EndHorizontal();
            }

            // package list
            {
                GUILayout.Space(16);
                GUILayout.Label($"Packages ({_Packages.Count}):", EditorStyles.boldLabel);

                foreach (var package in _Packages)
                {
                    bool isPackageInstalled = package.Value.Exist;
                    bool isUpdateExist = package.Value.UpdateExist;

                    GUI.backgroundColor = (isPackageInstalled) ? ((isUpdateExist) ? Color.yellow : Color.green) : Color.red;

                    if (GUILayout.Button(GetPrettyPackageName(package.Key), GUILayout.MinHeight(25)))
                    {
                        if (!isPackageInstalled || isUpdateExist)
                        {
                            AddPackage(package.Key);
                        }
                        else
                        {
                            InfoBox.ShowConfirmation(
                                $"You are about to remove the '{package.Key}' package!",
                                () => RemovePackage(package.Key)
                            );
                        }
                    }
                }
            }

            // symbol check
            bool symbolCheck = DefineSymbols.Contains(BackendSettingsHolder.instance.BackendSymbol);
            {
                GUILayout.Space(16);
                GUILayout.Label($"Backend Symbol ({BackendSettingsHolder.instance.BackendSymbol}):", EditorStyles.boldLabel);
                GUI.backgroundColor = (symbolCheck) ? Color.green : Color.red;

                GUI.enabled = (_InstalledPackageCount == _Packages.Count) && (_UpdatablePackageCount == 0);
                string buttonName = (symbolCheck) ? "Backend Symbol exist." : "Backend Symbol doesn't exist!";
                if (GUILayout.Button(buttonName, GUILayout.MinHeight(25)))
                {
                    if (symbolCheck)
                    {
                        InfoBox.ShowConfirmation(
                            "You are about to remove the 'Backend Define Symbol' definition!",
                            () => BackendSettingsHolder.instance.RemoveBackendSymbol()
                        );

                        return;
                    }

                    BackendSettingsHolder.instance.AddBackendSymbol();
                }
            }

            // steps
            {
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = default;

                GUILayout.Space(8);
                GUILayout.Label("Backend Integration Status:", EditorStyles.helpBox);

                bool packageCheck = (_InstalledPackageCount == _Packages.Count) && _UpdatablePackageCount == 0;
                GUI.backgroundColor = packageCheck ? Color.green : Color.red;
                GUILayout.Toggle(packageCheck, "All packages installed & updated");

                GUI.backgroundColor = symbolCheck ? Color.green : Color.red;
                GUILayout.Toggle(symbolCheck, "Backend define symbol exists");

                GUI.backgroundColor = default;
                GUILayout.Label("After the ticks turn green, Populate 'TalusBackend/Build Settings' and click 'Update Settings' button", EditorStyles.helpBox);
            }

            GUILayout.EndVertical();
        }

        private string GetPrettyPackageName(string package)
        {
            string[] splitPackageName = package.Split('.');
            string smartPackageName = splitPackageName[splitPackageName.Length - 1];

            return smartPackageName;
        }

        private void PopulatePackages(System.Action onComplete = null)
        {
            BackendApi api = new(BackendSettingsHolder.instance.ApiUrl, BackendSettingsHolder.instance.ApiToken);
            api.GetAllPackages((response) =>
            {
                _Packages.Clear();

                foreach (PackageModel model in response.packages)
                {
                    _Packages[model.package_id] = new Models.PackageStatus(false, model.hash, false);
                }

                onComplete.Invoke();
            });
        }

        private void RefreshPackages()
        {
            _InstalledPackageCount = 0;
            _UpdatablePackageCount = 0;

            PopulatePackages(() => ListPackages());
        }

        private void ListPackages()
        {
            if (_ListPackages != null && !_ListPackages.Request.IsCompleted) { return; }

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
                    InfoBox.Show("Error :(", _ListPackages.Request.Error.message, "OK");
                }

                RefreshWindowInstance();
            });
        }

        private void RemovePackage(string packageId)
        {
            if (_RemovePackage != null && !_RemovePackage.Request.IsCompleted) { return; }

            _RemovePackage = new RequestHandler<RemoveRequest>(Client.Remove(packageId), (statusCode) =>
            {
                string message = (statusCode == StatusCode.Success) ?
                _RemovePackage.Request.PackageIdOrName + " removed successfully!" :
                _RemovePackage.Request.Error.message;

                InfoBox.Show($"{statusCode} !", message, "OK");

                RefreshWindowInstance();
            });
        }

        private void AddPackage(string packageId)
        {
            if (_AddPackage != null && !_AddPackage.Request.IsCompleted) { return; }

            BackendApi api = new(BackendSettingsHolder.instance.ApiUrl, BackendSettingsHolder.instance.ApiToken);
            api.GetPackageInfo(packageId, package =>
            {
                _AddPackage = new RequestHandler<AddRequest>(Client.Add(package.url), (statusCode) =>
                {
                    string message = (statusCode == StatusCode.Success) ?
                    _AddPackage.Request.Result.packageId + " added successfully!" :
                    _AddPackage.Request.Error.message;

                    InfoBox.Show($"{statusCode} !", message, "OK");

                    RefreshWindowInstance();
                });
            });
        }

        private void CheckPackageVersion(string packageId, string packageHash)
        {
            BackendApi api = new(BackendSettingsHolder.instance.ApiUrl, BackendSettingsHolder.instance.ApiToken);
            api.GetPackageInfo(packageId, package =>
            {
                bool updateExist = !packageHash.Equals(package.hash);
                _Packages[packageId].UpdateExist = updateExist;

                if (updateExist)
                {
                    ++_UpdatablePackageCount;

                    RefreshWindowInstance();
                }
            });
        }

        private void RepaintWindowInstance()
        {
            if (s_Instance == null) { return; }

            s_Instance.Repaint();
        }

        private void RefreshWindowInstance(bool saveAssets = true)
        {
            if (saveAssets)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            RepaintWindowInstance();
        }

        private bool IsUnityReloading()
        {
            return ((_AddPackage != null && !_AddPackage.Request.IsCompleted)
                    || (_RemovePackage != null && !_RemovePackage.Request.IsCompleted)
                    || (EditorApplication.isCompiling || EditorApplication.isUpdating));
        }
    }
}