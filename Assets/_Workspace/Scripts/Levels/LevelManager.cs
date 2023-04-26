using QuintaEssenta.Library.DI;

public class LevelManager : BaseBehaviour
{
    [Inject]
    private Plug _plug;

    [Inject]
    private Menu _menu;

    [Inject]
    private Game _game;

    [Inject]
    private IGameObject _iGameObject;

    private void Start()
    {
        _menu.Enable();
        _game.Disable();
        DisableGameObjects();

        _plug.Disable();
    }

    private void EnableGameObjects()
    {
        _iGameObject.Enable();
    }

    private void DisableGameObjects()
    {
        _iGameObject.Disable();
    }

    public void StartGame()
    {
        _menu.Disable();
        _game.Enable();
        EnableGameObjects();
    }

    public void ShowPlug()
    {
        _plug.Enable();
    }
}
