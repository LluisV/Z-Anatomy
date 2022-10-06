using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class SearchEngine : MonoBehaviour
{
    private HashSet<GameObject> gameObjectFound = new HashSet<GameObject>();
    public TMP_InputField mainInputField;
    public GameObject emptyStateScreen;
    public GameObject deleteSearchBtn;
    public GameObject loadingAnim;

    private MeshManagement meshManagement;
    Lexicon treeViewCanvas;
    public static bool onSearch;
    public static SearchEngine Instance;

    private IEnumerator valueChangeCoroutine;
    private IEnumerator startsWithCoroutine;
    private IEnumerator containsCoroutine;

    private Visibility[] allParts;

    private int search = 0;

    private void Awake()
    {
        Instance = this;
        allParts = GlobalVariables.Instance.globalParent.GetComponentsInChildren<Visibility>();
    }

    void Start()
    {
        mainInputField.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        meshManagement = GetComponent<MeshManagement>();
        treeViewCanvas = GetComponent<Lexicon>();
    }

    public void ValueChangeCheck()
    {
        gameObjectFound.Clear();
        StopAllCoroutines();

        deleteSearchBtn.SetActive(mainInputField.text.Length > 0);

        if (mainInputField.text.Length >= 3)
        {
            treeViewCanvas.ReturnToOrigin();
            emptyStateScreen.SetActive(false);
            valueChangeCoroutine = waitToSearch();
            StartCoroutine(valueChangeCoroutine);

            if (!PanelsManagement.instance.lexOnScreen)
                PanelsManagement.instance.ShowLexicon();

            IEnumerator waitToSearch()
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                onSearch = true;
                
                loadingAnim.SetActive(true);

                if (startsWithCoroutine != null)
                    StopCoroutine(startsWithCoroutine);
                if (containsCoroutine != null)
                    StopCoroutine(containsCoroutine);

                Lexicon.Instance.ClearAllElements();

                startsWithCoroutine = StartWithChild(mainInputField.text.ToLower());
                containsCoroutine = ContainsChild(mainInputField.text.ToLower());
                search = 0;

                yield return StartCoroutine(startsWithCoroutine);
                yield return StartCoroutine(containsCoroutine);

                emptyStateScreen.SetActive(treeViewCanvas.elements.Count == 0);

                loadingAnim.SetActive(false);

                treeViewCanvas.ReturnToOrigin();
                watch.Stop();
                Debug.Log("Elapsed time: " + watch.Elapsed);
            }

        }
        else if(mainInputField.text.Length < 3 && onSearch)
        {
            treeViewCanvas.ResetAll();
            onSearch = false;
            emptyStateScreen.SetActive(false);
            loadingAnim.SetActive(false);
        }
    }

    IEnumerator StartWithChild(string input)
    {
        int i = 0;
        foreach (var item in allParts)
        {
            string childN = item.name.ToLower().RemoveAccents();

            if (childN.StartsWith(input.RemoveAccents()))
            {
                if (!gameObjectFound.Contains(item.gameObject))
                {
                    gameObjectFound.Add(item.gameObject);
                    treeViewCanvas.AddElement(item.gameObject);
                }
            }
            else if(item.nameScript.HasSynonims())
            {
                foreach (var synonym in item.nameScript.allSynonyms[Settings.languageIndex])
                {
                    if (synonym.ToLower().RemoveAccents().StartsWith(input.RemoveAccents()))
                    {
                        if (!gameObjectFound.Contains(item.gameObject))
                        {
                            gameObjectFound.Add(item.gameObject);
                            treeViewCanvas.AddElement(item.gameObject, synonym);
                        }
                    }
                }
            }
            i++;
            if (i % 100 == 0)
                yield return null;
        }
        yield return null;
    }

    IEnumerator ContainsChild(string input)
    {
        int i = 0;
        foreach (var item in allParts)
        {
            string childN = item.name.ToLower().RemoveAccents();

            if (childN.Contains(input.RemoveAccents()))
            {
                if (!gameObjectFound.Contains(item.gameObject))
                {
                    gameObjectFound.Add(item.gameObject);
                    treeViewCanvas.AddElement(item.gameObject);
                }
            }
            else if (item.nameScript.HasSynonims())
            {
                foreach (var synonym in item.nameScript.allSynonyms[Settings.languageIndex])
                {
                    if (synonym.ToLower().RemoveAccents().Contains(input.RemoveAccents()))
                    {
                        if (!gameObjectFound.Contains(item.gameObject))
                        {
                            gameObjectFound.Add(item.gameObject);
                            treeViewCanvas.AddElement(item.gameObject, synonym);
                        }
                    }
                }
            }
            i++;
            if (i % 100 == 0)
                yield return null;
        }
        yield return null;

    }

    IEnumerator RecursiveStartWithChild(Transform parent, string input)
    {
        search += parent.childCount;

        foreach (Transform child in parent)
        {
            string childN = child.name.ToLower().RemoveAccents();

            if (child.childCount > 0)
            {
                StartCoroutine(RecursiveStartWithChild(child, input));
            }
            if (childN.StartsWith(input.RemoveAccents()))
            {
                if(!gameObjectFound.Contains(child.gameObject))
                {
                    gameObjectFound.Add(child.gameObject);
                    treeViewCanvas.AddElement(child.gameObject);
                }
            }

            yield return null;
        }
        search -= parent.childCount;
    }

    IEnumerator RecursiveContainsChild(Transform parent, string input)
    {
        search += parent.childCount;

        foreach (Transform child in parent)
        {
            string childN = child.name.ToLower().RemoveAccents();

            if (child.childCount > 0)
            {
                StartCoroutine(RecursiveContainsChild(child, input));
            }
            if (childN.Contains(input.RemoveAccents()))
            {
                if (!gameObjectFound.Contains(child.gameObject))
                {
                    gameObjectFound.Add(child.gameObject);
                    treeViewCanvas.AddElement(child.gameObject);
                }
            }

            yield return null;

        }

        search -= parent.childCount;

    }

    public void SetText(string text)
    {
        mainInputField.text = text;
    }

    public void ClearSearch()
    {
        mainInputField.text = "";
        emptyStateScreen.SetActive(false);
        loadingAnim.SetActive(false);
    }
}
