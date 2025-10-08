namespace Api.Infrastructure.Utilities;

public readonly struct Either<TLeft, TRight>
{
    private readonly TLeft? _left;
    private readonly TRight? _right;
    public bool IsLeft { get; }
    public bool IsRight => !IsLeft;

    public TLeft Left => IsLeft
        ? _left!
        : throw new InvalidOperationException("Either does not contain a Left value.");

    public TRight Right => IsRight
        ? _right!
        : throw new InvalidOperationException("Either does not contain a Right value.");

    public Either(TLeft left)
    {
        _left = left;
        _right = default;
        IsLeft = true;
    }

    public Either(TRight right)
    {
        _right = right;
        _left = default;
        IsLeft = false;
    }

    public override string ToString() =>
        IsLeft ? $"{_left}" : $"{_right}";

    // Map allows functional-style transformations
    public Either<TNewLeft, TNewRight> Map<TNewLeft, TNewRight>(
        Func<TLeft, TNewLeft> mapLeft,
        Func<TRight, TNewRight> mapRight)
    {
        return IsLeft
            ? new Either<TNewLeft, TNewRight>(mapLeft(_left!))
            : new Either<TNewLeft, TNewRight>(mapRight(_right!));
    }

    // Implicit conversions make it easy to assign directly
    public static implicit operator Either<TLeft, TRight>(TLeft left) => new(left);
    public static implicit operator Either<TLeft, TRight>(TRight right) => new(right);
}
