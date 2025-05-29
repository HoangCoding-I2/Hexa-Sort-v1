using NaughtyAttributes;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Hexagon _hexagonPrefab;
    [SerializeField] private Renderer _cellRenderer;

    [Header(" Settings ")]
    [OnValueChanged("GenerateInitialHexagons")]
    [SerializeField] private Color[] _hexagonColors;
    
    [Header(" Highlight Settings ")]
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _highlightColor;
    [SerializeField] private float _highlightSpeed = 5f;

    private Color _currentColor;
    private Color _targetColor;

    public HexStack Stack { get; private set; }
    public bool IsOccupied => Stack != null;

    private void Awake()
    {
        if (_cellRenderer == null)
            _cellRenderer = GetComponent<Renderer>();
            
        _currentColor = _normalColor;
        _targetColor = _normalColor;
        
        if (_cellRenderer != null)
            _cellRenderer.material.color = _normalColor;
    }

    private void Update()
    {
        if (_cellRenderer != null && _currentColor != _targetColor)
        {
            _currentColor = Color.Lerp(_currentColor, _targetColor, Time.deltaTime * _highlightSpeed);
            _cellRenderer.material.color = _currentColor;
        }
    }

    public void SetStack(HexStack stack)
    {
        Stack = stack;
        SetHighlight(false);
    }

    public void SetHighlight(bool highlight)
    {
        _targetColor = highlight ? _highlightColor : _normalColor;
    }

    private void Start()
    {
        if(_hexagonColors.Length > 0)
        {
            GenerateInitialHexagons();
        }
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
