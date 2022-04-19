using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using ShadowQuality = UnityEngine.ShadowQuality;
using ShadowResolution = UnityEngine.ShadowResolution;
using UnityEngine.Rendering;

public class Settings : MonoBehaviour
{
    public static Settings instance;
    public GameObject settingsPanel;
    public GameObject helpPanel;
    public GameObject mainCanvas;
    public GameObject cubeCanvas;
    public GameObject dynamicLights;
    public GameObject staticLights;
    public GameObject aboutPanel;
    public GameObject welcomePanel;
    public GameObject adReminderPanel;
    public GameObject reflections;
    public TMP_Dropdown dropdownNamesLanguage;
    public TMP_Dropdown dropdownDescriptionsLanguage;
    public TMP_Dropdown dropdownResolution;
    public MeshManagement meshManagement;
    bool UIEnabled = false;
    List<GameObject> hiddenGo = new List<GameObject>();
    public static SystemLanguage namesLanguage = SystemLanguage.English;
    public static SystemLanguage descriptionsLanguage = SystemLanguage.English;
    public Slider fontSizeSlider;
    public GameObject settingsBtn;
    private int lastFontSliderValue = 0;
    private TextMeshProUGUI[] texts;
    private Camera cam;
    public ForwardRendererData rendererData;
    public Light mainLight;
    public Light mainLight2;
    public Toggle settingsAdReminderToggle;
    public Toggle adReminderToggle;
    public Toggle ambientOcclusionToggle;
    public Toggle antiAliasingToggle;
    public Toggle staticLightsToggle;
    public Toggle shadowsToggle;
    public Toggle highQltyLightsToggle;
    public Toggle reflectionsToggle;
    public RenderPipelineAsset perPixelLightingPipeline;
    public RenderPipelineAsset perVertexLightingPipeline;
    public MultilanguageText[] multilanguageTexts;
    private UniversalRenderPipelineAsset perPixelAsset;
    private UniversalRenderPipelineAsset perVertexAsset;
    private Vector2 settingsBtnPos;
    private void Awake()
    {
        instance = this; 
        cam = Camera.main;
        settingsBtnPos = settingsBtn.GetComponent<RectTransform>().anchoredPosition;
        // multilanguageTexts = (MultilanguageText[])Resources.FindObjectsOfTypeAll(typeof(MultilanguageText));
    }
    private void Start()
    {
        perPixelAsset = (UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)perPixelLightingPipeline;
        perVertexAsset = (UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)perVertexLightingPipeline;
        perPixelAsset.useAdaptivePerformance = true;
        perPixelAsset.useSRPBatcher = true;
        perVertexAsset.useAdaptivePerformance = true;
        perVertexAsset.useSRPBatcher = true;

        ReadPlayerPrefs();
        settingsPanel.SetActive(false);
        namesLanguage = Application.systemLanguage;
        SetNamesLanguage();
        SetOutline(false);
        NamesManagement.instance.GetTranslatedNames();
    }

