using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxSelection : MonoBehaviour
{
    public Canvas nonScaledCanvas;
    GameObject imgObj;
    Image img;
    RectTransform imgRect;
    Vector3 initialPos;
    Vector3 boxCentre;
    Vector3 boxSize;
    Camera cam;

    GameObject debugCube;
    ListOfColliders selectedObjects;
    public SelectedObjectsManagement selectionMng;

    // Start is called before the first frame update
    void Awake()
    {
        cam = Camera.main;
        debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        debugCube.transform.localScale = new Vector3(.001f, .001f, .001f);
        debugCube.GetComponent<MeshRenderer>().material.color = new Color(.25f, 0, 0, 1f);
        debugCube.AddComponent<BoxCollider>().isTrigger = true;
        debugCube.AddComponent<Rigidbody>().isKinematic = true; 
       // 
       // debugCube.layer = 6;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ActionControl.boxSelection || ActionControl.twoFingerOnScreen)
        {
            if(imgObj != null)
                Destroy(imgObj);
            return;
        }
            
        if(Input.GetMouseButtonDown(0))
        {
            selectedObjects = debugCube.AddComponent<ListOfColliders>();
            selectedObjects.selectedObjecstMng = selectionMng;
            imgObj = new GameObject();
            imgRect = imgObj.AddComponent<RectTransform>();
            img = imgObj.AddComponent<Image>();
            img.color = new Color(1, 1, 1, .1f);
            imgObj.transform.SetParent(nonScaledCanvas.transform, false);
            initialPos = Input.mousePosition;
            imgRect.position = initialPos;
            
        }
        if(Input.GetMouseButton(0))
        {
            Vector3 mouseDirection = initialPos - Input.mousePosition;
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


            boxCentre = cam.ScreenToWorldPoint(Input.mousePosition + mouseDirection / 2);
            boxCentre = boxCentre + cam.transform.forward * cam.orthographicSize;

            boxSize = mouseDirection / nonScaledCanvas.scaleFactor * cam.aspect * cam.orthographicSize / 2100;
            boxSize.z = 9;

            TransformCube();

            if (selectedObjects.currentCollisions.Count > 0)
                ActionControl.someObjectSelected = true;

        }
        if (Input.GetMouseButtonUp(0))
        {
            SelectedObjectsManagement.instance.EnableDisableButtons();
            Destroy(imgObj);
        }
    }

    private void TransformCube()
    {
        debugCube.transform.position = boxCentre;
        debugCube.transform.localScale = boxSize * 2;
        debugCube.transform.rotation = cam.transform.rotation;
    }

    public void ResetSelection()
    {
        //Pos aleatoria
        debugCube.transform.position = new Vector3();
        debugCube.transform.localScale = new Vector3(.001f, .001f, .001f);
    }
}
