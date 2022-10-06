using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NeumorphismUI
{
    public class Shadow : IModifier
    {
        static readonly List<UIVertex> vertCache = new List<UIVertex>(4096);

        public enum Mode
        {
            Light,
            Shadow,
        }

        Mode mode;

        public Shadow(Mode mode)
        {
            this.mode = mode;
        }

        public void ModifyMesh(Neumorphism neu, VertexHelper vh)
        {
            var output = vertCache;
            vh.GetUIVertexStream(output);

            Color color;
            Vector2 offset;
            if (mode == Mode.Light)
            {
                color = neu.lightColor;
                offset = new Vector2(-1, 1).normalized * neu.shadowOffset;
            }
            else
            {
                color = neu.shadowColor;
                offset = new Vector2(1, -1).normalized * neu.shadowOffset;
            }
            ApplyShadowZeroAlloc(output, color, 0, output.Count, offset.x, offset.y);
            vh.Clear();
            vh.AddUIVertexTriangleStream(output);

            output.Clear();
        }

        protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            UIVertex vt;

            var neededCapacity = verts.Count + end - start;
            if (verts.Capacity < neededCapacity)
                verts.Capacity = neededCapacity;

            for (int i = start; i < end; ++i)
            {
                vt = verts[i];
                verts.Add(vt);

                Vector3 v = vt.position;
                vt.position = v;
                vt.color = color;
                vt.uv2 = new Vector2(x, y);
                verts[i] = vt;
            }
        }
    }
}
