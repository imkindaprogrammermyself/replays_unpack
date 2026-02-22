using System.Collections.Generic;

namespace ReplaysUnpack;

public sealed class BitReader
{
    private readonly Stream _stream;
    private readonly Queue<int> _bitsCache = new();
    private int _readBits;

    public BitReader(Stream stream)
    {
        _stream = stream;
    }

    public BitReader(byte[] data) : this(new MemoryStream(data))
    {
    }

    public static int BitsRequired(int length)
    {
        if (length < 1)
        {
            return 0;
        }

        return (int)Math.Ceiling(Math.Log(length, 2));
    }

    public int BytesRead => (int)Math.Ceiling(_readBits / 8.0);

    public byte[] GetRest()
    {
        using var output = new MemoryStream();
        _stream.CopyTo(output);
        return output.ToArray();
    }

    public int Get(int nbits)
    {
        if (nbits == 0)
        {
            return 0;
        }

        var value = 0;
        while (nbits > 0)
        {
            var bit = GetNextBit();
            value = (value << 1) | bit;
            nbits--;
        }

        return value;
    }

    private int GetNextBit()
    {
        if (_bitsCache.Count == 0)
        {
            var nextByte = _stream.ReadByte();
            if (nextByte == -1)
            {
                throw new InvalidOperationException($"End of stream at bit {_readBits}.");
            }

            for (var i = 7; i >= 0; i--)
            {
                _bitsCache.Enqueue((nextByte >> i) & 1);
            }
        }

        _readBits++;
        return _bitsCache.Dequeue();
    }
}
