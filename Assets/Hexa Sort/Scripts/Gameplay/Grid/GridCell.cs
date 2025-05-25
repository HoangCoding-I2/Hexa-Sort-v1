using NaughtyAttributes;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Hexagon _hexagonPrefab;

    [Header(" Settings ")]
    [OnValueChanged("GenerateInitialHexagons")]
    [SerializeField] private Color[] _hexagonColors;

    public HexStack Stack { get; private set; }
    public bool IsOccupied => Stack != null;

    public void SetStack(HexStack stack)
    {
        Stack = stack;
    }

    private void GenerateInitialHexagons()
    {
        while(transform.childCount > 1)
        {
            Transform t = transform.GetChild(1);
            t.SetParent(null);
            DestroyImmediate(t.gameObject);
        }
        Stack = new GameObject("Initial Stack").AddComponent<HexStack>();
        Stack.transform.SetParent(transform);


        Stack.transform.localPosition = Vector3.up * .2f;

        for(int i = 0; i < _hexagonColors.Length; i++)
        {
            Vector3 spawnPosition = Stack.transform.TransformPoint(Vector3.up * i * .2f);
            Hexagon hexagonInstance = Instantiate(_hexagonPrefab, spawnPosition, Quaternion.identity);
            hexagonInstance.Color = _hexagonColors[i];

            Stack.AddHexagon(hexagonInstance);
        }

        
    }
}
