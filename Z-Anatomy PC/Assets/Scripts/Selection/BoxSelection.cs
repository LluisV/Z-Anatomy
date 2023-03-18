using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class BoxSelection : MonoBehaviour
{
    public Canvas nonScaledCanvas;
    public Material lineMat;
    public float lineWidth;
    public float lineLength;
    public float lineHeigth;
    public float lineRepetition;
    public float lineAnimSpeed;
    GameObject imgObj;
    Image img;
    RectTransform imgRect;
    Vector2 initialPos;
    Vector3 boxCenter;
    Vector3 boxSize;
    Camera cam;


    GameObject debugCube;
    Mesh mesh;
    ListOfColliders selectedObjects;
    public float minDistanceToSelect;

    public bool debug;

    private LineRenderer line;

    private bool blocked;

    // Start is called before the first frame update
    void Awake()
    {
        cam = Camera.main;
        debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        debugCube.transform.localScale = new Vector3(.001f, .001f, .001f);
        debugCube.GetComponent<MeshRenderer>().material.color = new Color(.25f, 0, 0, 1f);
        debugCube.AddComponent<BoxCollider>().isTrigger = true;
        debugCube.AddComponent<Rigidbody>().isKinematic = true;
        mesh = debugCube.GetComponent<MeshFilter>().mesh;
        if(debug)
            debugCube.layer = 6;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ActionControl.boxSelection || ActionControl.crossSectionsEnabled)
        {
            if(imgObj != null)
            {
                Destroy(imgObj);
                nonScaledCanvas.gameObject.SetActive(false);
            }
            return;
        }
            
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            blocked = EventSystem.current.IsPointerOverGameObject();
            initialPos = Mouse.current.position.ReadValue();
        }

        else if((Mouse.current.leftButton.isPressed) 
            && Vector2.Distance(initialPos, Mouse.current.position.ReadValue()) > minDistanceToSelect && !blocked)
        {
            if(imgObj == null)
            {
                selectedObjects = debugCube.AddComponent<ListOfColliders>();
                imgObj = new GameObject();
                imgRect = imgObj.AddComponent<RectTransform>();
                img = imgObj.AddComponent<Image>();
                img.color = new Color(1, 1, 1, .01f);
                imgObj.transform.SetParent(nonScaledCanvas.transform, false);
                imgRect.position = initialPos;
                line = imgObj.AddComponent<LineRenderer>();
                line.material = lineMat;
                line.startWidth = lineWidth;
                line.endWidth = lineWidth;
                line.positionCount = 8;
                line.loop = true;
                line.gameObject.layer = LayerMask.NameToLayer("UI");
                line.textureMode = LineTextureMode.Tile;
                line.sharedMaterial.SetFloat("_Heigth", lineHeigth);
                line.sharedMaterial.SetFloat("_Width", lineLength);
                line.sharedMaterial.SetFloat("_AnimSpeed", lineAnimSpeed);

                if (!Keyboard.current.leftCtrlKey.isPressed && !blocked)
                    SelectedObjectsManagement.Instance.DeselectAllObjects();
            }

            Vector2 mouseDirection = initialPos - Mouse.current.position.ReadValue();
            Vector2 newSize = mouseDirection / nonScaledCanvas.scaleFactor;
            newSize.x = Mathf.Abs(newSize.x);
            newSize.y = Mathf.Abs(newSize.y);

            if (newSize.magnitude < 2)
            {
                imgObj.SetActive(false);
                return;
            }

            imgObj.SetActive(true);

            imgRect.localScale = newSize / 100;
            float angle = Vector2.SignedAngle(Vector2.left, mouseDirection);
            if (angle > 0 && angle < 90)
                imgRect.pivot = new Vector2(0, 0);
            else if (angle > 90)
                imgRect.pivot = new Vector2(1, 0);
            else if (angle < 0 && angle > -90)
                imgRect.pivot = new Vector2(0, 1);
            else if(angle < -90)
                imgRect.pivot = new Vector2(1, 1);


            boxCenter = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue() + mouseDirection / 2);
            boxCenter = boxCenter + cam.transform.forward * cam.orthographicSize;

            boxSize = mouseDirection / nonScaledCanvas.scaleFactor * cam.aspect * cam.orthographicSize / 2100;
            boxSize.z = 20;

            TransformCube();

            InterpolatePoints();

            line.sharedMaterial.SetFloat("_Repeat", lineRepetition / cam.orthographicSize);

            line.startWidth = lineWidth * cam.orthographicSize;
            line.endWidth = lineWidth * cam.orthographicSize;

        }
        else if ((Mouse.current.leftButton.wasReleasedThisFrame) 
            && Vector2.Distance(initialPos, Mouse.current.position.ReadValue()) > minDistanceToSelect)
        {
            if (SelectedObjectsManagement.Instance.selectedObjects.Count > 0 && !blocked)
            {
                ActionControl.Instance.AddCommand(new SelectCommand(SelectedObjectsManagement.Instance.selectedObjects), false);
                ActionControl.Instance.UpdateButtons();
                if (SelectedObjectsManagement.Instance.selectedObjects.Count == 1)
                    SelectedObjectsManagement.Instance.ShowBodyPartInfo(SelectedObjectsManagement.Instance.selectedObjects[0]);
            }
            if(!blocked)
                TranslateObject.Instance.UpdateSelected();
            Lexicon.Instance.UpdateSelected();
            blocked = !EventSystem.current.IsPointerOverGameObject();
            Clear();
        }
    }

    private void TransformCube()
    {
        debugCube.transform.position = boxCenter;
        debugCube.transform.localScale = boxSize * 2;
        debugCube.transform.rotation = cam.transform.rotation;
    }

    public void ResetSelection()
    {
        //Pos aleatoria
        debugCube.transform.position = new Vector3();
        debugCube.transform.localScale = new Vector3(.001f, .001f, .001f);
    }

    private void InterpolatePoints()
    {
        line.SetPosition(0, debugCube.transform.TransformPoint(mesh.vertices[4]));
        line.SetPosition(2, debugCube.transform.TransformPoint(mesh.vertices[6]));
        line.SetPosition(4, debugCube.transform.TransformPoint(mesh.vertices[7]));
        line.SetPosition(6, debugCube.transform.TransformPoint(mesh.vertices[5]));

        //Interpolation
        line.SetPosition(1, (line.GetPosition(0) + line.GetPosition(2)) / 2);
        line.SetPosition(3, (line.GetPosition(2) + line.GetPosition(4)) / 2);
        line.SetPosition(5, (line.GetPosition(4) + line.GetPosition(6)) / 2);
        line.SetPosition(7, (line.GetPosition(6) + line.GetPosition(0)) / 2);
    }

    public void Clear()
    {
        if (selectedObjects != null)
            Destroy(selectedObjects);
        Destroy(imgObj);
    }

}
