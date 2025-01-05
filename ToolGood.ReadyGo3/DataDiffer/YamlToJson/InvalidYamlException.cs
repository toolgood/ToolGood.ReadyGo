using System;

namespace ToolGood.ReadyGo3.DataDiffer.YamlToJson
{
    /// <summary>
    /// This represents the exception entity when invalid YAML string has been detected.
    /// </summary>
    public class InvalidYamlException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidJsonException"/> class.
        /// </summary>
        public InvalidYamlException() : this("Invalid YAML string")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidJsonException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public InvalidYamlException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidJsonException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner <see cref="Exception"/> instance.</param>
        public InvalidYamlException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
