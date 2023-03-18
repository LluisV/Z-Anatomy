using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectTransformExtensions
{
    public static Vector2 GetSize(this RectTransform source) => source.rect.size;
    public static float GetWidth(this RectTransform source) => source.rect.size.x;
    public static float GetHeight(this RectTransform source) => source.rect.size.y;

    /// <summary>
    /// Sets the sources RT size to the same as the toCopy's RT size.
    /// </summary>
    public static void SetSize(this RectTransform source, RectTransform toCopy)
    {
        source.SetSize(toCopy.GetSize());
    }

    /// <summary>
    /// Sets the sources RT size to the same as the newSize.
    /// </summary>
    public static void SetSize(this RectTransform source, Vector2 newSize)
    {
        source.SetSize(newSize.x, newSize.y);
    }

    /// <summary>
    /// Sets the sources RT size to the new width and height.
    /// </summary>
    public static void SetSize(this RectTransform source, float width, float height)
    {
        source.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        source.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public static void SetWidth(this RectTransform source, float width)
    {
        source.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    public static void SetHeight(this RectTransform source, float height)
    {
        source.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }

    public static float GetLeft(this RectTransform rt)
    {
        return rt.offsetMin.x;
    }

    public static float GetRight(this RectTransform rt)
    {
        return -rt.offsetMax.x;
    }

    public static float GetTop(this RectTransform rt)
    {
        return -rt.offsetMax.y;
    }

    public static float GetBottom(this RectTransform rt)
    {
        return rt.offsetMin.y;
    }

    public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null) return;

        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }

}