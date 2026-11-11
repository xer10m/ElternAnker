# ElternAnker ⚓

> Ein KI-gestützter digitaler Begleiter für Eltern – für Reflexion im fordernden Familienalltag, fundierte kindzentrierte Handlungsimpulse und Entlastung.

---

## 🚀 Schnellstart

### Voraussetzungen
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Docker Desktop oder Podman (für Container via .NET Aspire)

### Anwendung starten (mit Aspire Dashboard)
```bash
# Startet Aspire Dashboard + API
dotnet run --project src/ElternAnker.AppHost
```
Das Aspire-Dashboard öffnet sich im Browser mit:
- Live-Logs & Distributed Tracing (OpenTelemetry)
- Health-Checks & Metriken
- Direkter Link zur API-Dokumentation

### Interaktive API-Dokumentation (Scalar UI)
Im `Development`-Modus ist die moderne **Scalar API-Referenz** verfügbar:
- **Scalar UI**: `http://localhost:<port>/scalar/v1`
- **OpenAPI Spezifikation**: `http://localhost:<port>/openapi/v1.json`

---

## 🛠️ CLI & Build-Befehle

Für einheitliche Workflows über Windows, Linux und CI stehen ein `Makefile` und ein PowerShell-Skript bereit:

| Aufgabe | Windows / Linux (Make) | Windows (PowerShell) |
|---|---|---|
| **Git Hooks aktivieren** | `make setup-hooks` | `.\build.ps1 -Target setup-hooks` |
| **Bauen** | `make build` | `.\build.ps1 -Target build` |
| **Tests ausführen** | `make test` | `.\build.ps1 -Target test` |
| **Code formatieren** | `make format` | `.\build.ps1 -Target format` |
| **Format-Prüfung (CI)** | `make format-check` | `.\build.ps1 -Target format-check` |
| **API direkt starten** | `make run` | `.\build.ps1 -Target run` |

---

## 🏛️ Architektur & Struktur

Das Projekt folgt einer **Vertical Slice Architecture** (VSA):
- Code ist nach **Features/Domänen** organisiert – **nicht** nach technischen Schichten (`Controllers/`, `Services/`).
- Jedes Feature kapselt seinen eigenen Endpunkt, Handler, DTOs und Logik (z. B. `src/ElternAnker.Api/Features/Ping/`).
- Shared Infrastructure liegt isoliert unter `src/ElternAnker.Api/Infrastructure/`.

```
ElternAnker/
├── src/
│   ├── ElternAnker.AppHost/          ← .NET Aspire Orchestrierung
│   ├── ElternAnker.ServiceDefaults/  ← Shared Observability, OTLP, Health
│   ├── ElternAnker.Infrastructure/   ← Domain-Modelle, AI-Abstraktionen & Stubs (VectorStore, StateMachine, Storage)
│   └── ElternAnker.Api/              ← Minimal-API mit Vertical Slice Architecture
│       ├── Features/
│       │   ├── Health/               ← Feature: Detail-Healthcheck
│       │   ├── Ping/                 ← Feature: Referenz-Slice (Endpoint + Handler)
│       │   └── Sessions/             ← Feature: AI Parenting Reflection Coach ("Abendlicht")
│       │       ├── StartSession/     ← POST /api/sessions/start
│       │       ├── Chat/             ← POST /api/sessions/{id}/chat
│       │       └── Feedback/         ← POST /api/sessions/{id}/feedback
│       └── Infrastructure/           ← API-spezifische DI-Extensions
├── tests/
│   └── ElternAnker.Api.Tests/        ← xUnit + Shouldly Integrationstests
│       └── Features/                 ← Test-Slices spiegeln API-Features
├── docs/
│   ├── adr/                          ← Architectural Decision Records
│   └── CONTRIBUTING.md               ← Entwickler-Leitfaden
├── AGENTS.md                         ← KI-Agenten-Kontext & Coding-Regeln
├── Directory.Build.props             ← Zentrale Compiler- & Analyzer-Einstellungen
└── .editorconfig                     ← Strikte C#-Code-Style-Vorgaben
```

---

## 🤖 KI- & Coding-Richtlinien

Für Mensch & KI-Assistenten gelten die Regeln in [**`AGENTS.md`**](file:///c:/Users/david/Develop/VCS/github/ElternAnker/AGENTS.md):
- File-scoped Namespaces & strikte Nullable-Behandlung
- Keine externen Heavyweight-Mediatoren (schlanke interne Handler)
- Kommerziell uneingeschränkt nutzbare Bibliotheken (Shouldly statt FluentAssertions)
