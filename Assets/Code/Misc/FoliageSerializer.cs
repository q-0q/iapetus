using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

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
            string json = JsonUtility.ToJson(foliageSceneData, true);
            var filePath = GetFilepathForScene(sceneName);
            File.WriteAllText(filePath, json);

            Debug.Log($"Saved FoliageSceneData array to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save FoliageSceneData: {e.Message}");
        }
    }

    public static FoliageSceneData LoadFoliageSceneData(string sceneName)
    {
        // var filePath = GetFilepathForScene(sceneName);
        // if (!File.Exists(filePath))
        // {
        //     Debug.LogError($"File does not exist at path: {filePath}");
        //     return null;
        // }

        try
        {
            var textAsset = Resources.Load<TextAsset>("FoliageSceneData/" + sceneName);
            FoliageSceneData foliageSceneData = JsonUtility.FromJson<FoliageSceneData>(textAsset.text);
            return foliageSceneData;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load matrix array: {e.Message}");
            return null;
        }
    }

    private static string GetFilepathForScene(string sceneName)
    {
        string dirPath = Path.Combine(Application.dataPath, "Resources", "FoliageSceneData");
        return Path.Combine(dirPath, sceneName + ".json");
    }
}