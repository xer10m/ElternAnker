# ADR-0001: Grundarchitektur – Vertical Slices, .NET Aspire und schlanke Abstraktionen

**Datum:** 2026-11-03  
**Status:** Accepted  
**Entscheider:** David, AI Assistant  

---

## Kontext

Für die Plattform **ElternAnker** soll eine moderne, wartbare und hochgradig erweiterbare Backend-Architektur auf Basis von **.NET 10** aufgesetzt werden. Das System wird voraussichtlich kommerziell betrieben und stark mit KI-Unterstützung (Pair Programming, AI-Agenten) weiterentwickelt.

Klassische Layered-Architekturen (`Controllers/`, `Services/`, `Repositories/`, `Models/`) führen bei wachsenden Projekten oft zu unübersichtlichen Monolithen, hoher Kopplung und Streuung von Fachlogik über viele Verzeichnisse. Zudem sind externe Abhängigkeiten hinsichtlich Lizenzen (z. B. FluentAssertions v8+, MediatR v12+) kritisch zu bewerten.

---

## Entscheidungen

### 1. Vertical Slice Architecture (VSA) statt Layer-Monolith
- Fachlogik, Request/Response-Modelle, Endpunkte und Handler werden pro Feature im Ordner `src/ElternAnker.Api/Features/<Domain>/<UseCase>/` gebündelt.
- Tests spiegeln diese Struktur 1:1 wider (`tests/ElternAnker.Api.Tests/Features/<Domain>/`).

### 2. .NET Aspire als Cloud-Ready Orchestrierungs-Framework
- `src/ElternAnker.AppHost` dient als zentraler Einstiegspunkt für lokale Entwicklung, Service-Discovery, Tracing und spätere Container/Cloud-Ressourcen (Datenbanken, Queues).
- `src/ElternAnker.ServiceDefaults` stellt einheitliche OpenTelemetry-Metriken, Tracing und Standard-Health-Checks bereit.

### 3. Schlanke eigene Handler statt MediatR
- Anstelle externer Mediator-Bibliotheken werden zwei schlanke generische Interfaces genutzt (`ElternAnker.Api.Infrastructure.Abstractions`):
  - `ICommandHandler<in TCommand, TResult>`
  - `IQueryHandler<in TQuery, TResult>`
- Das verhindert Abhängigkeiten zu proprietären/kommerziellen Lizenzen und hält die Abstraktion transparent.

### 4. Shouldly als Assertion-Bibliothek für automatisierte Tests
- FluentAssertions ab Version 8 erfordert für kommerzielle Projekte eine bezahlte Lizenz (Xceed).
- Für xUnit-Tests wird daher standardmäßig **`Shouldly`** (permissive Open-Source-Lizenz) verwendet.

### 5. API-Dokumentation mit OpenAPI (.NET 10) & Scalar UI
- Statt Swagger UI wird die moderne und entwicklerfreundliche **Scalar API Reference** (`Scalar.AspNetCore`) im Entwicklungsmodus eingebunden.

---

## Konsequenzen

**Positiv:**
- **AI-first**: KI-Agenten können vollständige Features in einem isolierten Ordner erstellen oder anpassen, ohne das Gesamtsystem zu manipulieren.
- **Wartbarkeit**: Features können unabhängig voneinander verändert oder gelöscht werden.
- **Rechtssicherheit**: Keine lizenzrechtlichen Risiken durch kommerzielle Open-Source-Abhängigkeiten.
- **Observability**: Aspire liefert sofort Dashboard, OTLP-Tracing und Health-Monitoring ohne manuelle Konfiguration.

**Negativ / Trade-offs:**
- Bei manchen Querschnittsfunktionen muss bewusst entschieden werden, ob Code dupliziert oder nach `Infrastructure/` ausgelagert wird.
- Geringfügiger Boilerplate für Handler-Registrierungen in `ServiceCollectionExtensions.cs`.

---

## Alternativen (abgelehnt)

| Alternative | Ablehnungsgrund |
|---|---|
| **Clean Architecture / Hexagonal mit Schichten-Ordnern** | Erzeugt zu viel Indirektion und Streuung; bremst schnelle Feature-Iterationen. |
| **MediatR Paket** | Lizenzänderungen und unnötige Laufzeit-Reflexion für einfache In-Process-Aufrufe. |
| **FluentAssertions v8** | Lizenzkosten für kommerzielle Nutzung (Xceed). |
| **Swagger UI / Swashbuckle** | Veraltet bzw. in .NET 9/10 durch native Microsoft OpenAPI-Bibliotheken abgelöst. |

