
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework.Internal;
using Unity.Networking.Transport.TLS;
using UnityEngine.UIElements.Experimental;

public struct Optional<T>
{
    public static readonly Optional<T> NoValue = new Optional<T>();

    readonly bool hasValue;
    readonly T value;

    public Optional(T value)
    {
        this.value = value;
        hasValue = true;
    }

    public T Value => hasValue ? value : throw new System.InvalidOperationException("Optional has no value");
    public bool HasValue => hasValue;

    public T GetValueOrDefault() => value; // if hasValue is false, it will return the default value of T
    public T GetValueOrDefault(T defaultValue) => hasValue ? value : defaultValue; // if hasValue is false, it will return the default value of T

    public TResult Match<TResult>(Func<T, TResult> OnValue, Func<TResult> OnNoValue)
        => hasValue ? OnValue(value) : OnNoValue();
    public void Match(Action<T> OnValue, Action OnNoValue)
    {
        if (hasValue)
            OnValue(value);
        else
            OnNoValue();
    }


    public Optional<TResult> Select<TResult>(Func<T, TResult> map)
        => hasValue ? new Optional<TResult>(map(value)) : Optional<TResult>.NoValue;

    public Optional<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> bind)
        => hasValue ? bind(value) : Optional<TResult>.NoValue;

    public static Optional<TResult> Combine<T1, T2, TResult>(Optional<T1> first, Optional<T2> second, Func<T1, T2, TResult> combiner)
    {
        if (first.HasValue && second.HasValue)
        {
            return new Optional<TResult>(combiner(first.value, second.value));
        }
        return Optional<TResult>.NoValue;
    }

    public static Optional<T> Some(T value) => new Optional<T>(value);
    public static Optional<T> None() => NoValue;

    public override bool Equals(object obj) => obj is Optional<T> other && Equals(other);

    public override int GetHashCode() => (hasValue.GetHashCode() * 397) ^ EqualityComparer<T>.Default.GetHashCode(value);

    public override string ToString() => hasValue ? $"Some({value})" : "None";

    public static implicit operator Optional<T>(T value) => new Optional<T>(value);

    public static implicit operator bool(Optional<T> value) => value.hasValue;

    public static implicit operator T(Optional<T> value) => value.value;



}


