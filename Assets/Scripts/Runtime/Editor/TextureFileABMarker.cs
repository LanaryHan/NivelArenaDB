using System;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Runtime.Editor
{
    [InitializeOnLoad]
    public class TextureFileABMarker
    {
        private const string Mark_AssetBundle = "Assets/@ResKit - AssetBundle Mark";
        private const string Mark_AssetBundleDirectory = "Assets/@ResKit - AssetBundle Mark Directory";
        
        static TextureFileABMarker()
        {
            Selection.selectionChanged = OnSelectionChanged;
        }

        private static void OnSelectionChanged()
        {
            var path = GetSelectedPathOrFallback();
            if (!string.IsNullOrEmpty(path))
            {
                Menu.SetChecked(Mark_AssetBundleDirectory, IsAllMarked(path));
            }
        }

        private static bool IsAllMarked(string path)
        {
            if (!path.StartsWith("Assets/Artworks/Textures"))
            {
                return false;
            }

            if (!Directory.Exists(path))
            {
                return false;
            }

            var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { path });
            if (guids == null || guids.Length == 0)
            {
                return false;
            }

            var textures = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();
            return textures.All(Marked);
        }

        [MenuItem(Mark_AssetBundleDirectory)]
        public static void MarkedFile()
        {
            var path = GetSelectedPathOrFallback();
            if (!Directory.Exists(path))
            {
                return;
            }

            if (!string.IsNullOrEmpty(path))
            {
                bool toAllWhat;
                if (Menu.GetChecked(Mark_AssetBundleDirectory))
                {
                    Menu.SetChecked(Mark_AssetBundleDirectory, false);
                    toAllWhat = false;
                }
                else
                {
                    Menu.SetChecked(Mark_AssetBundleDirectory, true);
                    toAllWhat = true;
                }

                var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { path });
                var textures = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();
                foreach (var file in textures)
                {
                    var ai = AssetImporter.GetAtPath(file);
                    var dir = new DirectoryInfo(file);
                    if (Marked(path))
                    {
                        if (!toAllWhat)
                        {
                            Menu.SetChecked(Mark_AssetBundle, false);
                            ai.assetBundleName = null;
                        }
                    }
                    else
                    {
                        if (toAllWhat)
                        {
                            Menu.SetChecked(Mark_AssetBundle, true);
                            ai.assetBundleName = dir.Name.Replace(".", "_");
                        }
                    }
                }

                AssetDatabase.RemoveUnusedAssetBundleNames();
            }
        }

        private static bool Marked(string path)
        {
            try
            {
                var ai = AssetImporter.GetAtPath(path);
                var dir = new DirectoryInfo(path);
                return string.Equals(ai.assetBundleName, dir.Name.Replace(".", "_").ToLower());
            }
#pragma warning disable CS0168
            catch (Exception _)
#pragma warning restore CS0168
            {
                return false;
            }
        }

        public static string GetSelectedPathOrFallback()
        {
            var path = string.Empty;

            foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);

                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    return path;
                }
            }

            return path;
        }
    }
}