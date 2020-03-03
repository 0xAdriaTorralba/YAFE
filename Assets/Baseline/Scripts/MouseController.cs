using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    private Renderer shader;
    private Material material;
    public float startPoint;
    private float i;
    // Start is called before the first frame update
    void Start()
    {
        shader = gameObject.GetComponent<Renderer>();
        material = shader.sharedMaterial;
        material.SetColor("_Zoom", new Vector4(startPoint, startPoint, 0.0f, 0.0f));
        material.SetColor("_Pan", new Vector4(1.7693831791955f, 0.00423684791873f, 0.0f, 0.0f));
        i = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f ){ // forward
            i *= 1.1f;
            material.SetColor("_Zoom", new Vector4(startPoint*(1/i), startPoint*(1/i), 0.0f, 0.0f));
        }else if (Input.GetAxis("Mouse ScrollWheel") < 0f ){ // backwards
            if (i > 1.0f){
                i /= 1.1f;
            }else{
                i = 1.0f;
            }
            material.SetColor("_Zoom", new Vector4(startPoint*(1/i), startPoint*(1/i), 0.0f, 0.0f));
        }
        Vector4 currentPosition = material.GetColor("_Pan");
        if (Input.GetKey(KeyCode.W))
        {
            currentPosition.y += 0.001f;
            material.SetColor("_Pan", currentPosition);
        }
        if (Input.GetKey(KeyCode.S))
        {
            currentPosition.y -= 0.001f;
            material.SetColor("_Pan", currentPosition);
        }
        if (Input.GetKey(KeyCode.A))
        {
            currentPosition.x -= 0.001f;
            material.SetColor("_Pan", currentPosition);
        }
        if (Input.GetKey(KeyCode.D))
        {
            currentPosition.x += 0.001f;
            material.SetColor("_Pan", currentPosition);
        }
    }
}
