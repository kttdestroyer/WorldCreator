using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MapManager : MonoBehaviour
{
    public static MapManager I;

    [Header("References")]
    public NationRegistry nations;

    [Header("Runtime Textures (auto)")]
    public Texture2D texLand;   // background land/sea
    public Texture2D texOwner;  // ownership overlay (nation colors)

    [Header("Colors")]
    public Color landColor = new Color(0.80f, 0.80f, 0.78f, 1f);
    public Color seaColor = new Color(0.25f, 0.35f, 0.55f, 1f);

    public int Width { get; private set; }
    public int Height { get; private set; }

    // Grids
    public bool[,] isLand;   // true land, false sea
    public int[,] owner;     // -1 sea, 0 neutral, >0 nations

    Renderer _rend;
    readonly List<RectInt> _dirty = new();

    void Awake()
    {
        I = this;
        _rend = GetComponent<Renderer>();
        InitMap();
    }

    public void InitMap()
    {
        GameSettings.GetMapSize(out int w, out int h);
        Width = w; Height = h;

        isLand = new bool[w, h];
        owner = new int[w, h];

        // Default: all sea
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                isLand[x, y] = false;
                owner[x, y] = -1; // sea has no owner
            }

        // Textures
        texLand = new Texture2D(w, h, TextureFormat.RGB565, false);
        texOwner = new Texture2D(w, h, TextureFormat.RGBA32, false); // allow alpha

        texLand.filterMode = FilterMode.Point;
        texOwner.filterMode = FilterMode.Point;

        // Initial draw
        RedrawAll();

        // Assign overlay texture to this renderer's material
        _rend.sharedMaterial.mainTexture = texOwner;
    }

    public void RedrawAll()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                texLand.SetPixel(x, y, isLand[x, y] ? landColor : seaColor);
                var c = OwnerToColor(owner[x, y]);
                texOwner.SetPixel(x, y, c);
            }
        texLand.Apply(false, false);
        texOwner.Apply(false, false);
        _dirty.Clear();
    }

    Color OwnerToColor(int id)
    {
        if (id < 0) return new Color(0, 0, 0, 0); // sea transparent on overlay
        return nations ? nations.GetColor(id) : Color.white;
    }

    public void PaintLandCircle(int cx, int cy, int radius, bool makeLand)
    {
        Circle(cx, cy, radius, (x, y) =>
        {
            isLand[x, y] = makeLand;
            if (!makeLand) owner[x, y] = -1;           // sea wipes ownership
            texLand.SetPixel(x, y, makeLand ? landColor : seaColor);
            texOwner.SetPixel(x, y, OwnerToColor(owner[x, y]));
        });
        MarkDirty(cx, cy, radius);
        ApplyDirty();
    }

    public void PaintOwnerCircle(int cx, int cy, int radius, int nationId)
    {
        Circle(cx, cy, radius, (x, y) =>
        {
            if (!isLand[x, y]) return;
            owner[x, y] = nations ? nations.ClampNationId(nationId) : nationId;
            texOwner.SetPixel(x, y, OwnerToColor(owner[x, y]));
        });
        MarkDirty(cx, cy, radius);
        ApplyDirty();
    }

    void Circle(int cx, int cy, int r, System.Action<int, int> action)
    {
        int r2 = r * r;
        for (int y = cy - r; y <= cy + r; y++)
            for (int x = cx - r; x <= cx + r; x++)
            {
                if (x < 0 || y < 0 || x >= Width || y >= Height) continue;
                int dx = x - cx, dy = y - cy;
                if (dx * dx + dy * dy <= r2) action(x, y);
            }
    }

    void MarkDirty(int cx, int cy, int r)
    {
        int x = Mathf.Clamp(cx - r, 0, Width - 1);
        int y = Mathf.Clamp(cy - r, 0, Height - 1);
        int w = Mathf.Clamp(cx + r, 0, Width - 1) - x + 1;
        int h = Mathf.Clamp(cy + r, 0, Height - 1) - y + 1;
        _dirty.Add(new RectInt(x, y, w, h));
    }

    void ApplyDirty()
    {
        // For simplicity we just Apply() fully. Later: pack/merge dirty rects + use Apply(false, false)
        texLand.Apply(false, false);
        texOwner.Apply(false, false);
        _dirty.Clear();
    }

    // Helper to convert screen/world to grid
    public bool ScreenToGrid(Camera cam, Vector2 screen, out int gx, out int gy)
    {
        gx = gy = 0;
        var ray = cam.ScreenPointToRay(screen);
        if (Physics.Raycast(ray, out var hit, 10000f))
        {
            if (hit.collider.gameObject != gameObject) return false;
            var uv = hit.textureCoord; // 0..1
            gx = Mathf.Clamp(Mathf.FloorToInt(uv.x * Width), 0, Width - 1);
            gy = Mathf.Clamp(Mathf.FloorToInt(uv.y * Height), 0, Height - 1);
            return true;
        }
        return false;
    }
}
