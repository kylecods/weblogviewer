using System.Threading.Channels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using Proto;
using Shared.Data;
using Shared.Extensions;
using Shared.Models;
namespace WebLog.Dashboard.ViewModels
{
	public sealed partial class WebLogViewModel(IDataStore dataStore, FileService.FileServiceClient client) : BaseViewModel
	{
		[ObservableProperty] private IQueryable<LogModel> _logs;
		
		[ObservableProperty]private FileLogLevel _logLevel;
    
		[ObservableProperty] private string? _searchText;
		

		[RelayCommand]
		private async Task LoadItems()
		{
			Logs = Enumerable.Empty<LogModel>().AsQueryable();
			await IsBusyFor(async () =>
			{
				var call = client.Fetch(new FileRequest
				{
					FilePath = "C:\\Users\\Kyle.rotich\\Documents\\HELPER_SQL_SCRIPTS\\api-mobile-log-20240915.log"
				});

				await foreach (var logs in call.ResponseStream.ReadAllAsync())
				{
					await foreach (var batch in SetLogModel(logs))
					{
						foreach (var logModel in batch)
						{
							dataStore.Entries.Add(logModel);
						}

					}

				}
			});
			
			Logs = dataStore.Entries.AsQueryable();

		}

		[RelayCommand]
		private void OnSearchChange()
		{
			Logs = !string.IsNullOrEmpty(SearchText) ? 
				Logs.Where(x => x.State!.ToString()!.Contains(SearchText)).AsQueryable() : 
				Enumerable.Empty<LogModel>().AsQueryable();
		}
		
		[RelayCommand]
		private void OnDropDownChange()
		{
			Logs = LogLevel != FileLogLevel.All ? 
				Logs.Where(x => x.LogLevel == LogLevel).AsQueryable() : 
				dataStore.Entries.AsQueryable();
		}

		
		private static async IAsyncEnumerable<IReadOnlyList<LogModel>> SetLogModel(FileResult? logs)
	    {
	        var channel = Channel.CreateUnbounded<IReadOnlyList<LogModel>>(
	            new UnboundedChannelOptions { AllowSynchronousContinuations = false, SingleReader = true, SingleWriter = true });
	        var logColors = new Dictionary<FileLogLevel, LogColor>
	        {
	            [FileLogLevel.Information] = new () { CssBackgroundColor = "#d1e7dd", CssForegroundColor = "#345c4a"},
	            [FileLogLevel.Error] = new() { CssBackgroundColor = "#f8d7da",CssForegroundColor = "#58151c"},
	            [FileLogLevel.Warning] = new() { CssBackgroundColor = "#fff3cd", CssForegroundColor = "#664d03"},
	            [FileLogLevel.None] = new() { CssBackgroundColor = "#ced4da", CssForegroundColor = "#747b81"},
	            [FileLogLevel.Debug] = new() { CssBackgroundColor = "#cff4fc", CssForegroundColor = "#055160"}
	        };
	        
	        var readTask = Task.Run(() =>
	        {
	            try
	            {
	                    var logLines = new LogModel[logs!.LogLines.Count];

	        
	                    for (var index = 0; index < logs.LogLines.Count; index++)
	                    {
	                        var response = logs.LogLines[index];
	                        var logLevel = response.LogLevel switch
	                        {
	                            Proto.LogLevel.Information => FileLogLevel.Information,
	                            Proto.LogLevel.Debug => FileLogLevel.Debug,
	                            Proto.LogLevel.Warning => FileLogLevel.Warning,
	                            Proto.LogLevel.Error => FileLogLevel.Error,
	                            _ => FileLogLevel.None
	                        };

	                        logLines[index] = new LogModel
	                        {
	                            Color = logColors[logLevel],
	                            LogLevel = logLevel,
	                            Timestamp = response.TimeStamp.ToDateTime(),
	                            State = response.Content,
	                            LineNumber = response.LineNumber
	                        };
	                    }

	                    // Channel is unbound so TryWrite always succeeds.
	                    channel.Writer.TryWrite(logLines);
	                
	            }
	            finally
	            {
	                channel.Writer.TryComplete();
	            }
	        });

	        await foreach (var batch in channel.GetBatchesAsync())
	        {
	            if (batch.Count == 1)
	            {
	                yield return batch[0];
	            }
	            else
	            {
	                yield return batch.SelectMany(b => b).ToList();
	            }
	        }

	        await readTask;

	    } 
	}
}
