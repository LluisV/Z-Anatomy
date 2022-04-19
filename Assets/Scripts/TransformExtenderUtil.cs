using UnityEngine;
using System.Collections;

public static class TransformExtenderUtil { 
    public static ArrayList CountDecendants( this Transform transform) {
        int childCount = transform.childCount;// direct child count.
		ArrayList childs = new ArrayList();
        foreach(Transform child in transform) {
			childs.Add(child);
			childs.AddRange(CountDecendants(child));
          
        }
        return childs;
    }
}