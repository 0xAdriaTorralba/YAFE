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
using UnityEngine;
using System.Numerics;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;



[RequireComponent(typeof(Button))]
public class ExportListener : MonoBehaviour, IPointerDownHandler {


    public GameObject mandelbrotGPU, juliaGPU;
    private byte[] _textureBytes;

    private Button button;

    public GameObject GOfractal;
    private Fractal fractal;
    private int countMandelbrot = 0;
    private int countJulia = 0;

    private TMP_Dropdown dropdownFormatMandelbrot, dropdownFormatJulia;

    private string formatMandelbrot = ".png", formatJulia = ".png";

    void Awake() {
        button = GetComponent<Button>();
        fractal = GOfractal.GetComponent<Fractal>();
        if (fractal is MandelbrotCPU){
            fractal = (MandelbrotCPU) fractal;
        }
        if (fractal is JuliaCPU){
            fractal = (JuliaCPU) fractal;
        }
        if (fractal is MandelbrotGPU){
            fractal = (MandelbrotGPU) fractal;
        }
        if (fractal is JuliaGPU){
            fractal = (JuliaGPU) fractal;
        }

        dropdownFormatMandelbrot = GameObject.Find("Format Mandelbrot Dropdown").GetComponent<TMP_Dropdown>();
        dropdownFormatJulia = GameObject.Find("Format Julia Dropdown").GetComponent<TMP_Dropdown>();



    }



    void Update(){
        if (fractal.GetFinished()){
            button.interactable = true;
        }else{
            button.interactable = false;
        }
    }



    private void DropdownValueChanged(TMP_Dropdown dropdown){
        if (string.Equals(dropdown.name, "Format Mandelbrot Dropdown")){
            formatMandelbrot = '.' + dropdown.captionText.text.ToString().ToLower();
        }
        if (string.Equals(dropdown.name, "Format Julia Dropdown")){
            formatJulia = '.' + dropdown.captionText.text.ToString().ToLower();
        }
    }



#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //

    void Start(){
        dropdownFormatMandelbrot.onValueChanged.AddListener( delegate  { DropdownValueChanged(dropdownFormatMandelbrot); });
        dropdownFormatJulia.onValueChanged.AddListener( delegate  { DropdownValueChanged(dropdownFormatJulia); });
    }

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
        if (fractal is FractalGPU){
            if (fractal is MandelbrotGPU){
                mandelbrotGPU.SetActive(true);
            }
            if (fractal is JuliaGPU){
                juliaGPU.SetActive(true);
            }
            yield return new WaitForSeconds(0.1f);
            texture = TextureToTexture2D(fractal.GetComponent<RawImage>().texture);
        }
        if (fractal is FractalCPU){
            Sprite image = fractal.GetComponent<Image>().sprite;
            texture = image.texture;
        }
        if (texture == null){
            LogsController.UpdateLogs(new string[] {"Cannot save! The image is not completed (yet)."}, "#FFA600");
            yield return null;
        }
        if (fractal is MandelbrotCPU || fractal is MandelbrotGPU){
            switch(formatMandelbrot){
                case ".png":
                    _textureBytes = texture.EncodeToPNG();
                    DownloadFile(gameObject.name, "OnFileDownload", "MandelbrotImage_"+countMandelbrot+formatMandelbrot, _textureBytes, _textureBytes.Length);
                    break;
                case ".jpg":
                    _textureBytes = texture.EncodeToJPG();
                    DownloadFile(gameObject.name, "OnFileDownload", "MandelbrotImage_"+countMandelbrot+formatMandelbrot, _textureBytes, _textureBytes.Length);
                    break;
                default:
                    _textureBytes = texture.EncodeToPNG();
                    DownloadFile(gameObject.name, "OnFileDownload", "MandelbrotImage_"+countMandelbrot+".png", _textureBytes, _textureBytes.Length);
                    break;
            }
        }

        if (fractal is JuliaCPU || fractal is JuliaGPU){
            switch(formatMandelbrot){
                case ".png":
                    _textureBytes = texture.EncodeToPNG();
                    DownloadFile(gameObject.name, "OnFileDownload", "JuliaImage_"+countJulia+formatJulia, _textureBytes, _textureBytes.Length);
                    break;
                case ".jpg":
                    _textureBytes = texture.EncodeToJPG();
                    DownloadFile(gameObject.name, "OnFileDownload", "JuliaImage_"+countJulia+formatJulia, _textureBytes, _textureBytes.Length);
                    break;
                default:
                    _textureBytes = texture.EncodeToPNG();
                    DownloadFile(gameObject.name, "OnFileDownload", "JuliaImage_"+countJulia+".png", _textureBytes, _textureBytes.Length);
                    break;
            }
        }
        if (fractal is MandelbrotGPU){
            mandelbrotGPU.SetActive(false);
        }
        if (fractal is JuliaGPU){
            juliaGPU.SetActive(false);
        }
    }

    // Called from browser
    public void OnFileDownload() {
        if (fractal is MandelbrotCPU || fractal is MandelbrotGPU){
            countMandelbrot++;
        }
        if (fractal is JuliaCPU || fractal is JuliaGPU){
            countJulia++;
        }
        LogsController.UpdateLogs(new string[] {"Image exported successfully!"}, "#75FF00");

    }

