using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;

public class FoliageSerializer
{
    [Serializable]
    public class FoliageSceneData
    {
        public FoliageSystem.FoliageSystemData[] FoliageSystemDatas;
    }

    public static void WriteFoliageSceneData(string sceneName, FoliageSystem.FoliageSystemData[] foliageSystemDatas)
    {
        try
        {
            FoliageSceneData foliageSceneData = new FoliageSceneData { FoliageSystemDatas = foliageSystemDatas };
            string json = JsonUtility.ToJson(foliageSceneData, false);
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
            var filePath = GetFilepathForScene(sceneName);
            using (FileStream fileStream = File.Create(filePath))
            using (GZipStream gzipStream = new GZipStream(fileStream, CompressionLevel.Optimal))
            {
                gzipStream.Write(jsonBytes, 0, jsonBytes.Length);
            }

            Debug.Log($"Saved compressed FoliageSceneData to: {filePath}");
            
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save FoliageSceneData: {e.Message}");
        }
    }

    public static FoliageSceneData LoadFoliageSceneData(string sceneName)
    {
        try
        {
            var textAsset = Resources.Load<TextAsset>("FoliageSceneData/" + sceneName);
            if (textAsset == null) return null;

            using var memoryStream = new MemoryStream(textAsset.bytes);
            using var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
            using var reader = new StreamReader(gzipStream);
            var json = reader.ReadToEnd();
            return JsonUtility.FromJson<FoliageSceneData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load FoliageSceneData: {e.Message}");
            return null;
        }
    }

    private static string GetFilepathForScene(string sceneName)
    {
        string dirPath = Path.Combine(Application.dataPath, "Resources", "FoliageSceneData");
        Directory.CreateDirectory(dirPath);
        return Path.Combine(dirPath, sceneName + ".bytes");
    }
}