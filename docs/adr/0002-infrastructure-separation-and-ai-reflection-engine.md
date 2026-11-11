# ADR-0002: Auslagerung von ElternAnker.Infrastructure und Grundgerüst des AI Reflection Coach

**Datum:** 2026-11-11  
**Status:** Accepted  
**Entscheider:** David, AI Assistant  

---

## Kontext

Mit der Einführung des ersten KI-gestützten Kernfeatures – dem Eltern-Reflexions-Coach (**„Abendlicht / Kindo“**) – wächst das Backend über einfache Ping- und Health-Endpunkte hinaus. Das Feature benötigt:
1. Einen mehrstufigen Dialog-Zustandsautomaten (`CollectingContext` $\rightarrow$ `HypothesisFallback` $\rightarrow$ `DeliveringImpulse` $\rightarrow$ `AwaitingFeedback`) mit Schutzmechanismen gegen Überlastung (Single-Question-Guardrail).
2. Semantisches Retrieval (RAG) über kuratierte pädagogische Inhaltsbausteine (`PedagogyChunk`).
3. Persistenz für Reflexions-Sessions und Chat-Verläufe (`ISessionStore`).

Die initiale Ablage dieser Komponenten unter `src/ElternAnker.Api/Infrastructure/` verwässerte die Grenze zwischen der HTTP-Präsentationsschicht (Minimal-APIs, Endpoint-Routing) und den technischen Adaptern/Domänenmodellen. Weder `ElternAnker.AppHost` (reiner Orchestrator) noch `ElternAnker.ServiceDefaults` (domänenunabhängige Observability/Plattform-Defaults) eignen sich für diese fachlichen Adapter.

---

## Entscheidung

### 1. Separates Klassenbibliothek-Projekt `ElternAnker.Infrastructure`
Wir lagern die technischen Adapter, Abstraktionen und Domänenmodelle in ein eigenständiges Projekt `src/ElternAnker.Infrastructure/` aus:
- **`Domain/`**: Domänenentitäten (`ReflectionSession`, `ChildProfile`, `PedagogyChunk`, `ChatMessage`, `SessionState`).
- **`Abstractions/`**: Zentrale Schnittstellen (`IPedagogyVectorStore`, `IDialogStateMachine`, `IRagEngine`, `ISessionStore`, `ICommandHandler`, `IQueryHandler`).
- **Adapter & Stubs**: In-Memory-Implementierungen (`InMemoryVectorStore`, `StubRagEngine`, `DialogStateMachine`, `InMemorySessionStore`).
- **DI-Bootstrap**: `AddInfrastructureServices()` zur Kapselung der Registrierungen.

### 2. Entschlacktes Web-API-Projekt `ElternAnker.Api`
`ElternAnker.Api` konzentriert sich ausschließlich auf:
- Minimal-API-Routing per Vertical Slices (`Features/Sessions/StartSession`, `Chat`, `Feedback`).
- Trennung von HTTP-`Request` und internem `Command`.
- Typisierte Result-Objekte (`ChatResult`, `FeedbackResult`).
- OpenAPI-/Scalar-Dokumentation.

### 3. Single-Question-Guardrail in der Dialog-State-Machine
Die State Machine stellt sicher, dass in jedem Turn maximal eine fokussierte Frage an die Eltern gestellt wird, um abendliche kognitive Überlastung zu verhindern.

---

## Konsequenzen

**Positiv:**
- **Klare Verantwortlichkeiten**: `Api` ist austauschbar (z. B. für alternative Trigger, CLI oder Tests), da die Kernlogik in `Infrastructure` gekapselt ist.
- **Saubere Aspire-Architektur**: `AppHost` orchestriert Container, `ServiceDefaults` sichert OTel/Resilienz, `Infrastructure` hält Logik/Adapter, `Api` serviert HTTP.
- **Wiederverwendbarkeit**: Handler-Abstraktionen und Domänenmodelle können von anderen Services oder Hintergrund-Workern referenziert werden.

**Negativ / Trade-offs:**
- Ein zusätzliches `.csproj`-Projekt in der Solution (`ElternAnker.slnx`).
- Zusätzliche Projekt-Referenzen in `Api.csproj` und `Api.Tests.csproj`.

---

## Alternativen (abgelehnt)

| Alternative | Ablehnungsgrund |
|---|---|
| **Verbleib in `ElternAnker.Api/Infrastructure/`** | Vermischt Transport-/Routing-Schicht mit Domänenmodellen und Adapter-Implementierungen. |
| **Ablage in `ElternAnker.ServiceDefaults`** | Verletzt das Single-Responsibility-Prinzip: `ServiceDefaults` ist für domänenunabhängige Plattform-Standards (OTel, Health, Polly) gedacht. |
| **Ablage in `ElternAnker.AppHost`** | Technisch unmöglich bzw. falsch: `AppHost` ist ein Dev-Time Orchestrator und kann von der API nicht referenziert werden (Zirkelbezug). |

