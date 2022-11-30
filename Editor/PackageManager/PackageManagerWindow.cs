using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using TalusBackendData.Editor.PackageManager.Requests;
using TalusBackendData.Editor.Utility;
using TalusBackendData.Editor.Models;
using TalusBackendData.Editor.Requests;

using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using PackageStatus = TalusBackendData.Editor.PackageManager.Models.PackageStatus;

namespace TalusBackendData.Editor.PackageManager
{
    internal class PackageManagerWindow : EditorWindow
    {
        private const string _WindowTitle = "Talus Package Manager";

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

            s_Instance = GetWindow<PackageManagerWindow>(_WindowTitle);
            s_Instance.minSize = new Vector2(450, 450);
            s_Instance.Show();
        }

        private void OnEnable()
        {
            RefreshPackages();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
        
        private void ShowPackagesMenu()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label($"Packages ({_Packages.Count}):", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                DrawButton(EditorGUIUtility.IconContent("Refresh"), Color.cyan, RefreshPackages);
            }
            GUILayout.EndVertical();
            
            foreach (KeyValuePair<string, PackageStatus> package in _Packages)
            {
                bool isPackageInstalled = package.Value.Exist;
                bool isUpdateExist = package.Value.UpdateExist;
                Color buttonBgColor = (isPackageInstalled) 
                        ? ((isUpdateExist) ? Color.yellow : Color.green) 
                        : Color.red;
                
                DrawButton(new GUIContent(package.Value.DisplayName), 
                    buttonBgColor, 
                    () => {
                        
                        if (!isPackageInstalled || isUpdateExist)
                        {
                            AddPackage(package.Key);
                            return;
                        }

                        InfoBox.ShowConfirmation(
                            $"You are about to remove the '{package.Key}' package!",
                            () => RemovePackage(package.Key)
                        );
                    }
                );
            }
        }

        private void OnGUI()
        {
            if (_ListPackages == null || !_ListPackages.IsCompleted)
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

            ShowPackagesMenu();
            ShowButtonDescriptions();

            GUILayout.EndVertical();
        }

        private void PopulatePackages(System.Action onComplete)
        {
            BackendApi.GetApi<GetPackagesRequest, PackagesModel>(
                new GetPackagesRequest(),
                response => {
                    _Packages.Clear();
                
                    foreach (PackageModel model in response.packages)
                    {
                        _Packages[model.package_id] = new PackageStatus {
                            Exist = false,
                            DisplayName = model.package_id,
                            Hash = model.hash,
                            UpdateExist = false
                        };
                    }

                    onComplete.Invoke();
                }
            );
        }

        private void RefreshPackages()
        {
            if (_ListPackages != null && !_ListPackages.IsCompleted) { return; }

            PopulatePackages(ListPackages);
        }

        private void ListPackages()
        {
            _ListPackages = new RequestHandler<ListRequest>(
                Client.List(), 
                statusCode => {

                    if (statusCode != StatusCode.Success)
                    {
                        InfoBox.Show("Error :(", _ListPackages.Request.Error.message, "OK");
                        return;
                    }
                    
                    foreach (PackageInfo package 
                        in _ListPackages.Request.Result.Where(package => _Packages.ContainsKey(package.name)))
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
                }
            );
        }

        private void RemovePackage(string packageId)
        {
            if (_RemovePackage != null && !_RemovePackage.IsCompleted) { return; }

            _RemovePackage = new RequestHandler<RemoveRequest>(
                Client.Remove(packageId), 
                statusCode => {
                    
                    string message = (statusCode == StatusCode.Success)
                            ? $"{_RemovePackage.Request.PackageIdOrName} removed successfully!"
                            : _RemovePackage.Request.Error.message;

                    InfoBox.Show($"{statusCode} !", message, "OK");
                }
            );
        }

        private void AddPackage(string packageId)
        {
            if (_AddPackage != null && !_AddPackage.IsCompleted) { return; }

            BackendApi.GetApi<GetPackageRequest, PackageModel>(
                new GetPackageRequest { PackageId = packageId }, 
                package => {
                    _AddPackage = new RequestHandler<AddRequest>(
                        Client.Add(package.url), 
                        statusCode => {
                            
                            string message = (statusCode == StatusCode.Success)
                                    ? $"{_AddPackage.Request.Result.packageId} added successfully!"
                                    : _AddPackage.Request.Error.message;

                            InfoBox.Show($"{statusCode} !", message, "OK");
                        }
                    );
                }
            );
        }

        private void CheckPackageVersion(string packageId, string packageHash)
        {
            BackendApi.GetApi<GetPackageRequest, PackageModel>(
                new GetPackageRequest { PackageId = packageId }, 
                package => {
                    bool updateExist = !packageHash.Equals(package.hash);
                    _Packages[packageId].UpdateExist = updateExist;
                }
            );
        }

        private bool IsUnityReloading()
        {
            return ((_AddPackage != null && !_AddPackage.IsCompleted)
                    || (_RemovePackage != null && !_RemovePackage.IsCompleted)
                    || (EditorApplication.isCompiling || EditorApplication.isUpdating));
        }
        
        private static void ShowInfoText(string text, Color color)
        {
            GUI.backgroundColor = color;
            GUILayout.Space(8);
            GUILayout.Label(text, EditorStyles.foldoutHeader);
        }

        private static void ShowButtonDescriptions()
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            {
                DrawButton(new GUIContent("Installed"), Color.green);
                DrawButton(new GUIContent("Update available"), Color.yellow);
                DrawButton(new GUIContent("Not installed"), Color.red);
            }
            GUILayout.EndHorizontal();
        }

        private static void DrawButton(GUIContent content, Color color, System.Action onClick = null)
        {
            GUI.backgroundColor = color;
            if (GUILayout.Button(content, GUILayout.Height(25f)))
            {
                onClick?.Invoke();
            }
        }
    }
}
