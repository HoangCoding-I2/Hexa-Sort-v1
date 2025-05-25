using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackSpawner : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform _stackPositionsParent;
    [SerializeField] private Hexagon _hexagonPrefab;
    [SerializeField] private HexStack _hexagonStackPrefab;

    [Header(" Settings ")]
    [NaughtyAttributes.MinMaxSlider(2,8)]
    [SerializeField] private Vector2Int _minMaxHexCount;
    [SerializeField] private Color[] _colors;
    
    [Header(" Animation Settings ")]
    [SerializeField] private float _spawnAnimationDuration = 0.6f;
    [SerializeField] private float _bounceHeight = 0.5f;
    [SerializeField] private LeanTweenType _bounceEaseType = LeanTweenType.easeOutElastic;
    
    private int _stackCounter;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        StackController.OnStackPlaced += StackPlacedCallback;
    }
    private void OnDestroy()
    {
        StackController.OnStackPlaced -= StackPlacedCallback;
    }

    private void StackPlacedCallback(GridCell gridCell)
    {
        _stackCounter++;

        if (_stackCounter >= 3)
        {
            _stackCounter = 0;
            GenerateStacks();
        }
    }

    private void Start()
    {
        GenerateStacks();
    }
    private void GenerateStacks()
    {
        // Phát âm thanh khi hiển thị stack mới
        if (AudioService.Instance != null)
            AudioService.Instance.PlayStackShow();
            
        for (int i = 0; i < _stackPositionsParent.childCount; i++)
        {
            GenerateStack(_stackPositionsParent.GetChild(i));
        }
    }

    private void GenerateStack(Transform parent)
    {
        // Tạo stack ở vị trí thấp hơn vị trí cuối cùng
        Vector3 originalPosition = parent.position;
        Vector3 startPosition = originalPosition - new Vector3(0, _bounceHeight, 0);
        
        HexStack hexStack = Instantiate(_hexagonStackPrefab, startPosition, Quaternion.identity, parent);
        hexStack.name = $"Stack {parent.GetSiblingIndex()}";

        // Tạo các hexagon
        int amount = Random.Range(_minMaxHexCount.x, _minMaxHexCount.y);
        int firstColorHexagonCount = Random.Range(0, amount);

        Color[] colorArray = GetRandomColors();
        
        for (int i = 0; i < amount; i++)
        {
            Vector3 hexagonLocalPos = Vector3.up * i * .2f;
            Vector3 spawnPosition = hexStack.transform.TransformPoint(hexagonLocalPos);
            Hexagon hexagonInstance = Instantiate(_hexagonPrefab, spawnPosition, Quaternion.identity, hexStack.transform);
            
            hexagonInstance.Color = i < firstColorHexagonCount ? colorArray[0] : colorArray[1];
            hexagonInstance.SetHexStack(hexStack);
            hexStack.AddHexagon(hexagonInstance);
        }
        
        // Tạo hiệu ứng đàn hồi bằng cách di chuyển stack lên cao rồi về vị trí ban đầu
        AnimateStackBounce(hexStack, originalPosition);
    }
    
    private void AnimateStackBounce(HexStack hexStack, Vector3 targetPosition)
    {
        // Tính vị trí vượt quá (overshoot) - cao hơn vị trí mục tiêu
        Vector3 overshootPosition = targetPosition + new Vector3(0, _bounceHeight * 0.3f, 0);
        
        // Bước 1: Di chuyển stack từ vị trí thấp lên cao vượt quá vị trí mục tiêu
        LeanTween.move(hexStack.gameObject, overshootPosition, _spawnAnimationDuration * 0.4f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => {
                // Bước 2: Di chuyển về vị trí chính xác với hiệu ứng đàn hồi
                LeanTween.move(hexStack.gameObject, targetPosition, _spawnAnimationDuration * 0.6f)
                    .setEase(_bounceEaseType);
            });
    }

    private Color[] GetRandomColors()
    {
        if (_colors == null || _colors.Length < 2)
        {
            Debug.LogError("Not enough colors available");
            return null;
        }

        // Copy mảng vào danh sách để dễ thao tác
        List<Color> colorList = new List<Color>(_colors);

        // Lấy màu ngẫu nhiên đầu tiên
        int firstIndex = Random.Range(0, colorList.Count);
        Color firstColor = colorList[firstIndex];
        colorList.RemoveAt(firstIndex);

        // Lấy màu thứ hai (sau khi đã bỏ màu đầu)
        int secondIndex = Random.Range(0, colorList.Count);
        Color secondColor = colorList[secondIndex];

        return new Color[] { firstColor, secondColor };
    }
}

