namespace TemplateBuilder.Core.Tests.FileProcessorTests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.Extensions.FileSystemGlobbing;
	using TemplateBuilder.Core.Models;
	using TemplateBuilder.Core.Tests.Abstract;
	using Xunit;

	public class MoveFilesTests : FileTestBase
	{
		[Fact]
		public async Task GivenAnEmptyDirectoryAndNoFileGroup_WhenMoveFilesIsCalled_TheDestinationDirectoryWillBeEmpty()
		{
			//arrange
			var origin = EnsureNewDirectory(Path.Join(TempPath, Guid.NewGuid().ToString()));
			var destination = EnsureNewDirectory(Path.Join(TempPath, Guid.NewGuid().ToString()));
			var groups = new List<FileGroup>
			{
				new FileGroup()
			};

			//act
			await FileProcessor.MoveFiles(origin, destination, groups).ConfigureAwait(false);

			//assert
			Assert.Empty(Directory.GetFiles(destination));
		}

		[Fact]
		public async Task GivenANonEmptyDirectoryAndNoFileGroup_WhenMoveFilesIsCalled_TheDestinationDirectoryWillBeEmpty()
		{
			//arrange
			var origin = EnsureNewDirectory(Path.Join(TempPath, Guid.NewGuid().ToString()));
			await File.WriteAllTextAsync(
				Path.Join(origin, Guid.NewGuid().ToString()),
				string.Empty).ConfigureAwait(false);
			var destination = EnsureNewDirectory(Path.Join(TempPath, Guid.NewGuid().ToString()));
			var groups = new List<FileGroup>
			{
				new FileGroup()
			};

			//act
			await FileProcessor.MoveFiles(origin, destination, groups).ConfigureAwait(false);

			//assert
			Assert.Empty(Directory.GetFiles(destination));
		}

		[Fact]
		public async Task GivenADirectoryWithOneFileAndNoFileGroup_WhenMoveFilesIsCalled_TheDestinationDirectoryWillBeEmpty()
		{
			//arrange
			var origin = EnsureNewDirectory(Path.Join(TempPath, Guid.NewGuid().ToString()));
			var filename = Guid.NewGuid().ToString();
			var filepath = Path.Join(origin, filename);
			await File.WriteAllTextAsync(
				filepath,
				string.Empty).ConfigureAwait(false);
			var destination = EnsureNewDirectory(Path.Join(TempPath, Guid.NewGuid().ToString()));
			var groups = new List<FileGroup>
			{
				new FileGroup()
			};

			//act
			await FileProcessor.MoveFiles(origin, destination, groups).ConfigureAwait(false);

			//assert
			Assert.Empty(Directory.GetFiles(destination));
		}

		[Fact]
		public async Task GivenADirectoryWithOneFile_AndAFileGroupWithThatFile_WhenMoveFilesIsCalled_TheDestinationDirectoryWillContainTheGivenFile()
		{
			//arrange
			var origin = EnsureNewDirectory(Path.Join(TempPath, Guid.NewGuid().ToString()));
			var filename = Guid.NewGuid().ToString();
			var filepath = Path.Join(origin, filename);
			await File.WriteAllTextAsync(
				filepath,
				string.Empty).ConfigureAwait(false);
			var destination = EnsureNewDirectory(Path.Join(TempPath, Guid.NewGuid().ToString()));
			var groups = new List<FileGroup>
			{
				new FileGroup{ Files = new List<FilePatternMatch> { new FilePatternMatch(filename, filename) } }
			};
			var expectedPath = Path.Join(destination, filename);

			//act
			await FileProcessor.MoveFiles(origin, destination, groups).ConfigureAwait(false);

			//assert
			Assert.Single(Directory.GetFiles(destination));
			Assert.Equal(expectedPath, Directory.GetFiles(destination).First());
		}
	}
}
