using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpMessage : MonoBehaviour
{
    private ExpandCollapseUI expandAnimation;
    public TextMeshProUGUI text;
    private RectTransform RT;
    private void Awake()
    {
        expandAnimation = GetComponent<ExpandCollapseUI>();
        RT = GetComponent<RectTransform>();
        RT.anchoredPosition = new Vector2(0, -150);
    }

    public void Show(string message, float seconds)
    {
        text.text = message;
        expandAnimation.ExpandButtonClick();
        StartCoroutine(WaitToDestroy(seconds));
    }

    IEnumerator WaitToDestroy(float length)
    {
        yield return new WaitForSeconds(length);
        expandAnimation.Collapse();
        Destroy(gameObject, 3);
    }
}
