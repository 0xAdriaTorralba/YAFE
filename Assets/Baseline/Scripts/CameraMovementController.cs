using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    
    private Renderer shader;
    private Material material;
    private float smoothScale;
    private Vector2 smoothPosition;

    private Transform mainCameraTransform;
    private Camera mainCamera;

    private float scale;
    private Vector2 newPosition;
    // Start is called before the first frame update
    void Start()
    {
        // Capture Camera of Scene
        mainCameraTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        smoothPosition = mainCameraTransform.position;
        smoothScale = mainCamera.orthographicSize;
        scale = 2.0f;

        // Capture Material Component
        //shader = gameObject.GetComponent<Renderer>();
        //material = shader.sharedMaterial;

        // Initialize for display purposes
        //material.SetColor("_Zoom", new Vector4(startZoom.x, startZoom.y, 0.0f, 0.0f));
        //material.SetColor("_Pan", new Vector4(1.7693831791955f, 0.00423684791873f, 0.0f, 0.0f));
    }

    private void HandleInputs(){

        // Capture inputs
        if (Input.GetAxis("Mouse ScrollWheel") > 0f ){ // forward
            scale *= .99f;
        }else if (Input.GetAxis("Mouse ScrollWheel") < 0f ){ // backwards
            scale *= 1.01f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            newPosition.y += .01f * scale;
        }
        if (Input.GetKey(KeyCode.S))
        {
            newPosition.y -= .01f * scale;
        }
        if (Input.GetKey(KeyCode.A))
        {
            newPosition.x -= .01f * scale;
        }
        if (Input.GetKey(KeyCode.D))
        {
            newPosition.x += .01f * scale;
        }

        // Apply Lerp function to smooth view
        smoothScale = Mathf.Lerp(smoothScale, scale, .03f);
        smoothPosition = Vector2.Lerp(smoothPosition, newPosition, .03f);

        // Clamping
        newPosition.x = Mathf.Clamp(newPosition.x, -2, 2);
        newPosition.y = Mathf.Clamp(newPosition.y, -2, 2);
        scale = Mathf.Clamp(scale, 1e-6f, 2);

        // Update values
        mainCameraTransform.position = smoothPosition;
        mainCamera.orthographicSize = smoothScale;
    }

    private void FixRatio(){
        float targetaspect = 1.0f;
        float aspect = (float) Screen.width / (float) Screen.height;
        float scaleheight = aspect / targetaspect;
        float scaleX = smoothScale;
        float scaleY = smoothScale;

        if (aspect < 1.0f){
            Rect rect = mainCamera.rect;
            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;
        
            mainCamera.rect = rect;
        }else{
            float scalewidth = 1.0f / scaleheight;

            Rect rect = mainCamera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            mainCamera.rect = rect;
        }

        //material.SetFloat("_Aspect", aspect);
        //material.SetColor("_Zoom", new Vector4(scaleX, scaleY, 0.0f, 0.0f));
        //material.SetColor("_Pan", new Vector4(smoothPosition.x, smoothPosition.y, 0.0f, 0.0f));
    }
    void Update(){
        HandleInputs();
        FixRatio();
    }


   
}