#elif UNITY_ANDROID || UNITY_IOS

    //
    // Android and iOS platforms
    //
        public void OnPointerDown(PointerEventData eventData) { }

    // // Listen OnClick event in standlone builds
    void Start() {
        dropdownFormatMandelbrot.onValueChanged.AddListener( delegate  { DropdownValueChanged(dropdownFormatMandelbrot); });
        dropdownFormatJulia.onValueChanged.AddListener( delegate  { DropdownValueChanged(dropdownFormatJulia); });
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





    private IEnumerator ExportImage(Fractal fractal){
        Texture2D texture = null;
        if (fractal is FractalGPU){
            //texture = GetTextureFromSurfaceShader(fractal.GetComponent<Image>().material, fractal.rp.pwidth, fractal.rp.pheight, RenderTexture.main);
            if (fractal is MandelbrotGPU){
                mandelbrotGPU.SetActive(true);
            }
            if (fractal is JuliaGPU){
                juliaGPU.SetActive(true);
            }
            yield return new WaitForSeconds(0.1f);
            texture = TextureToTexture2D(fractal.GetComponent<RawImage>().texture);
        }
        if (fractal is FractalCPU){
            Sprite image = fractal.GetComponent<Image>().sprite;
            texture = image.texture;
        }

        if (texture == null){
            LogsController.UpdateLogs(new string[] {"Cannot save! The image is not completed (yet)."}, "#FFA600");
            yield return null;
        }

        if (fractal is MandelbrotCPU || fractal is MandelbrotGPU){
            switch(formatMandelbrot){
                case ".png":
                    NativeGallery.SaveImageToGallery(texture.EncodeToPNG(), "Fractals Album", "MandelbrotImage_"+countMandelbrot+".png");
                    break;
                case ".jpg":
                    NativeGallery.SaveImageToGallery(texture.EncodeToJPG(), "Fractals Album", "MandelbrotImage_"+countMandelbrot+".jpg");
                    break;
                default:
                    NativeGallery.SaveImageToGallery(texture.EncodeToPNG(), "Fractals Album", "MandelbrotImage_"+countMandelbrot+".png");
                    break;
            }
            countMandelbrot++;
        }
        if (fractal is JuliaCPU || fractal is JuliaGPU){
            switch(formatJulia){
                case ".png":
                    NativeGallery.SaveImageToGallery(texture.EncodeToPNG(), "Fractals Album", "JuliaImage_"+countJulia+".png");
                    break;
                case ".jpg":
                    NativeGallery.SaveImageToGallery(texture.EncodeToJPG(), "Fractals Album", "JuliaImage_"+countJulia+".jpg");
                    break;
                default:
                    NativeGallery.SaveImageToGallery(texture.EncodeToPNG(), "Fractals Album", "JuliaImage_"+countJulia+".png");
                    break;
            }
            countJulia++;
        }
        LogsController.UpdateLogs(new string[] {"Image exported successfully!"}, "#75FF00");
        if (fractal is MandelbrotGPU){
            mandelbrotGPU.SetActive(false);
        }
        if (fractal is JuliaGPU){
            juliaGPU.SetActive(false);
        }
    }


#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }

    // // Listen OnClick event in standlone builds
    void Start() {
        dropdownFormatMandelbrot.onValueChanged.AddListener( delegate  { DropdownValueChanged(dropdownFormatMandelbrot); });
        dropdownFormatJulia.onValueChanged.AddListener( delegate  { DropdownValueChanged(dropdownFormatJulia); });
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

    private IEnumerator ExportImage(Fractal fractal){
        Texture2D texture = null;
        if (fractal is FractalGPU){
            if (fractal is MandelbrotGPU){
                mandelbrotGPU.SetActive(true);
            }
            if (fractal is JuliaGPU){
                juliaGPU.SetActive(true);
            }
            yield return new WaitForSeconds(0.1f);
            
            texture = TextureToTexture2D(fractal.GetComponent<RawImage>().texture);
        }
        if (fractal is FractalCPU){
            Sprite image = fractal.GetComponent<Image>().sprite;
            texture = image.texture;
        }

        if (texture == null){
            LogsController.UpdateLogs(new string[] {"Cannot save! The image is not completed (yet)."}, "#FFA600");
            yield return null;
        }

        if (fractal is MandelbrotCPU || fractal is MandelbrotGPU){
            switch(formatMandelbrot){
                case ".png":
                    SaveImage(texture.EncodeToPNG(), "MandelbrotImage_", ".png", ref countMandelbrot);
                    break;
                case ".jpg":
                    SaveImage(texture.EncodeToJPG(), "MandelbrotImage_", ".jpg", ref countMandelbrot);
                    break;
                default:
                    SaveImage(texture.EncodeToPNG(), "MandelbrotImage_", ".png", ref countMandelbrot);
                    break;
            }
        }
        if (fractal is JuliaCPU || fractal is JuliaGPU){
            switch(formatJulia){
                case ".png":
                    SaveImage(texture.EncodeToPNG(), "JuliaImage_", ".png", ref countJulia);
                    break;
                case ".jpg":
                    SaveImage(texture.EncodeToJPG(), "JuliaImage_", ".jpg", ref countJulia);
                    break;
                default:
                    SaveImage(texture.EncodeToPNG(), "JuliaImage_", ".png", ref countJulia);
                    break;
            }
        }
    }

    private void SaveImage(byte[] data, string fileName, string format, ref int count){
        var path = StandaloneFileBrowser.SaveFilePanel("Save Image", "", fileName+count+format, "");
        if (path.Length != 0){
            if (data != null)
                File.WriteAllBytes(path, data);
                count++;
        }else{
            LogsController.UpdateLogs(new string[] {"The image was not saved. (probably you closed the File Chooser)."}, "#FFA600");
            return;

        }
        LogsController.UpdateLogs(new string[] {"Image exported successfully!"}, "#75FF00");
        if (fractal is MandelbrotGPU){
            mandelbrotGPU.SetActive(false);
        }
        if (fractal is JuliaGPU){
            juliaGPU.SetActive(false);
        }

    }


#endif
}