using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

public static class StaticMethods
{

    public static Transform RecursiveFindChild(Transform parent, string childName)
    {
        childName = childName.Replace(".t", "").ToLower();
        foreach (Transform child in parent)
        {
            string childN = child.name.Replace(".t", "").ToLower();
            
            if (childN == childName && !child.CompareTag("Insertions") && !childN.Contains("-lin") && !childN.Contains(".label"))
            {
                return child;
            }
            else
            {
                Transform found = RecursiveFindChild(child, childName);
                if (found != null)
                {
                    return found;
                }
            }
        }
        
        return null;
    }



    public static string RemoveAccents(string texto) =>
        new String(
            texto.Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray()
        )
        .Normalize(NormalizationForm.FormC);

    public static void SetActiveRecursively(Transform parent, bool state)
    {
        if (!parent.name.Contains("-lin") && parent.GetComponent<Label>() == false)
        {
            parent.gameObject.SetActive(state);
            IsVisible parentVisibility = parent.GetComponent<IsVisible>();
            if (parentVisibility != null)
                parentVisibility.isVisible = state;
        }
        foreach (Transform child in parent)
        {
            if(!child.name.Contains("-lin") && child.GetComponent<Label>() == false)
            {
                child.gameObject.SetActive(state);
                IsVisible childVisibility = child.GetComponent<IsVisible>();
                if (childVisibility != null)
                    childVisibility.isVisible = state;
            }
            if (child.childCount > 0)
                SetActiveRecursively(child, state);
        }
    }

    public static void SetActiveParentsRecursively(Transform child, bool state)
    {
        if (child == null || child.name.Contains("-lin"))
            return;
        child.gameObject.SetActive(state);
        IsVisible isVisible = child.GetComponent<IsVisible>();
        if (isVisible != null)
            isVisible.isVisible = true;
        SetActiveParentsRecursively(child.parent, state);
    }

    public static Texture2D ResampleAndCrop(Texture2D source, int targetWidth, int targetHeight)
    {
        int sourceWidth = source.width;
        int sourceHeight = source.height;
        float sourceAspect = (float)sourceWidth / sourceHeight;
        float targetAspect = (float)targetWidth / targetHeight;
        int xOffset = 0;
        int yOffset = 0;
        float factor = 1;
        if (sourceAspect > targetAspect)
        { // crop width
            factor = (float)targetHeight / sourceHeight;
            xOffset = (int)((sourceWidth - sourceHeight * targetAspect) * 0.5f);
        }
        else
        { // crop height
            factor = (float)targetWidth / sourceWidth;
            yOffset = (int)((sourceHeight - sourceWidth / targetAspect) * 0.5f);
        }
        Color32[] data = source.GetPixels32();
        Color32[] data2 = new Color32[targetWidth * targetHeight];
        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                var p = new Vector2(Mathf.Clamp(xOffset + x / factor, 0, sourceWidth - 1), Mathf.Clamp(yOffset + y / factor, 0, sourceHeight - 1));
                // bilinear filtering
                var c11 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                var c12 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                var c21 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                var c22 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                var f = new Vector2(Mathf.Repeat(p.x, 1f), Mathf.Repeat(p.y, 1f));
                data2[x + y * targetWidth] = Color.Lerp(Color.Lerp(c11, c12, p.y), Color.Lerp(c21, c22, p.y), p.x);
            }
        }

        var tex = new Texture2D(targetWidth, targetHeight);
        tex.SetPixels32(data2);
        tex.Apply(true);
        return tex;
    }


    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }


    public static IEnumerable<int> AllIndexesOf(this string sourceString, string subString)
    {
        subString = Regex.Escape(subString);
        foreach (Match match in Regex.Matches(sourceString, subString))
        {
            yield return match.Index;
        }
    }
}
