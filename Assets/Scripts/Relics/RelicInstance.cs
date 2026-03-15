public abstract class RelicInstance 
{
    protected RelicInstance(RelicData data)
    {
        Data = data;
    }
    public RelicData Data { get; }
    public abstract void OnPickup();
}