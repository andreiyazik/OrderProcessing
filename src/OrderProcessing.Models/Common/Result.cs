namespace OrderProcessing.Models.Common;

public class Result
{
    public readonly List<Error> _errors = [];

    public bool IsSucceeded { get; }
    public bool HasErrors => _errors.Any();
    public IReadOnlyCollection<Error> Errors => _errors;

    protected Result()
    {
        IsSucceeded = true;
    }

    public Result(Error error)
    {
        IsSucceeded = false;
        _errors.Add(error);
    }

    public Result(IEnumerable<Error> errors)
    {
        IsSucceeded = false;
        _errors.AddRange(errors);
    }
}

public class Result<TValue> : Result
{
    public TValue Value { get; private set; }

    public Result(TValue value) : base()
    {
        Value = value;
    }

    public Result(IEnumerable<Error> errors) : base(errors)
    {
    }
}
