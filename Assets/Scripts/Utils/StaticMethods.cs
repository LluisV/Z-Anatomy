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

    public static Transform RecursiveFindChild(this Transform parent, string childName)
    {
        childName = childName.Replace(".t", "").Replace(".s", "").ToLower();
        foreach (Transform child in parent)
        {
            string childN = child.name.Replace(".t", "").Replace(".s","").ToLower();
            
            if (childN == childName && !child.CompareTag("Insertions") && !childN.Contains(".j") && !childN.Contains(".i"))
            {
                return child;
            }
            else
            {
                Transform found = child.RecursiveFindChild(childName);
                if (found != null)
                {
                    return found;
                }
            }
        }
        
        return null;
    }

    public static string RemovePunctuations(this string input)
    {
        return Regex.Replace(input, "[\"(),./:;\\[\\]{}]", string.Empty);
    }
    public static int ParentCount(this Transform parent)
    {
        int count = 0;
        while(parent.parent != null)
        {
            parent = parent.parent;
            count++;
        }
        return count;
    }

    public static string RemoveAccents(this string text) =>
        new String(
            text.Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray()
        )
        .Normalize(NormalizationForm.FormC);

    public static void SetActiveRecursively(this Transform parent, bool state, List<GameObject> changed = null)
    {
        if (!parent.name.Contains(".j") && !parent.name.Contains(".i") && parent.GetComponent<Label>() == false && !parent.CompareTag("Insertions"))
        {
            if (changed != null && ((state && !parent.gameObject.activeInHierarchy) || (!state && parent.gameObject.activeInHierarchy)))
                changed.Add(parent.gameObject);
            parent.gameObject.SetActive(state);
            Visibility parentVisibility = parent.GetComponent<Visibility>();
            if (parentVisibility != null)
                parentVisibility.isVisible = state;
        }
        foreach (Transform child in parent)
        {
            if(!child.name.Contains(".j") && !child.name.Contains(".i") && child.GetComponent<Label>() == false && !parent.CompareTag("Insertions"))
            {
                if (changed != null && ((state && !child.gameObject.activeInHierarchy) || (!state && child.gameObject.activeInHierarchy)))
                    changed.Add(child.gameObject);
                child.gameObject.SetActive(state);
                Visibility childVisibility = child.GetComponent<Visibility>();
                if (childVisibility != null)
                    childVisibility.isVisible = state;
            }
            if (child.childCount > 0)
                child.SetActiveRecursively(state);
        }
    }

    public static void SetActiveParentsRecursively(this Transform parent, bool state, List<GameObject> changed = null)
    {
        if (parent.CompareTag("GlobalParent") || parent == null || parent.name.Contains(".j") || parent.name.Contains(".i"))
            return;
        if (changed != null && ((state && !parent.gameObject.activeInHierarchy) || (!state && parent.gameObject.activeInHierarchy)))
            changed.Add(parent.gameObject);
        parent.gameObject.SetActive(state);
        Visibility isVisible = parent.GetComponent<Visibility>();
        if (isVisible != null)
            isVisible.isVisible = true;
        parent.parent.SetActiveParentsRecursively(state, changed);
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

    public static string RemoveSuffix(this string str)
    {
        int indexOfPoint = str.LastIndexOf('.');
        if (indexOfPoint != -1)
        {
            string suffix = str.Substring(indexOfPoint);
            if(suffix.Length <= 5)
                str = str.Replace(suffix, "");
        }
        return str;
    }

    public static bool IsRight(this string str)
    {
        int indexOfPoint = str.LastIndexOf('.');
        if (indexOfPoint != -1)
        {
            string suffix = str.Substring(indexOfPoint);
            return suffix.ToLower().Contains("r");
        }
        return false;
    }

    public static bool IsLeft(this string str)
    {
        int indexOfPoint = str.LastIndexOf('.');
        if (indexOfPoint != -1)
        {
            string suffix = str.Substring(indexOfPoint);
            return suffix.ToLower().Contains("l");
        }
        return false;
    }

    public static string RemoveRichTextTags(this string text)
    {
        int index = text.IndexOf('<');
        int index2 = text.IndexOf('>');
        while(index != -1)
        {
            text = text.Remove(index, index2 - index + 1);
            index = text.IndexOf('<');
            index2 = text.IndexOf('>');
        }

        return text;
    }

    public static string RemoveBodyPartLinks(this string text)
    {
        return text.Replace("<link><color=#EA7600>", "").Replace("</link></color>", "");
    }

    public static bool IsBodyPart(this GameObject go)
    {
        return go != null && go.GetComponent<BodyPart>() != null;
    }

    public static bool IsLabel(this GameObject go)
    {
        return go != null && go.GetComponent<Label>() != null;
    }

    public static bool IsGroup(this GameObject go)
    {
        return go != null && go.GetComponent<NameAndDescription>().originalName.EndsWith(".g");
    }

    public static Bounds GetBounds(List<GameObject> objects)
    {
        //List of selected scripts
        List<BodyPart> scripts = new List<BodyPart>();

        foreach (var obj in objects)
        {
            var script = obj.GetComponent<BodyPart>();
            if (script != null)
                scripts.Add(script);
        }

        if (scripts.Count == 0)
            return new Bounds();

        BodyPart first = scripts[0];

        //Calculate bounds for all objects
        Bounds bounds = new Bounds(first.center, first.bounds.size);

        for (int i = 1; i < scripts.Count; i++)
        {
            if (scripts[i] != null && scripts[i].bounds != null)
                bounds.Encapsulate(scripts[i].bounds);
        }

        return bounds;
    }

    public static T[] GetComponentsInDirectChildren<T>(this Transform gameObject) where T : Component
    {
        List<T> components = new List<T>();
        for (int i = 0; i < gameObject.childCount; ++i)
        {
            T component = gameObject.GetChild(i).GetComponent<T>();
            if (component != null)
                components.Add(component);
        }

        return components.ToArray();
    }
}
