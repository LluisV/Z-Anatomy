using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KeyColors : MonoBehaviour
{

    public static KeyColors Instance;

    public float skeletonWeight;
    public float insertionsWeight;
    public float musclesWeight;
    public float nervousWeight;
    public float lymphsWeight;
    public float regionsWeight;
    public float visceralWeight;

    [Header("Primary colors")]
    public List<Material> skeletonPrimary;
    public List<Material> insertionsPrimary;
    public List<Material> musclesPrimary;
    public List<Material> nervousPrimary;
    public List<Material> lymphsPrimary;
    public List<Material> regionsPrimary;
    public List<Material> visceralPrimary;

    [Space]
    [Space]
    [Header("Secondary colors")]
    public List<Material> skeletonSecondary;
    public List<Material> insertionsSecondary;
    public List<Material> musclesSecondary;
    public List<Material> nervousSecondary;
    public List<Material> lymphsSecondary;
    public List<Material> regionsSecondary;
    public List<Material> visceralSecondary;

    private bool bonesEnabled;
    private bool insertionsEnabled;
    private bool muscularEnabled;
    private bool nervesEnabled;
    private bool lymphsEnabled;
    private bool skinEnabled;
    private bool visceraEnabled;

    public bool update;

    private void Awake()
    {
        Instance = this;
    }

    public Material GetSecondaryColor(string tag, Material primary)
    {
        int index = -1;
        bool wasInSearch = false;
        switch (tag)
        {
            case "Skeleton":
                wasInSearch = true;
                index = skeletonPrimary.IndexOf(primary);
                if (index != -1)
                    return skeletonSecondary[index];
                break;

            /*case "Joints":
                return jointsToggle.isOn;*/

            case "Insertions":
                wasInSearch = true;
                index = insertionsPrimary.IndexOf(primary);
                if (index != -1)
                    return insertionsSecondary[index];
                break;

            case "Lymph":
                wasInSearch = true;
                index = lymphsPrimary.IndexOf(primary);
                if (index != -1)
                    return lymphsSecondary[index];
                break;

            case "Muscles":
                wasInSearch = true;
                index = musclesPrimary.IndexOf(primary);
                if (index != -1)
                    return musclesSecondary[index];
                break;

           /* case "Vascular":
                return cardiovascularToggle.isOn;*/

            case "Nervous":
                wasInSearch = true;
                index = nervousPrimary.IndexOf(primary);
                if (index != -1)
                    return nervousSecondary[index];
                break;

            case "Visceral":
                wasInSearch = true;
                index = visceralPrimary.IndexOf(primary);
                if (index != -1)
                    return visceralSecondary[index];
                break;

            case "BodyParts":
                wasInSearch = true;
                index = regionsPrimary.IndexOf(primary);
                if (index != -1)
                    return regionsSecondary[index];
                break;

            /* case "Fascia":
                 return muscularToggle.isOn;*/

            /*case "References":
                return referencesToggle.isOn;*/

            default:
                break;
        }

        if(wasInSearch)
            Debug.Log("Material not found: " + tag + " " + primary);
        return null;
    }

    private void SetLayerMaterial(List<TangibleBodyPart> bodyParts, bool state, SwitchButton CrossSectionsToggle)
    {
        if(state)
        {
            foreach (var bodyPart in bodyParts)
                bodyPart.SetSecondaryMaterial(CrossSectionsToggle.isOn);
        }
        else
        {
            foreach (var bodyPart in bodyParts)
                bodyPart.SetPrimaryMaterial(CrossSectionsToggle.isOn);
        }

        if(CrossSections.Instance.activePlane != null)
            CrossSections.Instance.activePlane.SendPositionToShader();
    }

    public void SetBonesKeyColors()
    {
        bonesEnabled = !bonesEnabled;
        SetLayerMaterial(GlobalVariables.Instance.bones, bonesEnabled, CrossSections.Instance.skeletalToggle);
    }

    public void SetInsertionsKeyColor()
    {
        insertionsEnabled = !insertionsEnabled;
        SetLayerMaterial(GlobalVariables.Instance.insertions, insertionsEnabled, CrossSections.Instance.skeletalToggle);
    }

    public void SetMusclesKeyColors()
    {
        muscularEnabled = !muscularEnabled;
        SetLayerMaterial(GlobalVariables.Instance.muscles, muscularEnabled, CrossSections.Instance.muscularToggle);
        SetInsertionsKeyColor();
    }

    public void SetNervesKeyColors()
    {
        nervesEnabled = !nervesEnabled;
        SetLayerMaterial(GlobalVariables.Instance.nerves, nervesEnabled, CrossSections.Instance.nervousToggle);
    }

    public void SetLymphsKeyColors()
    {
        lymphsEnabled = !lymphsEnabled;
        SetLayerMaterial(GlobalVariables.Instance.lymphs, lymphsEnabled, CrossSections.Instance.lymphsToggle);
    }

    public void SetVisceraKeyColors()
    {
        visceraEnabled = !visceraEnabled;
        SetLayerMaterial(GlobalVariables.Instance.viscera, visceraEnabled, CrossSections.Instance.visceralToggle);
    }

    public void SetRegionsKeyColors()
    {
        skinEnabled = !skinEnabled;
        SetLayerMaterial(GlobalVariables.Instance.regions, skinEnabled, CrossSections.Instance.regionsToggle);
    }

    public void ResetAll()
    {
        bonesEnabled = false;
        insertionsEnabled = false;
        muscularEnabled = false;
        nervesEnabled = false;
        lymphsEnabled = false;
        visceraEnabled = false;
        skinEnabled = false;

        SetLayerMaterial(GlobalVariables.Instance.bones, bonesEnabled, CrossSections.Instance.skeletalToggle);
        SetLayerMaterial(GlobalVariables.Instance.insertions, insertionsEnabled, CrossSections.Instance.skeletalToggle);
        SetLayerMaterial(GlobalVariables.Instance.muscles, muscularEnabled, CrossSections.Instance.muscularToggle);
        SetLayerMaterial(GlobalVariables.Instance.nerves, nervesEnabled, CrossSections.Instance.nervousToggle);
        SetLayerMaterial(GlobalVariables.Instance.lymphs, lymphsEnabled, CrossSections.Instance.lymphsToggle);
        SetLayerMaterial(GlobalVariables.Instance.viscera, visceraEnabled, CrossSections.Instance.visceralToggle);
        SetLayerMaterial(GlobalVariables.Instance.regions, skinEnabled, CrossSections.Instance.regionsToggle);

    }

    public bool HasKeyColor(string tag)
    {
        switch (tag)
        {
            case "Skeleton":
                return true;

            case "Insertions":
                return true;

            case "Muscles":
                return true;

            case "Nervous":
                return true;

            case "Lymph":
                return true;

            case "Visceral":
                return true;

            case "BodyParts":
                return true;
        }

        return false;
    }

    public float GetWeightByTag(string tag)
    {
        switch (tag)
        {
            case "Skeleton":
                return skeletonWeight;

            case "Insertions":
                return insertionsWeight;

            case "Muscles":
                return musclesWeight;

            case "Nervous":
                return nervousWeight;

            case "Lymph":
                return lymphsWeight;

            case "Visceral":
                return visceralWeight;

            case "BodyParts":
                return regionsWeight;
        }

        return 0.35f;
    }

    public bool IsActiveByTag(string tag)
    {
        switch (tag)
        {
            case "Skeleton":
                return bonesEnabled;

            case "Insertions":
                return insertionsEnabled;

            case "Muscles":
                return muscularEnabled;

            case "Nervous":
                return nervesEnabled;

            case "Lymph":
                return lymphsEnabled;

            case "Visceral":
                return visceraEnabled;

            case "BodyParts":
                return skinEnabled;
        }

        return false;
    }

    public void EnableByTag(string tag)
    {
        switch (tag)
        {
            case "Skeleton":
                ActionControl.Instance.AddCommand(new KeyColorCommand(SetBonesKeyColors), false);
                SetBonesKeyColors();
                break;

            case "Insertions":
                ActionControl.Instance.AddCommand(new KeyColorCommand(SetInsertionsKeyColor), false);
                SetInsertionsKeyColor();
                break;

            case "Muscles":
                ActionControl.Instance.AddCommand(new KeyColorCommand(SetMusclesKeyColors), false);
                SetMusclesKeyColors();
                break;

            case "Nervous":
                ActionControl.Instance.AddCommand(new KeyColorCommand(SetNervesKeyColors), false);
                SetNervesKeyColors();
                break;

            case "Lymph":
                ActionControl.Instance.AddCommand(new KeyColorCommand(SetLymphsKeyColors), false);
                SetLymphsKeyColors();
                break;

            case "Visceral":
                ActionControl.Instance.AddCommand(new KeyColorCommand(SetVisceraKeyColors), false);
                SetVisceraKeyColors();
                break;

            case "BodyParts":
                ActionControl.Instance.AddCommand(new KeyColorCommand(SetRegionsKeyColors), false);
                SetRegionsKeyColors();
                break;
        }

    }
}
