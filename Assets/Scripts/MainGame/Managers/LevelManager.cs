using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private AssetReference sectorAsset;
    [SerializeField] private Transform targetTransform;

    private GameObject _loadedSector;
    private AsyncOperationHandle<GameObject> _loadHandle;

    public void LoadAndGenerateSector()
    {
        var handle = Addressables.InstantiateAsync(sectorAsset, targetTransform.position, Quaternion.identity);
        handle.Completed += OnInstantiateCompleted;
    }

    public void GenerateSector()
    {
        if (_loadedSector == null) return;
        var handle = Addressables.InstantiateAsync(_loadedSector, targetTransform.position, Quaternion.identity);
        handle.Completed += OnInstantiateCompleted;
    }

    private void OnInstantiateCompleted(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded) { }
    }

    public void LoadSector()
    {
        if (_loadHandle.IsValid())
            Addressables.Release(_loadHandle);

        _loadHandle = Addressables.LoadAssetAsync<GameObject>(sectorAsset);
        _loadHandle.Completed += OnLoadCompleted;
    }

    private void OnLoadCompleted(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
            _loadedSector = handle.Result;
    }

    private void OnDestroy()
    {
        if (_loadHandle.IsValid())
            Addressables.Release(_loadHandle);
    }
}
