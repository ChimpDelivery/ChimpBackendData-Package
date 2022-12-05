using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using TalusBackendData.Editor.Utility;

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

        private readonly PackageManager _PackageManager = new();

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
            _PackageManager.RefreshPackages();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void ShowPackagesMenu()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label($"Packages ({_PackageManager.PackageCount}):", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                DrawButton(
                    EditorGUIUtility.IconContent("Refresh"), 
                    Color.cyan, 
                    () => _PackageManager.RefreshPackages()
                );
            }
            GUILayout.EndVertical();

            foreach (KeyValuePair<string, PackageStatus> package in _PackageManager.Packages)
            {
                bool isPackageInstalled = package.Value.Exist;
                bool isUpdateExist = package.Value.UpdateExist;
                Color buttonBgColor = (isPackageInstalled)
                        ? ((isUpdateExist) ? Color.yellow : Color.green)
                        : Color.red;

                DrawButton(new GUIContent(package.Value.DisplayName),
                    buttonBgColor,
                    () =>
                    {
                        if (!isPackageInstalled || isUpdateExist)
                        {
                            _PackageManager.AddPackage(package.Key);
                            return;
                        }

                        InfoBox.ShowConfirmation(
                            $"You are about to remove the '{package.Key}' package!",
                            () => _PackageManager.RemovePackage(package.Key)
                        );
                    }
                );
            }
        }

        private void OnGUI()
        {
            if (_PackageManager.IsPreparingList)
            {
                ShowInfoText("Preparing package list...", Color.yellow);
                return;
            }

            if (_PackageManager.IsReloading)
            {
                ShowInfoText("Wait for editor reloading...", Color.yellow);
                return;
            }

            GUILayout.BeginVertical();

            ShowPackagesMenu();
            ShowButtonDescriptions();

            GUILayout.EndVertical();
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
