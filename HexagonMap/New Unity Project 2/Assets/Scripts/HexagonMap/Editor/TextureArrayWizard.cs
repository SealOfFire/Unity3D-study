using UnityEditor;
using UnityEngine;

namespace HexagonMap.Editor
{
    /// <summary>
    /// 纹理数组向导
    /// </summary>
    public class TextureArrayWizard : ScriptableWizard
    {
        public Texture2D[] textures;

        /// <summary>
        /// 
        /// </summary>
        [MenuItem("Assets/Create/Texture Array")]
        private static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<TextureArrayWizard>("Create Texture Array", "Create");
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnWizardCreate()
        {
            if (this.textures.Length == 0)
            {
                return;
            }
            // 选取文件保存路径窗口
            string path = EditorUtility.SaveFilePanelInProject("Save Texture Array", "Texture Array", "asset", "Save Texture Array");
            if (path.Length == 0)
            {
                return;
            }

            // 创建纹理数组对象
            Texture2D t = textures[0];
            Texture2DArray textureArray = new Texture2DArray(t.width, t.height, textures.Length, t.format, t.mipmapCount > 1);
            textureArray.anisoLevel = t.anisoLevel;
            textureArray.filterMode = t.filterMode;
            textureArray.wrapMode = t.wrapMode;

            for (int i = 0; i < textures.Length; i++)
            {
                for (int m = 0; m < t.mipmapCount; m++)
                {
                    Graphics.CopyTexture(textures[i], 0, m, textureArray, i, m);
                }
            }

            AssetDatabase.CreateAsset(textureArray, path);
        }
    }
}