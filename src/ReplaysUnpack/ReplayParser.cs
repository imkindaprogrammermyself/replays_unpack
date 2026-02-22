namespace ReplaysUnpack;

public sealed class ReplayParser
{
    private readonly ReplayReader _reader;

    public ReplayParser(string replayPath)
    {
        _reader = new ReplayReader(replayPath);
    }

    public object GetInfo()
    {
        var replay = _reader.GetReplayData();
        return new
        {
            open = replay.EngineData,
            extra_data = replay.ExtraData,
            hidden = (object?)null,
            error = "Hidden data parsing is not yet implemented in C# port."
        };
    }
}
