using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CrossSections : MonoBehaviour
{
    public static CrossSections Instance;

    //Min pos => X = -3
    //Max pos => X = 3
    public GameObject sagitalPlane;

    //Min pos => Y = -5.4
    //Max pos => Y = 6.75
    public GameObject transversalPlane;

    //Min => Z = -1.75
    //Max => Z = 1.75
    public GameObject frontalPlane;

    public Slider frontalSlider;
    public Image backgroundFrontalSlider;
    public Image fillFrontalSlider;
    private float frontalInitialValue;

    public Slider transversalSlider;
    public Image backgroundTransversalSlider;
    public Image fillTransversalSlider;
    private float transversalInitialValue;

    public Slider sagitalSlider;
    public Image backgroundSagitalSlider;
    public Image fillSagitalSlider;
    private float sagitalInitialValue;


    [HideInInspector]
    public bool xPlane;
    [HideInInspector]
    public bool ixPlane;

    [HideInInspector]
    public bool yPlane;
    [HideInInspector]
    public bool iyPlane;

    [HideInInspector]
    public bool zPlane;
    [HideInInspector]
    public bool izPlane;

    [HideInInspector]
    public bool inverted = false;

    private Sprite defaultSprite;
    public Image globalImage;
    public ExpandCollapseUI planesOptionsPanel;

    public ToggleChangeColor sagitalToggle;
    public ToggleChangeColor frontalToggle;
    public ToggleChangeColor transversalToggle;
    public ToggleChangeColor globalToggle;

    public CrossSectionPlane XPlane;
    public CrossSectionPlane YPlane;
    public CrossSectionPlane ZPlane;

    [HideInInspector]
    public CrossSectionPlane activePlane;

    [Space]
    [Header("Body Sections Toggles")]
    public SwitchButton skeletalToggle;
    public SwitchButton jointsToggle;
    public SwitchButton lymphsToggle;
    public SwitchButton muscularToggle;
    public SwitchButton fasciaToggle;
    public SwitchButton arteriesToggle;
    public SwitchButton veinsToggle;
    public SwitchButton nervousToggle;
    public SwitchButton visceralToggle;
    public SwitchButton regionsToggle;
    public SwitchButton referencesToggle;

    [HideInInspector]
    public bool invertedSliders = false;

    private void Awake()
    {
        Instance = this;
        defaultSprite = globalImage.sprite;
        frontalInitialValue = frontalSlider.value;
        sagitalInitialValue = sagitalSlider.value;
        transversalInitialValue = transversalSlider.value;
    }

    public void ResetAll()
    {
        sagitalToggle.SetEnabledColor();
        frontalToggle.SetDisabledColor();
        transversalToggle.SetDisabledColor();

        frontalSlider.value = frontalInitialValue;
        sagitalSlider.value = sagitalInitialValue;
        transversalSlider.value = transversalInitialValue;

        skeletalToggle.SetOn();
        jointsToggle.SetOn();
        lymphsToggle.SetOn();
        muscularToggle.SetOn();
        fasciaToggle.SetOn();
        arteriesToggle.SetOn();
        veinsToggle.SetOn();
        nervousToggle.SetOn();
        visceralToggle.SetOn();
        regionsToggle.SetOn();
        referencesToggle.SetOn();

        SkeletalToggleClick();
        InsertionsToggleClick();
        JointsToggleClick();
        LymphsToggleClick();
        MuscularToggleClick();
        FasciaToggleClick();
        ArteriesToggleClick();
        VeinsToggleClick();
        NervousToggleClick();
        VisceralToggleClick();
        RegionsToggleClick();
        ReferencesToggleClick();

        if (inverted)
            InvertSliders();

        inverted = false;

        planesOptionsPanel.Collapse();
        planesOptionsPanel.isExpanded = false;

        ActionControl.crossSectionsEnabled = false;
    }

    public void InvertSliders()
    {
        invertedSliders = !invertedSliders;
        //Invert sliders
        Color background = backgroundFrontalSlider.color;
        Color fill = fillFrontalSlider.color;

        backgroundFrontalSlider.color = fill;
        fillFrontalSlider.color = background;
        backgroundTransversalSlider.color = fill;
        fillTransversalSlider.color = background;
        backgroundSagitalSlider.color = fill;
        fillSagitalSlider.color = background;
    }

    public void DeactivateAll()
    {
        sagitalPlane.SetActive(false);
        transversalPlane.SetActive(false);
        frontalPlane.SetActive(false);

        xPlane = false;
        ixPlane = false;
        yPlane = false;
        iyPlane = false;
        zPlane = false;
        izPlane = false;

        sagitalToggle.SetDisabledColor();
        transversalToggle.SetDisabledColor();
        frontalToggle.SetDisabledColor();
        globalToggle.SetDisabledColor();
        activePlane = null;

        globalImage.sprite = defaultSprite;

        ActionControl.crossSectionsEnabled = false;
    }

    public bool IsEnabledByTag(string tag)
    {
        switch (tag)
        {
            case "Skeleton":
                return skeletalToggle.isOn;

            case "Joints":
                return jointsToggle.isOn;

            case "Insertions":
                return skeletalToggle.isOn;

            case "Lymph":
                return lymphsToggle.isOn;

            case "Muscles":
                return muscularToggle.isOn;

            case "Fascia":
                return fasciaToggle.isOn;

            case "Arteries":
                return arteriesToggle.isOn;

            case "Veins":
                return veinsToggle.isOn;

            case "Nervous":
                return nervousToggle.isOn;

            case "Visceral":
                return visceralToggle.isOn;

            case "BodyParts":
                return regionsToggle.isOn;

            case "References":
                return referencesToggle.isOn;

            default:
                Debug.Log("TAG NOT FOUND");
                return false;
        }
    }

    #region Plane Click
    public void XClick()
    {
        DeactivateAll();
        sagitalSlider.gameObject.SetActive(true);
        frontalSlider.gameObject.SetActive(false);
        transversalSlider.gameObject.SetActive(false);

        xPlane = true;
        sagitalPlane.transform.rotation = Quaternion.Euler(90, 0, 90);
        sagitalPlane.SetActive(true);
        ActionControl.crossSectionsEnabled = true;

        activePlane = XPlane;

        globalToggle.SetEnabledColor();
    }

    public void InvertedXClick()
    {
        DeactivateAll();
        sagitalSlider.gameObject.SetActive(true);
        frontalSlider.gameObject.SetActive(false);
        transversalSlider.gameObject.SetActive(false);

        ixPlane = true;
        sagitalPlane.transform.rotation = Quaternion.Euler(90, 90, 0);
        sagitalPlane.SetActive(true);
        ActionControl.crossSectionsEnabled = true;

        activePlane = XPlane;

        globalToggle.SetEnabledColor();

    }

    public void ZClick()
    {
        DeactivateAll();
        transversalSlider.gameObject.SetActive(true);
        sagitalSlider.gameObject.SetActive(false);
        frontalSlider.gameObject.SetActive(false);

        zPlane = true;
        transversalPlane.transform.rotation = Quaternion.Euler(180, 0, 0);
        transversalPlane.SetActive(true);
        ActionControl.crossSectionsEnabled = true;
        activePlane = ZPlane;


        globalToggle.SetEnabledColor();

    }

    public void InvertedZClick()
    {
        DeactivateAll();
        transversalSlider.gameObject.SetActive(true);
        sagitalSlider.gameObject.SetActive(false);
        frontalSlider.gameObject.SetActive(false);
        izPlane = true;
        transversalPlane.transform.rotation = Quaternion.Euler(0, 0, 0);
        transversalPlane.SetActive(true);
        ActionControl.crossSectionsEnabled = true;
        activePlane = ZPlane;


        globalToggle.SetEnabledColor();

    }

    public void YClick()
    {
        DeactivateAll();
        transversalSlider.gameObject.SetActive(false);
        sagitalSlider.gameObject.SetActive(false);
        frontalSlider.gameObject.SetActive(true);

        yPlane = true;
        frontalPlane.transform.rotation = Quaternion.Euler(90, 0, 0);
        frontalPlane.SetActive(true);
        ActionControl.crossSectionsEnabled = true;
        activePlane = YPlane;


        globalToggle.SetEnabledColor();

    }

    public void InvertedYClick()
    {
        DeactivateAll();
        transversalSlider.gameObject.SetActive(false);
        sagitalSlider.gameObject.SetActive(false);
        frontalSlider.gameObject.SetActive(true);

        iyPlane = true;
        frontalPlane.transform.rotation = Quaternion.Euler(90, 0, 180);
        frontalPlane.SetActive(true);
        ActionControl.crossSectionsEnabled = true;
        activePlane = YPlane;


        globalToggle.SetEnabledColor();

    }

    public void NoCutClick()
    {
        DeactivateAll();
    }

    #endregion

    #region Sliders

    public void FrontalSliderChanged()
    {
        Vector3 newPos = frontalPlane.transform.position;
        newPos.z = frontalSlider.value;
        frontalPlane.transform.position = newPos;
    }

    public void SagitalSliderChanged()
    {
        Vector3 newPos = sagitalPlane.transform.position;
        newPos.x = sagitalSlider.value;
        sagitalPlane.transform.position = newPos;
    }

    public void TransversalSliderChanged()
    {
        Vector3 newPos = transversalPlane.transform.position;
        newPos.y = transversalSlider.value;
        transversalPlane.transform.position = newPos;

    }
    #endregion

    #region ADD/SUB

    public void SagitalAdd()
    {
        sagitalSlider.value += 0.01f * sagitalSlider.maxValue;
    }

    public void SagitalSub()
    {
        sagitalSlider.value -= 0.01f * sagitalSlider.maxValue;
    }

    public void TransversalAdd()
    {
        transversalSlider.value += 0.01f * transversalSlider.maxValue;

    }

    public void TransversalSub()
    {
        transversalSlider.value -= 0.01f * transversalSlider.maxValue;
    }

    public void FrontalAdd()
    {
        frontalSlider.value += 0.01f * frontalSlider.maxValue;
    }

    public void FrontalSub()
    {
        frontalSlider.value -= 0.01f * frontalSlider.maxValue;
    }

    #endregion

    #region Sections Toggle

    public void SkeletalToggleClick()
    {
        SetMaterial("Skeleton", skeletalToggle.isOn);
    }
    public void InsertionsToggleClick()
    {
        SetMaterial("Insertions", skeletalToggle.isOn);
    }
    public void JointsToggleClick()
    {
        SetMaterial("Joints", jointsToggle.isOn);
    }
    public void LymphsToggleClick()
    {
        SetMaterial("Lymph", lymphsToggle.isOn);
    }
    public void MuscularToggleClick()
    {
        SetMaterial("Muscles", muscularToggle.isOn);
    }
    public void FasciaToggleClick()
    {
        SetMaterial("Fascia", fasciaToggle.isOn);
    }
    public void ArteriesToggleClick()
    {
        SetMaterial("Arteries", arteriesToggle.isOn);
    }
    public void VeinsToggleClick()
    {
        SetMaterial("Veins", veinsToggle.isOn);
    }
    public void NervousToggleClick()
    {
        SetMaterial("Nervous", nervousToggle.isOn);
    }
    public void VisceralToggleClick()
    {
        SetMaterial("Visceral", visceralToggle.isOn);
    }
    public void RegionsToggleClick()
    {
        SetMaterial("BodyParts", regionsToggle.isOn);
    }
    public void ReferencesToggleClick()
    {
        SetMaterial("References", referencesToggle.isOn);
    }

    public void SetMaterial(string tag, bool enabled)
    {
        for (int i = 0; i < MeshManagement.Instance.rendererMaterials.Count; i++)
            if (GlobalVariables.Instance.allBodyPartRenderers[i].CompareTag(tag))
                foreach (Material material in GlobalVariables.Instance.allBodyPartRenderers[i].materials)
                    material.SetFloat("_PlaneEnabled", enabled ? 1f : 0f);
    }

    #endregion
}
