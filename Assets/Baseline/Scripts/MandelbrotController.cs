using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandelbrotController : MonoBehaviour
{
    
    private Renderer shader;
    private Material material;
    public float scale;
    public Vector2 position;
    public Vector2 startZoom;
    private float smoothScale;
    private Vector2 smoothPosition;
    // Start is called before the first frame update
    void Start()
    {
        shader = gameObject.GetComponent<Renderer>();
        material = shader.sharedMaterial;
        material.SetColor("_Zoom", new Vector4(startZoom.x, startZoom.y, 0.0f, 0.0f));
        material.SetColor("_Pan", new Vector4(1.7693831791955f, 0.00423684791873f, 0.0f, 0.0f));
    }

    private void HandleInputs(){
        if (Input.GetAxis("Mouse ScrollWheel") > 0f ){ // forward
            scale *= .99f;
        }else if (Input.GetAxis("Mouse ScrollWheel") < 0f ){ // backwards
            scale *= 1.01f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            position.y -= .01f * scale;
        }
        if (Input.GetKey(KeyCode.S))
        {
            position.y += .01f * scale;
        }
        if (Input.GetKey(KeyCode.A))
        {
            position.x += .01f * scale;
        }
        if (Input.GetKey(KeyCode.D))
        {
            position.x -= .01f * scale;
        }
    }

    private void UpdateShader(){
        HandleInputs();
        smoothScale = Mathf.Lerp(smoothScale, scale, .03f);
        smoothPosition = Vector2.Lerp(smoothPosition, position, .03f);
        float aspect = (float) Screen.width / (float) Screen.height;

        float scaleX = smoothScale;
        float scaleY = smoothScale;

        if (aspect > 1.0f){
            scaleY /= aspect;
        }else{
            scaleX *= aspect;
        }
        material.SetFloat("_Aspect", aspect);
        material.SetColor("_Zoom", new Vector4(scaleX, scaleY, 0.0f, 0.0f));
        material.SetColor("_Pan", new Vector4(smoothPosition.x, smoothPosition.y, 0.0f, 0.0f));
    }
    void FixedUpdate(){
        UpdateShader();
    }


   
}
