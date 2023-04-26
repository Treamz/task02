using UnityEngine;

public class Item : MonoBehaviour
{
    public float MoveSpeed { get; set; } = 0;
    public bool IsMove { get; set; } = false;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void FixedUpdate()
    {
        if (IsMove == true)
        {
            Move();
            ResetPosition();
        }
    }

    private void Move()
    {
        _transform.Translate(Vector3.down * MoveSpeed * Time.fixedDeltaTime);
    }

    private void ResetPosition()
    {
        if (_transform.localPosition.y <= -2.6f)
        {
            _transform.localPosition = new Vector3(0, 1.3f, 0);
        }
    }
}
