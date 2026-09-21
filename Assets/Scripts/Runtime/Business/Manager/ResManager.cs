using System.IO;
using QFramework;
using Runtime.Business.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.Business.Manager
{
    public class ResManager : Singleton<ResManager>
    {
        protected ResManager(){ }

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

        public Sprite LoadCardSprite(string id)
        {
            var path = GetCardAddress(id);
            var sprite = Load<Sprite>(path);
            return sprite;
        }

        public Sprite LoadSpecialCardSprite(string id)
        {
            var path = GetCardAddress(id, "Sign");
            var sprite = Load<Sprite>(path);
            return sprite;
        }

        public Sprite LoadExtensionCardSprite(string id)
        {
            var path = GetCardAddress(id, "Ex");
            var sprite = Load<Sprite>(path);
            return sprite;
        }

        public Sprite LoadPackSprite(Deck pack)
        {
            var path = GetDeckAddress(pack);
            var sprite = Load<Sprite>(path);
            return sprite;
        }

        #endregion
    }
}