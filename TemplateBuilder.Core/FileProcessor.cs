namespace TemplateBuilder.Core
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using Microsoft.Extensions.FileSystemGlobbing;
	using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
	using TemplateBuilder.Core.Helpers;
	using TemplateBuilder.Core.Models;
	using TemplateBuilder.Core.Models.Config;

	public static class FileProcessor
	{
		#region Public Methods

		/// <summary>
		/// Gets the files to move based on the given configuration
		/// </summary>
		/// <param name="sourceDirectory">The directory to search for files to move</param>
		/// <param name="config">The <see cref="TemplateConfig"/></param>
		/// <param name="promptResults">A <see cref="IDictionary{TKey, TValue}"/> of values from the prompts, where the key is the <see cref="AbstractPrompt.Id"/> and Value is the prompt result</param>
		/// <returns>A <see cref="FileGroup"/> of all files and the required template variables</returns>
		public static IEnumerable<FileGroup> GetFilesToMove(string sourceDirectory, TemplateConfig config, IDictionary<string, object> promptResults)
		{
			var filesToMove = new List<FileGroup>();
			var info = new DirectoryInfoWrapper(new DirectoryInfo(sourceDirectory));

			foreach (var file in config.Files)
			{
				var matcher = new Matcher();
				matcher.AddInclude(file.Glob);
				var result = matcher.Execute(info);

				filesToMove.Add(
					new FileGroup
					{
						Files = result.Files,
						VariablesToApply =
							promptResults
								.Where(v => file.Variables.Contains(v.Key))
								.ToDictionary(p => p.Key, p => p.Value)
					});
			}

			return filesToMove;
		}

		/// <summary>
		/// Moves the files.
		/// </summary>
		/// <param name="origin">The origin directory.</param>
		/// <param name="destination">The destination directory.</param>
		/// <param name="fileGroups">The file groups.</param>
		public static async Task MoveFiles(string origin, string destination, IEnumerable<FileGroup> fileGroups, CancellationToken cancellationToken = default)
		{
			foreach (var group in fileGroups)
			{
				await ProcessGroup(origin, destination, group, cancellationToken).ConfigureAwait(false);
			}
		}

		#endregion Public Methods

		#region Private Methods

		/// <summary>
		/// Processes the file group.
		/// </summary>
		/// <param name="origin">The origin directory.</param>
		/// <param name="destination">The destination directory.</param>
		/// <param name="group">The file group.</param>
		private static async Task ProcessGroup(string origin, string destination, FileGroup group, CancellationToken cancellationToken = default)
		{
			foreach (var file in group.Files)
			{
				var content = "";
				var originPath = Path.Join(origin, file.Path);
				var destinationPath = Path.Join(destination, file.Path);

				using (var stream = new StreamReader(originPath, Encoding.UTF8))
				{
					content = await stream
						.ReadToEndAsync()
						.ConfigureAwait(false);
				}

				var output = await MoustacheHelper
					.ApplyMoustache(content, group.VariablesToApply)
					.ConfigureAwait(false);

				Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
				await File.WriteAllTextAsync(destinationPath, output, cancellationToken).ConfigureAwait(false);
			}
		}

		#endregion Private Methods
	}
}
