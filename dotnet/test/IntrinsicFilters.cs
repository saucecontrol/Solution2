using System;
using System.Reflection;

using Xunit;

public sealed class IntrinsicFactAttribute<TIntrinsic> : FactAttribute
{
	public IntrinsicFactAttribute()
	{
		if (!typeof(TIntrinsic).IsSupported())
			Skip = "HWIntrinsic not supported.";
	}
}

public sealed class IntrinsicTheoryAttribute<TIntrinsic> : TheoryAttribute
{
	public IntrinsicTheoryAttribute()
	{
		if (!typeof(TIntrinsic).IsSupported())
			Skip = "HWIntrinsic not supported.";
	}
}

public static class TypeExtensions
{
	public static bool IsSupported(this Type type) =>
		(bool)type.GetProperty(nameof(IsSupported), BindingFlags.Static | BindingFlags.Public)!.GetValue(null)!;
}