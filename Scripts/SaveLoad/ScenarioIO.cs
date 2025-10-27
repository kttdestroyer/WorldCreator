using System.IO;
using UnityEngine;

public static class ScenarioIO
{
    public static void Save(string fileName, MapManager map, CityManager cities)
    {
        if (map == null) return;
        var data = new ScenarioData();
        data.width = map.Width;
        data.height = map.Height;

        int len = map.Width * map.Height;
        data.ownerFlat = new int[len];
        data.landFlat = new byte[len];

        int i = 0;
        for (int y = 0; y < map.Height; y++)
            for (int x = 0; x < map.Width; x++)
            {
                data.ownerFlat[i] = map.owner[x, y];
                data.landFlat[i] = (byte)(map.isLand[x, y] ? 1 : 0);
                i++;
            }
        if (cities) data.cities = cities.cityPoints;

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, json);
#if UNITY_EDITOR
        Debug.Log("Saved scenario to: " + path);
#endif
    }

    public static void Load(string fileName, MapManager map, CityManager cities)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path))
        {
#if UNITY_EDITOR
            Debug.LogWarning("Scenario not found: " + path);
#endif
            return;
        }
        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<ScenarioData>(json);
        if (data == null) return;

        // Re-init map if different size
        GameSettings.mapResolution = SizeToRes(data.width, data.height);
        map.InitMap();

        int i = 0;
        for (int y = 0; y < map.Height; y++)
            for (int x = 0; x < map.Width; x++)
            {
                map.owner[x, y] = data.ownerFlat[i];
                map.isLand[x, y] = data.landFlat[i] == 1;
                i++;
            }
        if (cities)
        {
            cities.cityPoints = data.cities != null ? data.cities : new System.Collections.Generic.List<CityPoint>();
        }
        map.RedrawAll();
    }

    static MapResolution SizeToRes(int w, int h)
    {
        if (w == 256 && h == 128) return MapResolution.Low256x128;
        if (w == 1024 && h == 512) return MapResolution.High1024x512;
        return MapResolution.Medium512x256;
    }
}
