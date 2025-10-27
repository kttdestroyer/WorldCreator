using UnityEngine;

public class ScenarioButtons : MonoBehaviour
{
    public MapManager map;
    public CityManager cities;

    public void Save() => ScenarioIO.Save("scenario.json", map, cities);
    public void Load() => ScenarioIO.Load("scenario.json", map, cities);

    public void SetResLow() { GameSettings.mapResolution = MapResolution.Low256x128; map.InitMap(); cities.cityPoints.Clear(); }
    public void SetResMedium() { GameSettings.mapResolution = MapResolution.Medium512x256; map.InitMap(); cities.cityPoints.Clear(); }
    public void SetResHigh() { GameSettings.mapResolution = MapResolution.High1024x512; map.InitMap(); cities.cityPoints.Clear(); }
}
