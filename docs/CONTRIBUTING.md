# Entwickler-Leitfaden (Contributing)

Willkommen im ElternAnker-Projekt! Dieser Leitfaden beschreibt die Standards und Workflows für die Entwicklung.

---

## 🔧 Lokales Setup

1. **Repository klonen**
2. **Git Pre-Commit Hook aktivieren** (verhindert unformatierte Commits):
   ```bash
   make setup-hooks
   # oder auf Windows:
   .\build.ps1 -Target setup-hooks
   ```

---

## 🚀 Neues Feature hinzufügen (Vertical Slice)

1. **Neuen Ordner anlegen**:
   `src/ElternAnker.Api/Features/<Domain>/<FeatureName>/`
2. **Dateien erstellen** (Vorlage: `Features/Ping/PingEndpoint.cs`):
   - Endpunkt mit `Map<Feature>Endpoints()`-Extension
   - Query/Command-Record (`sealed record`)
   - Response-Record (`sealed record`)
   - Handler implementiert `ICommandHandler` oder `IQueryHandler`
3. **DI-Registrierung**:
   - Handler in `src/ElternAnker.Api/Infrastructure/Extensions/ServiceCollectionExtensions.cs` registrieren
   - Endpunkt in `Program.cs` via `app.Map<Feature>Endpoints()` aufrufen
4. **Integrationstest schreiben**:
   - `tests/ElternAnker.Api.Tests/Features/<Domain>/<FeatureName>Tests.cs`
   - Nutze `WebApplicationFactory<Program>` und `Shouldly`

---

## 🧪 Code-Qualität vor dem Commit prüfen

Vor jedem Push bitte lokal ausführen:

```bash
# 1. Formatierung prüfen
dotnet format ElternAnker.slnx --verify-no-changes

# 2. Tests ausführen
dotnet test ElternAnker.slnx

# Alternativ via Makefile / PowerShell
make test
make format-check
```

---

## 📝 Architektur-Entscheidungen (ADRs)

Größere architektonische Änderungen (neue Datenbank, Authentifizierungs-Provider, Messaging) werden immer als **Architectural Decision Record** unter `docs/adr/` dokumentiert:
1. Vorlage kopieren: `docs/adr/0000-adr-template.md`
2. Nummerieren: `docs/adr/NNNN-<titel>.md`
3. Im PR mitdiskutieren und auf `Accepted` setzen

