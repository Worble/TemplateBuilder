namespace TemplateBuilder.Core.Tests.Abstract
{
	using System;
	using System.IO;

	/// <summary>
	/// A base class for tests which require the creation of files and directories. Provides a random directory on the temp path, which is automatically cleaned on test completion.
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	public abstract class FileTestBase : IDisposable
	{
		/// <summary>
		/// A random unique filename.
		/// </summary>
		/// <value>
		/// The filename.
		/// </value>
		internal virtual string Filename { get; } = Guid.NewGuid().ToString();

		/// <summary>
		/// A random unique file in a random temporary path.
		/// </summary>
		/// <value>
		/// The temporary file.
		/// </value>
		internal virtual string TempFile => Path.Join(TempPath, Filename);

		/// <summary>
		/// A random folder in the temp path.
		/// </summary>
		/// <value>
		/// The temporary path.
		/// </value>
		internal virtual string TempPath { get; } = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());

		protected FileTestBase()
		{
			_ = EnsureNewDirectory(TempPath);
		}

		public static string EnsureNewDirectory(string path)
		{
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
			Directory.CreateDirectory(path);
			return path;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// free managed resources
			}
			// free native resources
			if (Directory.Exists(TempPath))

			{
				Directory.Delete(TempPath, true);
			}
		}
	}
}
