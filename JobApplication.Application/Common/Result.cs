namespace JobApplication.Application.Common
{
    public enum ErrorType { None, NotFound, Validation, Conflict, Forbidden, Unauthorized }

    public class Result<T>
    {
        public T? Value { get; private set; }
        public ErrorType Error { get; private set; }
        public string? Message { get; private set; }
        public bool IsSuccess => Error == ErrorType.None;

        public static Result<T> Ok(T value) => new() { Value = value };
        public static Result<T> Fail(ErrorType error, string message) =>
            new() { Error = error, Message = message };
    }
}
