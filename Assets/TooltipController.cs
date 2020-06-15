using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipController : MonoBehaviour
{
    private Coroutine tooltipCorroutine;
    public void TestTooltip(){
        tooltipCorroutine = StartCoroutine(WaitAndShowTooltip(1f, "test on hover delay"));
    }

    private IEnumerator WaitAndShowTooltip(float seconds, string text){
        for (float i = 0; i <= seconds; i += Time.deltaTime){
            yield return null;
        }
        Tooltip.ShowTooltipStatic(text);
    }

    public void HideTooltip(){
        try{
            StopCoroutine(tooltipCorroutine);
        }finally{
            Tooltip.HideTooltipStatic();
        }
    }

}
