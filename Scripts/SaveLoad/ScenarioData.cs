using System;
using System.Collections.Generic;

[Serializable]
public class ScenarioData
{
    public int width;
    public int height;

    public int[] ownerFlat;   // length = w*h
    public byte[] landFlat;   // length = w*h, 1 land, 0 sea

    public List<CityPoint> cities = new List<CityPoint>();
}
