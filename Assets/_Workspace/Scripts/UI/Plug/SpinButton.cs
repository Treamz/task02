public class SpinButton : CustomButton, IGetAnimation
{
    public event IGetAnimation.PlayAnimationHandler OnPlayAnimation;

    private void OnEnable() =>
        OnPlayAnimation?.Invoke();
}
