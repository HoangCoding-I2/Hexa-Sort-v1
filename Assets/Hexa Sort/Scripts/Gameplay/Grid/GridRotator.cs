using System.Collections.Generic;
using UnityEngine;

public class GridRotator : MonoBehaviour
{
    [Header(" Settings ")]
    [SerializeField] private float _dragSpeed = 2f;
    [SerializeField] private float _snapAngle = 60f;
    [SerializeField] private float _snapSpeed = 10f;
    [SerializeField] private float _snapThreshold = 0.1f;
    
    private float _lastXPosition;
    private bool _isDragging = false;
    private Quaternion _targetRotation;
    private List<HexStack> _allStacks = new List<HexStack>();
    private bool _needsSnapping = false;
    
    private void Start()
    {
        _targetRotation = transform.rotation;
        // Cache tất cả stacks thay vì tìm kiếm mỗi frame
        _allStacks.AddRange(FindObjectsOfType<HexStack>());
    }
    
    private void Update()
    {
        // Xử lý input và rotation
        HandleRotationInput();
        
        // Chỉ thực hiện snap khi cần thiết
        if (_needsSnapping)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _snapSpeed);
            
            // Kiểm tra xem đã gần đến rotation target chưa
            if (Quaternion.Angle(transform.rotation, _targetRotation) < _snapThreshold)
            {
                transform.rotation = _targetRotation;
                _needsSnapping = false;
            }
        }
    }
    
    private void HandleRotationInput()
    {
        // Kiểm tra nếu đang có stack đang di chuyển thì không cho phép xoay
        if (IsAnyStackMoving())
            return;
        
        // Sử dụng GetMouseButtonDown với kiểm tra IsPointerOverGrid chỉ khi cần
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverGrid())
            {
                Vector2 mousePosition = Input.mousePosition;
                _isDragging = true;
                _lastXPosition = mousePosition.x;
                _needsSnapping = false;
            }
        }
        
        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            _isDragging = false;
            SnapRotation();
            _needsSnapping = true;
        }
        
        if (_isDragging)
        {
            Vector2 currentPosition = Input.mousePosition;
            float xDiff = currentPosition.x - _lastXPosition;
            float rotationAmount = xDiff * _dragSpeed / Screen.width;
            
            transform.Rotate(0f, rotationAmount, 0f, Space.World);
            _lastXPosition = currentPosition.x;
        }
    }
    
    private void SnapRotation()
    {
        float yRotation = transform.eulerAngles.y;
        float targetYRotation = Mathf.Round(yRotation / _snapAngle) * _snapAngle;
        _targetRotation = Quaternion.Euler(transform.eulerAngles.x, targetYRotation, transform.eulerAngles.z);
    }
    
    private bool IsPointerOverGrid()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            return hit.transform == transform || hit.transform.IsChildOf(transform);
        }
        
        return false;
    }
    
    private bool IsAnyStackMoving()
    {
        // Chỉ kiểm tra các stack đã được cache, tự động bỏ qua null references
        foreach (HexStack stack in _allStacks)
        {
            if (stack != null && LeanTween.isTweening(stack.gameObject))
                return true;
        }
        
        return false;
    }
}