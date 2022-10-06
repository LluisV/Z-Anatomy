using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class Collections : MonoBehaviour
{
    public static Collections instance;
    public GameObject collectionsCanvas;
    public RectTransform collectionsGrid;
    public GameObject collectionPrefab;
    public CreateCollectionPanel createCollectionPanel;
    public RawImage createElementImage;
    public TMP_InputField collectionNameTMP;
    public GameObject cube;
    private List<CollectionElement> collections = new List<CollectionElement>();
    [HideInInspector]
    public List<CollectionElement> selectedCollections = new List<CollectionElement>();
    private Camera cam;
    private string folder;
    private int counter = 0; // image #
    public enum Format { RAW, JPG, PNG, PPM };
    public Format format = Format.JPG;
    [HideInInspector]
    public bool selecting = false;
    public Button deleteBtn;
    private string imagePath;

    private void Awake()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/z-anatomy/collections/"))
            Directory.CreateDirectory(Application.persistentDataPath + "/z-anatomy/collections/");
    }

    private void Start()
    {
        instance = this;
        cam = Camera.main;
        LoadCollections();
    }

    private CollectionElement AddCollectionToGrid(string name, Color backgroundColor, string[] elementsEnabled, string imagePath, string path)
    {
        GameObject newCollection = Instantiate(collectionPrefab, collectionsGrid);
        CollectionElement collectionScript = newCollection.GetComponent<CollectionElement>();
        collections.Add(collectionScript);
        newCollection.gameObject.name = name;
        collectionScript.backgroundImage.color = backgroundColor;
        collectionScript.nameTMP.text = name;
        collectionScript.collectionData = new CollectionElementData();
        collectionScript.backgroundImage.color = backgroundColor;
        collectionScript.collectionData.backgroundColorHex = ColorUtility.ToHtmlStringRGB(backgroundColor);
        collectionScript.collectionData.name = name;
        collectionScript.collectionData.elements = elementsEnabled;
        collectionScript.collectionData.imagePath = imagePath;
        collectionScript.collectionData.path = path;
        collectionScript.image.texture = StaticMethods.LoadPNG(imagePath);
        return collectionScript;
    }

    public void OpenCreateCollectionClick()
    {
        cube.SetActive(false);
        cam.GetComponent<CameraController>().CenterImmediate();
        SelectedObjectsManagement.Instance.DeselectAllObjects();
        StartCoroutine(TakeScreenshot());
    }

    public void CloseCreateCollectionClick()
    {
        createCollectionPanel.gameObject.SetActive(false);
        try
        {
            if (File.Exists(imagePath))
                File.Delete(imagePath);
        }
        catch
        {
            Debug.Log("Error deleting image");
        }

    }

    public void CreateButtonClick()
    {
        if (string.IsNullOrEmpty(collectionNameTMP.text.Trim()))
        {
            PopUpManagement.Instance.Show("Name can't be empty");
            return;
        }
        if(FindCollection(collectionNameTMP.text))
        {
            PopUpManagement.Instance.Show("This name is already in use");
            return;
        }
        string path = GetCollectionPath(collectionNameTMP.text);
        CollectionElement collectionScript = AddCollectionToGrid(collectionNameTMP.text, createCollectionPanel.backgroundImage.color, GetActiveObjects(), imagePath, path);
        SaveCollection(collectionScript.collectionData, path);
        createCollectionPanel.gameObject.SetActive(false);
    }

    private bool FindCollection(string name)
    {
        foreach (var item in collections)
        {
            if (item.name.Equals(name))
                return true;
        }
        return false;
    }

    private string[] GetActiveObjects()
    {
        return GlobalVariables.Instance.globalParent.GetComponentsInChildren<BodyPart>()
        .Where(it => it.gameObject.activeInHierarchy && !it.gameObject.transform.parent.name.Contains(".labels"))
        .Select(it => it.GetComponent<NameAndDescription>().originalName + it.GetComponent<NameAndDescription>().leftRight)
        .ToArray();
    }

    private void LoadCollections()
    {
        string filepath = Application.persistentDataPath + "/z-anatomy/collections";
        DirectoryInfo d = new DirectoryInfo(filepath);
        foreach (var item in d.GetFiles())
        {
            CollectionElementData element = LoadCollection(item.Name);
            if (element != null)
            {
                Color col;
                ColorUtility.TryParseHtmlString("#" + element.backgroundColorHex, out col);
                AddCollectionToGrid(element.name, col, element.elements, element.imagePath, element.path);
            }
        }
    }

    private CollectionElementData LoadCollection(string name)
    {
        try
        {
            string path = Application.persistentDataPath + "/z-anatomy/collections/" + name;
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                stream.Position = 0;

                CollectionElementData data = (CollectionElementData)formatter.Deserialize(stream);
                stream.Close();
                Debug.Log("Collection file " + "z-anatomy/collections/" + name + " was LOADED");
                return data;
            }
            else
            {
                Debug.Log("Collection file " + "z-anatomy/collections/" + name + " was not found");
                return null;
            }
        }
        catch(System.Exception e)
        {
            Debug.Log("Error loading " + "z-anatomy/collections/" + name + " Error: " + e.Message);
            return null;
        }
    }
    
    public void DeleteCollectionClick()
    {
        bool error = false;
        //Try delete files
        try
        {
            foreach (var item in selectedCollections)
            {
                File.Delete(item.collectionData.path);
                File.Delete(item.collectionData.imagePath);
            }

        }
        catch
        {
            error = true;
            if (selectedCollections.Count == 1)
                PopUpManagement.Instance.Show("The selected collection could not be deleted.");
            else
                PopUpManagement.Instance.Show("The selected collections could not be deleted.");
        }

        if (error)
            return;
        //If everything went well -> delete from grid
        foreach (var item in selectedCollections)
        {
            RemoveCollectionFromGrid(item);
            collections.Remove(item);
        }
        selectedCollections.Clear();
        PopUpManagement.Instance.Show("Collections removed!");

        SelectEnd();
    }

    private void RemoveCollectionFromGrid(CollectionElement collection)
    {
        Destroy(collectionsGrid.Find(collection.gameObject.name).gameObject);
    }

    private string GetCollectionPath(string name)
    {
        return Application.persistentDataPath + "/z-anatomy/collections/" + name;
    }

    private void SaveCollection(CollectionElementData elementData, string path)
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, elementData);
            stream.Close();
            PopUpManagement.Instance.Show("Collection created!");
        }
        catch
        {
            PopUpManagement.Instance.Show("Something went wrong. Collection was not saved.");
        }
    }

    private IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();
        Rect rect;
        RenderTexture renderTexture;
        Texture2D screenShot;

        // creates off-screen render texture that can rendered into
        rect = new Rect(0, 0, 1920, 1080);
        renderTexture = new RenderTexture(1920, 1080, 24);
        screenShot = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        cam.targetTexture = renderTexture;
        cam.Render();

        // read pixels will read from the currently active render texture so make our offscreen 
        // render texture active and then read the pixels
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(rect, 0, 0);
        screenShot = StaticMethods.ResampleAndCrop(screenShot, 300, 300);

        // reset active camera texture and render texture
        cam.targetTexture = null;
        RenderTexture.active = null;

        // get our unique filename
        imagePath = UniqueFilename((int)rect.width, (int)rect.height);
        // pull in our file header/data bytes for the specified image format (has to be done from main thread)
        byte[] fileHeader = null;
        byte[] fileData = null;


        fileData = screenShot.EncodeToJPG();

        bool done = false;
        new System.Threading.Thread(() =>
        {
            // create file and write optional header with image bytes
            var f = System.IO.File.Create(imagePath);
            if (fileHeader != null) f.Write(fileHeader, 0, fileHeader.Length);
            f.Write(fileData, 0, fileData.Length);
            f.Close();
            Debug.Log(string.Format("Wrote screenshot {0} of size {1}", imagePath, fileData.Length));
            done = true;
        }).Start();

        while (!done)
            yield return null;

        createElementImage.texture = StaticMethods.LoadPNG(imagePath);
        Destroy(renderTexture);
        renderTexture = null;
        screenShot = null;

        createCollectionPanel.gameObject.SetActive(true);
        cube.SetActive(true);
    }

    private string UniqueFilename(int width, int height)
    {

        // if folder not specified by now use a good default
        if (folder == null || folder.Length == 0)
        {
            folder = Application.persistentDataPath;
            if (Application.isEditor)
            {
                // put screenshots in folder above asset path so unity doesn't index the files
                var stringPath = folder + "/..";
                folder = Path.GetFullPath(stringPath);
            }
            folder += "/screenshots";

            // make sure directoroy exists
            System.IO.Directory.CreateDirectory(folder);

            // count number of files of specified format in folder
            string mask = string.Format("screen_{0}x{1}*.{2}", width, height, format.ToString().ToLower());
            counter = Directory.GetFiles(folder, mask, SearchOption.TopDirectoryOnly).Length;
        }

        // use width, height, and counter for unique file name
        var filename = string.Format("{0}/screen_{1}x{2}_{3}.{4}", folder, width, height, counter, format.ToString().ToLower());

        // up counter for next call
        ++counter;

        // return unique filename
        return filename;
    }

    public void SelectStart()
    {
        selecting = true;
        deleteBtn.interactable = true;
        foreach (var collection in collections)
        {
            collection.EnableSelection();
        }
    }

    public void SelectEnd()
    {
        selecting = false;
        deleteBtn.interactable = false;
        foreach (var collection in collections)
        {
            collection.DisableSelection();
        }
    }
}
