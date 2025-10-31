public class CompositePredicate : IPredicate
{
    private readonly IPredicate[] predicates;

    public CompositePredicate(params IPredicate[] predicates)
    {
        this.predicates = predicates;
    }

    public bool Evaluate()
    {
        foreach (var predicate in predicates)
        {
            if (!predicate.Evaluate())
                return false;
        }
        return true;
    }
}