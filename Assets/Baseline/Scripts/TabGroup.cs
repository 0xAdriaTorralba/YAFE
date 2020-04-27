using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{

    public List<TabButton> tabButtons;

    public Sprite tabIdle, tabHover, tabActive;
    
    public TabButton selectedTab;

    public List<GameObject> objectsToSwap;

    private InterfaceController interfaceController;


    void Awake(){
        interfaceController = GameObject.FindGameObjectWithTag("InterfaceController").GetComponent<InterfaceController>();
        
    }

    void Start(){
        ResetPages();
        objectsToSwap[2].SetActive(true);
    }
    public void Suscribe (TabButton button){
        if (tabButtons == null){
            tabButtons = new List<TabButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button){
        ResetTabs();
        if (selectedTab == null || button != selectedTab){
            button.background.sprite = tabHover;
        }
        
    }

    public void OnTabExit(TabButton button){
        ResetTabs();
        button.background.sprite = tabIdle;
    }

    public void OnTabSelected(TabButton button){
        if (selectedTab != null && button == selectedTab){
            selectedTab = null;
            ResetPages();
            objectsToSwap[2].SetActive(true);
            return;
        }
        objectsToSwap[2].SetActive(false);

        selectedTab = button;
        ResetTabs();
        ResetPages();
        button.background.sprite = tabActive;
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++){
            if (i == index){
                objectsToSwap[i].SetActive(true);
                if (index == 0){
                    interfaceController.RestartDrawingCoroutines();
                }
            }else{
                objectsToSwap[i].SetActive(false);
                
            }

            
        }
    }

    public void ResetPages(){
        for (int i = 0; i < objectsToSwap.Count; i++){
            objectsToSwap[i].SetActive(false);
            interfaceController.StopAllDrawingCoroutines();
        }
    }

    public void ResetTabs(){
        foreach(TabButton button in tabButtons){
            if (selectedTab != null && selectedTab == button){ 
                continue; 
            }
            button.background.sprite = tabIdle;
        }

        
    }

}
