using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Nation
{
    public int id;
    public string name;
    public Color color = Color.white;
    public Sprite flag; // Optional
}

public class NationRegistry : MonoBehaviour
{
    public static NationRegistry I;

    [Tooltip("Define some starter nations here.")]
    public List<Nation> nations = new List<Nation>()
    {
        new Nation{ id=0, name="Neutral", color=new Color(0,0,0,0) },
        new Nation{ id=1, name="Blue", color=new Color(0.20f,0.55f,0.95f,1) },
        new Nation{ id=2, name="Red",  color=new Color(0.85f,0.25f,0.25f,1) },
        new Nation{ id=3, name="Green",color=new Color(0.20f,0.80f,0.35f,1) },
        new Nation{ id=4, name="Yellow",color=new Color(0.95f,0.85f,0.20f,1) },
    };

    Dictionary<int, Nation> _byId;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        _byId = new Dictionary<int, Nation>();
        foreach (var n in nations) _byId[n.id] = n;
    }

    public Color GetColor(int nationId)
    {
        if (_byId != null && _byId.TryGetValue(nationId, out var n)) return n.color;
        return new Color(0, 0, 0, 0);
    }

    public int ClampNationId(int id) => _byId.ContainsKey(id) ? id : 0;

    public string GetName(int id) => _byId.ContainsKey(id) ? _byId[id].name : "Unknown";
}
