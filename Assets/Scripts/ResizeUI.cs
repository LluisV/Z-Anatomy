using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizeUI : MonoBehaviour
{
    public RectTransform canvas;
    float savedPosition = 350;
    public RectTransform parent;
    public ScrollRect scrollRect;
    public Image hideImage1;
    private Image hideImage2;
    public float animationSpeed = 50; 
    bool onRoutine = false;
    bool mobile;
    float velocityMultiplyer;
    private void Start()
    {
        hideImage2 = GetComponent<Image>();
        mobile = SystemInfo.deviceType == DeviceType.Handheld;
        if (mobile)
            velocityMultiplyer = 8;
        else
            velocityMultiplyer = 25;
    }

    public void OnDrag()
    {
        float yVelocity = -Input.GetAxis("Mouse Y") * velocityMultiplyer;
        parent.sizeDelta = new Vector2(parent.sizeDelta.x, parent.sizeDelta.y + yVelocity);
        parent.anchoredPosition = new Vector2(0, -parent.sizeDelta.y / 2 + 10);
        if (parent.sizeDelta.y > canvas.sizeDelta.y - 240)
        {
            parent.sizeDelta = new Vector2(parent.sizeDelta.x, canvas.sizeDelta.y - 241);
        }
    }

    public void OnDragEnd()
    {
        savedPosition = parent.sizeDelta.y;
    }

    public void Collapse()
    {
        if (!onRoutine)
        {
            onRoutine = true;
            StartCoroutine(CollapseCoroutine());
        }
    }

    public void Show()
    {
        if(!onRoutine)
        {
            onRoutine = true;
            StartCoroutine(ShowCoroutine());
        }

    }

    IEnumerator CollapseCoroutine()
    {
        while(parent.sizeDelta.y > -2)
        {
            parent.sizeDelta = new Vector2(parent.sizeDelta.x, parent.sizeDelta.y - Time.deltaTime * animationSpeed);
            parent.anchoredPosition = new Vector2(0, -parent.sizeDelta.y / 2 + 10);
            yield return null;
        }
        onRoutine = false;
        hideImage1.enabled = false;
        hideImage2.enabled = false;
    }

    IEnumerator ShowCoroutine()
    {
        hideImage1.enabled = true;
        hideImage2.enabled = true;
        if (savedPosition < 350)
            savedPosition = 350;
        while (parent.sizeDelta.y < savedPosition)
        {
            parent.sizeDelta = new Vector2(parent.sizeDelta.x, parent.sizeDelta.y + Time.deltaTime * animationSpeed);
            parent.anchoredPosition = new Vector2(0, -parent.sizeDelta.y / 2 + 10);
            yield return null;
        }
        onRoutine = false;
    }

}
