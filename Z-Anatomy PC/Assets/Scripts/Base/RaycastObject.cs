using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// Detects clicks on objects and acts accordingly.
/// </summary>
public class RaycastObject : MonoBehaviour
{
    public static RaycastObject instance;

    public float clickRadius;
    private Canvas canvas;
    public TextMeshProUGUI highlightText;
    public Vector2 mouseTextOffset;

    [HideInInspector]
    public TangibleBodyPart bodyPartScript;
    [HideInInspector]
    public RaycastHit hit;
    [HideInInspector]
    public bool raycastBlocked;

    private Camera cam;
    private RectTransform highlightTextRt;
    private TangibleBodyPart prevbodyPartScript;
    private RaycastHit[] hits;
    private Vector2 firstMousePos = new Vector2();
    private GameObject objectSelected;

    private int layer_mask1;
    private int layer_mask2;
    private int layer_mask3;
    private int layer_mask4;
    private int layer_mask5;
    private LayerMask finalmask;

    private void Awake()
    {
        instance = this;
        cam = Camera.main;
        highlightTextRt = highlightText.GetComponent<RectTransform>();
        canvas = highlightText.GetComponentInParent<Canvas>();
    }

    void Start()
    {
        layer_mask1 = LayerMask.GetMask("Body");
        layer_mask2 = LayerMask.GetMask("Outline");
        layer_mask3 = LayerMask.GetMask("HighlightedOutline");
        layer_mask4 = LayerMask.GetMask("Cube");
        layer_mask5 = LayerMask.GetMask("Gizmo");
        finalmask = layer_mask1 | layer_mask2 | layer_mask3 | layer_mask4 | layer_mask5;
    }

    void Update()
    {
        // If click on UI
        if (raycastBlocked || EventSystem.current.IsPointerOverGameObject())
        {
            highlightText.text = "";
            return;
        }

        if (ActionControl.crossSectionsEnabled)
            HighlightWithPlane();
        else if (!ActionControl.creatingLocalNote)
            Highlight();
        Select();
        ShowContextualMenu();
        
    }

    private void LateUpdate()
    {
        highlightTextRt.position = Mouse.current.position.ReadValue() + mouseTextOffset * canvas.scaleFactor;
    }

