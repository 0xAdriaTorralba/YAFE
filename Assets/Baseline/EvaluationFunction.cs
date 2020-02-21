using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;


[System.Serializable]
public class DisplayParameters{
    public float dimX, dimY;
    public int resolutionX, resolutionY;
}
public class EvaluationFunction : MonoBehaviour
{

    public DisplayParameters displayParameters;
    public float lambda = -4;

    public Gradient gradient;

    private float minValue, maxValue;

    private Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;

        CreatePoints();
    }

    private void CreatePoints(){
        float minX = -displayParameters.dimX;
        float maxX = displayParameters.dimX;
        float minY = -displayParameters.dimY;
        float maxY = displayParameters.dimY;
        float windowWidth = maxX - minX;
        float windowHeight = maxY - minY;
        float viewPortX, viewPortY;

        int maxIters = 255;

        //UnityEngine.Vector3[,] points = new UnityEngine.Vector3[resolutionX,resolutionY];
        //int[,] indecies = new int[resolutionX, resolutionY];
        //Color[,] colors = new Color[resolutionX, resolutionY];
        UnityEngine.Vector3[] points = new UnityEngine.Vector3[displayParameters.resolutionX * displayParameters.resolutionY];
        int[] indecies = new int[displayParameters.resolutionX * displayParameters.resolutionY];
        Color[] colors = new Color[displayParameters.resolutionX * displayParameters.resolutionY];
        int index = 0;
        float step = 2f / (displayParameters.resolutionX * displayParameters.resolutionY);
		UnityEngine.Vector3 scale = UnityEngine.Vector3.one * step;
		UnityEngine.Vector3 position;
		position.y = 0f;
		position.z = 0f;
        int count = 0;
        for (int i = 0; i < displayParameters.resolutionX; ++i){
            for (int j = 0; j < displayParameters.resolutionY; j++){
                UnityEngine.Vector3 point = new UnityEngine.Vector3();
                point.x = minX + ((float) i / displayParameters.resolutionX) * windowWidth;
                point.y = minY + ((float) j / displayParameters.resolutionY) * windowHeight;
                point.z = evaluateFunction(point.x, point.y, maxIters);
                //point.z = 0;
                if (point.z < minValue){
                    minValue = point.z;
                }
                if (point.z > maxValue){
                    maxValue = point.z;
                }
                point.x = i;
                point.y = j;
                points[count] = point;
                indecies[count] = index;
                colors[count] = gradient.Evaluate(point.z);
                index++;
                count++;
            }

        }
        // UnityEngine.Vector3[] auxPoints = new UnityEngine.Vector3[resolutionX * resolutionY];
        // Color[] auxColors = new Color[resolutionX * resolutionY];
        // int[] auxIndecies = new int[resolutionX * resolutionY];
        // index = 0;
        // for (int i = 0; i  < dimX; i++){
        //     for (int j = 0; j < dimY; j++){
        //         auxPoints[index] = points[i, j];
        //         auxIndecies[index] = indecies[i, j];
        //         auxColors[index] = gradient.Evaluate(Mathf.InverseLerp(minValue, maxValue, auxPoints[index].z));
        //         index++;
        //     }
        // }
        // mesh.vertices = auxPoints;
        // mesh.colors = auxColors;
        // mesh.SetIndices(auxIndecies, MeshTopology.Points, 0);
        mesh.vertices = points;
        mesh.colors = colors;
        mesh.SetIndices(indecies, MeshTopology.Points, 0);
    }

    private float evaluateFunction(float x, float y, int maxIters){
        Complex z = new Complex(x, y);
        int i = 0;
        while (Complex.Abs(z) < 25 && i < maxIters){
            z = lambda*Complex.Pow(z, 2)*(z-1);
            i++;
        }
        return (float)Complex.Abs(z);
        //return i;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
