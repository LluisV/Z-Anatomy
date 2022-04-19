using UnityEngine;
using UnityEngine.UI;

public class BrushSelection : MonoBehaviour
{
    public Canvas nonScaledCanvas;
    GameObject imgObj;
    Image img;
    RectTransform imgRect;
    Vector3 brushCenter;
    public Camera cam;

    GameObject debugCylinder;
    public Sprite brushSprite;

    public float diameter;

    Vector3 direction;

    private float currentHitDistance;
    int layer_mask1;
    LayerMask raycastMask;
    SelectedObjectsManagement selectionManagement;

    public Slider diameterSlider;

    // Start is called before the first frame update
    void Awake()
    {
        // cam = Camera.main;
        selectionManagement = GetComponent<SelectedObjectsManagement>();
        imgObj = new GameObject();

        imgRect = imgObj.AddComponent<RectTransform>();
        imgRect.pivot = new Vector2(0.5f, 0.5f);
        img = imgObj.AddComponent<Image>();
        img.color = new Color(1, 1, 1, .1f);
        img.sprite = brushSprite;
        img.raycastTarget = false;
        imgObj.transform.SetParent(nonScaledCanvas.transform, false);
        imgRect.localScale = new Vector3(diameter, diameter, 1);
        imgObj.SetActive(false);

        layer_mask1 = LayerMask.GetMask("Body");
        raycastMask = layer_mask1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ActionControl.brushSelection || ActionControl.twoFingerOnScreen)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            imgObj.SetActive(true);
        }
        if (Input.GetMouseButton(0))
        {
            brushCenter = cam.ScreenToWorldPoint(Input.mousePosition);
            brushCenter = brushCenter + cam.transform.forward * cam.orthographicSize;

            imgRect.position = Input.mousePosition;

            Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            direction = brushCenter - mouseWorldPosition;

            RaycastHit[] collisions = Physics.SphereCastAll(mouseWorldPosition, diameter / 14 * cam.orthographicSize, direction, 25, raycastMask);
            if (collisions.Length > 0 && !UIManager.draggingUI)
            {
                ActionControl.someObjectSelected = true;
                foreach (var collider in collisions)
                {
                    BodyPart script = collider.transform.GetComponent<BodyPart>();
                    if(script != null)
                    {
                        script.isSelected = true;
                        selectionManagement.SelectObject(collider.transform.gameObject);
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            imgObj.SetActive(false);
            SelectedObjectsManagement.instance.EnableDisableButtons();
        }
    }

    public void UpdateDiameter()
    {
        diameter = diameterSlider.value;
        imgRect.localScale = new Vector3(diameter, diameter, 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(cam.ScreenToWorldPoint(Input.mousePosition), brushCenter);
        Gizmos.DrawWireSphere(brushCenter, diameter / 14 * cam.orthographicSize);
    }

    public void HideImage()
    {
        imgObj.SetActive(false);
    }
}
