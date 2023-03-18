using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Layers : MonoBehaviour
{
    public static Layers Instance;

    public TextAsset[] bonesLayers;
    public TextAsset[] ligamentsLayers;
    public TextAsset[] muscularLayers;
    public TextAsset[] arteriesLayers;
    public TextAsset[] veinsLayers;
    public TextAsset[] lymphsLayers;
    public TextAsset[] fasciaLayers;
    public TextAsset[] nervesLayers;
    public TextAsset[] visceralLayers;
    public TextAsset[] refsLayers;
    public TextAsset[] skinLayers;

    public List<GameObject>[] bonesLayersObjects;
    public List<GameObject>[] ligamentsLayersObjects;
    public List<GameObject>[] muscularLayersObjects;
    public List<GameObject>[] arteriesLayersObjects;
    public List<GameObject>[] veinsLayersObjects;
    public List<GameObject>[] lymphsLayersObjects;
    public List<GameObject>[] fasciaLayersObjects;
    public List<GameObject>[] nervesLayersObjects;
    public List<GameObject>[] visceralLayersObjects;
    public List<GameObject>[] refsLayersObjects;
    public List<GameObject>[] skinLayersObjects;

    [HideInInspector]
    public LexiconElement bonesLex;
    [HideInInspector]
    public LexiconElement ligamentsLex;
    [HideInInspector]
    public LexiconElement muscularLex;
    [HideInInspector]
    public LexiconElement veinsLex;
    [HideInInspector]
    public LexiconElement arteriesLex;
    [HideInInspector]
    public LexiconElement lymphsLex;
    [HideInInspector]
    public LexiconElement fasciaLex;
    [HideInInspector]
    public LexiconElement nervesLex;
    [HideInInspector]
    public LexiconElement visceralLex;
    [HideInInspector]
    public LexiconElement refsLex;
    [HideInInspector]
    public LexiconElement skinLex;


    [HideInInspector]
    public Slider bonesSlider;
    [HideInInspector]
    public Slider ligamentsSlider;
    [HideInInspector]
    public Slider muscularSlider;
    [HideInInspector]
    public Slider veinsSlider;
    [HideInInspector]
    public Slider arteriesSlider;
    [HideInInspector]
    public Slider lymphsSlider;
    [HideInInspector]
    public Slider fasciaSlider;
    [HideInInspector]
    public Slider nervesSlider;
    [HideInInspector]
    public Slider visceralSlider;
    [HideInInspector]
    public Slider refsSlider;
    [HideInInspector]
    public Slider skinSlider;

    private Dictionary<string, GameObject> allBodyParts;

    public bool isEnabled;

    private bool firstTime = true;

    private void Awake()
    {
        Instance = this;
    }

    public void ReadLayers()
    {
        if(isEnabled)
        {
            allBodyParts = new Dictionary<string, GameObject>();
            var bodyPartScripts = GlobalVariables.Instance.globalParent.GetComponentsInChildren<NameAndDescription>(true);

            foreach (var bodyPart in bodyPartScripts)
            {
                if(bodyPart != null)
                {
                    if (allBodyParts.ContainsKey(bodyPart.originalName))
                    {
                        Debug.Log("duplicated");
                        continue;
                    }
                    allBodyParts.Add(bodyPart.originalName, bodyPart.gameObject);
                }
            }

            ReadLayer(ref bonesLayersObjects, bonesLayers);
            ReadLayer(ref ligamentsLayersObjects, ligamentsLayers);
            ReadLayer(ref muscularLayersObjects, muscularLayers);
            ReadLayer(ref fasciaLayersObjects, fasciaLayers);
            ReadLayer(ref arteriesLayersObjects, arteriesLayers);
            ReadLayer(ref veinsLayersObjects, veinsLayers);
            ReadLayer(ref lymphsLayersObjects, lymphsLayers);
            ReadLayer(ref visceralLayersObjects, visceralLayers);
            ReadLayer(ref nervesLayersObjects, nervesLayers);
            ReadLayer(ref skinLayersObjects, skinLayers);
            ReadLayer(ref refsLayersObjects, refsLayers);

            bonesSlider.maxValue = bonesLayers.Length;

            if(firstTime)
                bonesSlider.value = bonesSlider.maxValue;

            ligamentsSlider.maxValue = ligamentsLayers.Length;
            muscularSlider.maxValue = muscularLayers.Length;
            fasciaSlider.maxValue = fasciaLayers.Length;
            arteriesSlider.maxValue = arteriesLayers.Length;
            veinsSlider.maxValue = veinsLayers.Length;
            lymphsSlider.maxValue = lymphsLayers.Length;
            visceralSlider.maxValue = visceralLayers.Length;
            nervesSlider.maxValue = nervesLayers.Length;
            skinSlider.maxValue = skinLayers.Length;
            refsSlider.maxValue = refsLayers.Length;

            firstTime = false;
        }
    }

    private void ReadLayer(ref List<GameObject>[] layerObjects, TextAsset[] texts)
    {
        layerObjects = new List<GameObject>[texts.Length];

        int i = 0;

        foreach (var layer in texts)
        {
            string[] parts = layer.text.Split('\n').Where(it => it.Length > 0).ToArray();
            layerObjects[i] = new List<GameObject>();

            for (int j = 1; j < parts.Length; j++)
            {
                var name = parts[j].Replace("\r", "");
                if (allBodyParts.ContainsKey(name))
                {
                    GameObject found = allBodyParts[name];
                    if (found != null)
                    {
                        allBodyParts.Remove(name);
                        layerObjects[i].Add(found.gameObject);
                    }
                    else
                        Debug.Log(parts[j]);
                }
                else if(allBodyParts.ContainsKey(name.RemoveSuffix()))
                {
                    GameObject found = allBodyParts[name.RemoveSuffix()];
                    if (found != null)
                    {
                        allBodyParts.Remove(name.RemoveSuffix());
                        layerObjects[i].Add(found.gameObject);
                    }
                    else
                        Debug.Log(parts[j]);
                }
                else
                    Debug.Log(parts[j]);
            }

            i++;
        }

    }

    private void ShowLayer(List<GameObject>[] layers, int layerNum)
    {
        if (!isEnabled)
            return;

        for (int i = 0; i < layerNum; i++)
        {
            foreach (var obj in layers[i])
            {
                BodyPartVisibility isVisibleScript = obj.GetComponent<BodyPartVisibility>();
                isVisibleScript.isVisible = true;
                obj.transform.SetActiveParentsRecursively(true, null);
                obj.SetActive(true);
            }
        }

        for (int i = layerNum; i < layers.Length; i++)
        {
            foreach (var obj in layers[i])
            {
                BodyPartVisibility isVisibleScript = obj.GetComponent<BodyPartVisibility>();
                isVisibleScript.isVisible = false;
                obj.SetActive(false);
            }
        }

        Lexicon.Instance.UpdateTreeViewCheckboxes(false);
    }

    public void SyncLayers()
    {
        if (!isEnabled)
            return;
        StopAllCoroutines();
        StartCoroutine(SyncAll());
    }

    IEnumerator SyncAll()
    {
        yield return new WaitForEndOfFrame();

        StartCoroutine(SyncLayer(bonesLayersObjects, bonesSlider));
        StartCoroutine(SyncLayer(ligamentsLayersObjects, ligamentsSlider));
        StartCoroutine(SyncLayer(muscularLayersObjects, muscularSlider));
        StartCoroutine(SyncLayer(fasciaLayersObjects, fasciaSlider));
        StartCoroutine(SyncLayer(arteriesLayersObjects, arteriesSlider));
        StartCoroutine(SyncLayer(veinsLayersObjects, veinsSlider));
        StartCoroutine(SyncLayer(lymphsLayersObjects, lymphsSlider));
        StartCoroutine(SyncLayer(visceralLayersObjects, visceralSlider));
        StartCoroutine(SyncLayer(nervesLayersObjects, nervesSlider));
        StartCoroutine(SyncLayer(skinLayersObjects, skinSlider));
        StartCoroutine(SyncLayer(refsLayersObjects, refsSlider));

        SelectedObjectsManagement.Instance.GetActiveObjects();

    }

    private IEnumerator SyncLayer(List<GameObject>[] layerObjects, Slider slider)
    {
        int maxValue = 0;

        List<int> empties = new List<int>();

        yield return new WaitForEndOfFrame();

        //Foreach layer
        for (int i = 0; i < layerObjects.Length; i++)
        {
            bool found = false;

            foreach (var obj in layerObjects[i])
            {
                if(obj.GetComponent<BodyPartVisibility>().isVisible)
                {
                    found = true;
                    maxValue = i + 1;
                    break;
                }
            }

            if (!found)
                empties.Add(i);

        }

        yield return new WaitForEndOfFrame();

        slider.SetValueWithoutNotify(maxValue);
       /* slider.value = maxValue;
        slider.SetEmpties(empties);
        slider.UpdateButtons(maxValue);*/
    }

    public void UpdateBones()
    {
        ShowLayer(bonesLayersObjects, Mathf.RoundToInt(bonesSlider.value));
    }

    public void UpdateLigaments()
    {
        ShowLayer(ligamentsLayersObjects, Mathf.RoundToInt(ligamentsSlider.value));
    }

    public void UpdateMuscles()
    {
        ShowLayer(muscularLayersObjects, Mathf.RoundToInt(muscularSlider.value));
    }

    public void UpdateFascia()
    {
        ShowLayer(fasciaLayersObjects, Mathf.RoundToInt(fasciaSlider.value));
    }

    public void UpdateArteries()
    {
        ShowLayer(arteriesLayersObjects, Mathf.RoundToInt(arteriesSlider.value));
    }

    public void UpdateVeins()
    {
        ShowLayer(veinsLayersObjects, Mathf.RoundToInt(veinsSlider.value));
    }

    public void UpdateLymphs()
    {
        ShowLayer(lymphsLayersObjects, Mathf.RoundToInt(lymphsSlider.value));
    }

    public void UpdateViscera()
    {
        ShowLayer(visceralLayersObjects, Mathf.RoundToInt(visceralSlider.value));
    }

    public void UpdateNerves()
    {
        ShowLayer(nervesLayersObjects, Mathf.RoundToInt(nervesSlider.value));
    }

    public void UpdateSkin()
    {
        ShowLayer(skinLayersObjects, Mathf.RoundToInt(skinSlider.value));
    }

    public void UpdateRefs()
    {
        ShowLayer(refsLayersObjects, Mathf.RoundToInt(refsSlider.value));
    }
}
