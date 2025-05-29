using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Collider _collider;
    [SerializeField] private LeanTweenType _rotationEase;
    public HexStack HexStack { get; private set; }
    
    public Color Color
    {
        get => _renderer.material.color;
        set => _renderer.material.color = value;
    }
    
    public void SetHexStack(HexStack hexStack)
    {
        HexStack = hexStack;
    }

    public void DisableCollider()
    {
         _collider.enabled = false;
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    public void MoveToLocal(Vector3 targetLocalPos)
    {
        LeanTween.cancel(gameObject);

        float delay = transform.GetSiblingIndex() * .02f;
        LeanTween.moveLocal(gameObject, targetLocalPos, .25f)
            .setEase(LeanTweenType.easeInCubic)
            .setDelay(delay);

        Vector3 direction = (targetLocalPos - transform.localPosition).With(y:0).normalized;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        LeanTween.rotateAround(gameObject, rotationAxis, 180, 0.25f)
            .setEase(_rotationEase)
            .setDelay(delay);
    }

    public void Vanish(float delay)
    {
        LeanTween.cancel(gameObject);

        LeanTween.scale(gameObject, Vector3.zero, .2f)
            .setEase(LeanTweenType.easeInOutBack)
            .setDelay(delay)
            .setOnComplete(() => Destroy(gameObject));
    }
}
