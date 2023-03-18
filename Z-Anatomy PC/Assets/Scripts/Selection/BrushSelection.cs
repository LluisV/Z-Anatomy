using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class BrushSelection : MonoBehaviour
{
    public static BrushSelection instance;
    public Canvas nonScaledCanvas;
    GameObject imgObj;
    Image img;
    RectTransform imgRect;
    Vector3 brushCenter;

    public Material lineMat;
    public float lineWidth;
    public float lineLength;
    public float lineHeigth;
    public float lineRepetition;
    public float lineAnimSpeed;
    public int linePoints;
    public float minDistanceToSelect;

    private Camera cam;

    public Sprite brushSprite;

    public float diameter;

    Vector3 direction;

    int layer_mask1;
    int layer_mask2;
    LayerMask raycastMask;

    private LineRenderer line;

    private List<GameObject> selected;
    Vector3 initialPos;

    private bool blocked;

    // Start is called before the first frame update
    void Awake()
    {
        cam = Camera.main;

        instance = this;

        imgObj = new GameObject();

        imgRect = imgObj.AddComponent<RectTransform>();
        imgRect.pivot = new Vector2(0.5f, 0.5f);
        img = imgObj.AddComponent<Image>();
        img.color = new Color(1, 1, 1, .01f);
        img.sprite = brushSprite;
        img.raycastTarget = false;
        imgObj.transform.SetParent(nonScaledCanvas.transform, false);
        imgRect.localScale = new Vector3(diameter, diameter, 1);
        imgObj.SetActive(false);

        layer_mask1 = LayerMask.GetMask("Body");
        layer_mask2 = LayerMask.GetMask("HighlightedOutline");
        raycastMask = layer_mask1 | layer_mask2;


        line = imgObj.AddComponent<LineRenderer>();
        line.material = lineMat;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.loop = true;
        line.gameObject.layer = LayerMask.NameToLayer("UI");
        line.textureMode = LineTextureMode.Tile;
        line.positionCount = linePoints;
        line.sharedMaterial.SetFloat("_Heigth", lineHeigth);
        line.sharedMaterial.SetFloat("_Width", lineLength);
        line.sharedMaterial.SetFloat("_AnimSpeed", lineAnimSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (!ActionControl.brushSelection || ActionControl.crossSectionsEnabled)
        {
            if (imgObj.activeInHierarchy)
            {
                imgObj.SetActive(false);
                nonScaledCanvas.gameObject.SetActive(false);
            }
            return;
        }


        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            blocked = EventSystem.current.IsPointerOverGameObject();
            selected = new List<GameObject>();
            initialPos = Mouse.current.position.ReadValue();
        }
        else if ((Mouse.current.leftButton.isPressed) && Vector2.Distance(initialPos, Mouse.current.position.ReadValue()) > minDistanceToSelect && !blocked)
        {
            if(!imgObj.activeInHierarchy)
            {
                imgObj.SetActive(true);
                if (!Keyboard.current.leftCtrlKey.isPressed && !blocked)
                    SelectedObjectsManagement.Instance.DeselectAllObjects();
            }

            var worldMousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            brushCenter = worldMousePos;

            Vector3 A = worldMousePos - cam.transform.forward * 10f;
            Vector3 B = worldMousePos + cam.transform.forward * 10f;

            imgRect.position = Mouse.current.position.ReadValue();

            direction = B - A;

            RaycastHit[] collisions = Physics.SphereCastAll(A, diameter / 14 * cam.orthographicSize, direction, 30, raycastMask);
            if (collisions.Length > 0)
            {
                foreach (var collider in collisions)
                {
                    TangibleBodyPart script = collider.transform.GetComponent<TangibleBodyPart>();
                    if(script != null)
                    {
                        script.SetSelectionState(true);
                        SelectedObjectsManagement.Instance.SelectObject(collider.transform.gameObject);
                        selected.Add(collider.transform.gameObject);
                    }
                }
            }

            line.sharedMaterial.SetFloat("_Repeat", lineRepetition / cam.orthographicSize);

            line.startWidth = lineWidth * cam.orthographicSize;
            line.endWidth = lineWidth * cam.orthographicSize;

            SetPoints(worldMousePos - cam.transform.forward * 20f);
        }
        else if ((Mouse.current.leftButton.wasReleasedThisFrame) 
            && imgObj.activeInHierarchy 
            && Vector2.Distance(initialPos, Mouse.current.position.ReadValue()) > minDistanceToSelect)
        {
            if(!blocked)
            {
                imgObj.SetActive(false);
                ActionControl.Instance.UpdateButtons();
                if (SelectedObjectsManagement.Instance.selectedObjects.Count > 0)
                    ActionControl.Instance.AddCommand(new SelectCommand(SelectedObjectsManagement.Instance.selectedObjects), false);
                if (SelectedObjectsManagement.Instance.selectedObjects.Count == 1)
                    SelectedObjectsManagement.Instance.ShowBodyPartInfo(SelectedObjectsManagement.Instance.selectedObjects[0]);
                TranslateObject.Instance.UpdateSelected();
                Lexicon.Instance.UpdateSelected();

            }
            blocked = !EventSystem.current.IsPointerOverGameObject();
        }
    }

    private void SetPoints(Vector3 center)
    {
        float angle = 360 / linePoints * Mathf.Deg2Rad;
        for (int i = 0; i < linePoints; i++)
        {
            Vector3 newPos = center + cam.transform.rotation * new Vector3(Mathf.Cos(angle * i), Mathf.Sin(angle * i)) * cam.orthographicSize * diameter / 13;
            line.SetPosition(i, newPos);
        }

    }

    public void UpdateDiameter()
    {
        if (diameter > 2)
            diameter = 2;
        if (diameter < 0.25f)
            diameter = 0.25f;
        imgObj.SetActive(true);
        brushCenter = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        brushCenter = brushCenter + cam.transform.forward * cam.orthographicSize;

        line.sharedMaterial.SetFloat("_Repeat", lineRepetition / cam.orthographicSize);

        line.startWidth = lineWidth * cam.orthographicSize;
        line.endWidth = lineWidth * cam.orthographicSize;

        SetPoints(cam.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        imgRect.position = Mouse.current.position.ReadValue();
        imgRect.localScale = new Vector3(diameter, diameter, 1);
    }

    private void OnDrawGizmos()
    {
        if (cam == null)
            cam = Camera.main;

        Gizmos.color = Color.red;
        Debug.DrawLine(cam.ScreenToWorldPoint(Mouse.current.position.ReadValue()), brushCenter);
        Gizmos.DrawWireSphere(brushCenter, diameter / 14 * cam.orthographicSize);
    }

    public void HideImage()
    {
        imgObj.SetActive(false);
    }
}