    private void ReadPlayerPrefs()
    {
        //Welcome Panel
        if(!PlayerPrefs.HasKey("FirstOpen") && GlobalVariables.instance.mobile)
        {
            welcomePanel.SetActive(true);
            MeshManagement.instance.globalParent.SetActive(false);
            PlayerPrefs.SetInt("FirstOpen", 1);
        }
        //Ad Reminder
       /* else if(PlayerPrefs.HasKey("AdReminder"))
        {
            if (PlayerPrefs.GetInt("AdReminder") == 1)
            {
                settingsAdReminderToggle.isOn = true;
                adReminderPanel.SetActive(true);
            }
            else
            {
                settingsAdReminderToggle.isOn = false;
            }
        }
        else
        {
            PlayerPrefs.SetInt("AdReminder", 1);
        }*/


        //Ambient Occlusion
        if (PlayerPrefs.HasKey("AmbientOcclusion"))
        {
            bool ambientOcclusion = PlayerPrefs.GetInt("AmbientOcclusion") == 1 ? true : false;
            for (int x = 0; x < rendererData.rendererFeatures.Count; x++)
            {
                if (rendererData.rendererFeatures[x].name == "Screen Space Ambient Occlusion")
                {
                    rendererData.rendererFeatures[x].SetActive(ambientOcclusion);
                }
            }
            ambientOcclusionToggle.isOn = ambientOcclusion;
        }

        bool antiAliasing = false;

        //Anti Aliasing
        if (PlayerPrefs.HasKey("AntiAliasing"))
        {
             antiAliasing = PlayerPrefs.GetInt("AntiAliasing") == 1 ? true : false;
        }

        UniversalAdditionalCameraData camData = cam.GetComponent<UniversalAdditionalCameraData>();
        if (antiAliasing)
        {
            camData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
        }
        else
        {
            camData.antialiasing = AntialiasingMode.None;
        }
        antiAliasingToggle.isOn = antiAliasing;
        

        //Resolution
        if (PlayerPrefs.HasKey("Resolution"))
        {
            dropdownResolution.value = PlayerPrefs.GetInt("Resolution");
        }
        SetResolution();

        //Shadows
        if (PlayerPrefs.HasKey("Shadows"))
        {
            shadowsToggle.isOn = PlayerPrefs.GetInt("Shadows") == 1 ? true : false;
        }
        SetShadows();

        //Lights
        if (PlayerPrefs.HasKey("highQltyLights"))
        {
            highQltyLightsToggle.isOn = PlayerPrefs.GetInt("highQltyLights") == 1 ? true : false;
        }
        SetLightQuality();

        //Static lights
        if (PlayerPrefs.HasKey("StaticLights"))
        {
            staticLightsToggle.isOn = PlayerPrefs.GetInt("StaticLights") == 1 ? true : false;
        }
        SetLights();

        //Reflections
        if (PlayerPrefs.HasKey("reflections"))
        {
            reflectionsToggle.isOn = PlayerPrefs.GetInt("reflections") == 1 ? true : false;
        }
        SetReflections();

        //Language
        if (PlayerPrefs.HasKey("NamesLanguage"))
        {
            dropdownNamesLanguage.value = PlayerPrefs.GetInt("NamesLanguage");
            SetNamesLanguage();
            TreeViewCanvas.instance.Reset();
        }
        else
        {
            PlayerPrefs.SetInt("NamesLanguage", 0);
            SetNamesLanguage();
            TreeViewCanvas.instance.Reset();
        }


        if (PlayerPrefs.HasKey("DescriptionsLanguage"))
        {
            dropdownDescriptionsLanguage.value = PlayerPrefs.GetInt("DescriptionsLanguage");
        }
        else
        {
            PlayerPrefs.SetInt("DescriptionsLanguage", 0);
        }
        SetDescriptionsLanguage();
    }

    public void OpenSettings()
    {
        if(!settingsPanel.activeInHierarchy)
            settingsPanel.SetActive(true);
        else
            settingsPanel.SetActive(false);
    }

