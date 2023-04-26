using UnityEngine;

public class ScaleAnimation : MonoBehaviour
{
    private float _duration = 4f;

    private IGetAnimation _getAnimation;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
        _getAnimation = GetComponent<IGetAnimation>();

        _getAnimation.OnPlayAnimation += delegate ()
        {
            PlayAnimation();
        };
    }

    private void FixedUpdate()
    {
        if (_transform.localScale.x < 1f)
        {
            _transform.localScale = new Vector3(
                x: _transform.localScale.x + (_duration * Time.fixedDeltaTime),
                y: _transform.localScale.y + (_duration * Time.fixedDeltaTime),
                z: _transform.localScale.z + (_duration * Time.fixedDeltaTime));
        }
    }

    private void PlayAnimation()
    {
        _transform.localScale = Vector3.zero;
    }
}
