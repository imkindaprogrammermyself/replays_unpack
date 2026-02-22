# Replays Unpack (.NET 10)

This repository has been migrated to a C#/.NET 10 codebase.

## Projects

- `src/ReplaysUnpack` - core library with replay reader, parser, and bit reader.
- `src/ReplaysUnpack.Cli` - CLI entry point (`--replay <path>`).
- `tests/ReplaysUnpack.Tests` - xUnit tests for core behavior.

## Build

```bash
dotnet build ReplaysUnpack.sln
```

## Test

```bash
dotnet test ReplaysUnpack.sln
```

## Run

```bash
dotnet run --project src/ReplaysUnpack.Cli -- --replay <path-to-replay>
```
