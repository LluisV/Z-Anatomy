#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace MTAssets.UltimateLODSystem
{
    /*
    * This is a script that is part of the essential library for "MT Assets" assets. 
    */

    [AddComponentMenu("")] //Hide this script in component menu.
    public class MTAssetsMathematics : MonoBehaviour
    {
        //This method, randomize a specific list
        public static List<T> RandomizeThisList<T>(List<T> list)
        {
            int count = list.Count;
            int last = count - 1;
            for (int i = 0; i < last; ++i)
            {
                int r = UnityEngine.Random.Range(i, count);
                var tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;
            }

            return list;
        }

        //This method return a half position between two Vector 3
        public static Vector3 GetHalfPositionBetweenTwoPoints(Vector3 pointA, Vector3 pointB)
        {
            return Vector3.Lerp(pointA, pointB, 0.5f);
        }
    }

    //Extensions classes
    public static class ListMethodsExtensions
    {
        //Return the count of elements in list
        public static void RemoveAllNullItems<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (list[i] == null)
                    list.RemoveAt(i);
        }
    }
}