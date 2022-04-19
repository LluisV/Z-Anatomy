using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ResponsiveCanvas : MonoBehaviour
{
    public Canvas canvas;
    public GameObject elementsParent;
    public GameObject panel;
    public float verticalSpacing;
    public float horizontalSpacing;
    private Vector2 resolution;

    [HideInInspector]
    public List<RectTransform> elements = new List<RectTransform>();

    private void Start()
    {
        foreach (Transform child in elementsParent.transform)
        {
            elements.Add(child.GetComponent<RectTransform>());
        }
        //UpdatePositions();
    }


    private void Awake()
    {
        resolution = new Vector2(Screen.width, Screen.height);
    }

    private void Update()
    {
        if (resolution.x != Screen.width || resolution.y != Screen.height)
        {
            resolution.x = Screen.width;
            resolution.y = Screen.height;
            UpdatePositions();
        }
    }

    public void UpdatePositions()
    {
        if (elements.Count == 0)
            return;

        float elementWidth = elements[0].rect.width;
        float elementHeight = elements[0].rect.height;

        int columns = (int)(resolution.x / canvas.scaleFactor / (elementWidth + horizontalSpacing));
        columns = (int)(resolution.x / canvas.scaleFactor / (elementWidth + horizontalSpacing + horizontalSpacing / columns * 2));

        RectTransform rt = elementsParent.GetComponent<RectTransform>();
        RectTransform panelRt = panel.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2((columns * (elementWidth + horizontalSpacing) + horizontalSpacing), (elements.Count / columns + 1) * (elementHeight + verticalSpacing) + verticalSpacing);
        rt.localPosition = new Vector2(Mathf.Abs(panelRt.rect.width - rt.rect.width)/2, rt.localPosition.y);


        Vector2 rootPosition = new Vector2();
        rootPosition.x = elementWidth / 2 + horizontalSpacing;
        rootPosition.y = -elementHeight / 2 - verticalSpacing;

        Vector2 newPos = new Vector2();

        for (int i = 0; i < elements.Count; i++)
        {
            newPos.x += elementWidth + horizontalSpacing;

            if(i % columns == 0)
            {
                newPos.x = rootPosition.x;
                if (i == 0)
                    newPos.y += rootPosition.y;
                else
                    newPos.y += -elementHeight - verticalSpacing;
            }

            elements[i].localPosition = newPos;
        }
    }

    public void DeleteElements()
    {
        foreach (Transform child in elementsParent.transform)
        {
            Destroy(child.gameObject);
        }
        elements.Clear();
    }
}
