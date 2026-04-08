using System;

namespace GMenu.Models.Common;

public readonly struct Result<TResult, TValue>(TResult result, TValue? value = default) where TResult : Enum
{
    public TValue? Value { get; } = value;
    public TResult ActualResult { get; } = result;
}