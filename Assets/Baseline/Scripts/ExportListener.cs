// using System.IO;
// using System.Text;
// using System.Runtime.InteropServices;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.EventSystems;
// using SFB;
// using TMPro;

// [RequireComponent(typeof(Button))]
// public class ExportListener : MonoBehaviour, IPointerDownHandler
// {

//     private Button buttonMandelbrot, buttonJulia;

//     private Mandelbrot fractalMandelbrot;
//     private Julia fractalJulia;
//     private static int countMandelbrot = 0;
//     private static int countJulia = 0;

//     private TMP_Dropdown dropdownFormatMandelbrot, dropdownFormatJulia;

//     private string formatMandelbrot = ".png", formatJulia = ".png";

//     private byte[] texture;

//     private LogsController logsController;

//     void Awake(){
//         buttonMandelbrot = GameObject.Find("Save Mandelbrot Button").GetComponent<Button>();
//         buttonJulia = GameObject.Find("Save Julia Button").GetComponent<Button>();
//         dropdownFormatMandelbrot = GameObject.Find("Format Mandelbrot Dropdown").GetComponent<TMP_Dropdown>();
//         dropdownFormatJulia = GameObject.Find("Format Julia Dropdown").GetComponent<TMP_Dropdown>();
//         fractalMandelbrot = GameObject.FindGameObjectWithTag("Mandelbrot").GetComponent<Mandelbrot>();
//         texture = fractalMandelbrot.GetComponent<Image>().sprite.texture.EncodeToPNG();
//         fractalJulia = GameObject.FindGameObjectWithTag("Julia").GetComponent<Julia>();
//         logsController = GameObject.FindGameObjectWithTag("LogsController").GetComponent<LogsController>();    
//     }

//     void Start()
//     {
//         buttonMandelbrot.onClick.AddListener(() => ExportImage(fractalMandelbrot));
//         buttonJulia.onClick.AddListener(() => ExportImage(fractalJulia));
//         dropdownFormatMandelbrot.onValueChanged.AddListener( delegate  { DropdownValueChanged(dropdownFormatMandelbrot); });
//         dropdownFormatJulia.onValueChanged.AddListener( delegate  { DropdownValueChanged(dropdownFormatJulia); });
//     }

//     #if UNITY_WEBGL && !UNITY_EDITOR
//     //
//     // WebGL
//     //
//     [DllImport("__Internal")]
//     private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

//     // Browser plugin should be called in OnPointerDown.
//     public void OnPointerDown(PointerEventData eventData) {
//         DownloadFile(gameObject.name, "OnFileDownload", "sample.png", texture, texture.Length);
//     }

//     // Called from browser
//     public void OnFileDownload() {
//         LogsController.UpdateLogs(new string[] {"Image exported successfully!"}, "#75FF00");
//     }

//     #endif

//     private void DropdownValueChanged(TMP_Dropdown dropdown){
//         if (string.Equals(dropdown.name, "Format Mandelbrot Dropdown")){
//             formatMandelbrot = '.' + dropdown.captionText.text.ToString().ToLower();
//         }
//         if (string.Equals(dropdown.name, "Format Julia Dropdown")){
//             formatJulia = '.' + dropdown.captionText.text.ToString().ToLower();
//         }
//     }



//     private void ExportImage(Fractal fractal){
//         Sprite image = fractal.GetComponent<Image>().sprite;
//         Texture2D texture = image.texture;
//         if (texture == null){
//             LogsController.UpdateLogs(new string[] {"Cannot save! The image is not completed (yet)."}, "#FFA600");
//             return;
//         }

//         if (fractal is Mandelbrot){
//             switch(formatMandelbrot){
//                 case ".png":
//                     SaveImage(texture.EncodeToPNG(), "MandelbrotImage_", ".png", ref countMandelbrot);
//                     break;
//                 case ".jpg":
//                     SaveImage(texture.EncodeToJPG(), "MandelbrotImage_", ".jpg", ref countMandelbrot);
//                     break;
//                 default:
//                     SaveImage(texture.EncodeToPNG(), "MandelbrotImage_", ".png", ref countMandelbrot);
//                     break;
//             }
//         }
//         if (fractal is Julia){
//             switch(formatJulia){
//                 case ".png":
//                     SaveImage(texture.EncodeToPNG(), "JuliaImage_", ".png", ref countJulia);
//                     break;
//                 case ".jpg":
//                     SaveImage(texture.EncodeToJPG(), "JuliaImage_", ".jpg", ref countJulia);
//                     break;
//                 default:
//                     SaveImage(texture.EncodeToPNG(), "JuliaImage_", ".png", ref countJulia);
//                     break;
//             }
//         }
//     }

//     private void SaveImage(byte[] data, string fileName, string format, ref int count){
//         var path = StandaloneFileBrowser.SaveFilePanel("Save Image", "", fileName+count+format, "");
//         if (path.Length != 0){
//             if (data != null)
//                 File.WriteAllBytes(path, data);
//                 count++;
//         }else{
//             LogsController.UpdateLogs(new string[] {"The image was not saved. (probably you closed the File Chooser)."}, "#FFA600");
//             return;

//         }
//         LogsController.UpdateLogs(new string[] {"Image exported successfully!"}, "#75FF00");

//     }

//     void Update()
//     {
//         if (fractalMandelbrot.GetFinished()){
//             buttonMandelbrot.interactable = true;
//         }else{
//             buttonMandelbrot.interactable = false;
//         }

//         if (fractalJulia.GetFinished()){
//             buttonJulia.interactable = true;
//         }else{
//             buttonJulia.interactable = false;
//         }
//     }
// }


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
        if (fractal is Mandelbrot){
            fractal = (Mandelbrot) fractal;
        }
        if (fractal is Julia){
            fractal = (Julia) fractal;
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
        if (fractal is Mandelbrot){
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

        if (fractal is Julia){
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

    private void ExportImage(Fractal fractal){
        Sprite image = fractal.GetComponent<Image>().sprite;
        Texture2D texture = image.texture;
        if (texture == null){
            LogsController.UpdateLogs(new string[] {"Cannot save! The image is not completed (yet)."}, "#FFA600");
            return;
        }

        if (fractal is Mandelbrot){
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
        if (fractal is Julia){
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