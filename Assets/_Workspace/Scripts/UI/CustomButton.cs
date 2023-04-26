using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using QuintaEssenta.Library.DI;

public class CustomButton : BaseBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private float _offset = 10;

    public UnityEvent OnClick;

    private RectTransform _rectTransform;

    private Vector3 _startPosition;

    protected override void Awake()
    {
        base.Awake();

        _rectTransform = GetComponent<RectTransform>();
        _startPosition = _rectTransform.anchoredPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition = new Vector3(
            x: _rectTransform.anchoredPosition.x,
            y: _rectTransform.anchoredPosition.y - _offset);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition = _startPosition;
    }
}
