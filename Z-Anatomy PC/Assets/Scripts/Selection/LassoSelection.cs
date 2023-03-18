using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class LassoSelection : MonoBehaviour
{
    public static LassoSelection instance;

    private Camera _cam;
    [SerializeField] private Line3D _linePrefab;

    private Line3D _currentLine;

    int[] tris;
    Vector3[] edges;


    Rigidbody rb;
    MeshCollider col;

    //Debug
    MeshFilter mf;
    MeshRenderer mr;

    public bool debug;

    private bool blocked;
    private bool firstFrame;

    void Awake()
    {
        instance = this;
        _cam = Camera.main;
    }


    void Update()
    {

        if (!ActionControl.lassoSelection || ActionControl.crossSectionsEnabled)
            return;
 

        Vector3 mousePos = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            blocked = EventSystem.current.IsPointerOverGameObject();

            _currentLine = Instantiate(_linePrefab, mousePos, Quaternion.identity);
            _currentLine.lineRenderer.enabled = true;
            SetUpLine();
            firstFrame = true;
        }

        else if (Mouse.current.leftButton.isPressed && !blocked)
        {
            if(firstFrame)
            {
                firstFrame = false;
                if (!Keyboard.current.leftCtrlKey.isPressed && !blocked)
                    SelectedObjectsManagement.Instance.DeselectAllObjects();
            }
            _currentLine.AddPoint(mousePos - _cam.transform.forward * 20f);
        }

        else if (Mouse.current.leftButton.wasReleasedThisFrame && _currentLine != null)
        {
            if(!blocked)
            {
                int length = _currentLine.lineRenderer.positionCount;

                if (length > 2)
                {
                    _currentLine.gameObject.AddComponent<ListOfColliders>().triggerExitEnabled = false;

                    //Create mesh
                    edges = new Vector3[length * 2];
                    _currentLine.lineRenderer.GetPositions(edges);
                    for (int i = 0; i < length; i++)
                        edges[i] = edges[i] - Camera.main.transform.forward;
                    tris = CreateTris();

                    //Create mesh clone
                    for (int i = 0; i < length; i++)
                        edges[i + length] = edges[i] + Camera.main.transform.forward * 25;
                    tris = CreateTris();

                    Mesh m = CreateMesh(edges, tris);

                    //Debug
                    if (debug)
                        mf.mesh = m;

                }

                _currentLine.lineRenderer.enabled = false;

                StartCoroutine(SelectObjects());
            }
            firstFrame = false;
            blocked = !EventSystem.current.IsPointerOverGameObject();
        }
    }

    IEnumerator SelectObjects()
    {
        yield return new WaitForFixedUpdate();

        if (SelectedObjectsManagement.Instance.selectedObjects.Count > 0)
        {
            ActionControl.Instance.AddCommand(new SelectCommand(SelectedObjectsManagement.Instance.selectedObjects), false);
            ActionControl.Instance.UpdateButtons();
        }

        TranslateObject.Instance.UpdateSelected();
        ActionControl.Instance.UpdateButtons();
        Lexicon.Instance.UpdateSelected();

        Clear();
    }

    private void SetUpLine()
    {
        //Debug
        if (debug)
        {
            mf = _currentLine.gameObject.AddComponent<MeshFilter>();
            mr = _currentLine.gameObject.AddComponent<MeshRenderer>();
        }

        rb = _currentLine.gameObject.AddComponent<Rigidbody>();
        col = _currentLine.gameObject.AddComponent<MeshCollider>();
        rb.isKinematic = true;
        col.convex = true;
        col.isTrigger = true;
    }

    Mesh CreateMesh(Vector3[] verts, int[] tris)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;

        rb.position = _currentLine.transform.position;
        _currentLine.transform.position = new Vector3();

        col.sharedMesh = mesh;

        return mesh;
    }


    int[] CreateTris()
    {
        List<int> trisList = new List<int>();
        for (var i = 0; i < edges.Length; i++)
        {
            trisList.Add(i);
            trisList.Add((i + 1) % (edges.Length - 1));
            trisList.Add(edges.Length - 1);
        }
        return trisList.ToArray();
    }

    public void Clear()
    {
        if (_currentLine != null && !debug)
            Destroy(_currentLine.gameObject);
    }
}