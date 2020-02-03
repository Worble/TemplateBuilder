namespace TemplateBuilder.Core.Helpers
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Stubble.Core.Builders;
	using Stubble.Core.Settings;

	public static class MoustacheHelper
	{
		/// <summary>
		/// Applies the moutache rendering to the given template string.
		/// </summary>
		/// <param name="template">The template.</param>
		/// <param name="variables">The prompt results.</param>
		/// <returns>The with moustache rendering applied</returns>
		public static async Task<string> ApplyMoustache(string template, IDictionary<string, object> variables)
		{
			var renderSettings = new RenderSettings
			{
				ThrowOnDataMiss = true,
				SkipHtmlEncoding = true
			};
			var stubble = new StubbleBuilder().Build();
			return await stubble
				.RenderAsync(template, variables, renderSettings)
				.ConfigureAwait(false);
		}
	}
}
