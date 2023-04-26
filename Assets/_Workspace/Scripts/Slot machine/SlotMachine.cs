using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuintaEssenta.Library.DI;

public class SlotMachine : BaseBehaviour, IGetAnimation, IGameObject
{
    [SerializeField]
    private float _spinSpeed;

    [SerializeField]
    private float _spinDuration;

    [SerializeField]
    private List<ItemsController> _itemsControllers = new List<ItemsController>();

    public event IGetAnimation.PlayAnimationHandler OnPlayAnimation;

    [GetComponent]
    private GameObject _gameObject;

    private void OnEnable() =>
        OnPlayAnimation?.Invoke();

    private void Start()
    {
        for (int i = 0; i < _itemsControllers.Count; i++)
            _itemsControllers[i].SetMoveSpeed(_spinSpeed);

        Dopita(); ///////////////////////////////////////////////////////////////////////////
    }

    private IEnumerator SpinDuration()
    {
        for (int i = 0; i < _itemsControllers.Count; i++)
        {
            _itemsControllers[i].Move();

            yield return new WaitForSeconds(_spinDuration);
        }
    }

    public void Spin()
    {
        if (_itemsControllers[_itemsControllers.Count-1].IsMove == false)
            StartCoroutine(SpinDuration());
    }

    public void Enable()
    {
        _gameObject.SetActive(true);
    }

    public void Disable()
    {
        _gameObject.SetActive(false);
    }

    //////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ///////////////////
    /// </summary>
    //////////////////////////////////////////////////////////////////////

    private void Dopita()
    {
        int atnot = 2;
        int levitan = 3;
        int krahoobr = 12;
        int dilenatn = 22;

        atnot = levitan + krahoobr + dilenatn;

        Nepisi();
    }

    private void Nepisi()
    {
        var parito = "2";
        var obitp = "3";
        var digma = ":";

        parito += obitp;

        Kachek();
    }

    private void Kachek()
    {
        float vezifi;
        float karlson = 1;
        float riju = 2.2f;
        float napapijri = 3.3f;

        vezifi = karlson * riju * napapijri;

        Tapok();
    }

    private void Tapok()
    {
        string aldos = "22222";
        string krita = "33333";
        string misrtka = "44444";
        string lopasti = "55555";

        aldos = krita;
        krita = misrtka;
        lopasti = aldos;

        Mika();
    }

    private void Mika()
    {
        float gajil = 2.2f;
        float india = 3.3f;
        float miksruta = 4.4f;
        float mahito = 5.5f;

        float bushe = Nagano(gajil, india, miksruta, mahito);
    }

    private float Nagano(float one, float two, float three, float four)
    {
        return one + two + three + four;
    }
}
