using System.Diagnostics.CodeAnalysis;

namespace Blazor2048.Core;

/// <summary>
/// 関数型プログラミングスタイルのResult型
/// 例外を使わずに成功/失敗とデータを表現する
/// </summary>
public readonly record struct Result<T>
{
    private readonly T? _value;
    private readonly string _errorMessage;

    [MemberNotNullWhen(true, nameof(_value))]
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess { get; }

    public T Value => IsSuccess ? _value! : throw new InvalidOperationException($"Cannot access value of failed result: {_errorMessage}");

    public string ErrorMessage => IsSuccess ? string.Empty : _errorMessage;

    private Result(T value)
    {
        _value = value;
        _errorMessage = string.Empty;
        IsSuccess = true;
    }

    private Result(string errorMessage)
    {
        _value = default;
        _errorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        IsSuccess = false;
    }

    /// <summary>
    /// 成功結果を作成
    /// </summary>
    public static Result<T> Success(T value) => new(value);

    /// <summary>
    /// 失敗結果を作成
    /// </summary>
    public static Result<T> Failure(string errorMessage) => new(errorMessage);

    /// <summary>
    /// Monad Pattern: Map関数 (Functor)
    /// 成功時のみ関数を適用し、失敗時はそのまま伝播
    /// </summary>
    public Result<U> Map<U>(Func<T, U> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);
        
        return IsSuccess 
            ? Result<U>.Success(mapper(_value))
            : Result<U>.Failure(_errorMessage);
    }

    /// <summary>
    /// Monad Pattern: Bind関数 (Monad)
    /// 成功時のみ関数を適用し、結果をフラット化
    /// </summary>
    public Result<U> Bind<U>(Func<T, Result<U>> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);
        
        return IsSuccess 
            ? binder(_value)
            : Result<U>.Failure(_errorMessage);
    }

    /// <summary>
    /// 成功時のみ副作用のある処理を実行
    /// </summary>
    public Result<T> DoOnSuccess(Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        
        if (IsSuccess)
            action(_value);
        
        return this;
    }

    /// <summary>
    /// 失敗時のみ副作用のある処理を実行
    /// </summary>
    public Result<T> DoOnFailure(Action<string> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        
        if (!IsSuccess)
            action(_errorMessage);
        
        return this;
    }

    /// <summary>
    /// 値を取得するか、デフォルト値を返す
    /// </summary>
    public T GetValueOrDefault(T defaultValue = default!) => 
        IsSuccess ? _value : defaultValue;

    /// <summary>
    /// 値を取得するか、関数でデフォルト値を生成
    /// </summary>
    public T GetValueOrDefault(Func<string, T> defaultFactory)
    {
        ArgumentNullException.ThrowIfNull(defaultFactory);
        return IsSuccess ? _value : defaultFactory(_errorMessage);
    }

    /// <summary>
    /// 失敗を例外に変換
    /// </summary>
    public T GetValueOrThrow() => IsSuccess ? _value : throw new InvalidOperationException(_errorMessage);

    /// <summary>
    /// 失敗を指定した例外に変換
    /// </summary>
    public T GetValueOrThrow<TException>(Func<string, TException> exceptionFactory)
        where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);
        return IsSuccess ? _value : throw exceptionFactory(_errorMessage);
    }

    public static implicit operator Result<T>(T value) => Success(value);

    public override string ToString() => 
        IsSuccess ? $"Success({_value})" : $"Failure({_errorMessage})";
}

/// <summary>
/// 値を持たないResult型（Unit Result）
/// </summary>
public readonly record struct Result
{
    private readonly string _errorMessage;

    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess { get; }

    public string ErrorMessage => IsSuccess ? string.Empty : _errorMessage;

    private Result(bool isSuccess, string errorMessage = "")
    {
        IsSuccess = isSuccess;
        _errorMessage = errorMessage;
    }

    public static Result Success() => new(true);
    public static Result Failure(string errorMessage) => new(false, errorMessage ?? throw new ArgumentNullException(nameof(errorMessage)));

    public Result<T> Map<T>(Func<T> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        
        return IsSuccess 
            ? Result<T>.Success(factory())
            : Result<T>.Failure(_errorMessage);
    }

    public Result Bind(Func<Result> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);
        
        return IsSuccess ? binder() : this;
    }

    public Result<T> Bind<T>(Func<Result<T>> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);
        
        return IsSuccess 
            ? binder()
            : Result<T>.Failure(_errorMessage);
    }

    public Result DoOnSuccess(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        
        if (IsSuccess)
            action();
        
        return this;
    }

    public Result DoOnFailure(Action<string> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        
        if (!IsSuccess)
            action(_errorMessage);
        
        return this;
    }

    public void GetResultOrThrow()
    {
        if (!IsSuccess)
            throw new InvalidOperationException(_errorMessage);
    }

    public void GetResultOrThrow<TException>(Func<string, TException> exceptionFactory)
        where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);
        
        if (!IsSuccess)
            throw exceptionFactory(_errorMessage);
    }

    public override string ToString() => 
        IsSuccess ? "Success" : $"Failure({_errorMessage})";
}
