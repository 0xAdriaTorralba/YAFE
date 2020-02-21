using UnityEngine;
using System.Collections;

public class PointCloud : MonoBehaviour
{
    private Mesh mesh;


    public int numPoints = 10000;

    // Use this for initialization
    void Start()
    {
        mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;
        //CreateMesh();
        CreatePoints();
    }

    void Update () {
        Vector3[] points = new Vector3[numPoints];
        int[] indecies = new int[numPoints];
        Color[] colors = new Color[numPoints];
        for (int i = 0; i < points.Length; ++i)
        {
            Vector3 point = mesh.vertices[i];
			Vector3 position = new Vector3(point.x, point.y, point.z);
			position.y = Mathf.Sin(Mathf.PI * (position.x + Time.time));
			point.x = position.x;
            point.y = position.y;
            point.z = position.z;
            mesh.vertices[i] = point;
            points[i] = point;
            indecies[i] = i;
            colors[i] = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
        }

        mesh.vertices = points;
        mesh.colors = colors;
        mesh.SetIndices(indecies, MeshTopology.Points, 0);
	}

    void CreatePoints(){
        Vector3[] points = new Vector3[numPoints];
        int[] indecies = new int[numPoints];
        Color[] colors = new Color[numPoints];

        float step = 2f / numPoints;
		Vector3 scale = Vector3.one * step;
		Vector3 position;
		position.y = 0f;
		position.z = 0f;
        for (int i = 0; i < points.Length; ++i)
        {
            Vector3 point;
			position.x = (i + 0.5f) * step - 1f;
			point.x = position.x;
            point.y = position.y;
            point.z = position.z;
			points[i] = point;
            indecies[i] = i;
            colors[i] = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
        }

        mesh.vertices = points;
        mesh.colors = colors;
        mesh.SetIndices(indecies, MeshTopology.Points, 0);

    }


    void CreateMesh()
    {
        Vector3[] points = new Vector3[numPoints];
        int[] indecies = new int[numPoints];
        Color[] colors = new Color[numPoints];
        for (int i = 0; i < points.Length; ++i)
        {
            points[i] = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
            indecies[i] = i;
            colors[i] = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
        }

        mesh.vertices = points;
        mesh.colors = colors;
        mesh.SetIndices(indecies, MeshTopology.Points, 0);

    }
}
