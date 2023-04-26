using UnityEngine;

public class MoveAnimation : MonoBehaviour
{
    private float _duration = 4000f;
    private float _offset = 500;

    private IGetAnimation _getAnimation;
    private RectTransform _rectTransform;

    private Vector2 _startPosition;

    [SerializeField]
    private bool _isComplete = false;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _startPosition = _rectTransform.anchoredPosition;
        _getAnimation = GetComponent<IGetAnimation>();

        _getAnimation.OnPlayAnimation += delegate ()
        {
            PlayAnimation();
        };
    }

    private void FixedUpdate()
    {
        if (_isComplete == false)
            Move();
    }

    private void Move()
    {
        if (Vector3.Distance(_rectTransform.position, _startPosition) > 0.1f)
        {
            _rectTransform.anchoredPosition = Vector3.MoveTowards(
                _rectTransform.anchoredPosition,
                _startPosition,
                _duration * Time.fixedDeltaTime);
        }
        
        if (_rectTransform.anchoredPosition == _startPosition)
        {
            _isComplete = true;
        }
    }

    private void PlayAnimation()
    {
        _rectTransform.anchoredPosition = new Vector3(
            x: _rectTransform.anchoredPosition.x,
            y: _rectTransform.anchoredPosition.y - _offset);
    }
}
