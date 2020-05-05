using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{

    private Color selectedColor = new Color(0f, 0.5647059f, 0.6196079f);
    private Color hoverColor = new Color(0.1529412f, 0.2862745f, 0.427451f);
    private Color idleColor =  new Color(0.1076451f, 0.2006943f, 0.3867925f);

    public List<TabButton> tabButtons;

   // public Sprite tabIdle, tabHover, tabActive;
    
    public TabButton selectedTab;

    public List<GameObject> objectsToSwap;

    private InterfaceController interfaceController;


    void Awake(){
        interfaceController = GameObject.FindGameObjectWithTag("InterfaceController").GetComponent<InterfaceController>();

        
    }

    void Start(){
        ResetPages();
        objectsToSwap[objectsToSwap.Count-1].SetActive(true);

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
            button.GetComponent<Image>().color = hoverColor;
        }
        
    }

    public void OnTabExit(TabButton button){
        ResetTabs();
        if (button != selectedTab || selectedTab == null){
            button.GetComponent<Image>().color = idleColor;
        }
    }

    public void OnTabSelected(TabButton button){
        if (selectedTab != null && button == selectedTab){
            selectedTab = null;
            ResetPages();
            button.GetComponent<Image>().color = hoverColor;
            objectsToSwap[objectsToSwap.Count-1].SetActive(true);
            return;
        }
        objectsToSwap[0].SetActive(false);

        selectedTab = button;
        ResetTabs();
        ResetPages();
        button.GetComponent<Image>().color = selectedColor;
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++){
            if (i == index){
                objectsToSwap[i].SetActive(true);
                if (index == objectsToSwap.Count - 1){
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
            button.GetComponent<Image>().color = idleColor;
        }

        
    }

}
