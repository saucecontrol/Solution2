using System;
using System.Runtime.Intrinsics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

public static unsafe partial class Premultiply
{
	public static void PremultiplyScalar(ReadOnlySpan<uint> pixels)
	{
		fixed(uint* ptr = &MemoryMarshal.GetReference(pixels))
		{
			PremultiplyScalar(ptr, (uint)pixels.Length);
		}
	}

	public static void PremultiplyAvx2(ReadOnlySpan<uint> pixels)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(pixels.Length, Vector256<uint>.Count);

		fixed (uint* ptr = &MemoryMarshal.GetReference(pixels))
		{
			PremultiplyAvx2(ptr, (uint)pixels.Length);
		}
	}

	public static void PremultiplyAdvSimd(ReadOnlySpan<uint> pixels)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(pixels.Length, Vector128<uint>.Count);

		fixed (uint* ptr = &MemoryMarshal.GetReference(pixels))
		{
			PremultiplyAdvSimd(ptr, (uint)pixels.Length);
		}
	}

	public static void PremultiplyXplatVector128(ReadOnlySpan<uint> pixels)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(pixels.Length, Vector128<uint>.Count);

		fixed (uint* ptr = &MemoryMarshal.GetReference(pixels))
		{
			PremultiplyXplatVector128(ptr, (uint)pixels.Length);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void PremultiplyScalar(uint* pixdata, nuint pixcnt)
	{
		byte* ptr = (byte*)pixdata;
		byte* end = (byte*)(pixdata + pixcnt);

		while (ptr < end)
		{
			uint a = ptr[3] * 0x8081u;
			uint r = ptr[2] * a >> 23;
			uint g = ptr[1] * a >> 23;
			uint b = ptr[0] * a >> 23;

			ptr[2] = (byte)r;
			ptr[1] = (byte)g;
			ptr[0] = (byte)b;
			ptr += sizeof(uint);
		}
	}
}
