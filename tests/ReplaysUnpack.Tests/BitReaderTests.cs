using ReplaysUnpack;

namespace ReplaysUnpack.Tests;

public sealed class BitReaderTests
{
    [Fact]
    public void ReadBitsNormallyAllArray()
    {
        using var stream = new MemoryStream([0x01]);
        var bitReader = new BitReader(stream);

        Assert.Equal(0, bitReader.Get(1));
        Assert.Equal(1, bitReader.Get(7));
        Assert.Empty(bitReader.GetRest());
    }

    [Fact]
    public void ReadBitsNormallyOnlyFirstByte()
    {
        using var stream = new MemoryStream([0xF0, 0x02]);
        var bitReader = new BitReader(stream);

        Assert.Equal(1, bitReader.Get(1));
        Assert.Equal(3, bitReader.Get(2));
        Assert.Equal(2, bitReader.Get(2));
        Assert.Equal([0x02], bitReader.GetRest());
    }

    [Fact]
    public void ReadBitsNormallyTwoBytes()
    {
        using var stream = new MemoryStream([0xF0, 0x02]);
        var bitReader = new BitReader(stream);

        Assert.Equal(1, bitReader.Get(1));
        Assert.Equal(3, bitReader.Get(2));
        Assert.Equal(2, bitReader.Get(2));
        Assert.Equal(0, bitReader.Get(8));
        Assert.Equal(2, bitReader.Get(3));
        Assert.Empty(bitReader.GetRest());
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(4, 2)]
    [InlineData(5, 3)]
    public void BitsRequired(int objectSize, int bits)
    {
        Assert.Equal(bits, BitReader.BitsRequired(objectSize));
    }
}
