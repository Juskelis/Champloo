public class OnKillerDash : OnDash {
    protected override void OnStart()
    {
        base.OnStart();
        currentDashes = 0;
    }

    protected override void OnCooledDown()
    {
        currentDashes = 1;
        base.OnCooledDown();
    }
}
