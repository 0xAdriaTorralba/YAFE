using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MandelbulbController : MonoBehaviour
{
    private Camera mandelbulbCamera;
    private Slider sliderDegree; 

    private TextMeshProUGUI textDegree;

    private RaymarchGeneric raymarching;

    void Awake(){
        mandelbulbCamera = GetComponent<Camera>();
        sliderDegree = GameObject.FindGameObjectWithTag("SliderDegree").GetComponent<Slider>();
        textDegree = GameObject.Find("Degree Text").GetComponent<TextMeshProUGUI>();
        raymarching = GetComponent<RaymarchGeneric>();
    }

    void Start()
    {
        mandelbulbCamera.Render();
        sliderDegree.onValueChanged.AddListener( delegate { SliderValueChanged(sliderDegree); });
    }

    private void SliderValueChanged(Slider slider){
        if (string.Equals(slider.tag, "SliderDegree")){
            raymarching.power = (int) slider.value;
            textDegree.text = (int) slider.value + "";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