    public void OpenAbout()
    {
        aboutPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void CloseAbout()
    {
        aboutPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
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
            settingsBtn.SetActive(true);
            settingsBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-65, -65);
        }
        else
        {
            foreach (GameObject go in hiddenGo)
            {
                go.SetActive(UIEnabled);
            }
            hiddenGo.Clear();
            settingsBtn.GetComponent<RectTransform>().anchoredPosition = settingsBtnPos;
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
                    labelScript.name = child.name.Replace("-line", "").Replace(".t", "");
                    labelScript.SetText(labelScript.name);
                }
            }
            if(child.childCount > 0)
                ApplyNameTranslation(child);
        }
    }

    public void SetNamesLanguage()
    {
        switch (dropdownNamesLanguage.value)
        {
            case 0:
                namesLanguage = Application.systemLanguage;
                break;
            case 1:
                namesLanguage = SystemLanguage.English;
                break;
            case 2:
                //Latin
                namesLanguage = SystemLanguage.Unknown;
                break;
            case 3:
                namesLanguage = SystemLanguage.French;
                break;
            case 4:
                namesLanguage = SystemLanguage.Spanish;
                break;
            case 5:
                namesLanguage = SystemLanguage.Portuguese;
                break;
            case 6:
                namesLanguage = SystemLanguage.Italian;
                break;
            case 7:
                namesLanguage = SystemLanguage.Dutch;
                break;
            case 8:
                namesLanguage = SystemLanguage.German;
                break;
            case 9:
                namesLanguage = SystemLanguage.Polish;
                break;
            default:
                namesLanguage = SystemLanguage.English;
                break;
        }

        PlayerPrefs.SetInt("NamesLanguage", dropdownNamesLanguage.value);
        ApplyNameTranslation(meshManagement.globalParent.transform);
        SetMultilanguageTextsTranslations();
    }

    private void SetMultilanguageTextsTranslations()
    {
        foreach (var text in multilanguageTexts)
        {
            text.Translate();
        }
    }

    public void SetDescriptionsLanguage()
    {
        switch(dropdownDescriptionsLanguage.value)
        {
            case 0:
                descriptionsLanguage = Application.systemLanguage;
                break;
            case 1:
                descriptionsLanguage = SystemLanguage.English;
                break;
            case 2:
                descriptionsLanguage = SystemLanguage.Spanish;
                break;
            case 3:
                descriptionsLanguage = SystemLanguage.French;
                break;
            case 4:
                descriptionsLanguage = SystemLanguage.Portuguese;
                break;
        }
        PlayerPrefs.SetInt("DescriptionsLanguage", dropdownDescriptionsLanguage.value);
    }

    public void UpdateFontSize()
    {
        texts = FindObjectsOfType<TextMeshProUGUI>(true);
        foreach (var text in texts)
        {
            text.fontSize += fontSizeSlider.value - lastFontSliderValue;
            text.fontSizeMax += fontSizeSlider.value - lastFontSliderValue;
        }
        lastFontSliderValue = (int)fontSizeSlider.value;
    }

    public void CloseHelpPanel()
    {
        helpPanel.SetActive(false);
    }

    public void OpenHelpPanel()
    {
        helpPanel.SetActive(true);
    }

    public void SetAdReminder()
    {
        if (adReminderToggle.isOn)
            settingsAdReminderToggle.isOn = false;
        PlayerPrefs.SetInt("AdReminder", settingsAdReminderToggle.isOn ? 1 : 0);
    }

    //---------Graphics-----------//
    public void SetResolution()
    {
        switch (dropdownResolution.value)
        {
            case 0:
                perPixelAsset.renderScale = 1;
                perVertexAsset.renderScale = 1;
                break;
            case 1:
                perPixelAsset.renderScale = 0.75f;
                perVertexAsset.renderScale = 0.75f;
                break;
            case 2:
                perPixelAsset.renderScale = 0.5f;
                perVertexAsset.renderScale = 0.5f;
                break;
            default:
                perPixelAsset.renderScale = 1;
                perVertexAsset.renderScale = 1;
                break;
        }
        PlayerPrefs.SetInt("Resolution", dropdownResolution.value);
    }

    public void SetShadows()
    {
        if(shadowsToggle.isOn)
        {
            mainLight.shadows = LightShadows.Soft;
            mainLight2.shadows = LightShadows.Soft;
            PlayerPrefs.SetInt("Shadows", 1);
        }
        else
        {
            mainLight.shadows = LightShadows.None;
            mainLight2.shadows = LightShadows.None;
            PlayerPrefs.SetInt("Shadows", 0);
        }

    }

    public void SetAntiAliasing()
    {
        UniversalAdditionalCameraData camData = cam.GetComponent<UniversalAdditionalCameraData>();
        if(antiAliasingToggle.isOn)
        {
            camData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
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

    public void SetLights()
    {
        dynamicLights.SetActive(!staticLightsToggle.isOn);
        staticLights.SetActive(staticLightsToggle.isOn);
        if (staticLightsToggle.isOn)
            PlayerPrefs.SetInt("StaticLights", 1);
        else
            PlayerPrefs.SetInt("StaticLights", 0);
    }

    public void SetLightQuality()
    {
        if (highQltyLightsToggle.isOn)
        {
            QualitySettings.renderPipeline = perPixelLightingPipeline;
            PlayerPrefs.SetInt("highQltyLights", 1);
        }
        else
        {
            QualitySettings.renderPipeline = perVertexLightingPipeline;
            PlayerPrefs.SetInt("highQltyLights", 0);
        }
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

    public void SetReflections()
    {
        reflections.SetActive(reflectionsToggle.isOn);
        if (reflectionsToggle.isOn)
        {
            PlayerPrefs.SetInt("reflections", 1);
        }
        else
            PlayerPrefs.SetInt("reflections", 0);
    }

}
