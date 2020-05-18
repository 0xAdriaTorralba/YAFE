using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CameraMovementController : MonoBehaviour
{

    private CoordinatesListener coordinatesListener;

    private FractalGPU fractal;

    private bool smoothMovement = true;

    public float scale = 5.0f;


    private float smoothScale;
    private Vector2 newPosition, smoothPosition;

    void Awake(){
        fractal = GetComponent<FractalGPU>();
        coordinatesListener = GetComponent<CoordinatesListener>();
    }

    void Start()
    {
        if (fractal is MandelbrotGPU){
            fractal = (MandelbrotGPU) fractal;
        }
        if (fractal is JuliaGPU){
            fractal = (JuliaGPU) fractal;
        }
    }

    private void HandleInputs(){

        // Capture inputs
        if (Input.GetAxis("Mouse ScrollWheel") > 0f  || Input.GetKey(KeyCode.Plus)){ // forward
            scale *= .98f;

        }else if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKey(KeyCode.Minus)){ // backwards
            scale *= 1.02f;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition.y -= .01f * scale;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition.y += .01f * scale;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition.x += .01f * scale;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition.x -= .01f * scale;
        }


    }
    
    void Update(){
        if (coordinatesListener.getIsPointerIn()){
            HandleInputs();
 
        }
        if (smoothMovement){
            // Apply Lerp function to smooth view
            smoothScale = Mathf.Lerp(smoothScale, scale, .05f);
            smoothPosition = Vector2.Lerp(smoothPosition, newPosition, .05f);
        }else{
            smoothScale = scale;
            smoothPosition = newPosition;
        }

        // Clamping
        newPosition.x = Mathf.Clamp(newPosition.x, -2, 2);
        newPosition.y = Mathf.Clamp(newPosition.y, -2, 2);
        scale = Mathf.Clamp(scale, 5e-6f, 2);

        fractal.UpdateZoom(smoothScale);
        fractal.UpdatePosition(smoothPosition.x, smoothPosition.y);
        
    }

    public void SetZoom(float zoom){
        scale = zoom;
    }

    public void SetPosition(float x, float y){
        newPosition.x = x;
        newPosition.y = y;
    }

    public void ToggleSmoothMovement(){
        smoothMovement = !smoothMovement;
    }
   
}
