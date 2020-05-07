using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomePageController : MonoBehaviour
{
    
    private GameObject dialogBox;

    void Awake(){
        dialogBox = GameObject.FindGameObjectWithTag("DialogBox");
        dialogBox.SetActive(false);
    }
    public void ToggleAboutDialog(){
        dialogBox.SetActive(!dialogBox.activeInHierarchy);
    }
}
