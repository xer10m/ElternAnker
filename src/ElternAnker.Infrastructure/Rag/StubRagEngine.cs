using System.Globalization;
using System.Text;
using ElternAnker.Infrastructure.Domain;

namespace ElternAnker.Infrastructure.Rag;

public sealed class StubRagEngine : Abstractions.IRagEngine
{
    public Task<string> GenerateImpulseAsync(
        ReflectionSession session,
        List<PedagogyChunk> contextChunks,
        CancellationToken cancellationToken = default)
    {
        PedagogyChunk? primaryChunk = contextChunks.FirstOrDefault();
        StringBuilder sb = new();

        sb.AppendLine("Reflexionsimpuls:");
        if (primaryChunk is not null)
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"- Perspektivwechsel: {primaryChunk.ReframingPerspective}");
            if (primaryChunk.ActionableIdeas.Count > 0)
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $"- Konkreter Impuls: {primaryChunk.ActionableIdeas[0]}");
            }

            sb.AppendLine(CultureInfo.InvariantCulture, $"- Frage zur Reflexion: {primaryChunk.ReflectionQuestion}");
        }
        else
        {
            sb.AppendLine("- Perspektivwechsel: Hinter herausforderndem Verhalten steht oft ein ungestilltes Grundbedürfnis nach Nähe oder Autonomie.");
            sb.AppendLine("- Konkreter Impuls: Atme dreimal tief durch, bevor du in die Situation gehst.");
            sb.AppendLine("- Frage zur Reflexion: Was brauchte dein Kind im Kern in diesem Moment?");
        }

        return Task.FromResult(sb.ToString().Trim());
    }
}

