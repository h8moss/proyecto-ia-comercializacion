using UnityEngine;

[RequireComponent(typeof(DetectionArc))]
public class DetectionArcVisualizer : MonoBehaviour
{
    [SerializeField] private int meshResolution = 20; // more = smoother arc
    [SerializeField] private Material visionConeMaterial;
    [SerializeField] private Color visionColor = new Color(1f, 0f, 0f, 0.3f);


    private Mesh visionMesh;
    private MeshFilter meshFilter;
    private DetectionArc arc;


    void Start()
    {
        arc = GetComponent<DetectionArc>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        var renderer = gameObject.AddComponent<MeshRenderer>();
        visionConeMaterial.color = visionColor;
        renderer.material = visionConeMaterial;
        renderer.sortingLayerName = "Normal";
        renderer.sortingOrder = 1;
        visionMesh = new Mesh();
        meshFilter.mesh = visionMesh;
    }

    void LateUpdate()
    {
        DrawVisionCone();
    }

    private void DrawVisionCone()
    {
        int rayCount = meshResolution;
        float angleStep = arc.Angle * 2 / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero; // origin point (local space)

        for (int i = 0; i <= rayCount; i++)
        {
            float currentAngle = arc.Angle - (angleStep * i);
            Vector3 dir = Quaternion.Euler(0, 0, currentAngle) * transform.right;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, arc.Length, arc.VisionObstacles);

            // If wall hit, stop the mesh there instead of at full radius
            float dst = hit.collider != null ? hit.distance : arc.Length;
            vertices[i + 1] = transform.InverseTransformPoint(transform.position + dir * dst);
        }

        for (int i = 0; i < rayCount; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        visionMesh.Clear();
        visionMesh.vertices = vertices;
        visionMesh.triangles = triangles;
    }
}
