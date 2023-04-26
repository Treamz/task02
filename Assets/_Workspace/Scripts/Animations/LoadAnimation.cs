using UnityEngine;

public class LoadAnimation : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 500;

    private Transform _transform;

    private float _rotationZ;

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        Vector3 rotation = new Vector3(0, 0, _rotationZ - _rotationSpeed * Time.deltaTime);

        _transform.Rotate(rotation);
    }
}
