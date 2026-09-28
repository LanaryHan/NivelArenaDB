using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime.Business.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ZEvent;
using Object = UnityEngine.Object;

namespace Runtime.Business.Manager
{
    public class ResManager : Singleton<ResManager>
    {
        private sealed class SpriteCacheEntry
        {
            public AsyncOperationHandle<Sprite> Handle;
            public int RefCount;
        }

        private static readonly Dictionary<string, AsyncOperationHandle> _dialogHandles = new();
        private static readonly Dictionary<string, SpriteCacheEntry> _spriteCache = new();
        public ResManager(){ }

        #region Common

        public TObject Load<TObject>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default;
            }

            var handle = Addressables.LoadAssetAsync<TObject>(key);
            handle.WaitForCompletion();
            TObject result = default;
            var isOk = handle.IsDone && handle.Status is AsyncOperationStatus.Succeeded;
            if (isOk)
            {
                result = handle.Result;
            }
            else
            {
                Addressables.Release(handle);
            }

            return result;
        }

        public async UniTask<TObject> LoadAsync<TObject>(string key)
        {
            var handle = Addressables.LoadAssetAsync<TObject>(key);
            await AsyncHandle(handle);
            return handle.Result;
        }

        public TObject LoadPrefab<TObject>(string key) where TObject : Object
        {
            if (!key.EndsWith(".prefab"))
            {
                key += ".prefab";
            }

            var obj = Load<TObject>(key);
            if (obj == null)
            {
                return null;
            }
            
            return obj;
        }

        public TObject LoadPrefabAndInstantiate<TObject>(string key, Transform parent = null) where TObject : Object
        {
            var obj = LoadPrefab<TObject>(key);
            if (obj == null)
            {
                return null;
            }

            return Object.Instantiate(obj, parent);
        }

        private async UniTask AsyncHandle<TObject>(AsyncOperationHandle<TObject> handle, CancellationToken cancellationToken = default)
        {
            while (!handle.IsDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await handle.ToUniTask(cancellationToken: cancellationToken);
            }

            if (handle.Status is not AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(handle);
                throw handle.OperationException;
            }
        }
        
        private string GetDialogAddress(Type type)
        {
            return $"{type.Name}/Dialog/{type.Name}.prefab";
        }

        public async UniTask<TObject> LoadDialogAsync<TObject>(Type type)
        {
            var key = GetDialogAddress(type);
            var handle = Addressables.LoadAssetAsync<TObject>(key);
            TryAddDialogHandle(key, handle);
            await AsyncHandle(handle);
            return handle.Result;
        }

        public void ReleaseDialog(Type type)
        {
            var key = GetDialogAddress(type);
            if (_dialogHandles.Remove(key, out var handle))
            {
                Addressables.Release(handle);
            }
        }

        private void TryAddDialogHandle(string key, AsyncOperationHandle handle)
        {
            if (!_dialogHandles.TryAdd(key, handle))
            {
                _dialogHandles.Remove(key, out var oldHandle);
                Addressables.Release(oldHandle);
                _dialogHandles[key] = handle;
            }
        }
        
        #endregion

        #region Data Sprite

        private string GetCardAddress(string id, string addition = "")
        {
            var split = id.Split('-');
            var path = Path.Combine(split[0], id);
            return $"{path}{addition}.png";
        }
        
        private string GetDeckAddress(Deck deck)
        {
            var packEntry = DataManager.Instance.GetPack(deck);
            return $"Packs/{packEntry.BundleRes}.png";
        }

        private SpriteCacheEntry AcquireSprite(string key)
        {
            if (_spriteCache.TryGetValue(key, out var entry))
            {
                entry.RefCount++;
                return entry;
            }

            entry = new SpriteCacheEntry
            {
                Handle = Addressables.LoadAssetAsync<Sprite>(key),
                RefCount = 1,
            };
            _spriteCache.Add(key, entry);
            return entry;
        }

        private Sprite LoadSprite(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            var entry = AcquireSprite(key);
            entry.Handle.WaitForCompletion();
            if (entry.Handle.Status is AsyncOperationStatus.Succeeded)
            {
                return entry.Handle.Result;
            }

            RemoveFailedSprite(key, entry);
            return null;
        }

        private async UniTask<Sprite> LoadSpriteAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            var entry = AcquireSprite(key);
            try
            {
                await entry.Handle.ToUniTask();
                if (entry.Handle.Status is not AsyncOperationStatus.Succeeded)
                {
                    throw entry.Handle.OperationException;
                }

                return entry.Handle.Result;
            }
            catch
            {
                RemoveFailedSprite(key, entry);
                throw;
            }
        }

        private void RemoveFailedSprite(string key, SpriteCacheEntry failedEntry)
        {
            if (!_spriteCache.TryGetValue(key, out var currentEntry) || currentEntry != failedEntry)
            {
                return;
            }

            _spriteCache.Remove(key);
            if (failedEntry.Handle.IsValid())
            {
                Addressables.Release(failedEntry.Handle);
            }
        }

        private void ReleaseSprite(string key)
        {
            if (string.IsNullOrEmpty(key) || !_spriteCache.TryGetValue(key, out var entry))
            {
                return;
            }

            entry.RefCount--;
            if (entry.RefCount > 0)
            {
                return;
            }

            _spriteCache.Remove(key);
            if (entry.Handle.IsValid())
            {
                Addressables.Release(entry.Handle);
            }
        }

        public Sprite LoadCardSprite(string id)
        {
            var path = GetCardAddress(id);
            return LoadSprite(path);
        }

        public UniTask<Sprite> LoadCardSpriteAsync(string id)
        {
            var path = GetCardAddress(id);
            return LoadSpriteAsync(path);
        }

        public void ReleaseCardSprite(string id)
        {
            ReleaseSprite(GetCardAddress(id));
        }

        public Sprite LoadSpecialCardSprite(string id)
        {
            var path = GetCardAddress(id, "Sign");
            return LoadSprite(path);
        }

        public void ReleaseSpecialCardSprite(string id)
        {
            ReleaseSprite(GetCardAddress(id, "Sign"));
        }

        public Sprite LoadExtensionCardSprite(string id)
        {
            var path = GetCardAddress(id, "Ex");
            return LoadSprite(path);
        }

        public void ReleaseExtensionCardSprite(string id)
        {
            ReleaseSprite(GetCardAddress(id, "Ex"));
        }

        public Sprite LoadPackSprite(Deck pack)
        {
            var path = GetDeckAddress(pack);
            return LoadSprite(path);
        }

        public void ReleasePackSprite(Deck pack)
        {
            ReleaseSprite(GetDeckAddress(pack));
        }

        #endregion
    }
}
