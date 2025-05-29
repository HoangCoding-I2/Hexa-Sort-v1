using System;
using UnityEngine;

public class StackController : MonoBehaviour
{
    [Header(" Settings ")]
    [SerializeField] private LayerMask _hexagonLayerMask;
    [SerializeField] private LayerMask _gridHexagonLayerMask;
    [SerializeField] private LayerMask _groundLayerMask;
    private HexStack _currentStack;
    private Vector3 _currentStackInitialPosition;

    [Header(" Data ")]
    private GridCell _targetCell;
    private GridCell _previousHighlightedCell; // Lưu trữ cell đã highlight trước đó

    [Header(" Actions ")]
    public static Action<GridCell> OnStackPlaced;
    

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            HandleMouseDown();
        }
        else if (Input.GetMouseButton(0) && _currentStack != null)
        {
            HandleMouseDrag();
        }
        else if (Input.GetMouseButtonUp(0) && _currentStack != null)
        {
            HandleMouseUp();
        }
    }

    private void HandleMouseDown()
    {
        RaycastHit hit;
        Physics.Raycast(GetCursorRay(), out hit, 500, _hexagonLayerMask);

        if (hit.collider == null)
        {
            return;
        }
        _currentStack = hit.collider.GetComponent<Hexagon>().HexStack;
        _currentStackInitialPosition = _currentStack.transform.position;
    }
    
    private void HandleMouseDrag()
    {
        RaycastHit hit;
        Physics.Raycast(GetCursorRay(), out hit, 500, _gridHexagonLayerMask);

        if (hit.collider == null)
        {
            DragAboveGround();
        }
        else
        {
            DragAboveGridCell(hit);
        }
    }

    private void HandleMouseUp()
    {
        // Tắt highlight của cell trước đó nếu có
        if (_previousHighlightedCell != null)
        {
            _previousHighlightedCell.SetHighlight(false);
            _previousHighlightedCell = null;
        }

        if (_targetCell == null)
        {
            _currentStack.transform.position = _currentStackInitialPosition;
            _currentStack = null;
            return;
        }

        _currentStack.transform.position = _targetCell.transform.position.With(y: .2f);
        _currentStack.transform.SetParent(_targetCell.transform);
        _currentStack.DisableColliders();

        _targetCell.SetStack(_currentStack);

        // Phát âm thanh khi đặt hexagon
        if (AudioService.Instance != null)
            AudioService.Instance.PlayHexagonPlace();

        OnStackPlaced?.Invoke(_targetCell);

        _currentStack = null;
        _targetCell = null;

        // Sau khi đặt, kiểm tra game over
        GameManager.Instance.CheckGameOver();
    }

    private void DragAboveGround()
    {
        // Tắt highlight của cell trước đó nếu có
        if (_previousHighlightedCell != null)
        {
            _previousHighlightedCell.SetHighlight(false);
            _previousHighlightedCell = null;
        }

        RaycastHit hit;
        Physics.Raycast(GetCursorRay(), out hit, 500, _groundLayerMask);
        if (hit.collider == null)
        {
            return;
        }
        Vector3 targetPosition = hit.point.With(y: 2);
        _currentStack.transform.position = Vector3.MoveTowards(_currentStack.transform.position, targetPosition, Time.deltaTime * 30);
        _targetCell = null;
    }
    
    private void DragAboveGridCell(RaycastHit hit)
    {
        GridCell gridCell = hit.collider.GetComponent<GridCell>();
        if (gridCell.IsOccupied)
        {
            // Nếu grid cell đã bị chiếm, xử lý như khi kéo trên mặt đất
            DragAboveGround();
        }
        else
        {
            DragAboveEmptyGridCell(gridCell);
        }
    }
    
    private void DragAboveEmptyGridCell(GridCell gridCell)
    {
        // Nếu đang di chuyển đến một cell mới, tắt highlight của cell trước đó
        if (_previousHighlightedCell != null && _previousHighlightedCell != gridCell)
        {
            _previousHighlightedCell.SetHighlight(false);
        }

        // Bật highlight cho cell hiện tại
        gridCell.SetHighlight(true);
        _previousHighlightedCell = gridCell;

        Vector3 targetPosition = gridCell.transform.position.With(y: 2);
        _currentStack.transform.position = Vector3.MoveTowards(_currentStack.transform.position, targetPosition, Time.deltaTime * 30);

        _targetCell = gridCell;
    }

    private Ray GetCursorRay() => Camera.main.ScreenPointToRay(Input.mousePosition);
    
    private void OnDestroy()
    {
        // Đảm bảo tắt highlight khi hủy component
        if (_previousHighlightedCell != null)
        {
            _previousHighlightedCell.SetHighlight(false);
        }
    }
}