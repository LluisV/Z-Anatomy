using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public GameObject defaultCenter;
    [HideInInspector]
    public Vector3 cameraCenter;

    [HideInInspector]
    public GameObject defaultTarget;
    [HideInInspector]
    public float lastDistance = 10;
    [HideInInspector]
    public float distance = 10.0f;
    public float defaulDistance;
    [HideInInspector]
    public float xSpeed = 250.0f;
    [HideInInspector]
    public float ySpeed = 120.0f;
    [HideInInspector]
    public float yMinLimit = -90;
    [HideInInspector]
    public float yMaxLimit = 90;
    public float centerMovementVelocity = 1;
    public float cameraZoomAnimationTime = 100;

    float x = 0.0f;
    float y = 0.0f;
    float z = 0.0f;

    private EventSystem eventSys;

    [HideInInspector]
    public bool movingCenter;

    public Toggle movingCenterToggle;
    //MOBILE 

    public float zoomRate = 20.0f;
    public float zoomDampening = 5.0f;
    Camera cam;

    Vector2 lastMousePosition;
    private bool onRotateCoroutine;

    void Start()
    {
        cam = GetComponent<Camera>();
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        defaultTarget = target;
        cameraCenter = defaultCenter.transform.position;
        eventSys = EventSystem.current;
        distance = defaulDistance;
        lastMousePosition = cameraCenter;
    }


    float prevDistance;
    bool lerping;

    void Update()
    {
        //If dragging UI
        if (UIManager.draggingUI)
            return;
        //If click on UI
        if (Input.touchCount > 0 && eventSys.IsPointerOverGameObject(Input.GetTouch(0).fingerId) ||(Input.GetMouseButton(0) && eventSys.IsPointerOverGameObject()))
            return;

        if (distance < 0.005f) distance = 0.005f;
        if (distance > 10) distance = 10;
        //-------------------MOBILE------------------------//
        if (GlobalVariables.instance.mobile)
        {
            
            if (Input.touchCount == 2 && !lerping)
            {

                ActionControl.twoFingerOnScreen = true;

                Touch touchZero = Input.GetTouch(0);
                Vector2 touchZeroPreviousPosition = touchZero.position - touchZero.deltaPosition;
                Touch touchOne = Input.GetTouch(1);
                Vector2 touchOnePreviousPosition = touchOne.position - touchOne.deltaPosition;

                float prevTouchDeltaMag = (touchZeroPreviousPosition - touchOnePreviousPosition).magnitude;
                float TouchDeltaMag = (touchZero.position - touchOne.position).magnitude;
                float deltaMagDiff = prevTouchDeltaMag - TouchDeltaMag;

                //TWO FINGER MOVEMENT
                if (Mathf.Abs(deltaMagDiff) < 17)
                {
                    Vector3 A = cam.ScreenToWorldPoint(new Vector3(touchZero.position.x, touchZero.position.y, distance));
                    Vector3 B = cam.ScreenToWorldPoint(new Vector3(touchZeroPreviousPosition.x, touchZeroPreviousPosition.y, distance));
                    Vector3 fingerDirection = B - A;
                    //distance = (transform.position - cameraCenter).magnitude;
                    cameraCenter = Vector3.MoveTowards(cameraCenter, cameraCenter + fingerDirection, Time.deltaTime * 100);                
                    //cameraCenter += fingerDirection * centerMovementVelocity * Time.deltaTime * (Mathf.Sqrt(distance) + 20);
                    var rotation = Quaternion.Euler(y, x, 0);
                    SetCameraPosition(rotation);
                }
                //Zoom
                else 
                    distance += deltaMagDiff * Time.deltaTime * zoomRate * 0.0025f * Mathf.Abs(distance);
               
            }
            else
                ActionControl.twoFingerOnScreen = false;

            //ORBIT
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && !ActionControl.boxSelection && !ActionControl.brushSelection && !lerping)
            {
                Touch touchZero = Input.GetTouch(0);
                Vector2 touchZeroPreviousPosition = touchZero.position - touchZero.deltaPosition;
                Vector3 A = cam.ScreenToWorldPoint(new Vector3(touchZero.position.x, touchZero.position.y, distance));
                Vector3 B = cam.ScreenToWorldPoint(new Vector3(touchZeroPreviousPosition.x, touchZeroPreviousPosition.y, distance));
                Vector3 fingerDirection = B - A;
                if (movingCenter)
                {
                    distance = (transform.position - cameraCenter).magnitude;
                    cameraCenter += fingerDirection * centerMovementVelocity * Time.deltaTime * Mathf.Sqrt(distance) * 10;
                    var rotation = Quaternion.Euler(y, x, 0);
                    SetCameraPosition(rotation);
                }
                else
                {
                    UpdatePosition();
                }

            }
        }
        else
        {
            //----------------------PC--------------------------//

            if(Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                distance -= Input.GetAxis("Mouse ScrollWheel") * cam.orthographicSize;
                var rotation = Quaternion.Euler(y, x, 0);
                SetCameraPosition(rotation);
            }
            
            //MOVE
            if (Input.GetMouseButton(0) && !ActionControl.boxSelection && !ActionControl.brushSelection && !lerping)
            {
                Vector3 A = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
                Vector3 B = cam.ScreenToWorldPoint(new Vector3(lastMousePosition.x, lastMousePosition.y, distance));
                Vector3 mouseDirection = B - A;
                cameraCenter = Vector3.MoveTowards(cameraCenter, cameraCenter + mouseDirection, Time.deltaTime * 100);
                var rotation = Quaternion.Euler(y, x, 0);
                SetCameraPosition(rotation);
            }
            lastMousePosition = Input.mousePosition;
            //ROTATE
            if (Input.GetMouseButton(1) && !lerping)
            {
                var pos = Input.mousePosition;
                var dpiScale = 1f;
                if (Screen.dpi < 1) dpiScale = 1;
                if (Screen.dpi < 200) dpiScale = 1;
                else dpiScale = Screen.dpi / 200f;

                if (pos.x < 380 * dpiScale && Screen.height - pos.y < 250 * dpiScale) return;

                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                if (z != 0)
                {
                    if (y == 90)
                        x = -z;
                    else
                        x = z;
                    z = 0;
                }
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                y = ClampAngle(y, yMinLimit, yMaxLimit);
                var rotation = Quaternion.Euler(y, x, z);
                transform.rotation = rotation;
                SetCameraPosition(rotation);
            }
        }
    }

    private void LateUpdate()
    {
        cam.orthographicSize = distance;
    }

    private void UpdatePosition()
    {
        if(Input.touchCount > 0)
        {
            x += Input.GetTouch(0).deltaPosition.x * xSpeed * 0.0005f;
            if (z != 0)
            {
                if (y == 90)
                    x = -z;
                else
                    x = z;
                z = 0;
            }
            y -= Input.GetTouch(0).deltaPosition.y * ySpeed * 0.0005f;
            z = 0;
        }

        y = ClampAngle(y, yMinLimit, yMaxLimit);
        var rotation = Quaternion.Euler(y, x, z);
        transform.rotation = rotation;
        SetCameraPosition(rotation);
    }

    IEnumerator LerpCameraPos(float prevDistance, float newDistance)
    {
        lerping = true;
        var rot = Quaternion.Euler(y, x, 0);
        Vector3 po;
        Vector3 prevPosition = transform.position;
        float time = 0;
        while (time < cameraZoomAnimationTime)
        {
            float t = time / cameraZoomAnimationTime;
            //Smooth step
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            distance = Mathf.Lerp(prevDistance, newDistance, t);
            po = rot * new Vector3(0.0f, 0.0f, -distance - 0.5f) + cameraCenter;
            transform.position = Vector3.Lerp(prevPosition, po, t);
            time += Time.deltaTime;
            yield return null;
        }
        distance = newDistance;
        UpdatePosition();
        lerping = false;
    }
    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, -90, 90);
    }

    public void UpdateDistance(float newDistance)
    {
        if(!movingCenter)
            StartCoroutine(LerpCameraPos(distance, newDistance));
    }

    public void MovingCenter()
    {
        movingCenter = movingCenterToggle.isOn;
    }

    public void ResetCamera()
    {
        target = defaultTarget;
        cameraCenter = defaultCenter.transform.position;
        x = 0;
        y = 0;
        z = 0;
        distance = defaulDistance;
        cam.orthographicSize = distance;
        UpdatePosition();
    }

    public bool IsPointOnGameScreen(Vector2 point)
    {
        return !(point.x == 0 || point.y == 0 || point.x >= Screen.width - 1 || point.y >= Screen.height - 1);
    }

    public void SetCameraPosition(Quaternion rotation)
    {
        var position = rotation * new Vector3(0.0f, 0.0f, -distance - 0.5f) + cameraCenter;
        transform.position = position;
    }

    public void SetCameraRotation(Vector3 rot)
    {
        if(!onRotateCoroutine)
            StartCoroutine(LerpRotation(rot, 0.15f));
    }

    IEnumerator LerpRotation(Vector3 rot, float duration)
    {
        onRotateCoroutine = true;
        float time = 0;
        Quaternion startValue = transform.rotation;
        Quaternion endValue = Quaternion.Euler(rot.x, rot.y, 0);
        while (time < duration)
        {
            float t = time / duration;
            //Smooth step
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            transform.rotation = Quaternion.Lerp(startValue, endValue, t);
            time += Time.deltaTime;
            SetCameraPosition(transform.rotation);
            yield return null;
        }
        transform.rotation = endValue;
        SetCameraPosition(transform.rotation);
        x = rot.y;
        y = rot.x;
        z = 0;
        onRotateCoroutine = false;
    }

    public void CenterImmediate()
    {
        //List of selected scripts
        List<BodyPart> scripts = new List<BodyPart>();

        SelectedObjectsManagement.instance.GetActiveObjects();

        foreach (var script in SelectedObjectsManagement.instance.activeObjects)
        {
            scripts.Add(script.GetComponent<BodyPart>());
        }
        if (scripts.Count == 0)
            return;
        BodyPart first = scripts[0];

        //Calculate bounds for all objects
        Bounds bounds = new Bounds(first.center, first.bounds.size);

        for (int i = 1; i < scripts.Count; i++)
        {
            if (scripts[i] != null && scripts[i].bounds != null)
                bounds.Encapsulate(scripts[i].bounds);
        }

        cameraCenter = bounds.center;
        lastDistance = distance;
        float newDistance = bounds.extents.magnitude;
        if (newDistance > 7)
            newDistance = 7;
        distance = newDistance; 
        transform.position = new Vector3(0.0f, 0.0f, -distance - 0.5f) + cameraCenter;
        UpdatePosition();
    }

    public void CenterView(bool updateDistance)
    {
        //No objects deleted, no objects selected -> default center
        /*if (SelectedObjectsManagement.instance.selectedObjects.Count == 0 && SelectedObjectsManagement.instance.deletedObjects.Count == 0)
        {
            target = null;
            cameraCenter = defaultCenter.transform.position;
            lastDistance = distance;
            if(updateDistance)
                UpdateDistance(defaulDistance);
        }*/
        //No objets selected-> center is the medium point of all active objects
        if (SelectedObjectsManagement.instance.selectedObjects.Count == 0)// && SelectedObjectsManagement.instance.deletedObjects.Count > 0)
        {
            //List of selected scripts
            List<BodyPart> scripts = new List<BodyPart>();

            SelectedObjectsManagement.instance.GetActiveObjects();

            foreach (var script in SelectedObjectsManagement.instance.activeObjects)
            {
                scripts.Add(script.GetComponent<BodyPart>());
            }
            if (scripts.Count == 0)
                return;
            BodyPart first = scripts[0];

            //Calculate bounds for all objects
            Bounds bounds = new Bounds(first.center, first.bounds.size);

            for (int i = 1; i < scripts.Count; i++)
            {
                if(scripts[i] != null && scripts[i].bounds != null)
                    bounds.Encapsulate(scripts[i].bounds);
            }

            cameraCenter = bounds.center;
            lastDistance = distance;
            float newDistance = bounds.extents.magnitude * 1.5f;
            if (newDistance > 7)
                newDistance = 7;
            if (updateDistance)
                UpdateDistance(newDistance);
        }
        else if (SelectedObjectsManagement.instance.selectedObjects.Count == 1)
        {
            BodyPart script = SelectedObjectsManagement.instance.selectedObjects[0].GetComponent<BodyPart>();
            if (script == null)
                return;
            target = script.gameObject;
            cameraCenter = script.center;
            lastDistance = distance;
            float newDistance = script.distanceToCamera;
            if (newDistance > 7)
                newDistance = 7;
            if (updateDistance)
                UpdateDistance(newDistance);
        }
        //Some objects selected -> the center is the medium point of all selected objects
        else
        {
            //List of selected scripts
            List<BodyPart> scripts = new List<BodyPart>();

            foreach (var item in SelectedObjectsManagement.instance.selectedObjects)
            {
                BodyPart script = item.GetComponent<BodyPart>();
                if(script != null)
                    scripts.Add(script);
            }

            if (scripts.Count == 0)
                return;

            BodyPart first = scripts[0];

            //Calculate bounds for all objects
            Bounds bounds = new Bounds(first.center, first.bounds.size);

            for (int i = 1; i < scripts.Count; i++)
            {
                bounds.Encapsulate(scripts[i].bounds);
            }

            cameraCenter = bounds.center;
            lastDistance = distance;
            float newDistance = bounds.extents.magnitude * 1.5f;
            if (newDistance > 7)
                newDistance = 7;
            if (updateDistance)
                UpdateDistance(newDistance);
        }
    }
}
