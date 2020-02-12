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
		private readonly string _originDirectory;
		private readonly string _destinationDirectory;

		public MoveFilesTests()
		{
			_originDirectory = EnsureNewDirectory(Path.Join(TempPath, Guid.NewGuid().ToString()));
			_destinationDirectory = EnsureNewDirectory(Path.Join(TempPath, Guid.NewGuid().ToString()));
		}

		[Fact]
		public async Task GivenAnEmptyDirectoryAndNoFileGroup_WhenMoveFilesIsCalled_ThenTheDestinationDirectoryWillBeEmpty()
		{
			//arrange
			var groups = new List<FileGroup>
			{
				new FileGroup()
			};

			//act
			await FileProcessor.MoveFiles(_originDirectory, _destinationDirectory, groups).ConfigureAwait(false);

			//assert
			Assert.Empty(Directory.GetFiles(_destinationDirectory));
		}

		[Fact]
		public async Task GivenAnEmptyDirectoryAndAFileGroupWithNoValidFiles_WhenMoveFilesIsCalled_ThenAFileNotFoundExceptionWillbeThrown()
		{
			//arrange
			var filename = Guid.NewGuid().ToString();
			var groups = new List<FileGroup>
			{
				new FileGroup
				{
					Files = new List<FilePatternMatch>
					{
						new FilePatternMatch(filename, filename )
					}
				}
			};

			//act
			await Assert
				.ThrowsAsync<FileNotFoundException>(() => FileProcessor.MoveFiles(_originDirectory, _destinationDirectory, groups))
				.ConfigureAwait(false);

			//assert
		}

		[Fact]
		public async Task GivenANonEmptyDirectoryAndNoFileGroup_WhenMoveFilesIsCalled_ThenTheDestinationDirectoryWillBeEmpty()
		{
			//arrange
			await File.WriteAllTextAsync(
				Path.Join(_originDirectory, Guid.NewGuid().ToString()),
				string.Empty).ConfigureAwait(false);
			var groups = new List<FileGroup>
			{
				new FileGroup()
			};

			//act
			await FileProcessor.MoveFiles(_originDirectory, _destinationDirectory, groups).ConfigureAwait(false);

			//assert
			Assert.Empty(Directory.GetFiles(_destinationDirectory));
		}

		[Fact]
		public async Task GivenADirectoryWithOneFileAndNoFileGroup_WhenMoveFilesIsCalled_ThenTheDestinationDirectoryWillBeEmpty()
		{
			//arrange
			var filename = Guid.NewGuid().ToString();
			var filepath = Path.Join(_originDirectory, filename);
			await File.WriteAllTextAsync(
				filepath,
				string.Empty).ConfigureAwait(false);
			var groups = new List<FileGroup>
			{
				new FileGroup()
			};

			//act
			await FileProcessor.MoveFiles(_originDirectory, _destinationDirectory, groups).ConfigureAwait(false);

			//assert
			Assert.Empty(Directory.GetFiles(_destinationDirectory));
		}

		[Fact]
		public async Task GivenADirectoryWithOneFile_AndAFileGroupWithThatFile_WhenMoveFilesIsCalled_ThenTheDestinationDirectoryWillContainTheGivenFile()
		{
			//arrange
			var filename = Guid.NewGuid().ToString();
			var filepath = Path.Join(_originDirectory, filename);
			await File.WriteAllTextAsync(
				filepath,
				string.Empty).ConfigureAwait(false);
			var groups = new List<FileGroup>
			{
				new FileGroup{ Files = new List<FilePatternMatch> { new FilePatternMatch(filename, filename) } }
			};
			var expectedPath = Path.Join(_destinationDirectory, filename);

			//act
			await FileProcessor.MoveFiles(_originDirectory, _destinationDirectory, groups).ConfigureAwait(false);

			//assert
			Assert.Single(Directory.GetFiles(_destinationDirectory));
			var file = Directory.GetFiles(_destinationDirectory).First();
			Assert.Equal(expectedPath, Directory.GetFiles(_destinationDirectory).First());
			var fileContents = await File
				.ReadAllTextAsync(file)
				.ConfigureAwait(false);
			Assert.Equal(string.Empty, fileContents);
		}

		[Theory]
		[InlineData(new object[] { 2 })]
		[InlineData(new object[] { 12 })]
		[InlineData(new object[] { 20 })]
		public async Task GivenMultipleFilesAndAFileGroupWithMultipleFiles_WhenMoveFilesIsCalled_ThenTheDestinationDirectoryWillContainTheRightAmountOfFiles(int amount)
		{
			//arrange
			var filePatternMatchList = new List<FilePatternMatch>();
			for (var i = 0; i < amount; i++)
			{
				var filename = Guid.NewGuid().ToString();
				var filepath = Path.Join(_originDirectory, filename);
				filePatternMatchList.Add(new FilePatternMatch(filename, filename));
				await File.WriteAllTextAsync(
					filepath,
					string.Empty).ConfigureAwait(false);
			}
			var groups = new List<FileGroup>
			{
				new FileGroup{ Files = filePatternMatchList}
			};

			//act
			await FileProcessor.MoveFiles(_originDirectory, _destinationDirectory, groups).ConfigureAwait(false);

			//assert
			Assert.Equal(amount, Directory.GetFiles(_destinationDirectory).Length);
		}

		[Fact]
		public async Task GivenADirectoryWithOneFileUnderASubDirectory_AndAFileGroupWithThatFileNotUnderASubDirectory_WhenMoveFilesIsCalled_ThenAFileNotFoundExceptionWillBeThrown()
		{
			//arrange
			var subDirectory = EnsureNewDirectory(Path.Join(_originDirectory, Guid.NewGuid().ToString()));
			var filename = Guid.NewGuid().ToString();
			var filepath = Path.Join(subDirectory, filename);
			await File.WriteAllTextAsync(
				filepath,
				string.Empty).ConfigureAwait(false);
			var groups = new List<FileGroup>
			{
				new FileGroup{ Files = new List<FilePatternMatch> { new FilePatternMatch(filename, filename) } }
			};

			//act
			await Assert
				.ThrowsAsync<FileNotFoundException>(() => FileProcessor.MoveFiles(_originDirectory, _destinationDirectory, groups))
				.ConfigureAwait(false);

			//assert
		}

		[Fact]
		public async Task GivenADirectoryWithOneFileUnderASubDirectory_AndOneFileUnderRoot_AndAFileGroupsThatMatchBoth_WhenMoveFilesIsCalled_ThenBothFilesWillBeFound()
		{
			//arrange
			var subDirName = Guid.NewGuid().ToString();
			var subDirectory = EnsureNewDirectory(Path.Join(_originDirectory, subDirName));
			var filenameOne = Guid.NewGuid().ToString();
			var filepathOne = Path.Join(subDirectory, filenameOne);
			await File.WriteAllTextAsync(
				filepathOne,
				string.Empty).ConfigureAwait(false);
			var filenameTwo = Guid.NewGuid().ToString();
			var filepathTwo = Path.Join(_originDirectory, filenameTwo);
			await File.WriteAllTextAsync(
				filepathTwo,
				string.Empty).ConfigureAwait(false);
			var groups = new List<FileGroup>
			{
				new FileGroup
				{
					Files = new List<FilePatternMatch>
					{
						new FilePatternMatch(
							Path.Join(subDirName, filenameOne),
							Path.Join(subDirName, filenameOne)),
						new FilePatternMatch(filenameTwo, filenameTwo)
					}
				}
			};

			//act
			await FileProcessor.MoveFiles(_originDirectory, _destinationDirectory, groups).ConfigureAwait(false);

			//assert
			Assert.Single(Directory.GetFiles(_destinationDirectory));
			Assert.Single(Directory.GetFiles(subDirectory));
		}

		[Fact]
		public async Task GivenADirectoryWithOneFileAndContent_AndAFileGroupWithThatFile_WhenMoveFilesIsCalled_ThenTheDestinationDirectoryWillContainTheGivenFileAndItsContent()
		{
			//arrange
			const string expectedContents = "Test";
			var filename = Guid.NewGuid().ToString();
			var filepath = Path.Join(_originDirectory, filename);
			await File.WriteAllTextAsync(
				filepath,
				expectedContents).ConfigureAwait(false);
			var groups = new List<FileGroup>
			{
				new FileGroup{ Files = new List<FilePatternMatch> { new FilePatternMatch(filename, filename) } }
			};
			var expectedPath = Path.Join(_destinationDirectory, filename);

			//act
			await FileProcessor.MoveFiles(_originDirectory, _destinationDirectory, groups).ConfigureAwait(false);

			//assert
			Assert.Single(Directory.GetFiles(_destinationDirectory));
			var file = Directory.GetFiles(_destinationDirectory).First();
			Assert.Equal(expectedPath, Directory.GetFiles(_destinationDirectory).First());
			var actualContents = await File
				.ReadAllTextAsync(file)
				.ConfigureAwait(false);
			Assert.Equal(expectedContents, actualContents);
		}

		[Fact]
		public async Task GivenADirectoryWithOneFileAndAMoustacheVariable_AndAFileGroupWithThatFileAndNoDefinedVariables_WhenMoveFilesIsCalled_ThenTheDestinationDirectoryWillContainTheGivenFile_AndItsContentWithoutVariablesSubstitued()
		{
			//arrange
			var expectedContents = string.Empty;
			var filename = Guid.NewGuid().ToString();
			var filepath = Path.Join(_originDirectory, filename);
			await File.WriteAllTextAsync(
				filepath,
				"{{Value}}").ConfigureAwait(false);
			var groups = new List<FileGroup>
			{
				new FileGroup{ Files = new List<FilePatternMatch> { new FilePatternMatch(filename, filename) } }
			};

			//act
			await FileProcessor.MoveFiles(_originDirectory, _destinationDirectory, groups).ConfigureAwait(false);

			//assert
			Assert.Single(Directory.GetFiles(_destinationDirectory));
			var file = Directory.GetFiles(_destinationDirectory).First();
			var actualContents = await File
				.ReadAllTextAsync(file)
				.ConfigureAwait(false);
			Assert.Equal(expectedContents, actualContents);
		}

		[Fact]
		public async Task GivenADirectoryWithOneFileAndAMoustacheVariable_AndAFileGroupWithThatFileAndDefinedVariables_WhenMoveFilesIsCalled_ThenTheDestinationDirectoryWillContainTheGivenFile_AndItsContentWithVariablesSubstitued()
		{
			//arrange
			var kvp = new KeyValuePair<string, string>("Value", "Test");
			var filename = Guid.NewGuid().ToString();
			var filepath = Path.Join(_originDirectory, filename);
			await File.WriteAllTextAsync(
				filepath,
				$"{{{{{kvp.Key}}}}}").ConfigureAwait(false);
			var groups = new List<FileGroup>
			{
				new FileGroup{ Files = new List<FilePatternMatch> { new FilePatternMatch(filename, filename) }, VariablesToApply = new Dictionary<string, object>{ { kvp.Key, kvp.Value} } }
			};

			//act
			await FileProcessor.MoveFiles(_originDirectory, _destinationDirectory, groups).ConfigureAwait(false);

			//assert
			Assert.Single(Directory.GetFiles(_destinationDirectory));
			var file = Directory.GetFiles(_destinationDirectory).First();
			var actualContents = await File
				.ReadAllTextAsync(file)
				.ConfigureAwait(false);
			Assert.Equal(kvp.Value, actualContents);
		}

		[Fact]
		public async Task GivenADirectoryWithTwoFilesAndAMoustacheVariable_AndAFileGroupWithThatFileAndDefinedVariables_AndOneWithout_WhenMoveFilesIsCalled_ThenTheDestinationDirectoryWillContainAFileWithSubsitutedVarables_AndOneWithout()
		{
			//arrange
			const int expectedCount = 2;
			var kvp = new KeyValuePair<string, string>("Value", "Test");
			var filenameOne = Guid.NewGuid().ToString();
			var filepathOne = Path.Join(_originDirectory, filenameOne);
			await File.WriteAllTextAsync(
				filepathOne,
				$"{{{{{kvp.Key}}}}}").ConfigureAwait(false);

			var filenameTwo = Guid.NewGuid().ToString();
			var filepathTwo = Path.Join(_originDirectory, filenameTwo);
			await File.WriteAllTextAsync(
				filepathTwo,
				$"{{{{{kvp.Key}}}}}").ConfigureAwait(false);

			var groups = new List<FileGroup>
			{
				new FileGroup
				{
					Files = new List<FilePatternMatch>
					{
						new FilePatternMatch(filenameOne, filenameOne)
					},
					VariablesToApply = new Dictionary<string, object>
					{ { kvp.Key, kvp.Value} }
				},
				new FileGroup
				{
					Files = new List<FilePatternMatch>
					{
						new FilePatternMatch(filenameTwo, filenameTwo)
					}
				}
			};

			//act
			await FileProcessor.MoveFiles(_originDirectory, _destinationDirectory, groups).ConfigureAwait(false);

			//assert
			Assert.Equal(expectedCount, Directory.GetFiles(_destinationDirectory).Length);

			var fileOne = Directory.GetFiles(_destinationDirectory).First(f => f == Path.Join(_destinationDirectory, filenameOne));
			var actualContents = await File
				.ReadAllTextAsync(fileOne)
				.ConfigureAwait(false);
			Assert.Equal(kvp.Value, actualContents);

			var fileTwo = Directory.GetFiles(_destinationDirectory).First(f => f == Path.Join(_destinationDirectory, filenameTwo));
			actualContents = await File
				.ReadAllTextAsync(fileTwo)
				.ConfigureAwait(false);
			Assert.Equal(string.Empty, actualContents);
		}
	}
}
