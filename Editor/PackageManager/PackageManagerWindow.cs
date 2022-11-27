using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using UnityEngine;

using TalusBackendData.Editor.PackageManager.Requests;
using TalusBackendData.Editor.Utility;
using TalusBackendData.Editor.Models;

using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using PackageStatus = TalusBackendData.Editor.PackageManager.Models.PackageStatus;

namespace TalusBackendData.Editor.PackageManager
{
    internal class PackageManagerWindow : EditorWindow
    {
        private static PackageManagerWindow s_Instance;
        private static PackageManagerWindow Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = GetWindow<PackageManagerWindow>();
                }

                return s_Instance;
            }
        }

        private readonly Dictionary<string, PackageStatus> _Packages = new();
        
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

        private void ShowInfoText(string text, Color color)
        {
            GUI.backgroundColor = color;
            GUILayout.Space(8);
            GUILayout.Label(text, EditorStyles.foldoutHeader);
        }

        // shows descriptions of buttons
        private void ShowHeaderMenu()
        {
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            {
                GUI.backgroundColor = Color.green; GUILayout.Button("Installed");
                GUI.backgroundColor = Color.yellow; GUILayout.Button("Update available");
                GUI.backgroundColor = Color.red; GUILayout.Button("Not installed");
                GUILayout.Space(30);
                GUI.backgroundColor = Color.cyan;

                if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh"), GUILayout.Width(60f)))
                {
                    RefreshPackages();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void ShowPackagesMenu()
        {
            GUILayout.Space(16);
            GUILayout.Label($"Packages ({_Packages.Count}):", EditorStyles.boldLabel);

            foreach (KeyValuePair<string, PackageStatus> package in _Packages)
            {
                bool isPackageInstalled = package.Value.Exist;
                bool isUpdateExist = package.Value.UpdateExist;

                GUI.backgroundColor = (isPackageInstalled) 
                        ? ((isUpdateExist) ? Color.yellow : Color.green) 
                        : Color.red;

                if (GUILayout.Button(package.Value.DisplayName, GUILayout.MinHeight(25)))
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

        private void OnGUI()
        {
            if (_ListPackages == null || !_ListPackages.Request.IsCompleted)
            {
                ShowInfoText("Preparing package list...", Color.yellow);
                return;
            }

            if (IsUnityReloading())
            {
                ShowInfoText("Wait for editor reloading...", Color.yellow);
                return;
            }

            GUILayout.BeginVertical();

            ShowHeaderMenu();
            ShowPackagesMenu();

            GUILayout.EndVertical();
        }

        private void PopulatePackages(System.Action onComplete = null)
        {
            var api = new BackendApi(BackendSettingsHolder.instance.ApiUrl, BackendSettingsHolder.instance.ApiToken);
            api.GetAllPackages((response) =>
            {
                _Packages.Clear();

                foreach (PackageModel model in response.packages)
                {
                    _Packages[model.package_id] = new PackageStatus
                    {
                        Exist = false,
                        DisplayName = model.package_id,
                        Hash = model.hash,
                        UpdateExist = false
                    };
                }

                onComplete?.Invoke();
            });
        }

        private void RefreshPackages()
        {
            PopulatePackages(ListPackages);
        }

        private void ListPackages()
        {
            if (_ListPackages != null && !_ListPackages.Request.IsCompleted) { return; }

            _ListPackages = new RequestHandler<ListRequest>(
                Client.List(), 
                statusCode => {

                    if (statusCode != StatusCode.Success)
                    {
                        InfoBox.Show("Error :(", _ListPackages.Request.Error.message, "OK");
                        return;
                    }


                    foreach (PackageInfo package in _ListPackages.Request.Result
                        .Where(package => _Packages.ContainsKey(package.name)))
                    {
                        bool isGitPackage = (package.source == PackageSource.Git);
                        string packageHash = (isGitPackage) ? package.git.hash : string.Empty;

                        _Packages[package.name] = new PackageStatus {
                            Exist = true,
                            DisplayName = package.displayName,
                            Hash = packageHash,
                            UpdateExist = false
                        };

                        if (isGitPackage)
                        {
                            CheckPackageVersion(package.name, packageHash);
                        }
                    }

                    RefreshWindowInstance(false);
                }
            );
        }

        private void RemovePackage(string packageId)
        {
            if (_RemovePackage != null && !_RemovePackage.Request.IsCompleted) { return; }

            _RemovePackage = new RequestHandler<RemoveRequest>(
                Client.Remove(packageId), 
                statusCode => {
                    
                    string message = (statusCode == StatusCode.Success)
                            ? $"{_RemovePackage.Request.PackageIdOrName} removed successfully!"
                            : _RemovePackage.Request.Error.message;

                    InfoBox.Show($"{statusCode} !", message, "OK");
                    RefreshWindowInstance();
                }
            );
        }

        private void AddPackage(string packageId)
        {
            if (_AddPackage != null && !_AddPackage.Request.IsCompleted) { return; }

            var api = new BackendApi(BackendSettingsHolder.instance.ApiUrl, BackendSettingsHolder.instance.ApiToken);
            api.GetPackageInfo(packageId, package => {
                _AddPackage = new RequestHandler<AddRequest>(
                    Client.Add(package.url), 
                    statusCode => {
                        
                        string message = (statusCode == StatusCode.Success)
                                ? $"{_AddPackage.Request.Result.packageId} added successfully!"
                                : _AddPackage.Request.Error.message;

                        InfoBox.Show($"{statusCode} !", message, "OK");
                        RefreshWindowInstance();
                    }
                );
            });
        }

        private void CheckPackageVersion(string packageId, string packageHash)
        {
            var api = new BackendApi(BackendSettingsHolder.instance.ApiUrl, BackendSettingsHolder.instance.ApiToken);
            api.GetPackageInfo(packageId, package =>
            {
                bool updateExist = !packageHash.Equals(package.hash);
                _Packages[packageId].UpdateExist = updateExist;

                if (updateExist)
                {
                    RefreshWindowInstance(false);
                }
            });
        }

        private bool IsUnityReloading()
        {
            return ((_AddPackage != null && !_AddPackage.Request.IsCompleted)
                    || (_RemovePackage != null && !_RemovePackage.Request.IsCompleted)
                    || (EditorApplication.isCompiling || EditorApplication.isUpdating));
        }

        private static void RefreshWindowInstance(bool saveAssets = true)
        {
            if (saveAssets)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Instance.Repaint();
        }
    }
}
