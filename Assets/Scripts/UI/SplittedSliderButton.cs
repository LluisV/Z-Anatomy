using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Nobi.UiRoundedCorners;

public class SplittedSliderButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    private Image img;
    private int index;
    private Color enabledColor;
    private Color disabledColor;
    private Color emptyColor;

    private SplittedSlider slider;
    private ImageWithRoundedCorners roundCorners;

    private float radius;

    private void Awake()
    {
        img = GetComponent<Image>();
        roundCorners = GetComponent<ImageWithRoundedCorners>();
    }

    private void Update()
    {
        if (slider == null)
            return;

        if(Mouse.current.leftButton.wasReleasedThisFrame)
            slider.dragging = false;
        if(slider.dragging && slider.value != index + 1 && Mathf.Abs(Mouse.current.delta.x.ReadValue()) > 0 && Mouse.current.position.ReadValue().x > transform.position.x )
            OnClick();
        if (slider.dragging && index == 0 && Mathf.Abs(Mouse.current.delta.x.ReadValue()) > 0 && Mouse.current.position.x.ReadValue() < transform.position.x)
            slider.OnClick(0);
    }

    public void Init(Color enabledColor, Color disabledColor, Color emptyColor, int index, float radius, SplittedSlider slider)
    {
        this.enabledColor = enabledColor;
        this.disabledColor = disabledColor;
        this.emptyColor = emptyColor;
        this.index = index;
        this.slider = slider;
        this.radius = radius;
    }

    public void SetEnabled()
    {
        if (img == null)
            img = GetComponent<Image>();

        img.color = enabledColor;
    }

    public void SetDisabled()
    {
        if(img == null)
            img = GetComponent<Image>();

        img.color = disabledColor;
    }

    public void SetEmpty()
    {
        if (img == null)
            img = GetComponent<Image>();

        img.color = emptyColor;
    }

    public void OnClick()
    {
        slider.OnClick(index + 1);
    }

    public void SetRadius()
    {
        if(roundCorners == null)
            roundCorners = GetComponent<ImageWithRoundedCorners>();
        roundCorners.radius = radius;
        roundCorners.Refresh();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnClick();
            slider.dragging = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        /*if(eventData.button == PointerEventData.InputButton.Left)
        {
            slider.dragging = false;
            Debug.Log("PointerUp");
        }*/
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(slider.dragging && slider.value != index + 1)
            OnClick();
    }
}
