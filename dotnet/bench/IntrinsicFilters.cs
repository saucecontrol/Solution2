using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Attributes;

public class Avx2Attribute : FilterConfigBaseAttribute
{
	public Avx2Attribute() : base(new SimpleFilter(_ => Avx2.IsSupported)) { }
}

public class Sse2Attribute : FilterConfigBaseAttribute
{
	public Sse2Attribute() : base(new SimpleFilter(_ => Sse2.IsSupported)) { }
}

public class Arm64Attribute : FilterConfigBaseAttribute
{
	public Arm64Attribute() : base(new SimpleFilter(_ => AdvSimd.Arm64.IsSupported)) { }
}
