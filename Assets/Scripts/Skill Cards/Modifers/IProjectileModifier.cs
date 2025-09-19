using System.Collections;

public interface IProjectileModifier 
{
    public IEnumerator Modify(ProjectileShotData shotData);
    public bool DelayShot { get; }
}