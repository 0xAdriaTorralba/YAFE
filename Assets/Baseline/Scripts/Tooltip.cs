using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private static Tooltip instance;

    private TextMeshProUGUI toolTipText;
    private RectTransform backgroundRectTransform, textRectTransform;

    void Awake(){
        instance = this;
        backgroundRectTransform = transform.Find("Background").GetComponent<RectTransform>();
        textRectTransform = transform.Find("TooltipText").GetComponent<RectTransform>();
        toolTipText = transform.Find("TooltipText").GetComponent<TextMeshProUGUI>();
        HideTooltip();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, Camera.main, out localPoint);
        transform.localPosition = localPoint;
    }

    private void ShowTooltip(string toolTipString){
        gameObject.SetActive(true);
        toolTipText.text = toolTipString;
        float textPaddingSize = 4f;
        Vector2 backgroundSize = new Vector2(toolTipText.preferredWidth + textPaddingSize * 2f, toolTipText.preferredHeight + textPaddingSize * 2f);
        backgroundRectTransform.sizeDelta = backgroundSize;
        textRectTransform.sizeDelta = backgroundSize;
    }

    private void HideTooltip(){
        gameObject.SetActive(false);
    }


    public static void ShowTooltipStatic(string tooltipString){
        instance.ShowTooltip(tooltipString);
    }

    public static void HideTooltipStatic(){
        instance.HideTooltip();
    }
}
