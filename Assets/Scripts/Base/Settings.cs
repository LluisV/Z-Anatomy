using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Linq;
using UnityFx.Outline.URP;
using System.Reflection;
using UnityEngine.Experimental.Rendering.Universal;

public class Settings : MonoBehaviour
{
    public static Settings Instance;


    public GameObject mainCanvas;
    public GameObject cubeCanvas;
    public GameObject dynamicLights;

    public TMP_Dropdown dropDownLanguageSettings;
    public TMP_Dropdown dropDownLanguageBar;
    public TMP_Dropdown dropdownResolution;
    public TMP_Dropdown dropdownMSAA;

    public MeshManagement meshManagement;
    bool UIEnabled = false;
    List<GameObject> hiddenGo = new List<GameObject>();
    public static SystemLanguage language = SystemLanguage.English;
    public static int languageIndex = 0;
    private Camera cam;
    public UniversalRendererData rendererData;

    public Light mainLight;

    public Toggle fullscreenToggle;
    public Toggle ambientOcclusionToggle;
    public Toggle antiAliasingToggle;
    public Toggle shadowsToggle;
    public Toggle zoomToMouseToggle;
    public Toggle zoomSelectedToggle;
    public Toggle nameOnMouseToggle;
    public Toggle limitRotationToggle;
    public Toggle HDRToggle;

    public CustomSlider zoomVelocitySlider;
    public CustomSlider rotationVelocitySlider;

    public UniversalRenderPipelineAsset pipelineAsset;
    private MultilanguageText[] multilanguageTexts;

    public TextMeshProUGUI versionText;
    private bool first = true;

