# Chess — Educational C# Project

A recovered university chess project with a rules engine, an interactive console client, an ASP.NET API and a Unity board interface. The original source snapshot was recovered from the author's university archive; generated caches and build outputs are excluded.

## Context

Second-year university project, preserved as learning history. It is not a production multiplayer service.

## Components

- `engine/Chess`: C#/.NET 6 chess engine (board representation, FEN, legal moves, check/checkmate/stalemate).
- `engine/test`: interactive console game. Despite its directory name, this is not an automated test suite.
- `server`: ASP.NET Web API / Entity Framework 6 (.NET Framework 4.7.2), SQL Server persistence and a separate bundled copy of the rules library.
- `client`: .NET Framework 4.7.2 HTTP client library.
- `unity`: Unity 2021.3.15f1 board interface, scenes and original project assets.

## Running locally

For the console version, install the .NET 6 SDK/runtime:

```sh
dotnet run --project engine/test/test.csproj
```

Enter a listed move, press Enter for a random legal move, or enter `q` to exit. For the API use Visual Studio with the ASP.NET workload, .NET Framework 4.7.2 targeting pack and SQL Server/LocalDB; restore NuGet packages and open `server/ChessApi.sln`. Connection strings in `Web.config` use local Windows authentication. Database provisioning was not reconstructed or validated. Open `unity` with the recorded Unity editor version for the board UI; online integration is not claimed as verified.

## Limitations

The server contains educational placeholder users/passwords and lacks production authentication. Do not expose it publicly. Console execution, server runtime and Unity gameplay have separate validation boundaries; see `VALIDATION.md`. The original empty root solution is retained as history; use the paths above for the recovered implementation.

## Attribution / License

Recovered from Egor Sharonov's university source archive. Original comments and bundled project assets are preserved. Dependencies retain their upstream licenses through NuGet/Unity packages. No project-wide license has been assigned, and this restoration makes no claim to relicense third-party artwork or dependencies.

## Status

Historical educational project; source recovery, not a modernization of gameplay or server behavior.
