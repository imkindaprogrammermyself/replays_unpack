using System.Text.Json;
using ReplaysUnpack;

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: replays-unpack --replay <path>");
    return 1;
}

var replayPath = string.Empty;
for (var i = 0; i < args.Length; i++)
{
    if (args[i] == "--replay" && i + 1 < args.Length)
    {
        replayPath = args[i + 1];
    }
}

if (string.IsNullOrWhiteSpace(replayPath))
{
    Console.Error.WriteLine("Missing required argument --replay <path>");
    return 1;
}

var parser = new ReplayParser(replayPath);
var info = parser.GetInfo();
Console.WriteLine(JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true }));
return 0;
