# ElternAnker – AI Agent Context

> **Short Context**
> ElternAnker ist eine digitale Begleit-Plattform für Eltern, die KI-gestützte Unterstützung, Ressourcen und Community-Funktionen bereitstellt.
> Das Backend ist als ASP.NET Core 10 Minimal-API mit Vertical Slice Architecture aufgebaut – jedes Feature lebt vollständig in seinem eigenen Ordner.
> Die Infrastruktur wird über **.NET Aspire** orchestriert (`ElternAnker.AppHost`); Observability, Health-Checks und Service-Discovery werden zentral über `ElternAnker.ServiceDefaults` konfiguriert.
> Der Codestil wird über `.editorconfig` und Roslyn-Analyzer erzwungen; CI schlägt fehl, wenn Regeln verletzt werden.

---

## CLI Cheat-Sheet

```bash
# Build
dotnet build ElternAnker.slnx

# Run tests (all)
dotnet test ElternAnker.slnx --logger "console;verbosity=normal"

# Run tests (watch)
dotnet watch test --project tests/ElternAnker.Api.Tests

# Format & style check
dotnet format ElternAnker.slnx
dotnet format ElternAnker.slnx --verify-no-changes   # CI-Check: exit 1 on diff

# Start via Aspire (recommended)
dotnet run --project src/ElternAnker.AppHost

# Start API directly (Development)
dotnet run --project src/ElternAnker.Api --environment Development

# Build via Makefile / build.ps1
make build
make test
make format
.\build.ps1 -Target test
```

---

## Code Style Rules

1. **File-scoped namespaces** – Immer `namespace Foo.Bar;` statt Block-Namespaces. Erzwungen via `.editorconfig` (`csharp_style_namespace_declarations = file_scoped:error`).
2. **Nullable Reference Types** – `<Nullable>enable</Nullable>` in allen Projekten. Null-Verstöße (CS8600–CS8625) sind **Errors**.
3. **Primary Constructors** (C# 12+) – Bevorzuge `class Foo(IBar bar)` statt separater Felder + Ctor, wo sinnvoll.
4. **Keine statischen Abhängigkeiten ohne Abstraktion** – Keine direkten `static`-Aufrufe auf externe Services; immer über Interface-Injection.
5. **Records für DTOs & Responses** – Verwende `sealed record` für alle Request/Response-Objekte.
6. **Private Felder mit `_`-Präfix** – `private readonly IFoo _foo;` (erzwungen per Naming Rule).
7. **Keine magischen Strings** – Konstanten oder `nameof()` verwenden; niemals rohe String-Literale für Routing, Claims oder Config-Keys.

---

## Architecture Rules

- **Vertical Slice**: Jedes Feature/Use-Case bekommt einen eigenen Ordner unter `src/ElternAnker.Api/Features/<Domain>/<UseCase>/`.
- **Kein shared „Services/"- oder „Controllers/"-Ordner** auf Top-Level. Shared Code gehört nach `Infrastructure/`.
- **Eigene Handler-Abstraktionen**: Verwende die schlanken Interfaces `ICommandHandler<TCommand, TResult>` und `IQueryHandler<TQuery, TResult>` aus `ElternAnker.Api.Infrastructure.Abstractions` anstelle schwerer externer Mediator-Bibliotheken.
- **Endpoint-Registrierung via Extension-Method**: Jedes Feature exportiert eine `MapXyzEndpoints()`-Methode; `Program.cs` ruft diese auf.
- **Neue Features**: Kopiere den `Ping`-Slice als Vorlage (`PingEndpoint.cs` enthält Route, Request/Query, Response und Handler).
- **Tests spiegeln Feature-Struktur**: `tests/ElternAnker.Api.Tests/Features/<Domain>/` – Tests nutzen `Shouldly` als Assertion-Library (vollständig kommerziell nutzbar).
- **Keine direkten DbContext-Aufrufe in Endpoints** – Immer über Repository- oder Handler-Abstraktion.
- **ADR für architektonische Entscheidungen**: Leg unter `docs/adr/` eine neue ADR-Datei an (Vorlage: `docs/adr/0000-adr-template.md`).
- **Aspire AppHost**: Neue Backend-Services in `src/ElternAnker.AppHost/AppHost.cs` via `builder.AddProject<Projects.XYZ>("name")` registrieren.
- **ServiceDefaults**: Jeder neue Service ruft `builder.AddServiceDefaults()` in `Program.cs` auf, um Observability (OTLP, Health-Checks, Service-Discovery) zu erhalten.

---

## Directory Structure

```
ElternAnker/
├── src/
│   ├── ElternAnker.AppHost/          ← Aspire Orchestration
│   │   └── AppHost.cs
│   ├── ElternAnker.ServiceDefaults/  ← Shared Observability / Health / Service-Discovery
│   │   └── Extensions.cs
│   └── ElternAnker.Api/
│       ├── Features/
│       │   ├── Health/
│       │   │   └── HealthEndpoint.cs
│       │   └── Ping/
│       │       └── PingEndpoint.cs
│       ├── Infrastructure/
│       │   └── Extensions/
│       │       └── ServiceCollectionExtensions.cs
│       └── Program.cs
├── tests/
│   └── ElternAnker.Api.Tests/
│       └── Features/
│           ├── Health/
│           │   └── HealthEndpointTests.cs
│           └── Ping/
│               └── PingEndpointTests.cs
├── docs/
│   └── adr/
│       └── 0000-adr-template.md
├── AGENTS.md
├── .editorconfig
├── .gitattributes
├── Directory.Build.props
├── Makefile
├── build.ps1
└── ElternAnker.slnx
```