    private void Awake()
    {
        Instance = this; 
        cam = Camera.main;
        multilanguageTexts = Resources.FindObjectsOfTypeAll(typeof(MultilanguageText)) as MultilanguageText[];

        DebugManager.instance.enableRuntimeUI = false;
        versionText.text += " " + Application.version;

        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            dropdownResolution.transform.parent.gameObject.SetActive(false);
            fullscreenToggle.transform.parent.gameObject.SetActive(false);
        }

    }
    private void Start()
    {
        ReadPlayerPrefs();
        SetOutline(false);
    }

    private void ReadPlayerPrefs()
    {
        //Fullscreen
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        }
        //Ambient Occlusion
        if (PlayerPrefs.HasKey("AmbientOcclusion"))
        {
            bool ambientOcclusion = PlayerPrefs.GetInt("AmbientOcclusion") == 1;
            for (int x = 0; x < rendererData.rendererFeatures.Count; x++)
            {
                if (rendererData.rendererFeatures[x].name == "Screen Space Ambient Occlusion")
                {
                    rendererData.rendererFeatures[x].SetActive(ambientOcclusion);
                }
            }
            ambientOcclusionToggle.isOn = ambientOcclusion;
        }

        //Anti Aliasing
        bool antiAliasing = PlayerPrefs.GetInt("AntiAliasing", 1) == 1;
        //MSAA
        dropdownMSAA.value = PlayerPrefs.GetInt("MSAA", 1);
        //HDR
        HDRToggle.isOn = PlayerPrefs.GetInt("HDR", 1) == 1;
        pipelineAsset.supportsHDR = HDRToggle.isOn;

        UniversalAdditionalCameraData camData = cam.GetComponent<UniversalAdditionalCameraData>();
        if (antiAliasing)
        {
            camData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            camData.antialiasingQuality = AntialiasingQuality.High;
        }
        else
        {
            camData.antialiasing = AntialiasingMode.None;
        }
        antiAliasingToggle.isOn = antiAliasing;

        //Shadows
        shadowsToggle.isOn = PlayerPrefs.GetInt("Shadows", 1) == 1;    
        SetShadows();

        //Language
        dropDownLanguageSettings.value = GetSystemLanguage();
        if (PlayerPrefs.HasKey("Language"))
            dropDownLanguageSettings.value = PlayerPrefs.GetInt("Language");
        dropDownLanguageBar.value = dropDownLanguageSettings.value;

        //Zoom to mouse
        zoomToMouseToggle.isOn = PlayerPrefs.GetInt("ZoomToMouse", 1) == 1;
        //Zoom selected
        zoomSelectedToggle.isOn = PlayerPrefs.GetInt("ZoomSelected", 1) == 1;
        //NameOnMouse
        nameOnMouseToggle.isOn = PlayerPrefs.GetInt("NameOnMouse", 1) == 1;
        //LimintRotation
        limitRotationToggle.isOn = PlayerPrefs.GetInt("LimitRotation", 0) == 1;


        //Zoom velocity
        zoomVelocitySlider.value = PlayerPrefs.GetFloat("ZoomVelocity", 1);
        //Rotation velocity
        rotationVelocitySlider.value = PlayerPrefs.GetFloat("RotationVelocity", 1);
    }

    public void HideUI()
    {
        if (!UIEnabled)
        {
            foreach (Transform child in mainCanvas.transform)
            {
                if (!child.name.Equals("SettingsPanel"))
                {
                    if(child.gameObject.activeInHierarchy)
                        hiddenGo.Add(child.gameObject);
                    child.gameObject.SetActive(UIEnabled);
                }
            }
            foreach(Transform child in cubeCanvas.transform)
            {
                if (child.gameObject.activeInHierarchy)
                    hiddenGo.Add(child.gameObject);
                child.gameObject.SetActive(UIEnabled);             
            }
        }
        else
        {
            foreach (GameObject go in hiddenGo)
            {
                go.SetActive(UIEnabled);
            }
            hiddenGo.Clear();
        }
        UIEnabled = !UIEnabled;
    }

    void ApplyNameTranslation(Transform parent)
    {
        foreach (Transform child in parent)
        {
            NameAndDescription script = child.gameObject.GetComponent<NameAndDescription>();
            if(script != null)
            {
                script.SetTranslatedName();
                Label labelScript = child.GetComponent<Label>();
                if(labelScript != null)
                {
                    labelScript.name = child.name.Replace(".j", "").Replace(".i","").Replace(".t", "").Replace(".s","");
                    labelScript.SetText(labelScript.name);
                }
            }
            if(child.childCount > 0)
                ApplyNameTranslation(child);
        }
    }

    private void SetMultilanguageTextsTranslations()
    {
        foreach (var text in multilanguageTexts)
        {
            text.Translate();
        }
    }

    public int GetSystemLanguage()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.English:
                return 0;
            case SystemLanguage.Unknown:
                return 1;
            case SystemLanguage.Spanish:
                return 2;
            case SystemLanguage.French:
                return 3;
            case SystemLanguage.Portuguese:
                return 4;
        }

        return 0;
    }

    public void SetLanguage(bool fromSettings = true)
    {
        //Dropdown calls onValueChange on start
        if (first)
        {
            first = false;
            return;
        }
        int value;
        if (fromSettings)
        {
            value = dropDownLanguageSettings.value;
            dropDownLanguageBar.SetValueWithoutNotify(value);
        }
        else
        {
            value = dropDownLanguageBar.value;
            dropDownLanguageSettings.SetValueWithoutNotify(value);
        }

        switch (value)
        {
            case 0:
                language = SystemLanguage.English;
                languageIndex = 0;
                break;
            case 1:
                language = SystemLanguage.Unknown;
                languageIndex = 1;
                break;
            case 2:
                language = SystemLanguage.Spanish;
                languageIndex = 3;
                break;
            case 3:
                language = SystemLanguage.French;
                languageIndex = 2;
                break;
            case 4:
                language = SystemLanguage.Portuguese;
                languageIndex = 4;
                break;
        }

        PlayerPrefs.SetInt("Language", dropDownLanguageSettings.value);
        ApplyNameTranslation(GlobalVariables.Instance.globalParent.transform);
        SetMultilanguageTextsTranslations();
        HighlightText.GetTranslatedNames();
        Lexicon.Instance.ResetAll();
        HierarchyBar.Instance.Set();
        var camTarget = CameraController.instance.target;
        if(camTarget != null)
        {
            var nameScript = camTarget.GetComponent<NameAndDescription>();
            if (nameScript != null)
                nameScript.SetDescription();
        }
    }


    public void SetZoomVelocity()
    {
        CameraController.instance.zoomVelocity = zoomVelocitySlider.value;
        PlayerPrefs.SetFloat("ZoomVelocity", CameraController.instance.zoomVelocity);

    }

    public void SetRotationVelocity()
    {
        CameraController.instance.rotationVelocity = rotationVelocitySlider.value;
        PlayerPrefs.SetFloat("RotationVelocity", CameraController.instance.rotationVelocity);
    }

    public void SetZoomToMouse()
    {
        ActionControl.zoomToMouse = zoomToMouseToggle.isOn;
        PlayerPrefs.SetInt("ZoomToMouse", ActionControl.zoomToMouse ? 1 : 0);
    }

    public void SetZoomSelected()
    {
        ActionControl.zoomSelected = zoomSelectedToggle.isOn;
        PlayerPrefs.SetInt("ZoomSelected", ActionControl.zoomSelected ? 1 : 0);

    }

    public void SetNameOnMouse()
    {
        ActionControl.nameOnMouse = !ActionControl.nameOnMouse;
        PlayerPrefs.SetInt("NameOnMouse", ActionControl.nameOnMouse ? 1 : 0);
    }

    public void SetLimitRotation()
    {
        ActionControl.limitRotation = !ActionControl.limitRotation;
        PlayerPrefs.SetInt("LimitRotation", ActionControl.limitRotation ? 1 : 0);
    }

    //---------Graphics-----------//

    public void SetResolutionValues()
    {
        dropdownResolution.ClearOptions();
        List<string> m_DropOptions = new List<string>();
        foreach (var item in ResolutionManager.Instance.WindowedResolutions)
        {
            string res = item.x + " x " + item.y;
            m_DropOptions.Add(res);
            // Debug.Log(res);
        }
        dropdownResolution.AddOptions(m_DropOptions);

        if(PlayerPrefs.GetInt("Fullscreen", 1) == 1)
            dropdownResolution.SetValueWithoutNotify(PlayerPrefs.GetInt("Resolution", ResolutionManager.Instance.currWindowedRes));
        else
            dropdownResolution.value = PlayerPrefs.GetInt("Resolution", ResolutionManager.Instance.currWindowedRes);

    }

    public void SetResolution()
    {
        if (PlayerPrefs.GetInt("Fullscreen", 1) == 0 && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            ResolutionManager.Instance.SetResolution(dropdownResolution.value, false);
            PlayerPrefs.SetInt("Resolution", dropdownResolution.value);
        }
    }

    public void SetShadows()
    {
        if(shadowsToggle.isOn)
        {
            mainLight.shadows = LightShadows.Soft;
            PlayerPrefs.SetInt("Shadows", 1);
        }
        else
        {
            mainLight.shadows = LightShadows.None;
            PlayerPrefs.SetInt("Shadows", 0);
        }

    }

    public void SetAntiAliasing()
    {
        UniversalAdditionalCameraData camData = cam.GetComponent<UniversalAdditionalCameraData>();
        if(antiAliasingToggle.isOn)
        {
            camData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            PlayerPrefs.SetInt("AntiAliasing", 1);
        }
        else
        {
            camData.antialiasing = AntialiasingMode.None;
            PlayerPrefs.SetInt("AntiAliasing", 0);
        }
    }

    public void SetAmbientOcclusion()
    {
        for (int x = 0; x < rendererData.rendererFeatures.Count; x++)
        {
            if (rendererData.rendererFeatures[x].name == "Screen Space Ambient Occlusion")
            {
                rendererData.rendererFeatures[x].SetActive(ambientOcclusionToggle.isOn);
            }
        }
        if(ambientOcclusionToggle.isOn)
            PlayerPrefs.SetInt("AmbientOcclusion", 1);
        else
            PlayerPrefs.SetInt("AmbientOcclusion", 0);
    }

    public void SetOutline(bool state)
    {
        for (int x = 0; x < rendererData.rendererFeatures.Count; x++)
        {
            if (rendererData.rendererFeatures[x].name == "Outline")
            {
                rendererData.rendererFeatures[x].SetActive(state);
            }
        }
    }

    public void SetMSAA()
    {

        switch(dropdownMSAA.value)
        {
            case 0:
                pipelineAsset.msaaSampleCount = 0;
                break;
            case 1:
                pipelineAsset.msaaSampleCount = 2;
                break;
            case 2:
                pipelineAsset.msaaSampleCount = 4;
                break;
            case 3:
                pipelineAsset.msaaSampleCount = 8;
                break;
        }
        PlayerPrefs.SetInt("MSAA", dropdownMSAA.value);
    }

    public void SetHDR()
    {
        pipelineAsset.supportsHDR = HDRToggle.isOn;
        PlayerPrefs.SetInt("HDR", HDRToggle.isOn ? 1 : 0);
    }

}
