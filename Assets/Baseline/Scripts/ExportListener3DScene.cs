using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

[RequireComponent(typeof(Button))]
public class ExportListener3DScene : MonoBehaviour, IPointerDownHandler {


    private byte[] _textureBytes;

    private Button button;

    private GameObject fractal;
    private int countMandelbulb = 0;



    void Awake() {
        button = GetComponent<Button>();
        fractal = GameObject.FindGameObjectWithTag("Mandelbulb");
    }



    void Update(){
    }



#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //


    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);


    private Texture2D TextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);
        return texture2D;
    }

    // Broser plugin should be called in OnPointerDown.
    public void OnPointerDown(PointerEventData eventData) {
        StartCoroutine(ExportImage());
    }

    private IEnumerator ExportImage(){
        Texture2D texture = null;
        texture = TextureToTexture2D(fractal.GetComponent<RawImage>().texture);
        _textureBytes = texture.EncodeToPNG();
        DownloadFile(gameObject.name, "OnFileDownload", "MandelbulbImage_"+countMandelbulb+".png", _textureBytes, _textureBytes.Length);
        countMandelbulb++;
        yield return new WaitForEndOfFrame();
    }

    // Called from browser
    public void OnFileDownload() {

    }

#elif UNITY_ANDROID || UNITY_IOS

    //
    // Android and iOS platforms
    //
        public void OnPointerDown(PointerEventData eventData) { }

    // // Listen OnClick event in standlone builds

    void Start(){
        button.onClick.AddListener(OnClick);
    }


    public void OnClick() {
        StartCoroutine(ExportImage(fractal));
    }

    private Texture2D TextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);
        return texture2D;
    }


    private IEnumerator ExportImage(GameObject fractal){
        Texture2D texture = null;
        texture = TextureToTexture2D(fractal.GetComponent<RawImage>().texture);
        NativeGallery.SaveImageToGallery(texture.EncodeToPNG(), "Fractals Album", "MandelbulbImage_"+countMandelbulb+".png");
        countMandelbulb++;
        yield return new WaitForEndOfFrame();
    }


#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }

    // // Listen OnClick event in standlone builds


    void Start(){
        button.onClick.AddListener(OnClick);
    }

    public void OnClick() {
        StartCoroutine(ExportImage(fractal));
    }

     private Texture2D TextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);
        return texture2D;
    }

    private IEnumerator ExportImage(GameObject fractal){
        Texture2D texture = null;
        texture = TextureToTexture2D(fractal.GetComponent<RawImage>().texture);
        SaveImage(texture.EncodeToPNG(), "MandelbulbImage_", ".png", ref countMandelbulb);
        yield return new WaitForEndOfFrame();
    }

    private void SaveImage(byte[] data, string fileName, string format, ref int count){
        var path = StandaloneFileBrowser.SaveFilePanel("Save Image", "", fileName+count+format, "");
        if (path.Length != 0){
            if (data != null)
                File.WriteAllBytes(path, data);
                count++;
        }else{
            return;
        }


    }


#endif
}