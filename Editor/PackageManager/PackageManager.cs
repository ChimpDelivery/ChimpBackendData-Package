using System.Linq;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using TalusBackendData.Editor.Utility;
using TalusBackendData.Editor.Models;
using TalusBackendData.Editor.Requests;
using TalusBackendData.Editor.PackageManager.Requests;

using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using PackageStatus = TalusBackendData.Editor.PackageManager.Models.PackageStatus;

namespace TalusBackendData.Editor.PackageManager
{
    public class PackageManager
    {
        public Dictionary<string, PackageStatus> Packages { get; } = new();
        public int PackageCount => Packages.Count;

        public bool IsReloading =>
            ((_AddPackage != null && !_AddPackage.IsCompleted) ||
            (_RemovePackage != null && !_RemovePackage.IsCompleted) ||
            (EditorApplication.isCompiling || EditorApplication.isUpdating));

        private RequestHandler<ListRequest> _ListPackages;
        public bool IsPreparingList => _ListPackages == null || !_ListPackages.IsCompleted;

        private RequestHandler<AddRequest> _AddPackage;
        private RequestHandler<RemoveRequest> _RemovePackage;

        public bool IsPackageExist(string packageId)
        {
            return Packages[packageId].Exist;
        }
        
        public void RefreshPackages(System.Action onComplete = null)
        {
            if (_ListPackages != null && !_ListPackages.IsCompleted) { return; }

            PopulatePackages(() => ListPackages(onComplete));
        }

        public void RemovePackage(string packageId)
        {
            if (_RemovePackage != null && !_RemovePackage.IsCompleted) { return; }

            _RemovePackage = new RequestHandler<RemoveRequest>(
                Client.Remove(packageId),
                statusCode =>
                {
                    string message = (statusCode == StatusCode.Success)
                        ? $"{_RemovePackage.Request.PackageIdOrName} removed successfully!"
                        : _RemovePackage.Request.Error.message;

                    InfoBox.Show($"{statusCode} !", message, "OK");
                }
            );
        }

        public void AddPackage(string packageId, System.Action<bool> onComplete = null)
        {
            if (_AddPackage != null && !_AddPackage.IsCompleted) { return; }

            BackendApi.GetApi<GetPackageRequest, PackageModel>(
                new GetPackageRequest { PackageId = packageId },
                onFetchComplete: package =>
                {
                    _AddPackage = new RequestHandler<AddRequest>(
                        Client.Add(package.url),
                        statusCode =>
                        {
                            bool success = (statusCode == StatusCode.Success);
                            string message = (success)
                                ? $"{_AddPackage.Request.Result.packageId} added successfully!"
                                : _AddPackage.Request.Error.message;

                            onComplete?.Invoke(success);

                            InfoBox.Show($"{statusCode} !", message, "OK");
                        }
                    );
                }
            );
        }

        public void CheckPackageVersion(string packageId, string packageHash)
        {
            BackendApi.GetApi<GetPackageRequest, PackageModel>(
                new GetPackageRequest { PackageId = packageId },
                onFetchComplete: package =>
                {
                    bool updateExist = !packageHash.Equals(package.hash);
                    Packages[packageId].UpdateExist = updateExist;
                }
            );
        }

        private void PopulatePackages(System.Action onComplete)
        {
            BackendApi.GetApi<GetPackagesRequest, PackagesModel>(
                new GetPackagesRequest(),
                onFetchComplete: response =>
                {
                    Packages.Clear();

                    foreach (PackageModel model in response.packages)
                    {
                        Packages[model.package_id] = new PackageStatus
                        {
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

        private void ListPackages(System.Action onComplete = null)
        {
            _ListPackages = new RequestHandler<ListRequest>(
                Client.List(),
                statusCode =>
                {
                    ListRequest request = _ListPackages.Request;
                    
                    if (statusCode != StatusCode.Success)
                    {
                        InfoBox.Show("Error :(", request.Error.message, "OK");
                        return;
                    }
                    
                    foreach (PackageInfo package in request.Result.Where(package => Packages.ContainsKey(package.name)))
                    {
                        bool isGitPackage = package.source == PackageSource.Git;
                        string packageHash = isGitPackage ? package.git.hash : string.Empty;

                        Packages[package.name] = new PackageStatus 
                        {
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

                    onComplete?.Invoke();
                }
            );
        }
    }
}