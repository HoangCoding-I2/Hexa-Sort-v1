using System.Collections.Generic;
using UnityEngine;

public class HexStack : MonoBehaviour
{
    public List<Hexagon> Hexagons { get; private set; }
    

    public void AddHexagon(Hexagon hexagon)
    {
        if (Hexagons == null)
        {
            Hexagons = new List<Hexagon>();
        }
        Hexagons.Add(hexagon);
        hexagon.SetParent(transform);
    }

    public void DisableColliders()
    {
        foreach (Hexagon hexagon in Hexagons)
        {
            hexagon.DisableCollider();
        }
    }

    public Color GetTopHexagonColor()
    {
        return Hexagons[^1].Color;// ^1 → phần tử cuối cùng
    }
    public bool ContainsHexagon(Hexagon hexagon) => Hexagons.Contains(hexagon);
    public void RemoveHexagon(Hexagon hexagon)
    {
        Hexagons.Remove(hexagon);

        if (Hexagons.Count <= 0)
            DestroyImmediate(gameObject);// dùng để xóa đối tượng ngay lập tức, kể cả trong Editor.
    }

}
