using UnityEngine;

public enum MapResolution { Low256x128, Medium512x256, High1024x512 }

public static class GameSettings
{
    // Change default here or via an options UI later
    public static MapResolution mapResolution = MapResolution.Medium512x256;

    public static void GetMapSize(out int w, out int h)
    {
        switch (mapResolution)
        {
            case MapResolution.Low256x128: w = 256; h = 128; break;
            case MapResolution.High1024x512: w = 1024; h = 512; break;
            default: w = 512; h = 256; break;
        }
    }
}
