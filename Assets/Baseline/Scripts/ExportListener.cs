using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using TMPro;

[RequireComponent(typeof(Button))]
public class ExportListener : MonoBehaviour, IPointerDownHandler {

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

    // Broser plugin should be called in OnPointerDown.
    public void OnPointerDown(PointerEventData eventData) {
        Sprite image = fractal.GetComponent<Image>().sprite;
        Texture2D texture = image.texture;
        if (texture == null){
            LogsController.UpdateLogs(new string[] {"Cannot save! The image is not completed (yet)."}, "#FFA600");
            return;
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
                    DownloadFile(gameObject.name, "OnFileDownload", "MandelbrotImage_"+countMandelbrot+formatMandelbrot, _textureBytes, _textureBytes.Length);
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
                    DownloadFile(gameObject.name, "OnFileDownload", "JuliaImage_"+countJulia+formatJulia, _textureBytes, _textureBytes.Length);
                    break;
            }
        }
    }

    // Called from browser
    public void OnFileDownload() {
        countMandelbrot++;
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
        ExportImage(fractal);
    }

    private void ExportImage(Fractal fractal){
        Texture2D texture = null;
        if (fractal is FractalGPU){
            //texture = GetTextureFromSurfaceShader(fractal.GetComponent<Image>().material, fractal.rp.pwidth, fractal.rp.pheight);
        }
        if (fractal is FractalCPU){
            Sprite image = fractal.GetComponent<Image>().sprite;
            texture = image.texture;
        }

        if (texture == null){
            LogsController.UpdateLogs(new string[] {"Cannot save! The image is not completed (yet)."}, "#FFA600");
            return;
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
        ExportImage(fractal);
    }

    public Texture2D GetTextureFromSurfaceShader(Material mat, int width, int height)
    {
        //Create render texture:
        RenderTexture temp = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
 
        //Create a Quad:
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        MeshRenderer rend = quad.GetComponent<MeshRenderer>();
        rend.material = mat;
        Vector3 quadScale = quad.transform.localScale / (float)((Screen.height / 2.0) / Camera.main.orthographicSize);
        quad.transform.position = Vector3.forward;
 
        //Setup camera:
        GameObject camGO = new GameObject("CaptureCam");
        Camera cam = camGO.AddComponent<Camera>();
        cam.renderingPath = RenderingPath.Forward;
        cam.orthographic = true;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(1, 1, 1, 0);
        if (cam.rect.width < 1 || cam.rect.height < 1)
        {
            cam.rect = new Rect(cam.rect.x, cam.rect.y, 1, 1);
        }
        cam.orthographicSize = 0.5f;
        cam.rect = new Rect(0, 0, quadScale.x, quadScale.y);
        cam.aspect = quadScale.x / quadScale.y;
        cam.targetTexture = temp;
        cam.allowHDR = false;
 
 
        //Capture image and write to the render texture:

        temp = cam.targetTexture;
 
        //Apply changes:
        Texture2D newTex = new Texture2D(temp.width, temp.height, TextureFormat.ARGB32, true, true);
        RenderTexture.active = cam.targetTexture;
        cam.Render();
        newTex.ReadPixels(new Rect(0, 0, temp.width, temp.height), 0, 0);
        newTex.Apply();
 
        //Clean up:
        RenderTexture.active = null;
        temp.Release();
        Destroy(quad);
        Destroy(camGO);
 
        return newTex;
    }

    private void ExportImage(Fractal fractal){
        Texture2D texture = null;
        if (fractal is FractalGPU){
            texture = GetTextureFromSurfaceShader(fractal.GetComponent<Image>().material, fractal.rp.pwidth, fractal.rp.pheight);
        }
        if (fractal is FractalCPU){
            Sprite image = fractal.GetComponent<Image>().sprite;
            texture = image.texture;
        }

        if (texture == null){
            LogsController.UpdateLogs(new string[] {"Cannot save! The image is not completed (yet)."}, "#FFA600");
            return;
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

    }


#endif
}