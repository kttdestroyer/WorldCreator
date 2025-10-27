using UnityEngine;
using UnityEngine.EventSystems;

public enum PaintMode { Land, Nation, CityPlacement }

public class PaintTool : MonoBehaviour
{
    public Camera cam;
    public MapManager map;
    public CityManager cities;
    public NationRegistry nations;

    [Header("Paint")]
    public PaintMode mode = PaintMode.Land;
    public int brushRadius = 4;
    public int selectedNationId = 1; // choose via UI later
    public bool landBrushMakeLand = true; // true=raise, false=flood

    void Update()
    {
        if (cam == null || map == null) return;
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButton(0))
        {
            if (map.ScreenToGrid(cam, Input.mousePosition, out int gx, out int gy))
            {
                switch (mode)
                {
                    case PaintMode.Land:
                        map.PaintLandCircle(gx, gy, brushRadius, landBrushMakeLand);
                        break;
                    case PaintMode.Nation:
                        map.PaintOwnerCircle(gx, gy, brushRadius, selectedNationId);
                        break;
                    case PaintMode.CityPlacement:
                        cities?.PlaceCityAt(gx, gy);
                        break;
                }
            }
        }
        // Right mouse toggles land brush raise/flood quickly
        if (mode == PaintMode.Land && Input.GetMouseButtonDown(1))
            landBrushMakeLand = !landBrushMakeLand;
    }

    // Simple UI hooks
    public void SetMode_Land() => mode = PaintMode.Land;
    public void SetMode_Nation() => mode = PaintMode.Nation;
    public void SetMode_City() => mode = PaintMode.CityPlacement;
    public void SetNationId(int id) => selectedNationId = Mathf.Max(0, id);
    public void SetBrushSize(float v) => brushRadius = Mathf.Clamp(Mathf.RoundToInt(v), 1, 128);
    public void ToggleLandRaise(bool raise) => landBrushMakeLand = raise;
}
