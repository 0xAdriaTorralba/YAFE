using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//[RequireComponent(typeof(Image))]
public class TabButtonPage1 : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{

    public TabGroupPage1 tabGroup;

    //public Image background;


    void Awake()
    {
        //background = GetComponent<Image>();
        tabGroup.Suscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData){
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData){
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData){
        tabGroup.OnTabExit(this);
    }
}
