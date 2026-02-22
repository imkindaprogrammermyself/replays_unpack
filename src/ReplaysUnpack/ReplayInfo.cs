namespace ReplaysUnpack;

public sealed record ReplayInfo(
    string Game,
    Dictionary<string, object?> EngineData,
    List<object?> ExtraData,
    byte[] DecryptedData
);
