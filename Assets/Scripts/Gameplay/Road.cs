using UnityEngine;

[RequireComponent(typeof(Path))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Road : MonoBehaviour
{
    [Range(0.05f, 1f)]
    [SerializeField] private float spacing = 1f;
    [SerializeField] private float roadWidth = 1f;
    [SerializeField] private bool autoUpdate;
    [SerializeField] private float tiling = 1f;

    public bool AutoUpdate => autoUpdate;

    public void UpdateRoad()
    {
        BezierCurve curve = GetComponent<Path>().Curve;
        Vector3[] points = curve.CalculateEvenlySpacedPoints(spacing);
        GetComponent<MeshFilter>().mesh = CreateRoadMesh(points);

        int textureRepeat = Mathf.RoundToInt(tiling * points.Length * spacing * 0.05f);
        GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(1, textureRepeat);
    }

    private Mesh CreateRoadMesh(Vector3[] points)
    {
        Vector3[] verts = new Vector3[points.Length * 2];
        Vector2[] uvs = new Vector2[verts.Length];
        int[] tris = new int[6 * (points.Length - 1)];

        for (int i = 0, vertIndex = 0, triIndex = 0; i < points.Length; i++, vertIndex += 2, triIndex += 6)
        {
            Vector3 forward = Vector3.zero;
            if (i < points.Length - 1) 
            {
                forward += points[i + 1] - points[i];
            }
            if (i > 0)
            {
                forward += points[i] - points[i - 1];
            }
            forward.Normalize();
            Vector3 left = new Vector3(-forward.z, 0f, forward.x);

            verts[vertIndex] = points[i] + left * roadWidth * 0.5f;
            verts[vertIndex + 1] = points[i] - left * roadWidth * 0.5f;

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
}
