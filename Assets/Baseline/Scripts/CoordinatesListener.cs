﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CoordinatesListener : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    private double x, y;

    private bool isPointerIn = false;

    private Coroutine coroutine;

    private RectTransform _screenRectTransform;

    void Awake(){
        _screenRectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData){
        
    }

    public void OnPointerEnter(PointerEventData eventData){
        isPointerIn = true;
        try{
            StopCoroutine(coroutine);
        }catch{}
        coroutine = StartCoroutine(UpdatePosition(eventData));
    }

    private IEnumerator UpdatePosition(PointerEventData eventData){
        while(isPointerIn){
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_screenRectTransform, eventData.position, Camera.main, out Vector2 localClick); 
            x = (int)(localClick.x + (_screenRectTransform.rect.width / 2.0));
            y = (int)(localClick.y + (_screenRectTransform.rect.height / 2.0));
            yield return null;
        }
    }

    public bool getIsPointerIn(){
        return this.isPointerIn;
    }

    public void OnPointerExit(PointerEventData eventData){
        isPointerIn = false;
    }

    public double getX(){
        return this.x;
    }

    public double getY(){
        return this.y;
    }

}
