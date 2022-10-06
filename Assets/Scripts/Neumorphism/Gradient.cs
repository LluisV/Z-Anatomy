using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NeumorphismUI
{
    public class Gradient : IModifier
    {
        RectTransform rectTransform = null;

        public void ModifyMesh(Neumorphism neu, VertexHelper vh)
        {
            if (rectTransform == null)
            {
                rectTransform = neu.GetComponent<RectTransform>();
            }
            var rect = rectTransform.rect;

            if (rect.width > rect.height)
            {
                rect.y -= (rect.width - rect.height) * 0.5f;
                rect.height = rect.width;
            }
            else
            {
                rect.x -= (rect.height - rect.width) * 0.5f;
                rect.width = rect.height;
            }

            var vert = default(UIVertex);
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vert, i);
                vert.uv1 = Rect.PointToNormalized(rect, vert.position);
                vh.SetUIVertex(vert, i);
            }
        }

    }
}
