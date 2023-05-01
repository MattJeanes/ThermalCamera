namespace ThermalCamera.App.Data;

public class DataResult
{
    protected bool _success;
    protected string _errorMessage;

    public DataResult()
    {
        _success = true;
    }

    public DataResult(string errorMessage)
    {
        _errorMessage = errorMessage;
    }

    public bool Success => _success;
    public string ErrorMessage => _success ? throw new InvalidOperationException() : _errorMessage!;

    public static DataResult GetSuccess()
    {
        return new DataResult();
    }

    public static DataResult Failure(string errorMessage)
    {
        return new DataResult(errorMessage);
    }

    public static DataResult<T> GetSuccess<T>(T result)
    {
        return new DataResult<T>(result);
    }

    public static DataResult<T> GetFailure<T>(string errorMessage)
    {
        return new DataResult<T>(errorMessage);
    }
}


public class DataResult<T> : DataResult
{
    protected T _result;

    public T Result => _success ? _result! : throw new InvalidOperationException();

    public DataResult(T result)
    {
        _result = result;
    }

    public DataResult(string errorMessage) : base(errorMessage) { }

    public DataResult() : base() { }
}