    ///<summary>
    ///Returns the first BodyPart hit by the given RaycastHits array, that is after a plane, depending on the active Cross Section
    ///</summary>
    ///<param name="hits">Array of RaycastHits obtained from a raycast</param>
    ///<param name="firsthit">The first RaycastHit to start the search</param>
    ///<returns>The first BodyPart hit by the RaycastHits array after the active plane, null if hits is null or empty or if there's an error</returns>
    private TangibleBodyPart GetFirstAfterPlane(RaycastHit[] hits, RaycastHit firsthit)
    {
        if (hits == null || hits.Length == 0)
            return null ;

        TangibleBodyPart bodyPartScript = firsthit.collider.GetComponent<TangibleBodyPart>();

        try
        {
            RaycastHit firstAfterPlane = firsthit;

            if (CrossSections.Instance.xPlane && firsthit.point.x > CrossSections.Instance.sagitalPlane.transform.position.x)
            {
                var collisions = hits.Where(it => it.point.x < CrossSections.Instance.sagitalPlane.transform.position.x).OrderByDescending(it => it.point.x);
                var last = hits.Where(it => it.point.x >= CrossSections.Instance.sagitalPlane.transform.position.x && !it.transform.CompareTag("Untagged") && !CrossSections.Instance.IsEnabledByTag(it.transform.tag)).OrderByDescending(it => it.point.x);
                if (last.Count() > 0)
                    firstAfterPlane = last.First();
                else
                    firstAfterPlane = collisions.First();

            }
            else if (CrossSections.Instance.ixPlane && firsthit.point.x < CrossSections.Instance.sagitalPlane.transform.position.x)
            {
                var collisions = hits.Where(it => it.point.x > CrossSections.Instance.sagitalPlane.transform.position.x).OrderBy(it => it.point.x);
                var last = hits.Where(it => it.point.x <= CrossSections.Instance.sagitalPlane.transform.position.x && !it.transform.CompareTag("Untagged") && !CrossSections.Instance.IsEnabledByTag(it.transform.tag)).OrderBy(it => it.point.x);
                if (last.Count() > 0)
                    firstAfterPlane = last.First();
                else
                    firstAfterPlane = collisions.First();
            }

            else if (CrossSections.Instance.zPlane && firsthit.point.y > CrossSections.Instance.transversalPlane.transform.position.y)
            {
                var collisions = hits.Where(it => it.point.y < CrossSections.Instance.transversalPlane.transform.position.y).OrderByDescending(it => it.point.y);
                var last = hits.Where(it => it.point.y >= CrossSections.Instance.transversalPlane.transform.position.y && !it.transform.CompareTag("Untagged") && !CrossSections.Instance.IsEnabledByTag(it.transform.tag)).OrderByDescending(it => it.point.y);
                if (last.Count() > 0)
                    firstAfterPlane = last.First();
                else
                    firstAfterPlane = collisions.First();
            }
            else if (CrossSections.Instance.izPlane && firsthit.point.y < CrossSections.Instance.transversalPlane.transform.position.y)
            {
                var collisions = hits.Where(it => it.point.y > CrossSections.Instance.transversalPlane.transform.position.y).OrderBy(it => it.point.y);
                var last = hits.Where(it => it.point.y <= CrossSections.Instance.transversalPlane.transform.position.y && !it.transform.CompareTag("Untagged") && !CrossSections.Instance.IsEnabledByTag(it.transform.tag)).OrderBy(it => it.point.y);

                if (last.Count() > 0)
                    firstAfterPlane = last.First();
                else
                    firstAfterPlane = collisions.First();
            }

            else if (CrossSections.Instance.yPlane && firsthit.point.z < CrossSections.Instance.frontalPlane.transform.position.z)
            {
                var collisions = hits.Where(it => it.point.z > CrossSections.Instance.frontalPlane.transform.position.z).OrderBy(it => it.point.z);
                var last = hits.Where(it => it.point.z <= CrossSections.Instance.frontalPlane.transform.position.z && !it.transform.CompareTag("Untagged") && !CrossSections.Instance.IsEnabledByTag(it.transform.tag)).OrderBy(it => it.point.z);

                if (last.Count() > 0)
                    firstAfterPlane = last.First();
                else
                    firstAfterPlane = collisions.First();
            }
            else if (CrossSections.Instance.iyPlane && firsthit.point.z > CrossSections.Instance.frontalPlane.transform.position.z)
            {
                var collisions = hits.Where(it => it.point.z < CrossSections.Instance.frontalPlane.transform.position.z).OrderByDescending(it => it.point.z);
                var last = hits.Where(it => it.point.z >= CrossSections.Instance.frontalPlane.transform.position.z && !it.transform.CompareTag("Untagged") && !CrossSections.Instance.IsEnabledByTag(it.transform.tag)).OrderByDescending(it => it.point.z);

                if (last.Count() > 0)
                    firstAfterPlane = last.First();
                else
                    firstAfterPlane = collisions.First();
            }

            bodyPartScript = firstAfterPlane.collider.GetComponent<TangibleBodyPart>();
            return bodyPartScript;
        }
        catch
        {
            return null;
        }


    }

    /// <summary>
    /// Function to handle the case where the user clicked the empty space of the 3D View.
    /// </summary>
    private void ClickedNull()
    {
        if (ActionControl.creatingLocalNote)
            return;

        if (!ActionControl.boxSelection && !ActionControl.brushSelection && !ActionControl.multipleSelection)
            ActionControl.Instance.CollapseAll();
        if (SelectedObjectsManagement.Instance.selectedObjects.Count > 0)
        {
            SelectedObjectsManagement.Instance.DeselectAllObjects(false);
            ActionControl.Instance.AddCommand(new SelectCommand(new List<GameObject>()), false);
        }
        CameraController.instance.SetTarget(null);
        NamesManagement.Instance.SetDesc(NamesManagement.NO_SELECTION);
        SelectedObjectsManagement.Instance.lastParentSelected = null;
        ActionControl.Instance.UpdateButtons();
        TranslateObject.Instance.SetGizmoCenter();
    }

