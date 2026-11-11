using ElternAnker.Infrastructure.Domain;

namespace ElternAnker.Infrastructure.VectorStore;

public sealed class InMemoryVectorStore : Abstractions.IPedagogyVectorStore
{
    private static readonly IReadOnlyList<PedagogyChunk> _preloadedChunks =
    [
        new PedagogyChunk(
            Id: "chunk-autonomy-01",
            Topic: "Autonomiephase & Anziehen",
            AgeRange: "2-5",
            CoreConcept: "Bedürfnis nach Selbstwirksamkeit und Wahlfreiheit bei Übergängen.",
            ReframingPerspective: "Das Kind trotzt nicht gegen die Eltern, sondern übt eigenständiges Entscheiden.",
            ActionableIdeas:
            [
                "Zwei vorselektierte Optionen zur Wahl stellen ('Roter oder blauer Pullover?')",
                "Spielerischen Wettbewerb einführen ('Schaffst du es, bevor die Sanduhr abläuft?')"
            ],
            ReflectionQuestion: "Welche kleinen Entscheidungen könnte dein Kind morgen früh selbst treffen?"),

        new PedagogyChunk(
            Id: "chunk-bedtime-01",
            Topic: "Schlafbegleitung & Abendunruhe",
            AgeRange: "1-6",
            CoreConcept: "Trennungsangst und sensorische Überreizung nach fordernden Tagen.",
            ReframingPerspective: "Einschlafschwierigkeiten sind oft der Versuch, Nähe und Bindung nachzuholen.",
            ActionableIdeas:
            [
                "5 Minuten ungeteilte Kuschelzeit ohne Smartphone vor dem Einschlafen",
                "Sanfter Rückblick: 'Was war heute dein schönster und dein schwerster Moment?'"
            ],
            ReflectionQuestion: "Was bräuchte dein Kind heute Abend am meisten, um innerlich zur Ruhe zu kommen?"),

        new PedagogyChunk(
            Id: "chunk-emotional-regulation-01",
            Topic: "Gefühlsausbrüche & Wut",
            AgeRange: "3-8",
            CoreConcept: "Unreife des präfrontalen Kortex; Ko-Regulation statt Belehrung im Wutanfall.",
            ReframingPerspective: "Ein Wutanfall ist keine Manipulation, sondern ein überflutetes Nervensystem.",
            ActionableIdeas:
            [
                "Präsenz anbieten ohne viele Worte ('Ich bin hier bei dir')",
                "Körperliche Erdung anbieten (ruhiges Durchatmen vorleben, Hand auf den Rücken)"
            ],
            ReflectionQuestion: "Was hat die Situation in dir selbst ausgelöst, bevor du reagiert hast?")
    ];

    public Task<IReadOnlyList<PedagogyChunk>> SearchAsync(
        string query,
        int ageYears,
        int limit = 3,
        CancellationToken cancellationToken = default)
    {
        List<PedagogyChunk> results = _preloadedChunks
            .Take(Math.Max(1, limit))
            .ToList();

        return Task.FromResult<IReadOnlyList<PedagogyChunk>>(results);
    }
}

