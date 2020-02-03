namespace TemplateBuilder.Core.Models
{
	using System.Collections.Generic;
	using Microsoft.Extensions.FileSystemGlobbing;

	public class FileGroup
	{
		public IEnumerable<FilePatternMatch> Files { get; set; } = new List<FilePatternMatch>();
		public IDictionary<string, object> VariablesToApply { get; set; } = new Dictionary<string, object>();
	}
}
