using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Org.BouncyCastle.Crypto.Engines;

namespace ReplaysUnpack;

public sealed class ReplayReader
{
    private static readonly byte[] ReplaySignature = [0x12, 0x32, 0x34, 0x11];

    private static readonly byte[] WowsBlowfishKey = [
        0x29, 0xB7, 0xC9, 0x09, 0x38, 0x3F, 0x84, 0x88,
        0xFA, 0x98, 0xEC, 0x4E, 0x13, 0x19, 0x79, 0xFB
    ];

    private static readonly byte[] WowpBlowfishKey = [
        0xDE, 0x72, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF,
        0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF
    ];

    private static readonly byte[] WotBlowfishKey = [
        0xDE, 0x72, 0xBE, 0xA0, 0xDE, 0x04, 0xBE, 0xB1,
        0xDE, 0xFE, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF
    ];

    private static readonly Dictionary<string, byte[]> TypeToKey = new(StringComparer.OrdinalIgnoreCase)
    {
        ["wowsreplay"] = WowsBlowfishKey,
        ["wotreplay"] = WotBlowfishKey,
        ["wowpreplay"] = WowpBlowfishKey,
    };

    private readonly string _replayPath;
    private readonly bool _dumpBinaryData;
    private readonly string _type;

    public ReplayReader(string replayPath, bool dumpBinary = false)
    {
        _replayPath = replayPath;
        _dumpBinaryData = dumpBinary;

        if (!File.Exists(_replayPath))
        {
            throw new FileNotFoundException($"File does not exist: {_replayPath}");
        }

        _type = Path.GetExtension(_replayPath).TrimStart('.');
        if (!TypeToKey.ContainsKey(_type))
        {
            throw new ArgumentException("Replay must use one of these extensions: wowsreplay, wotreplay, wowpreplay");
        }
    }

    public ReplayInfo GetReplayData(bool isCompressed = true)
    {
        using var stream = File.OpenRead(_replayPath);
        using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);

        var signature = reader.ReadBytes(4);
        if (!signature.SequenceEqual(ReplaySignature))
        {
            throw new InvalidDataException($"File {_replayPath} is not a valid replay");
        }

        var blocksCount = reader.ReadInt32();

        var engineBlockSize = reader.ReadInt32();
        var engineData = ParseJsonToDictionary(reader.ReadBytes(engineBlockSize));

        var extraData = new List<object?>();
        for (var i = 0; i < blocksCount - 1; i++)
        {
            var blockSize = reader.ReadInt32();
            var block = reader.ReadBytes(blockSize);
            if (block.Length == 0)
            {
                extraData.Add(null);
            }
            else
            {
                extraData.Add(JsonSerializer.Deserialize<object>(block));
            }
        }

        var game = _type.ToLowerInvariant() switch
        {
            "wowsreplay" => "wows",
            "wotreplay" => "wot",
            "wowpreplay" => "wowp",
            _ => throw new InvalidOperationException("Unsupported replay type")
        };

        var trailingData = reader.ReadBytes((int)(stream.Length - stream.Position));
        var decryptedData = isCompressed
            ? Decompress(DecryptData(trailingData))
            : trailingData;

        if (_dumpBinaryData)
        {
            var output = $"{Path.GetFileName(_replayPath)}.hex";
            File.WriteAllBytes(output, decryptedData);
        }

        return new ReplayInfo(game, engineData, extraData, decryptedData);
    }

    private byte[] DecryptData(byte[] dirtyData)
    {
        var engine = new BlowfishEngine();
        engine.Init(false, new Org.BouncyCastle.Crypto.Parameters.KeyParameter(TypeToKey[_type]));

        using var output = new MemoryStream();

        long? previousBlock = null;
        for (var index = 0; index + 8 <= dirtyData.Length; index += 8)
        {
            if (index == 0)
            {
                continue;
            }

            var source = dirtyData.AsSpan(index, 8).ToArray();
            var target = new byte[8];
            engine.ProcessBlock(source, 0, target, 0);

            var decryptedBlock = BitConverter.ToInt64(target, 0);
            if (previousBlock.HasValue)
            {
                decryptedBlock ^= previousBlock.Value;
            }

            previousBlock = decryptedBlock;
            output.Write(BitConverter.GetBytes(decryptedBlock));
        }

        return output.ToArray();
    }

    private static byte[] Decompress(byte[] compressed)
    {
        using var input = new MemoryStream(compressed);
        using var zlib = new ZLibStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        zlib.CopyTo(output);
        return output.ToArray();
    }

    private static Dictionary<string, object?> ParseJsonToDictionary(byte[] bytes)
    {
        using var doc = JsonDocument.Parse(bytes);
        return (Dictionary<string, object?>)ConvertElement(doc.RootElement)!;
    }

    private static object? ConvertElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => element.EnumerateObject().ToDictionary(p => p.Name, p => ConvertElement(p.Value)),
            JsonValueKind.Array => element.EnumerateArray().Select(ConvertElement).ToList(),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt64(out var i) ? i : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.ToString(),
        };
    }
}
