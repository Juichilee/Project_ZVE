using System.Collections.Generic;
using UnityEngine;

public class AISensor : MonoBehaviour
{
    public float sightRange= 20f;
    public float chaseRange = 10f;
    public float angle = 30f;
    public float height = 2f;
    public int scanFrequency = 30;

    public LayerMask layers;
    public LayerMask occlusionLayers;
    public List<GameObject> Objects = new List<GameObject>();

    public Collider[] colliders = new Collider[50];
    private Mesh mesh; 
    private int count;
    private float scanInterval;
    private float scanTimer;

    void Start()
    {
        scanInterval = 1.0f / scanFrequency;
    }

    void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0)
        {
            scanTimer += scanInterval;
            Scan();
        }
    }

    void SetUpTriangleVertices(ref Vector3[] vertices, ref int index, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        vertices[index++] = v1;
        vertices[index++] = v2;
        vertices[index++] = v3;
    }

    Mesh CreateWedgeMesh() 
    {
        Mesh mesh = new();

        int segments = 10;
        int numTriangles = segments * 4 + 4;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 botCenter = Vector3.zero;
        Vector3 botLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * sightRange;
        Vector3 botRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * sightRange;

        Vector3 topCenter = botCenter + Vector3.up * height;
        Vector3 topLeft = botLeft + Vector3.up * height;
        Vector3 topRight = botRight + Vector3.up * height;

        int vertices_index = 0;

        // Left Rectangle
        SetUpTriangleVertices(ref vertices, ref vertices_index, botCenter, botLeft, topLeft);
        SetUpTriangleVertices(ref vertices, ref vertices_index, topLeft, topCenter, botCenter);

        // Right Rectangle
        SetUpTriangleVertices(ref vertices, ref vertices_index, botCenter, topCenter, topRight);
        SetUpTriangleVertices(ref vertices, ref vertices_index, topRight, botCenter, botCenter);

        float currAngle = -angle;
        float deltaAngle = angle * 2 / segments;

        // Creating Circular Edge
        for (int i = 0; i < segments; i++)
        {
            botLeft = Quaternion.Euler(0, currAngle, 0) * Vector3.forward * sightRange;
            botRight = Quaternion.Euler(0, currAngle + deltaAngle, 0) * Vector3.forward * sightRange;

            topLeft = botLeft + Vector3.up * height;
            topRight = botRight + Vector3.up * height;

            // Edge 
            SetUpTriangleVertices(ref vertices, ref vertices_index, botLeft, botRight, topRight);
            SetUpTriangleVertices(ref vertices, ref vertices_index, topRight, topLeft, botLeft);

            // Top and Bot Triangles
            SetUpTriangleVertices(ref vertices, ref vertices_index, topCenter, topLeft, topRight);
            SetUpTriangleVertices(ref vertices, ref vertices_index, botCenter, botRight, botLeft);

            currAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; i++)
            triangles[i] = i;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(this.transform.position,
                                              sightRange,
                                              colliders,
                                              layers, 
                                              QueryTriggerInteraction.Collide);

        Objects.Clear();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].gameObject;
            if (IsInSight(obj)) 
                Objects.Add(obj);
        }
    }

    public bool IsInSight(GameObject obj) {
        Vector3 originPos = this.transform.position;
        Vector3 objPos = obj.transform.position;

        // Check if in Chase Range
        if (Vector3.Distance(originPos, objPos) < chaseRange) 
            return true;

        // Check if Inside Height
        Vector3 direction = objPos - originPos;
        if (direction.y < 0 || direction.y > height)
            return false;

        // Check if Inside Angle Range
        direction.y = 0;
        float angleDiff = Vector3.Angle(this.transform.forward, direction);
        if (angleDiff > angle) 
            return false;

        // See if anything is blocking view from player
        originPos.y += height / 2;
        objPos.y += originPos.y;
        return !Physics.Linecast(originPos, objPos, occlusionLayers);
    }

    private void OnValidate() 
    {
        mesh = CreateWedgeMesh();
    }

    private void OnDrawGizmos()
    {
        Color cyan = Color.cyan;
        cyan.a = 0.5f;
        Color red = Color.red;
        red.a = 0.5f;
        Color green = Color.green;
        green.a = 0.5f;
        // Draw Wedge
        if (mesh)
        {
            Gizmos.color = red;
            Gizmos.DrawMesh(mesh, this.transform.position, this.transform.rotation);
        }
        
        // Draw Range Sphere
        Gizmos.DrawWireSphere(this.transform.position, sightRange);
        
        Gizmos.color = cyan;
        Gizmos.DrawWireSphere(this.transform.position, chaseRange);
        
        // Make a red sphere on objects in range but not yet seen
        Gizmos.color = red;
        foreach (var collider in colliders)
        {
            if (collider)
                Gizmos.DrawSphere(collider.transform.position, 0.6f);
        }

        // Make a green sphere on objects In Sense Range
        Gizmos.color = green;
        foreach (var obj in Objects)
            if (obj)
                Gizmos.DrawSphere(obj.transform.position, 0.6f);
    }
}
