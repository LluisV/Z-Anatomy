using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class Notes : MonoBehaviour
{
    public static Notes instance;

    private RaycastHit hit;
    int layer_mask1;
    int layer_mask2;
    int layer_mask3;
    LayerMask finalmask;

    public Canvas canvas;
    public NoteGizmo gizmoPrefab;
    public GameObject notePrefab;

    [HideInInspector]
    public bool gizmoPlaced = false;
    private NoteGizmo currentGizmo;

    private Line3D currentLine;
    [SerializeField] private Line3D linePrefab;

    TangibleBodyPart clickedBp;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        layer_mask1 = LayerMask.GetMask("Body");
        layer_mask2 = LayerMask.GetMask("Outline");
        layer_mask3 = LayerMask.GetMask("HighlightedOutline");
        finalmask = layer_mask1 | layer_mask2 | layer_mask3;
        currentGizmo = Instantiate(gizmoPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        if (!ActionControl.creatingLocalNote && !ActionControl.creatingGlobalNote)
            return;

        if(!gizmoPlaced)
        {
            Ray raycast = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(raycast, out hit, 100, finalmask))
            {
                clickedBp = hit.transform.GetComponent<TangibleBodyPart>();
                if(clickedBp != null && (ActionControl.creatingGlobalNote || clickedBp == ContextualMenu.Instance.contextObject))
                {
                    currentGizmo.hit = hit;
                    currentGizmo.gameObject.SetActive(true);
                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        gizmoPlaced = true;
                        currentLine = Instantiate(linePrefab, hit.point, Quaternion.identity);
                        currentLine.lineRenderer.positionCount = 2;
                        currentLine.lineRenderer.SetPosition(0, hit.point);
                    }
                }
                else
                    currentGizmo.gameObject.SetActive(false);
            }
            else if(!gizmoPlaced)
                currentGizmo.gameObject.SetActive(false);
        }
        else if(gizmoPlaced)
        {
            currentLine.lineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            if (Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
            {
                currentGizmo.placed = true;
                currentGizmo.note = CreateNote();
                
                currentGizmo = Instantiate(currentGizmo);
                currentGizmo.placed = false;
                currentGizmo.gameObject.SetActive(false);

                gizmoPlaced = false;
                StartCoroutine(WaitForRaycast());
                IEnumerator WaitForRaycast()
                {
                    yield return null;
                    ActionControl.creatingLocalNote = false;
                    ActionControl.creatingGlobalNote = false;
                    CameraController.instance.raycaster.enabled = true;
                }
            }
        }

    }


    private Note CreateNote()
    {
        GameObject noteGo = Instantiate(notePrefab, canvas.transform);
        Note note = noteGo.GetComponent<Note>();
        note.line = currentLine;
        note.gizmo = currentGizmo;
        noteGo.transform.position = Mouse.current.position.ReadValue();
        clickedBp.AddNote(note);
        return note;
    }
}