    /// <summary>
    /// Handles the object selection.
    /// </summary>
    private void Select()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            firstMousePos = Mouse.current.position.ReadValue();
        }
        //Mouse up as button
        else if ((Mouse.current.leftButton.wasReleasedThisFrame) 
            && Vector3.Distance(firstMousePos, Mouse.current.position.ReadValue()) < 10f)
        {
            //If clicked 'nothing'
            if (objectSelected == null)
                ClickedNull();

            else if (LayerMask.LayerToName(objectSelected.layer).Equals("Cube"))
            {
                GizmoFace faceClicked = GizmoBehaviour.instance.GetHitFace(hit);
                GizmoBehaviour.instance.SetCameraRotation(faceClicked);
                if (ActionControl.crossSectionsEnabled)
                    CrossPlanesGizmo.Instance.SetPlane(faceClicked);
            }
            else if(!ActionControl.creatingLocalNote)
            {
                Label labelScript = objectSelected.GetComponent<Label>();

                if (bodyPartScript != null)
                    bodyPartScript.ObjectClicked();

                if (labelScript != null)
                    labelScript.Click();
            }
            objectSelected = null;
        }
    }

    /// <summary>
    /// Casts a ray from the mouse position and highlights the object hit.
    /// </summary>
    private void Highlight()
    {
        var mousePos = Mouse.current.position.ReadValue();
        var worldMousePos = cam.ScreenToWorldPoint(mousePos);

        Ray centarRay = cam.ScreenPointToRay(mousePos);
        bool raycastHit = Physics.Raycast(centarRay, out hit, 100, finalmask);

        bool sphereHit = false;

        if(!raycastHit)
        {
            Vector3 A = worldMousePos - cam.transform.forward * 10f;
            Vector3 B = worldMousePos + cam.transform.forward * 10f;
            Vector3 direction = B - A;
            sphereHit = Physics.SphereCast(A, clickRadius / 14 * cam.orthographicSize, direction, out hit, 30, finalmask);
        }

        if (raycastHit || sphereHit)
        {
            objectSelected = hit.transform.gameObject;

            bodyPartScript = objectSelected.GetComponent<TangibleBodyPart>();

            if (prevbodyPartScript != null && bodyPartScript != prevbodyPartScript)
                prevbodyPartScript.MouseExit();

            prevbodyPartScript = bodyPartScript;

            if (bodyPartScript != null)
            {
                bodyPartScript.MouseEnter();
                if (ActionControl.nameOnMouse) //|| Shortcuts.Instance.IsShortcutPressed(Shortcuts.Instance.showMouseName))
                    highlightText.text = bodyPartScript.nameScript.name;
                else
                    highlightText.text = "";
            }
            else
                highlightText.text = "";
        }
        else
        {
            if (bodyPartScript != null)
            {
                bodyPartScript.MouseExit();
                highlightText.text = "";
                bodyPartScript = null;
            }
            objectSelected = null;
        }
    }

    /// <summary>
    /// Casts a ray from the mouse position and highlights the object hit after a cross section.
    /// </summary>
    private void HighlightWithPlane()
    {
        try
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            //Check individual hit
            Physics.Raycast(ray, out hit, 100, finalmask);
            //Invert ray direction to detect backfaces
            ray.origin = ray.GetPoint(100);
            ray.direction = -ray.direction;
            hits = Physics.RaycastAll(ray, 100, finalmask);

            if(hits.Length > 0)
            {

                objectSelected = GetFirstAfterPlane(hits, hit).gameObject;
                bodyPartScript = objectSelected.GetComponent<TangibleBodyPart>();


                if (prevbodyPartScript != null && bodyPartScript != prevbodyPartScript)
                    prevbodyPartScript.MouseExit();

                prevbodyPartScript = bodyPartScript;

                if (bodyPartScript != null)
                {
                    bodyPartScript.MouseEnter();
                    if (ActionControl.nameOnMouse )//|| Shortcuts.Instance.IsShortcutPressed(Shortcuts.Instance.showMouseName))
                        highlightText.text = bodyPartScript.nameScript.name;
                    else
                        highlightText.text = "";
                }
                else
                    highlightText.text = "";
            }
            else
            {
                if (bodyPartScript != null)
                {
                    bodyPartScript.MouseExit();
                    highlightText.text = "";
                    bodyPartScript = null;
                }
                objectSelected = null;
            }

        }
        catch
        {
            return;
        }
    }

    /// <summary>
    /// Displays a contextual menu when the right mouse button is clicked and released.
    /// </summary>
    private void ShowContextualMenu()
    {
        if(Mouse.current.rightButton.wasPressedThisFrame && (bodyPartScript != null || SelectedObjectsManagement.Instance.selectedObjects.Count > 0))
        {
            firstMousePos = Mouse.current.position.ReadValue();
            if (bodyPartScript != null)
                ContextualMenu.Instance.contextObject = bodyPartScript.gameObject;
            else
                ContextualMenu.Instance.contextObject = null;
        }
        else if (Mouse.current.rightButton.wasReleasedThisFrame && Vector3.Distance(firstMousePos, Mouse.current.position.ReadValue()) < 10f)
        {
            ContextualMenu.Instance.Show();
        }
    }

}
