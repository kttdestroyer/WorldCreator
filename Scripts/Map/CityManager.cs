using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CityPoint
{
    public int gridX, gridY;
    public int ownerNationId = 0; // neutral at start
    public float capture = 0f;
    public float captureMax = 100f;
    public int incomeMetal = 1;
    public int incomeOil = 1;
    public int incomeMoney = 3;
}

public class CityManager : MonoBehaviour
{
    public static CityManager I;
    public MapManager map;
    public NationRegistry nations;
    public List<CityPoint> cityPoints = new List<CityPoint>();

    [Header("Visuals")]
    public float gizmoRadius = 6f; // purely editor/preview

    void Awake() { I = this; }

    public void PlaceCityAt(int gx, int gy)
    {
        if (!map || gx < 0 || gy < 0 || gx >= map.Width || gy >= map.Height) return;
        if (!map.isLand[gx, gy]) return; // only on land
        // Avoid duplicates on same cell
        foreach (var c in cityPoints) if (c.gridX == gx && c.gridY == gy) return;

        cityPoints.Add(new CityPoint { gridX = gx, gridY = gy, ownerNationId = 0 });
        // Optional: immediate small neutral paint dot to visualize
        map.PaintOwnerCircle(gx, gy, 1, 0);
    }

    void OnDrawGizmos()
    {
        if (map == null) return;
        Gizmos.matrix = transform.localToWorldMatrix;
        foreach (var c in cityPoints)
        {
            var world = GridToWorld(c.gridX, c.gridY);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(world, gizmoRadius * 0.01f * map.Width); // scale approx
        }
    }

    Vector3 GridToWorld(int gx, int gy)
    {
        // Project back onto the WorldQuad plane (assumes unit quad scaled to fit world).
        // We use renderer bounds to place roughly.
        var rend = map.GetComponent<Renderer>();
        var b = rend.bounds;
        float u = (gx + 0.5f) / map.Width;
        float v = (gy + 0.5f) / map.Height;
        return new Vector3(
            Mathf.Lerp(b.min.x, b.max.x, u),
            b.center.y,
            Mathf.Lerp(b.min.z, b.max.z, v)
        );
    }
}
