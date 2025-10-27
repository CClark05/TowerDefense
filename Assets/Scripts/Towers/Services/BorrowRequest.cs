public class BorrowRequest
{
    public readonly TowerDataHolder borrower;
    public readonly TowerDataHolder lender;
    public readonly SkillInstance instance;
    public int originalPlayCount { get; private set; }
    public int playCount;
    public BorrowRequest(TowerDataHolder borrower, TowerDataHolder lender, SkillInstance instance)
    {
        this.borrower = borrower;
        this.lender = lender;
        this.instance = instance;
        originalPlayCount = instance.PlayCount;
        playCount = originalPlayCount;
    }
    public bool fulfilled;
    
}