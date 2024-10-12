using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Shared.Extensions;

/// <summary>
/// https://github.com/dotnet/aspire/blob/main/src/Shared/ChannelExtensions.cs#L26
/// </summary>
public static class ChannelExtensions
{
    public static async IAsyncEnumerable<IReadOnlyList<T>> GetBatchesAsync<T>(this Channel<T> channel,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        DateTime? lastRead = null;
        
        TimeSpan? minReadInterval = TimeSpan.FromMilliseconds(100);
        
        while (!cancellationToken.IsCancellationRequested)
        {
            List<T>? batch = null;

            if (await channel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                if (minReadInterval != null && lastRead != null)
                {
                    var s = lastRead.Value.Add(minReadInterval.Value) - DateTime.UtcNow;
                    if (s > TimeSpan.Zero)
                    {
                        await Task.Delay(s, cancellationToken).ConfigureAwait(false);
                    }
                }
                
                
                while (!cancellationToken.IsCancellationRequested && channel.Reader.TryRead(out var log))
                {
                    batch ??= [];
                    batch.Add(log);
                }

                if (!cancellationToken.IsCancellationRequested && batch is not null)
                {
                    yield return batch;
                }
            }
            else
            {
                break;
            }
        }
    }
}