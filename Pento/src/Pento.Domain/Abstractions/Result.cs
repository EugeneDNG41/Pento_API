﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Pento.Domain.Abstractions;

public class Result
{
    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Failure<TValue>(Error error) =>
        new(default, false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    public static implicit operator TValue(Result<TValue> result) => result.Value;

    public static implicit operator Task<TValue>(Result<TValue> result) =>
        Task.FromResult(result.Value);


    public async Task<TValue> ToValueAsync()
    {
        return await Task.FromResult(Value);
    }

    public Task<Result<TValue>> ToTaskAsync()
    {
        return Task.FromResult(this);
    }
    public static Result<TValue> ValidationFailure(Error error) =>
        new(default, false, error);
}
