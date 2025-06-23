namespace Common.Domain.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
