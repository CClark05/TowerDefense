public interface IOnHit 
{
    public void OnHit(HitData hitData);
    public int Priority => DelayDamage ? 1: 0; //Set to 1 for delayed damage 
    public bool DelayDamage => false;
}
