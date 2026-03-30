using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class FoliageChunkManager : MonoBehaviour
{
    public static FoliageChunkManager Instance;

    [Header("Culling Settings")]
    public float chunkSize = 20f;
    public float renderDistance = 150f;
    [Tooltip("Distance outside the screen to keep rendering (prevents pop-in)")]
    public float frustumPadding = 5f;

    [Header("LOD Settings")]
    public float lodFullDistance = 50f; // 100% density up to here
    [Range(0, 1)] public float lodFarDensity = 0.5f; // 50% density at distance

    // We group by Material + Mesh to handle different foliage types
    private Dictionary<System.ValueTuple<Mesh, Material>, Dictionary<Vector3Int, List<Matrix4x4>>> _masterRegistry = new();
    private Dictionary<System.ValueTuple<Mesh, Material>, Dictionary<Vector3Int, Matrix4x4[][]>> _bakedChunks = new();

    private Camera _camera;
    private void Awake() => Instance = this;

    private void Start()
    {
        _camera = Camera.main;
        Invoke(nameof(BakeAll), 0.5f);
    }

    public void RegisterFoliage(Mesh mesh, Material mat, Matrix4x4[] instances)
    {
        var key = (mesh, mat);
        if (!_masterRegistry.ContainsKey(key)) _masterRegistry[key] = new Dictionary<Vector3Int, List<Matrix4x4>>();

        foreach (var matrix in instances)
        {
            Vector3 pos = matrix.GetColumn(3);
            Vector3Int chunkPos = Vector3Int.FloorToInt(pos / chunkSize);

            if (!_masterRegistry[key].ContainsKey(chunkPos)) _masterRegistry[key][chunkPos] = new List<Matrix4x4>();
            _masterRegistry[key][chunkPos].Add(matrix);
        }
    }

    // Call this after all FoliageSystems have registered their data (e.g., end of Start)
    public void BakeAll()
    {
        _bakedChunks.Clear();
        foreach (var entry in _masterRegistry)
        {
            var chunkDict = new Dictionary<Vector3Int, Matrix4x4[][]>();
            foreach (var chunk in entry.Value)
            {
                // Graphics.RenderMeshInstanced has a limit of 1023 instances per call
                chunkDict[chunk.Key] = BatchMatrices(chunk.Value);
            }
            _bakedChunks[entry.Key] = chunkDict;
        }
        _masterRegistry.Clear(); // Free memory
    }

    private Matrix4x4[][] BatchMatrices(List<Matrix4x4> fullList)
    {
        int count = fullList.Count;
        int batches = Mathf.CeilToInt(count / 1023f);
        Matrix4x4[][] batched = new Matrix4x4[batches][];
        for (int i = 0; i < batches; i++)
        {
            int size = Mathf.Min(1023, count - (i * 1023));
            batched[i] = new Matrix4x4[size];
            fullList.CopyTo(i * 1023, batched[i], 0, size);
        }
        return batched;
    }

    void Update()
    {
        
        Vector3 camPos = _camera.transform.position;
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);
        
        // --- APPLY PADDING ---
        // We move each plane outward by the padding amount
        for (int i = 0; i < 6; i++) 
        {
            planes[i].distance += frustumPadding;
        }

        foreach (var entry in _bakedChunks)
        {
            Mesh mesh = entry.Key.Item1;
            Material mat = entry.Key.Item2;
            RenderParams rp = new RenderParams(mat);

            foreach (var chunk in entry.Value)
            {
                Vector3 chunkCenter = new Vector3(
                    chunk.Key.x * chunkSize + chunkSize / 2, 
                    camPos.y, 
                    chunk.Key.z * chunkSize + chunkSize / 2
                );

                float dist = Vector3.Distance(camPos, chunkCenter);
                if (dist > renderDistance) continue;

                Bounds b = new Bounds(chunkCenter, new Vector3(chunkSize, 50f, chunkSize));
                if (!GeometryUtility.TestPlanesAABB(planes, b)) continue;

                // --- LOD CALCULATION ---
                // If far away, we only loop through a percentage of the batches/matrices
                float densityPercent = (dist > lodFullDistance) ? lodFarDensity : 1.0f;

                foreach (var batch in chunk.Value)
                {
                    int countToRender = Mathf.CeilToInt(batch.Length * densityPercent);
                    if (countToRender <= 0) continue;

                    // Use the overload that allows specifying a count
                    Graphics.RenderMeshInstanced(rp, mesh, 0, batch, countToRender);
                }
            }
        }
    }
    
    private void Shuffle(List<Matrix4x4> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Matrix4x4 temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}