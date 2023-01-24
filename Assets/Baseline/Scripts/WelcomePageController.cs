using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WelcomePageController : MonoBehaviour
{
    
    private GameObject dialogBox;
    private Image image;
    private TextMeshProUGUI text;

    private Button github, linkedin;

    void Awake(){
        dialogBox = GameObject.FindGameObjectWithTag("DialogBox");
        image = dialogBox.GetComponent<Image>();
        text = GameObject.Find("About Text").GetComponent<TextMeshProUGUI>();
        github = GameObject.FindGameObjectWithTag("Github").GetComponent<Button>();
        linkedin = GameObject.FindGameObjectWithTag("LinkedIn").GetComponent<Button>();
        dialogBox.SetActive(false);
    }

    void Start(){
        github.onClick.AddListener(() => GoToGithub());
        linkedin.onClick.AddListener(() => GoToLinkedIn());
    }

    private void GoToGithub(){
        Application.OpenURL("https://github.com/0xAdriaTorralba");
    }

    private void GoToLinkedIn(){
        Application.OpenURL("https://www.linkedin.com/in/adriatorralba/");
    }


    public void ToggleAboutDialog(){
        if (dialogBox.activeInHierarchy){
            StartCoroutine(FadeImage(true));

        }else{
            StartCoroutine(FadeImage(false));
        }
    }

    private IEnumerator FadeImage(bool fadeAway){
        // fade from opaque to transparent
        if (fadeAway){
            // loop over 1 second backwards
            for (float i = 1f; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                image.color = new Color(image.color.r, image.color.g, image.color.b, i);
                text.color = new Color(text.color.r, text.color.g, text.color.b, i);
                yield return null;
            }
            dialogBox.SetActive(false);
        }
        // fade from transparent to opaque
        else
        {
            dialogBox.SetActive(true);  
            // loop over 1 second
            for (float i = 0; i <= 1f; i += Time.deltaTime)
            {
                // set color with i as alpha
                image.color = new Color(image.color.r, image.color.g, image.color.b, i);
                text.color = new Color(text.color.r, text.color.g, text.color.b, i);
                yield return null;
            }


        }
    }
}
