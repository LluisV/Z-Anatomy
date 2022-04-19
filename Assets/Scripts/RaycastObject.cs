using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class RaycastObject : MonoBehaviour
{
    private Vector3 firstMousePos = new Vector3();
    private GameObject objectSelected;
    private RaycastHit hit;
    int layer_mask1;
    int layer_mask2;
    int layer_mask3;
    LayerMask finalmask; 
    private EventSystem eventSys;
    private SelectedObjectsManagement selectionManagement;
    [HideInInspector]
    public bool raycastBlocked;
    public static RaycastObject instance;
    private Camera cam;
    public TextMeshProUGUI descriptionTMPro;
    private void Awake()
    {
        instance = this;
        cam = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        layer_mask1 = LayerMask.GetMask("Body");
        layer_mask2 = LayerMask.GetMask("Outline");
        layer_mask3 = LayerMask.GetMask("Cube");
        finalmask = layer_mask1 | layer_mask2 | layer_mask3;
        eventSys = EventSystem.current;
        selectionManagement = GetComponent<SelectedObjectsManagement>();
    }

    // Update is called once per frame
    void Update()
    {

       // If click on UI
        if ((Input.GetMouseButton(0) && eventSys.IsPointerOverGameObject()) || (Input.touchCount > 0 && eventSys.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) || raycastBlocked)
                return;
        

        //SELECTION ON CROSS PLANES

        if(ActionControl.crossSectionsEnabled)
        {
            if(Input.GetMouseButtonDown(0))
            {
                firstMousePos = Input.mousePosition;
            }
            else if(Input.GetMouseButtonUp(0) && firstMousePos == Input.mousePosition)
            {
                try
                {
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                    RaycastHit[] hits;
                    RaycastHit firstHit;
                    //Check individual hit
                    Physics.Raycast(ray, out firstHit, 100, finalmask);
                    //Invert ray direction to detect backfaces
                    ray.origin = ray.GetPoint(100);
                    ray.direction = -ray.direction;
                    hits = Physics.RaycastAll(ray, 100, finalmask);
                    GetFirstAfterPlane(hits, firstHit);
                }
                catch
                {
                    return;
                }
            }
        }




        else
        {
            //--------------------MOBILE------------
            if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
            {
                Ray raycast = cam.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(raycast, out hit, 25, finalmask))
                {
                    firstMousePos = Input.GetTouch(0).position;
                    objectSelected = hit.collider.gameObject;
                }
            }
            //Mouse up as button
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && firstMousePos == new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, firstMousePos.z))
            {
                if (LayerMask.LayerToName(objectSelected.layer).Equals("Cube"))
                {
                    CubeFace faceClicked = CubeBehaviour.instance.GetHitFace(hit);
                    CubeBehaviour.instance.SetCameraRotation(faceClicked);
                    if (ActionControl.crossSectionsEnabled)
                        CrossPlanesCube.instance.SetPlane(faceClicked);
                }
                else
                {
                    BodyPart bodyPartScript = objectSelected.GetComponent<BodyPart>();
                    Label labelScript = objectSelected.GetComponent<Label>();

                    if (bodyPartScript != null)
                        bodyPartScript.ObjectClicked();

                    if (labelScript != null)
                        labelScript.Click();
                }

            }

            //-------------------PC---------------

            if (Input.GetMouseButtonDown(0))
            {
                Ray raycast = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(raycast, out hit, 100, finalmask))
                {
                    firstMousePos = Input.mousePosition;
                    objectSelected = hit.collider.gameObject;
                }
            }
            //Mouse up as button
            else if (!GlobalVariables.instance.mobile && Input.GetMouseButtonUp(0) && firstMousePos == Input.mousePosition)
            {
                if (LayerMask.LayerToName(objectSelected.layer).Equals("Cube"))
                {
                    CubeFace faceClicked = CubeBehaviour.instance.GetHitFace(hit);
                    CubeBehaviour.instance.SetCameraRotation(faceClicked);
                    if (ActionControl.crossSectionsEnabled)
                        CrossPlanesCube.instance.SetPlane(faceClicked);
                }
                else
                {
                    BodyPart bodyPartScript = objectSelected.GetComponent<BodyPart>();
                    Label labelScript = objectSelected.GetComponent<Label>();

                    if (bodyPartScript != null)
                        bodyPartScript.ObjectClicked();

                    if (labelScript != null)
                        labelScript.Click();
                }
            }
        }
       
    }

    private void GetFirstAfterPlane(RaycastHit[] hits, RaycastHit firsthit)
    {
        if (hits == null || hits.Length == 0)
            return;

        BodyPart bodyPartScript = firsthit.collider.GetComponent<BodyPart>();
        if (bodyPartScript.isSelected)
        {
            bodyPartScript.ObjectClicked();
            return;
        }

        try
        {
            RaycastHit firstAfterPlane = firsthit;
            if (OnPlaneClick.instance.xPlane && firsthit.point.x > OnPlaneClick.instance.sagitalPlane.transform.position.x)
                firstAfterPlane = hits.Where(it => it.point.x < OnPlaneClick.instance.sagitalPlane.transform.position.x).OrderByDescending(it => it.point.x).First();
            else if (OnPlaneClick.instance.ixPlane && firsthit.point.x < OnPlaneClick.instance.sagitalPlane.transform.position.x)
                firstAfterPlane = hits.Where(it => it.point.x > OnPlaneClick.instance.sagitalPlane.transform.position.x).OrderBy(it => it.point.x).First();

            else if (OnPlaneClick.instance.zPlane && firsthit.point.y > OnPlaneClick.instance.transversalPlane.transform.position.y)
                firstAfterPlane = hits.Where(it => it.point.y < OnPlaneClick.instance.transversalPlane.transform.position.y).OrderByDescending(it => it.point.y).First();
            else if (OnPlaneClick.instance.izPlane && firsthit.point.y < OnPlaneClick.instance.transversalPlane.transform.position.y)
                firstAfterPlane = hits.Where(it => it.point.y > OnPlaneClick.instance.transversalPlane.transform.position.y).OrderBy(it => it.point.y).First();

            else if (OnPlaneClick.instance.yPlane && firsthit.point.z < OnPlaneClick.instance.frontalPlane.transform.position.z)
                firstAfterPlane = hits.Where(it => it.point.z > OnPlaneClick.instance.frontalPlane.transform.position.z).OrderBy(it => it.point.z).First();
            else if (OnPlaneClick.instance.iyPlane && firsthit.point.z > OnPlaneClick.instance.frontalPlane.transform.position.z)
                firstAfterPlane = hits.Where(it => it.point.z < OnPlaneClick.instance.frontalPlane.transform.position.z).OrderByDescending(it => it.point.z).First();

            bodyPartScript = firstAfterPlane.collider.GetComponent<BodyPart>();
            if (bodyPartScript != null)
                bodyPartScript.ObjectClicked();
        }
        catch
        {

        }


    }
}
