public class BorrowRequest
{
    public TowerDataHolder borrower;
    public TowerDataHolder lender;
    public SkillData card;
    public bool fulfilled;
    public int playCount = 1;
}