using System;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

using Xunit;

public class PremultiplyTests
{
	private uint[] _allValues;
	private uint[] _allValuesPremultiplied;
	private uint[] _randomValues;

	private static void PremultiplyReference(Span<uint> data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			uint argb = data[i];

			uint a = argb >> 24;
			uint r = argb >> 16 & 0xff;
			uint g = argb >>  8 & 0xff;
			uint b = argb       & 0xff;

			b = b * a / byte.MaxValue;
			g = g * a / byte.MaxValue;
			r = r * a / byte.MaxValue;

			data[i] = a << 24 | r << 16 | g << 8 | b;
		}
	}

	public PremultiplyTests()
	{
		_allValues = new uint[256 * 256];
		for (uint i = 0; i < 256; i++)
		{
			for (uint j = 0; j < 256; j++)
			{
				_allValues[i * 256 + j] = i << 24 | j << 16 | j << 8 | j;
			}
		}

		_randomValues = new uint[256];
		var random = new Random(32897);
		random.NextBytes(MemoryMarshal.AsBytes(_randomValues.AsSpan()));

		_allValuesPremultiplied = [.. _allValues];
		PremultiplyReference(_allValuesPremultiplied);
	}

	private (uint[], uint[]) ConvertLengthLimited(Action<ReadOnlySpan<uint>> convertFunc, int length)
	{
		uint[] refValues = [.. _randomValues];
		PremultiplyReference(refValues.AsSpan(0, length));

		uint[] tstValues = [.. _randomValues];
		convertFunc(tstValues.AsSpan(0, length));

		return (refValues, tstValues);
	}

	[Fact]
	public void AllValuesScalar()
	{
		uint[] tstValues = [.. _allValues];
		Premultiply.PremultiplyScalar(tstValues);

		Assert.Equal(tstValues, _allValuesPremultiplied);
	}

	[IntrinsicFact<Avx2>]
	public void AllValuesAvx2()
	{
		uint[] tstValues = [.. _allValues];
		Premultiply.PremultiplyAvx2(tstValues);

		Assert.Equal(tstValues, _allValuesPremultiplied);
	}

	[IntrinsicFact<AdvSimd.Arm64>]
	public void AllValuesAdvSimd()
	{
		uint[] tstValues = [.. _allValues];
		Premultiply.PremultiplyAdvSimd(tstValues);

		Assert.Equal(tstValues, _allValuesPremultiplied);
	}

	[Fact]
	public void AllValuesXplatVector128()
	{
		uint[] tstValues = [.. _allValues];
		Premultiply.PremultiplyXplatVector128(tstValues);

		Assert.Equal(tstValues, _allValuesPremultiplied);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(33)]
	[InlineData(240)]
	[InlineData(256)]
	public void LengthLimitedScalar(int len)
	{
		var (refValues, tstValues) = ConvertLengthLimited(Premultiply.PremultiplyScalar, len);

		Assert.Equal(tstValues, refValues);
	}

	[IntrinsicTheory<Avx2>]
	[InlineData(32)]
	[InlineData(33)]
	[InlineData(63)]
	[InlineData(65)]
	[InlineData(240)]
	[InlineData(256)]
	public void LengthLimitedAvx2(int len)
	{
		var (refValues, tstValues) = ConvertLengthLimited(Premultiply.PremultiplyAvx2, len);

		Assert.Equal(tstValues, refValues);
	}

	[IntrinsicTheory<AdvSimd.Arm64>]
	[InlineData(16)]
	[InlineData(17)]
	[InlineData(31)]
	[InlineData(33)]
	[InlineData(240)]
	[InlineData(256)]
	public void LengthLimitedAdvSimd(int len)
	{
		var (refValues, tstValues) = ConvertLengthLimited(Premultiply.PremultiplyAdvSimd, len);

		Assert.Equal(tstValues, refValues);
	}

	[Theory]
	[InlineData(16)]
	[InlineData(17)]
	[InlineData(31)]
	[InlineData(33)]
	[InlineData(240)]
	[InlineData(256)]
	public void LengthLimitedXplatVector128(int len)
	{
		var (refValues, tstValues) = ConvertLengthLimited(Premultiply.PremultiplyXplatVector128, len);

		Assert.Equal(tstValues, refValues);
	}
}
