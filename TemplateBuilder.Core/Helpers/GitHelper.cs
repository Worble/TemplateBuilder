namespace TemplateBuilder.Core.Helpers
{
	using System.IO;
	using LibGit2Sharp;

	public static class GitHelper
	{
		/// <summary>
		/// Downloads the git repo to a temporary folder location
		/// </summary>
		/// <param name="url">The .git URL.</param>
		/// <returns></returns>
		public static string GetFromUrl(string url)
		{
			var dir = TempFolderHelper.GetTempFolder();
			Directory.CreateDirectory(dir);
			Repository.Clone(url, dir);
			return dir;
		}
	}
}
