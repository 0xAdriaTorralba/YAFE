using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DisplayParameters2{
    public int dimX, dimY;
}
public class ShaderController : MonoBehaviour
{
    public float numSteps;
    public bool random;
    private Renderer shader; 
    private int i;
    private Material material;
    private bool increase;
    // Start is called before the first frame update
    void Start()
    {
        shader = gameObject.GetComponent<Renderer>();
        material = shader.sharedMaterial;
        i = 0;
        increase = true;
        random = false;
        StartCoroutine(RandomFractal());
    }

    void FixedUpdate(){
        if (!random){
            if (increase){
                i++;
            }else{
                i--;
            }
            material.SetColor("_Seed", new Vector4(-1 + 2.0f*(i/numSteps), -1 + 2.0f*(i/numSteps), 0.0f, 0.0f));

            if (i > numSteps){
                increase = false;
            }
            if (i < 0){
                increase = true;
            }
        }
    }

    IEnumerator RandomFractal(){
        while (true){
            yield return new WaitForSeconds(2.0f);
            if (random){
                material.SetColor("_Seed", new Vector4(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f, 0.0f));
            }else{
                continue;
            }
        }


    }
}
