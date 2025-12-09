using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Runtime.Editor
{
    public class TextureRenamer : EditorWindow
    {
        [MenuItem("Tools/批量重命名文件")]
        public static void ShowWindow()
        {
            GetWindow<TextureRenamer>("图片重命名工具");
        }

        private string fileType = "*.png";
        private string searchPath = "";
        private string prefix = "";
        private string separator = "-";

        void OnGUI()
        {
            GUILayout.Label("图片重命名设置", EditorStyles.boldLabel);
            fileType = EditorGUILayout.TextField("文件类型", fileType);
            searchPath = EditorGUILayout.TextField("文件路径", searchPath);
            prefix = EditorGUILayout.TextField("前缀", prefix);
            separator = EditorGUILayout.TextField("分隔符", separator);

            if (GUILayout.Button("批量重命名"))
            {
                RenameTextures();
            }
        }
        private void RenameTextures()
        {
            
            var path = Path.Combine(Application.dataPath, "Artworks/Textures", searchPath);
            if (!Directory.Exists(path))
            {
                EditorUtility.DisplayDialog("错误", "路径不存在", "确定");
                return;
            }

            var files = Directory.GetFiles(path, fileType, SearchOption.TopDirectoryOnly)
                .Where(file => !file.EndsWith(".meta"))
                .OrderBy(file => file).ToArray();
            
            AssetDatabase.StartAssetEditing();
            var count = 0;
            try
            {
                for (var i = 0; i < files.Length; i++)
                {
                    var file = files[i];
                    var assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
                    var insertZero = i >= 9 ? "0" : "00";
                    var newName = $"{prefix}{separator}{insertZero}{i + 1}";
                    var result = AssetDatabase.RenameAsset(assetPath, newName);
                    if (string.IsNullOrEmpty(result))
                    {
                        Debug.Log($"重命名成功:{Path.GetFileName(file)} => {newName}");
                        count++;
                    }
                    else
                    {
                        Debug.LogError($"重命名失败:{result}");
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }

            EditorUtility.DisplayDialog("完成", $"已重命名{count}个文件", "确定");
        }
    }
}