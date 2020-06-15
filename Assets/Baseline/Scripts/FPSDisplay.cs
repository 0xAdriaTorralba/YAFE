using UnityEngine;
using System.Collections;
using TMPro;
 
public class FPSDisplay : MonoBehaviour
{
	float deltaTime = 0.0f;
    public TextMeshProUGUI fpsLabel;
 
	void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

	}
 
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
 
		GUIStyle style = new GUIStyle();
 
		Rect rect = new Rect(0, h -70, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} FPS)", msec, fps);
        fpsLabel.text = text;
		//GUI.Label(rect, text, style);
	}
}