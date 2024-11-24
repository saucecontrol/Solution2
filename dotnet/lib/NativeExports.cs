using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

public unsafe class NativeExports
{
	[UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)], EntryPoint = "PremultiplyScalar")]
	public static void PremultiplyScalarExport(uint* pixdata, nuint pixcnt)
	{
		Premultiply.PremultiplyScalar(pixdata, pixcnt);
	}

	[UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)], EntryPoint = "PremultiplySimd")]
	public static void PremultiplySimdExport(uint* pixdata, nuint pixcnt)
	{
		if (Avx2.IsSupported && pixcnt >= (uint)Vector256<uint>.Count)
		{
			Premultiply.PremultiplyAvx2(pixdata, pixcnt);
			return;
		}

		if (AdvSimd.IsSupported && pixcnt >= (uint)Vector128<uint>.Count)
		{
			Premultiply.PremultiplyAdvSimd(pixdata, pixcnt);
			return;
		}

		Premultiply.PremultiplyScalar(pixdata, pixcnt);
	}
}