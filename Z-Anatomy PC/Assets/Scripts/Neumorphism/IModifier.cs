using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NeumorphismUI
{
    public interface IModifier
    {
        void ModifyMesh(Neumorphism neu, VertexHelper verts);
    }
}
