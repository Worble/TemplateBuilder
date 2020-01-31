namespace TemplateBuilder.Core.Models.Config
{
	using System.Collections.Generic;

	public class TemplateConfig
	{
		public List<TemplateFileConfig> Files { get; set; } = new List<TemplateFileConfig>();
	}
}
