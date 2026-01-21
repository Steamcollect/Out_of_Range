// csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

[ExecuteAlways]
[AddComponentMenu("UI/UI Shear")]
public class UI_Shear : MaskableGraphic
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private Vector2 shear = Vector2.zero;

    public Sprite Sprite
    {
        get => sprite;
        set
        {
            if (sprite == value) return;
            sprite = value;
            SetAllDirty();
        }
    }

    public Vector2 Shear
    {
        get => shear;
        set
        {
            if (shear == value) return;
            shear = value;
            SetVerticesDirty();
        }
    }

    public override Texture mainTexture
    {
        get
        {
            if (sprite == null)
                return s_WhiteTexture;
            return sprite.texture == null ? s_WhiteTexture : sprite.texture;
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect rect = rectTransform.rect;
        if (rect.width == 0f || rect.height == 0f)
            return;

        // Build base quad positions (clockwise: BL, TL, TR, BR)
        Vector3[] positions = new Vector3[4];
        positions[0] = new Vector3(rect.xMin, rect.yMin); // BL
        positions[1] = new Vector3(rect.xMin, rect.yMax); // TL
        positions[2] = new Vector3(rect.xMax, rect.yMax); // TR
        positions[3] = new Vector3(rect.xMax, rect.yMin); // BR

        // UVs
        Vector2[] uvs = new Vector2[4];
        if (sprite != null)
        {
            Vector4 uv = DataUtility.GetOuterUV(sprite); // (uMin, vMin, uMax, vMax)
            uvs[0] = new Vector2(uv.x, uv.y); // BL
            uvs[1] = new Vector2(uv.x, uv.w); // TL
            uvs[2] = new Vector2(uv.z, uv.w); // TR
            uvs[3] = new Vector2(uv.z, uv.y); // BR
        }
        else
        {
            uvs[0] = new Vector2(0f, 0f);
            uvs[1] = new Vector2(0f, 1f);
            uvs[2] = new Vector2(1f, 1f);
            uvs[3] = new Vector2(1f, 0f);
        }

        // Create UIVertex array, apply shear if any
        UIVertex[] verts = new UIVertex[4];
        for (int i = 0; i < 4; i++)
        {
            UIVertex v = UIVertex.simpleVert;
            v.position = positions[i];
            v.color = color;
            v.uv0 = uvs[i];
            verts[i] = v;
        }

        if (shear != Vector2.zero)
        {
            for (int i = 0; i < verts.Length; i++)
            {
                UIVertex v = verts[i];

                float nx = (v.position.x - rect.xMin) / rect.width;
                float ny = (v.position.y - rect.yMin) / rect.height;

                v.position.x += (ny - 0.5f) * shear.x * rect.width;
                v.position.y += (nx - 0.5f) * shear.y * rect.height;

                verts[i] = v;
            }
        }

        // Add quad
        vh.AddUIVertexQuad(verts);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        SetAllDirty();
    }
#endif
}
