using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AISensor : MonoBehaviour
{
    public float distance = 10f;
    public float angle = 30f;
    public float height = 1f;
    public Color meshColor = Color.red;
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

    Mesh CreateWedgeMesh() 
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = segments * 4 + 4;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 botCenter = Vector3.zero;
        Vector3 botLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 botRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = botCenter + Vector3.up * height;
        Vector3 topLeft = botLeft + Vector3.up * height;
        Vector3 topRight = botRight + Vector3.up * height;

        int vert = 0;

        // Left 
        vertices[vert++] = botCenter;
        vertices[vert++] = botLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = botCenter;

        // Right
        vertices[vert++] = botCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = botRight;
        vertices[vert++] = botCenter;

        float currAngle = -angle;
        float deltaAngle = angle * 2 / segments;

        for (int i = 0; i < segments; i++)
        {
            botLeft = Quaternion.Euler(0, currAngle, 0) * Vector3.forward * distance;
            botRight = Quaternion.Euler(0, currAngle + deltaAngle, 0) * Vector3.forward * distance;

            topLeft = botLeft + Vector3.up * height;
            topRight = botRight + Vector3.up * height;

            // Edge 
            vertices[vert++] = botLeft;
            vertices[vert++] = botRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = botLeft;

            // Top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            // Bot
            vertices[vert++] = botCenter;
            vertices[vert++] = botRight;
            vertices[vert++] = botLeft;

            currAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(this.transform.position,
                                              distance,
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
        Vector3 direction = objPos - originPos;

        if (direction.y < 0 || direction.y > height)
            return false;
        direction.y = 0;
        float angleDiff = Vector3.Angle(this.transform.forward, direction);
        if (angleDiff > angle) 
            return false;

        originPos.y += height / 2;
        objPos.y += originPos.y;
        if (Physics.Linecast(originPos, objPos, occlusionLayers))
            return false;
        
        return true;
    }

    private void OnValidate() 
    {
        mesh = CreateWedgeMesh();
    }

    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, this.transform.position, this.transform.rotation);
        }
        
        Gizmos.DrawWireSphere(this.transform.position, distance);
        foreach (var collider in colliders)
        {
            if (collider)
                Gizmos.DrawSphere(collider.transform.position, 0.6f);
        }

        Gizmos.color = Color.green;
        foreach (var obj in Objects)
            Gizmos.DrawSphere(obj.transform.position, 0.6f);




    }
}
