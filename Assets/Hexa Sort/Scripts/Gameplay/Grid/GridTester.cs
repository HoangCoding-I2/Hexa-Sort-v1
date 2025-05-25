using UnityEngine;
using NaughtyAttributes;

public class GridTester : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Grid _grid;

    [Header(" Settings ")]
    [OnValueChanged("UpdateGridPos")]//Mỗi khi giá trị của biến _gridPos thay đổi trong Inspector, hàm UpdateGridPos() sẽ được tự động gọi.
    [SerializeField] private Vector3Int _gridPos;

    private void UpdateGridPos() => transform.position = _grid.CellToWorld(_gridPos);

}
