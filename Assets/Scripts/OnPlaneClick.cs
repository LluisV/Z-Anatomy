using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnPlaneClick : MonoBehaviour
{
    public static OnPlaneClick instance;

    public Button noPlaneBtn;
    public Button sagitalBtn;
    public Button transversalBtn;
    public Button frontalBtn;
    public Button invertBtn;

    //Min pos => X = -6.7
    //Max pos => X = -2
    public GameObject sagitalPlane;

    //Min pos => Y = -5.4
    //Max pos => Y = 6.75
    public GameObject transversalPlane;

    //Min => Z = -2.2
    //Max => Z = 0
    public GameObject frontalPlane;

    public Slider frontalSlider;
    public Slider transversalSlider;
    public Slider sagitalSlider;

    private ColorBlock defaultColor;
    private ColorBlock pressedColor;

    [HideInInspector]
    public bool xPlane;
    [HideInInspector]
    public bool ixPlane;

    [HideInInspector]
    public bool yPlane;
    [HideInInspector]
    public bool iyPlane;

    [HideInInspector]
    public bool zPlane;
    [HideInInspector]
    public bool izPlane;

    [HideInInspector]
    public bool inverted = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        defaultColor = noPlaneBtn.colors;
        pressedColor = noPlaneBtn.colors;
        pressedColor.normalColor = new Color(.2f, .2f, .2f);
        pressedColor.selectedColor = new Color(.2f, .2f, .2f);
        pressedColor.pressedColor = new Color(.2f, .2f, .2f);
        pressedColor.highlightedColor = new Color(.2f, .2f, .2f);
    }

    public void XClick()
    {
        DeactivateAll();
        xPlane = true;
        sagitalPlane.transform.rotation = Quaternion.Euler(90, 0, 90);
        sagitalSlider.gameObject.SetActive(true);
        sagitalPlane.SetActive(true);
    }

    public void InvertedXClick()
    {
        DeactivateAll();
        ixPlane = true;
        sagitalPlane.transform.rotation = Quaternion.Euler(90, 90, 0);
        sagitalSlider.gameObject.SetActive(true);
        sagitalPlane.SetActive(true);
    }

    public void ZClick()
    {
        DeactivateAll();
        zPlane = true;
        transversalPlane.transform.rotation = Quaternion.Euler(180, 0, 0);
        transversalSlider.gameObject.SetActive(true);
        transversalPlane.SetActive(true);
    }

    public void InvertedZClick()
    {
        DeactivateAll();
        izPlane = true;
        transversalPlane.transform.rotation = Quaternion.Euler(0, 0, 0);
        transversalSlider.gameObject.SetActive(true);
        transversalPlane.SetActive(true);
    }

    public void YClick()
    {
        DeactivateAll();
        yPlane = true;
        frontalPlane.transform.rotation = Quaternion.Euler(90, 0, 0);
        frontalSlider.gameObject.SetActive(true);
        frontalPlane.SetActive(true);
    }

    public void InvertedYClick()
    {
        DeactivateAll();
        iyPlane = true;
        frontalPlane.transform.rotation = Quaternion.Euler(90, 0, 180);
        frontalSlider.gameObject.SetActive(true);
        frontalPlane.SetActive(true);
    }

    // --------------- Old code--------------//

    public void NoCutClick()
    {
        DeactivateAll();
        noPlaneBtn.colors = pressedColor;
    }

    public void FrontalSliderChanged()
    {
        Vector3 newPos = frontalPlane.transform.position;
        newPos.z = frontalSlider.value;
        frontalPlane.transform.position = newPos;
    }

    public void SagitalSliderChanged()
    {
        Vector3 newPos = sagitalPlane.transform.position;
        newPos.x = sagitalSlider.value;
        sagitalPlane.transform.position = newPos;
    }

    public void TransversalSliderChanged()
    {
        Vector3 newPos = transversalPlane.transform.position;
        newPos.y = transversalSlider.value;
        transversalPlane.transform.position = newPos;

    }

    private void DeactivateAll()
    {
        sagitalPlane.SetActive(false);
        transversalPlane.SetActive(false);
        frontalPlane.SetActive(false);

        frontalSlider.gameObject.SetActive(false);
        sagitalSlider.gameObject.SetActive(false);
        transversalSlider.gameObject.SetActive(false);

        invertBtn.interactable = false;

        noPlaneBtn.colors = defaultColor;
        sagitalBtn.colors = defaultColor;
        transversalBtn.colors = defaultColor;
        frontalBtn.colors = defaultColor;

        xPlane = false;
        ixPlane = false;
        yPlane = false;
        iyPlane = false;
        zPlane = false;
        izPlane = false;
    }
}
