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

    private TMP_Dropdown dropdownType;

    void Awake(){
        mandelbulbCamera = GetComponent<Camera>();
        sliderDegree = GameObject.FindGameObjectWithTag("SliderDegree").GetComponent<Slider>();
        dropdownType = GameObject.Find("Type Dropdown").GetComponent<TMP_Dropdown>();
        textDegree = GameObject.Find("Degree Text").GetComponent<TextMeshProUGUI>();
        raymarching = GetComponent<RaymarchGeneric>();
    }

    void Start()
    {
        mandelbulbCamera.Render();
        sliderDegree.onValueChanged.AddListener( delegate { SliderValueChanged(sliderDegree); });
        dropdownType.onValueChanged.AddListener( delegate { DropdownValueChanged(dropdownType); });

    }

    private void DropdownValueChanged(TMP_Dropdown dropdown){
        if (string.Equals(dropdown.name, "Type Dropdown")){
            switch(dropdown.captionText.text.ToString()){
                case "Mandelbulb":
                    raymarching.type = 1;
                    break;
                case "Sierpinski Tetrahedron":
                    raymarching.type = 2;
                    break;
                case "Menger Sponge":
                    raymarching.type = 3;
                    break;
                case "Others":
                    raymarching.type = 1;
                    break;
                default:
                    raymarching.type = 1;
                    break;
            }
        }
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
