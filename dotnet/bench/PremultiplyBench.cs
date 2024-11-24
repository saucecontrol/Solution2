using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;

BenchmarkRunner.Run<PremultiplyBench>();

[ShortRunJob]
[DisassemblyDiagnoser]
public class PremultiplyBench
{
	private uint[] Pixels = new uint[1 << 16];

	[Benchmark(Baseline = true)]
	public void Scalar() => Premultiply.PremultiplyScalar(Pixels);

	[Benchmark, Avx2]
	public void Avx2() => Premultiply.PremultiplyAvx2(Pixels);

	[Benchmark, Arm64]
	public void AdvSimd() => Premultiply.PremultiplyAdvSimd(Pixels);

	[Benchmark]
	public void XplatVector128() => Premultiply.PremultiplyXplatVector128(Pixels);
}
