public class DrunkenMelee : Sword
{
    protected override void OnEndSpecial()
    {
        base.OnEndSpecial();
        InHand = true;
    }
}
