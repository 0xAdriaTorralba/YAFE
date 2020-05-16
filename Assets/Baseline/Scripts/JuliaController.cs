using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JuliaController : MonoBehaviour
{
    public float numSteps;
    public bool random;

    private Texture2D tex2D;
    private int i;
    private Material material;
    private Image image;
    private bool increase;
    // Start is called before the first frame update
    void Start()
    {
        material = gameObject.GetComponent<Image>().material;
        image = gameObject.GetComponent<Image>();
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
            image.material = material;

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
                image.material = material;
            }else{
                continue;
            }
        }


    }
}
