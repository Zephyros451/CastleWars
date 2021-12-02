 using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Path))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Road : MonoBehaviour
{
    [Range(0.05f, 5f)]
    [SerializeField] private float spacing = 1f;
    [SerializeField] private float roadWidth = 0.1f;

    [SerializeField, HideInInspector] private float tiling = 1f;
    [SerializeField, HideInInspector] private MeshRenderer renderer;
    [SerializeField, HideInInspector] private Material material;

    public void CreateRoad()
    {
        BezierCurve curve = GetComponent<Path>().Curve;
        Vector3[] points = curve.CalculateEvenlySpacedPoints(spacing);
        GetComponent<MeshFilter>().mesh = CreateRoadMesh(points);

        int textureRepeat = Mathf.RoundToInt(tiling * points.Length * spacing * 0.01f);
        material = new Material(renderer.sharedMaterial);
        material.mainTextureScale = new Vector2(1, textureRepeat);
        renderer.material = material;
    }

    private Mesh CreateRoadMesh(Vector3[] points)
    {
        Vector3[] verts = new Vector3[points.Length * 2];
        Vector2[] uvs = new Vector2[verts.Length];
        int[] tris = new int[6 * (points.Length - 1)];

        for (int i = 0, vertIndex = 0, triIndex = 0; i < points.Length; i++, vertIndex += 2, triIndex += 6)
        {
            Vector3 forward = new Vector3(0f, 0f, 0f);
            if (i < points.Length - 1) 
            {
                forward += points[i + 1] - points[i];
            }
            if (i > 0)
            {
                forward += points[i] - points[i - 1];
            }
            forward.Normalize();
            Vector3 left = new Vector3(-forward.z, forward.y, forward.x);

            verts[vertIndex] = points[i] + left * roadWidth * Random.Range(0.42f,0.58f);
            verts[vertIndex + 1] = points[i] - left * roadWidth * Random.Range(0.42f, 0.58f);

            float completionPercent = i / (float)(points.Length - 1);
            uvs[vertIndex] = new Vector2(0, completionPercent);
            uvs[vertIndex + 1] = new Vector2(1, completionPercent);

            if(i < points.Length - 1)
            {
                tris[triIndex] = vertIndex;
                tris[triIndex + 1] = vertIndex + 2;
                tris[triIndex + 2] = vertIndex + 1;

                tris[triIndex + 3] = vertIndex + 1;
                tris[triIndex + 4] = vertIndex + 2;
                tris[triIndex + 5] = vertIndex + 3;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;

        return mesh;
    }

#if UNITY_EDITOR
    public void InitializeRoad()
    {
        tiling = 24f;
        var roadMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/New_Materials/Road_mat 1.mat");
        renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial = roadMaterial;

        CreateRoad();
    }
#endif
}
