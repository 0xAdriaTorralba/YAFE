using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fractal : MonoBehaviour
{

    public Mesh mesh;
    public Material material;

    public int maxDepth;

    private int depth;

    public float childScale;

    // Start is called before the first frame update
    private void Start()
    {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
        
    }

    private void InitializeTrivial(Fractal parent)
    {
        mesh = parent.mesh;
        material = parent.material;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        childScale = parent.childScale;
        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = Vector3.up * (0.5f + 0.5f * childScale);
    }

    private void InitializeUp(Fractal parent)
    {
        mesh = parent.mesh;
        material = parent.material;
        //maxDepth = parent.maxDepth;
        childScale = parent.childScale;
        //transform.parent = parent.transform;
        // Apply linear applications transformations
        // Serpinski Triangle
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = Vector3.up * (0.5f + 0.5f * childScale);
        Destroy(parent.gameObject);

    }

    private void InitializeStay(Fractal parent)
    {
        mesh = parent.mesh;
        material = parent.material;
        //maxDepth = parent.maxDepth;
        childScale = parent.childScale;
        //transform.parent = parent.transform;
        // Apply linear applications transformations
        // Serpinski Triangle
        transform.localScale = Vector3.one * childScale;
        //transform.localPosition = Vector3.one * (childScale);
        Destroy(parent.gameObject);

    }

    private void InitializeRight(Fractal parent)
    {
        mesh = parent.mesh;
        material = parent.material;
        //maxDepth = parent.maxDepth;
        childScale = parent.childScale;
        //transform.parent = parent.transform;
        // Apply linear applications transformations
        // Serpinski Triangle
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = Vector3.right * (0.5f + 0.5f * childScale);
        Destroy(parent.gameObject);

    }

    private void Reverse(Fractal parent)
    {
        mesh = parent.mesh;
        material = parent.material;
        //maxDepth = parent.maxDepth;
        childScale = parent.childScale;
        //transform.parent = parent.transform;
        // Apply linear applications transformations
        // Serpinski Triangle
        transform.localScale = parent.transform.localScale * 1f/childScale;
        //transform.localPosition = Vector3.one * (-0.5f - 0.5f * 1f/childScale);
        Destroy(parent.gameObject);

        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
            if (o.name == "Fractal Child")
            {
                Destroy(o);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            new GameObject("Fractal Child").AddComponent<Fractal>().InitializeUp(this);
            new GameObject("Fractal Child").AddComponent<Fractal>().InitializeRight(this);
            new GameObject("Fractal Child").AddComponent<Fractal>().InitializeStay(this);

        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            new GameObject("Reverse").AddComponent<Fractal>().Reverse(this);

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("SampleScene");
            /*
                        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
                        {
                            if (o.name == "Fractal Child")
                            {
                                Destroy(o);
                            }
                        }

                        new GameObject("Hidden Fractal").AddComponent<Fractal>().Start();
                    }*/

        }
    }
}
