namespace TemplateBuilder.Core.Helpers
{
	using System;
	using System.IO;

	public static class TempFolderHelper
	{
		private const string TEMP_FOLDER = "TemplateBuilderTemporaryFiles";
		private static string TempFolderPath => Path.Join(Path.GetTempPath(), TEMP_FOLDER);

		/// <summary>
		/// Creates a new temporary folder.
		/// </summary>
		/// <returns>The temporary folder name.</returns>
		public static string GetTempFolder()
		{
			var dir = Path.Join(TempFolderPath, Guid.NewGuid().ToString());
			if (Directory.Exists(dir))
			{
				return GetTempFolder();
			}
			else
			{
				Directory.CreateDirectory(dir);
				return dir;
			}
		}

		/// <summary>
		/// Cleans up all temporary folders.
		/// </summary>
		public static void CleanupAllTempFolders()
		{
			if (Directory.Exists(TempFolderPath))
			{
				// this is a workaround for read-only files (which some of gits are)
				var directory = new DirectoryInfo(TempFolderPath) { Attributes = FileAttributes.Normal };
				foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
				{
					info.Attributes = FileAttributes.Normal;
				}
				Directory.Delete(TempFolderPath, true);
			}
		}
	}
}
