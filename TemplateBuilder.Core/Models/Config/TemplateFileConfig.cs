namespace TemplateBuilder.Core.Models.Config
{
	using System.Collections.Generic;

	public class TemplateFileConfig
	{
		public string Glob { get; set; } = "";
		public List<string> Variables { get; set; } = new List<string>();
	}
}
