using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(CanvasRenderer))]
public class UISplineRenderer : MaskableGraphic
{
    [Header("Mesh Source")]
    public MeshFilter sourceMeshFilter;

    // Automatically grab the mesh filter if it's on the same GameObject
    protected override void Awake()
    {
        base.Awake();
        if (sourceMeshFilter == null) sourceMeshFilter = GetComponent<MeshFilter>();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (sourceMeshFilter == null || sourceMeshFilter.sharedMesh == null) 
            return;

        Mesh mesh = sourceMeshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = mesh.uv;
        int[] triangles = mesh.triangles;

        // Populate the UI VertexHelper with the 3D spline mesh data
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 uv = (uvs != null && uvs.Length == vertices.Length) ? uvs[i] : Vector2.zero;
            vh.AddVert(vertices[i], color, uv);
        }

        for (int i = 0; i < triangles.Length; i += 3)
        {
            vh.AddTriangle(triangles[i], triangles[i + 1], triangles[i + 2]);
        }
    }

    private void Update()
    {
        // Force the UI to redraw whenever the spline mesh updates in the editor or runtime
        SetVerticesDirty();
    }
}