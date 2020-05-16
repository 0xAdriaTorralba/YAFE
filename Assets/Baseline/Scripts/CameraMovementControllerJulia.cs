using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovementControllerJulia : MonoBehaviour
{

    private CoordinatesListener coordinatesListener;

    private JuliaGPU fractal;

    public static float scale = 5.0f;

    private float smoothScale;
    private static Vector2 newPosition, smoothPosition;

    void Awake(){
        fractal = GetComponent<JuliaGPU>();
        coordinatesListener = GetComponent<CoordinatesListener>();
    }

    void Start()
    {
    }

    private void HandleInputs(){

        // Capture inputs
        if (Input.GetAxis("Mouse ScrollWheel") > 0f ){ // forward
            scale *= .98f;
        }else if (Input.GetAxis("Mouse ScrollWheel") < 0f ){ // backwards
            scale *= 1.02f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            newPosition.y -= .01f * scale;
        }
        if (Input.GetKey(KeyCode.S))
        {
            newPosition.y += .01f * scale;
        }
        if (Input.GetKey(KeyCode.A))
        {
            newPosition.x += .01f * scale;
        }
        if (Input.GetKey(KeyCode.D))
        {
            newPosition.x -= .01f * scale;
        }


    }
    
    void Update(){
        if (coordinatesListener.getIsPointerIn()){
            HandleInputs();
        }

        // Apply Lerp function to smooth view
        smoothScale = Mathf.Lerp(smoothScale, scale, .05f);
        smoothPosition = Vector2.Lerp(smoothPosition, newPosition, .05f);

        // Clamping
        newPosition.x = Mathf.Clamp(newPosition.x, -2, 2);
        newPosition.y = Mathf.Clamp(newPosition.y, -2, 2);
        scale = Mathf.Clamp(scale, 7e-6f, 2);

        fractal.UpdateZoom(smoothScale);
        fractal.UpdatePosition(smoothPosition.x, smoothPosition.y);

    }

    public static void ResetValues(){
        scale = 2.0f;
        newPosition.x = -0.5f;
        newPosition.y = 0.0f;
    }
   
}
