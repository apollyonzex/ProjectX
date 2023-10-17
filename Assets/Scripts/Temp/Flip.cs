using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Effects/Flip", 3)]

public class Flip : BaseMeshEffect
{
    List<UIVertex> vertexCache = new List<UIVertex>();
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }

        vh.GetUIVertexStream(vertexCache);

        ApplyFlip(vertexCache, graphic.rectTransform.rect.center);

        vh.Clear();
        vh.AddUIVertexTriangleStream(vertexCache);
        vertexCache.Clear();
    }

    void ApplyFlip(List<UIVertex> vertexCache, Vector2 pivot)
    {
        int vertexCount = vertexCache.Count;
        for (int i = 0; i < vertexCount; i++)
        {
            UIVertex veretx = vertexCache[i];
            veretx.position.x = 2 * pivot.x - veretx.position.x;
            vertexCache[i] = veretx;
        }
    }
}