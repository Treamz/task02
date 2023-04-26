using UnityEngine;
using QuintaEssenta.Library.DI;

public class CustomMenu : BaseBehaviour
{
    [GetComponent]
    protected GameObject _gameObject;

    public void Enable()
    {
        _gameObject.SetActive(true);
    }

    public void Disable()
    {
        _gameObject.SetActive(false);
    }
}
