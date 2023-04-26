using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ItemsController : MonoBehaviour
{
    public bool IsMove { get; set; }

    [SerializeField]
    private List<Item> _items = new List<Item>();

    private Vector3[] _defaultPositions =
    { 
        new Vector3(0, 1.3f, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, -1.3f, 0),
    };

    private IEnumerator StopMoveRoutine()
    {
        yield return new WaitForSeconds(3f);

        bool[] isFull = new bool[_defaultPositions.Length];

        for (int i = 0; i < _items.Count; i++)
        {
            _items[i].IsMove = false;

            int random = Random.Range(0, 3);

            if (isFull[random] == false)
            {
                _items[i].transform.localPosition = _defaultPositions[random];
                isFull[random] = true;
            }
            else
            {
                for (int k = 0; k < isFull.Length; k++)
                {
                    if (isFull[k] == false)
                    {
                        _items[i].transform.localPosition = _defaultPositions[k];
                        isFull[k] = true;
                        break;
                    }
                }
            }
        }

        IsMove = false;
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        for (int i = 0; i < _items.Count; i++)
            _items[i].MoveSpeed = moveSpeed;
    }

    public void Move()
    {
        if (IsMove == false)
        {
            for (int i = 0; i < _items.Count; i++)
                _items[i].IsMove = true;

            StartCoroutine(StopMoveRoutine());

            IsMove = true;
        }
    }
}
