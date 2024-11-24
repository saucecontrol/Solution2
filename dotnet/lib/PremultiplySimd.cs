using System.Diagnostics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Runtime.CompilerServices;

/*
SIMD forms take advantage of the fact that we can easily multiply by 256 or 257
This approximation yields correct results for all combinations of color/alpha values:
  a * c * 0x8081 >> 23 ~= 257a * (256c + 1) >> 24

This diagram explains the cost savings for the x86 form:

                ______________________________ The actual alpha multiply
               /
              /
(alpha * 257 * ((color << 8) + 1) >> 16) >> 8
        \               \     \    \      \
         \               \     \    \      \__ Free as part of narrowing word to byte
          \               \     \    \
           \               \     \    \_______ Free as part of `pmulhuw`
            \               \     \
             \               \     \__________ Free for odd bytes if included in "set alpha to 1" mask
              \               \
               \               \______________ Free as part of widening byte to word
                \
                 \____________________________ Free as part of `pshufb` to broadcast alpha
*/

public static unsafe partial class Premultiply
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void PremultiplyAvx2(uint* pixdata, nuint pixcnt)
	{
		Debug.Assert(pixcnt >= (uint)Vector256<uint>.Count);

		var vmaska = Vector256.Create(Vector128.Create(0x03030303, 0x07070707, 0x0b0b0b0b, 0x0f0f0f0f).AsByte());
		var vmaskh = Vector256.Create((ushort)0xff00);
		var vmasko = Vector256.Create(0xffff0001u).AsUInt16();
		var vmaske = Vector256<ushort>.One;

		uint* ptr = pixdata;
		uint* end = pixdata + pixcnt - Vector256<uint>.Count;

		var vlast = Vector256.Load(end);
		LoopTop:

		do
		{
			var vi = Vector256.Load(ptr);
			var va = Avx2.Shuffle(vi.AsByte(), vmaska).AsUInt16();
			var ve = vi.AsUInt16() << 8     | vmaske;
			var vo = vi.AsUInt16() & vmaskh | vmasko;

			ve = Avx2.MultiplyHigh(ve, va) >> 8;
			vo = Avx2.MultiplyHigh(vo, va) & vmaskh;

			(vo | ve).AsUInt32().Store(ptr);
			ptr += Vector256<uint>.Count;
		}
		while (ptr <= end);

		if (ptr < end + Vector256<uint>.Count)
		{
			ptr = end;
			vlast.Store(ptr);
			goto LoopTop;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void PremultiplyAdvSimd(uint* pixdata, nuint pixcnt)
	{
		Debug.Assert(pixcnt >= (uint)Vector128<uint>.Count);

		var vmaska = Vector128.Create(0x03030303, 0x07070707, 0x0b0b0b0b, 0x0f0f0f0f).AsByte();
		var vmaskh = Vector128.Create((ushort)0xff00);
		var vmasko = Vector128.Create(0xffff0001u).AsUInt16();
		var vmaske = Vector128<ushort>.One;

		uint* ptr = pixdata;
		uint* end = pixdata + pixcnt - Vector128<uint>.Count;

		var vlast = Vector128.Load(end);
		LoopTop:

		do
		{
			var vi = Vector128.Load(ptr);
			var va = AdvSimd.Arm64.VectorTableLookup(vi.AsByte(), vmaska).AsUInt16();
			var ve = vi.AsUInt16() << 8     | vmaske;
			var vo = vi.AsUInt16() & vmaskh | vmasko;

			var vel = AdvSimd.MultiplyWideningLower(ve.GetLower(), va.GetLower());
			var veh = AdvSimd.MultiplyWideningUpper(ve, va);
			var vol = AdvSimd.MultiplyWideningLower(vo.GetLower(), va.GetLower());
			var voh = AdvSimd.MultiplyWideningUpper(vo, va);

			ve = AdvSimd.ShiftRightLogicalNarrowingUpper(AdvSimd.ShiftRightLogicalNarrowingLower(vel, 16), veh, 16);
			vo = AdvSimd.ShiftRightLogicalNarrowingUpper(AdvSimd.ShiftRightLogicalNarrowingLower(vol, 16), voh, 16);

			ve >>= 8;
			vo &= vmaskh;

			(vo | ve).AsUInt32().Store(ptr);
			ptr += Vector128<uint>.Count;
		}
		while (ptr <= end);

		if (ptr < end + Vector128<uint>.Count)
		{
			ptr = end;
			vlast.Store(ptr);
			goto LoopTop;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void PremultiplyXplatVector128(uint* pixdata, nuint pixcnt)
	{
		Debug.Assert(pixcnt >= (uint)Vector128<uint>.Count);

		var vmaska = Vector128.Create(0x03030303, 0x07070707, 0x0b0b0b0b, 0x0f0f0f0f).AsByte();
		var vmaskh = Vector128.Create((ushort)0xff00);
		var vmasko = Vector128.Create(0xffff0001u).AsUInt16();
		var vmaske = Vector128<ushort>.One;

		uint* ptr = pixdata;
		uint* end = pixdata + pixcnt - Vector128<uint>.Count;

		var vlast = Vector128.Load(end);
		LoopTop:

		do
		{
			var vi = Vector128.Load(ptr).AsByte();
			var va = Vector128.Shuffle(vi, vmaska).AsUInt16();
			var ve = vi.AsUInt16() << 8     | vmaske;
			var vo = vi.AsUInt16() & vmaskh | vmasko;

			var (val, vah) = Vector128.Widen(va);
			var (vel, veh) = Vector128.Widen(ve);
			var (vol, voh) = Vector128.Widen(vo);

			vel = vel * val >> 24;
			veh = veh * vah >> 24;
			vol = vol * val >> 16;
			voh = voh * vah >> 16;

			ve = Vector128.Narrow(vel, veh);
			vo = Vector128.Narrow(vol, voh) & vmaskh;

			(vo | ve).AsUInt32().Store(ptr);
			ptr += Vector128<uint>.Count;
		}
		while (ptr <= end);

		if (ptr < end + Vector128<uint>.Count)
		{
			ptr = end;
			vlast.Store(ptr);
			goto LoopTop;
		}
	}
}
