namespace TemplateBuilder.Core.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class InvalidPromptTypeException : Exception
	{
		public InvalidPromptTypeException()
		{
		}

		public InvalidPromptTypeException(string message) : base(message)
		{
		}

		public InvalidPromptTypeException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidPromptTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
