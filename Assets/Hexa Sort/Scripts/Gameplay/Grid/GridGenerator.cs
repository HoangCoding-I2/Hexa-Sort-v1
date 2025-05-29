using NaughtyAttributes;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Grid _grid;
    [SerializeField] private GameObject _hexagon;

    [Header(" Settings ")]
    [OnValueChanged("GenerateGrid")]
    [SerializeField] private int _gridSize;

    private void GenerateGrid()
    {
        transform.Clear();
        float maxDistance = _grid.CellToWorld(new Vector3Int(1, 0, 0)).magnitude * _gridSize;
        for (int i = -_gridSize; i <= _gridSize; i++)//x
        {
            for (int j = -_gridSize; j <= _gridSize; j++)//y
            {
                Vector3 spawnPosition = _grid.CellToWorld(new Vector3Int(i, j, 0));
                
                if (spawnPosition.magnitude > maxDistance)
                {
                    continue;
                }
                Instantiate(_hexagon, spawnPosition, Quaternion.identity, transform);
            }
        }
    }

}
