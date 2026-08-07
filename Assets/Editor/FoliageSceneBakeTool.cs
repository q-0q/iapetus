using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SomeNamespace
{
    public class FoliageSceneBakeTool
    {
        [MenuItem("Tools/Bake FoliageSceneData for current Scene")]
        public static void RunActionOnAllTargets()
        {
            FoliageSystem[] foliageSystems = Object.FindObjectsByType<FoliageSystem>(FindObjectsSortMode.None);
            if (foliageSystems.Length == 0) return;

            var foliageSystemDatas = new FoliageSystem.FoliageSystemData[foliageSystems.Length];


            for (var i = 0; i < foliageSystems.Length; i++)
            {
                var comp = foliageSystems[i];
                foliageSystemDatas[i] = comp.GenerateFoliageSystemData();
            }
        
            FoliageSerializer.WriteFoliageSceneData(SceneManager.GetActiveScene().name, foliageSystemDatas);
        }
    }
}