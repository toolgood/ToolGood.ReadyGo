using System;
using System.Text;
using System.Text.Json.Nodes;

namespace ToolGood.ReadyGo.JsonDiffPatch.Diffs.Formatters
{
    /// <summary>
    /// Defines methods to format <see cref="JsonDiffDelta"/> into RFC6902 Json Patch format.
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc6902"/>.
    /// </summary>
    public class JsonPatchDeltaFormatter : DefaultDeltaFormatter<JsonNode>
    {
        private const string PropertyNameOperation = "op";
        private const string PropertyNamePath = "path";
        private const string PropertyNameValue = "value";
        
        private const string OperationNameAdd = "add";
        private const string OperationNameRemove = "remove";
        private const string OperationNameReplace = "replace";

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPatchDeltaFormatter"/> class.
        /// </summary>
        public JsonPatchDeltaFormatter()
            : base(true)
        {
            PathBuilder = new();
        }

        /// <summary>
        /// Gets the <see cref="StringBuilder"/> used to build the Json Pointer path.
        /// </summary>
        protected StringBuilder PathBuilder { get; }

        /// <summary>
        /// Creates the default result value as a new <see cref="JsonArray"/>.
        /// </summary>
        /// <returns>A new <see cref="JsonArray"/>.</returns>
        protected override JsonNode? CreateDefault()
        {
            return new JsonArray();
        }

        /// <summary>
        /// Formats a single array item change and appends the corresponding Json Patch operation.
        /// </summary>
        /// <param name="arrayChange">The array item change to format.</param>
        /// <param name="left">The left value of the array item.</param>
        /// <param name="existingValue">The existing Json Patch operations.</param>
        /// <returns>The formatted result.</returns>
        protected override JsonNode? FormatArrayElement(in JsonDiffDelta.ArrayChangeEntry arrayChange,
            JsonNode? left, JsonNode? existingValue)
        {
            using var _ = new PropertyPathScope(PathBuilder, arrayChange.Index);
            return base.FormatArrayElement(arrayChange, left, existingValue);
        }

        /// <summary>
        /// Formats a single object property change and appends the corresponding Json Patch operation.
        /// </summary>
        /// <param name="delta">The property delta to format.</param>
        /// <param name="left">The left value of the property.</param>
        /// <param name="propertyName">The name of the changed property.</param>
        /// <param name="existingValue">The existing Json Patch operations.</param>
        /// <returns>The formatted result.</returns>
        protected override JsonNode? FormatObjectProperty(ref JsonDiffDelta delta, JsonNode? left, 
            string propertyName, JsonNode? existingValue)
        {
            using var _ = new PropertyPathScope(PathBuilder, propertyName);
            return base.FormatObjectProperty(ref delta, left, propertyName, existingValue);
        }

        /// <summary>
        /// Formats an <see cref="DeltaKind.Added"/> delta as an <c>add</c> Json Patch operation.
        /// </summary>
        /// <param name="delta">The delta to format.</param>
        /// <param name="existingValue">The existing Json Patch operations.</param>
        /// <returns>The formatted result.</returns>
        protected override JsonNode? FormatAdded(ref JsonDiffDelta delta, JsonNode? existingValue)
        {
            var op = new JsonObject
            {
                {PropertyNameOperation, OperationNameAdd},
                {PropertyNamePath, PathBuilder.ToString()},
                {PropertyNameValue, delta.GetAdded()}
            };
            existingValue!.AsArray().Add(op);
            return existingValue;
        }

        /// <summary>
        /// Formats a <see cref="DeltaKind.Modified"/> delta as a <c>replace</c> Json Patch operation.
        /// </summary>
        /// <param name="delta">The delta to format.</param>
        /// <param name="left">The left value the delta applies to.</param>
        /// <param name="existingValue">The existing Json Patch operations.</param>
        /// <returns>The formatted result.</returns>
        protected override JsonNode? FormatModified(ref JsonDiffDelta delta, JsonNode? left, JsonNode? existingValue)
        {
            var op = new JsonObject
            {
                {PropertyNameOperation, OperationNameReplace},
                {PropertyNamePath, PathBuilder.ToString()},
                {PropertyNameValue, delta.GetNewValue()}
            };
            existingValue!.AsArray().Add(op);
            return existingValue;
        }

        /// <summary>
        /// Formats a <see cref="DeltaKind.Deleted"/> delta as a <c>remove</c> Json Patch operation.
        /// </summary>
        /// <param name="delta">The delta to format.</param>
        /// <param name="left">The left value the delta applies to.</param>
        /// <param name="existingValue">The existing Json Patch operations.</param>
        /// <returns>The formatted result.</returns>
        protected override JsonNode? FormatDeleted(ref JsonDiffDelta delta, JsonNode? left, JsonNode? existingValue)
        {
            var op = new JsonObject
            {
                {PropertyNameOperation, OperationNameRemove},
                {PropertyNamePath, PathBuilder.ToString()}
            };
            existingValue!.AsArray().Add(op);
            return existingValue;
        }

        /// <summary>
        /// Formats an <see cref="DeltaKind.ArrayMove"/> delta.
        /// </summary>
        /// <param name="delta">The delta to format.</param>
        /// <param name="left">The left value the delta applies to.</param>
        /// <param name="existingValue">The existing Json Patch operations.</param>
        /// <returns>The formatted result.</returns>
        /// <exception cref="InvalidOperationException">
        /// Array move operations cannot be represented in Json Patch format.
        /// </exception>
        protected override JsonNode? FormatArrayMove(ref JsonDiffDelta delta, JsonNode? left, JsonNode? existingValue)
        {
            // This should never happen. Array move operations should have been flattened into deletes and adds.
            throw new InvalidOperationException("Array move cannot be formatted.");
        }

        /// <summary>
        /// Formats a <see cref="DeltaKind.Text"/> delta.
        /// </summary>
        /// <param name="delta">The delta to format.</param>
        /// <param name="left">The left text value the delta applies to.</param>
        /// <param name="existingValue">The existing Json Patch operations.</param>
        /// <returns>The formatted result.</returns>
        /// <exception cref="NotSupportedException">
        /// Text diff is not supported by Json Patch format.
        /// </exception>
        protected override JsonNode? FormatTextDiff(ref JsonDiffDelta delta, JsonValue? left, JsonNode? existingValue)
        {
            throw new NotSupportedException("Text diff is not supported by Json Patch.");
        }

        private readonly struct PropertyPathScope : IDisposable
        {
            private readonly StringBuilder _pathBuilder;
            private readonly int _startIndex;
            private readonly int _length;

            public PropertyPathScope(StringBuilder pathBuilder, string propertyName)
            {
                _pathBuilder = pathBuilder;
                _startIndex = pathBuilder.Length;
                pathBuilder.Append('/');
                pathBuilder.Append(Escape(propertyName));
                _length = pathBuilder.Length - _startIndex;
            }

            public PropertyPathScope(StringBuilder pathBuilder, int index)
            {
                _pathBuilder = pathBuilder;
                _startIndex = pathBuilder.Length;
                pathBuilder.Append('/');
                pathBuilder.Append(index.ToString("D"));
                _length = pathBuilder.Length - _startIndex;
            }

            public void Dispose()
            {
                _pathBuilder.Remove(_startIndex, _length);
            }

            private static string Escape(string str)
            {
                // Escape Json Pointer as per https://datatracker.ietf.org/doc/html/rfc6901#section-3
                var sb = new StringBuilder(str);
                for (var i = 0; i < sb.Length; i++)
                {
                    if (sb[i] == '/')
                    {
                        sb.Insert(i, '~');
                        sb[++i] = '1';
                    }
                    else if (sb[i] == '~')
                    {
                        sb.Insert(i, '~');
                        sb[++i] = '0';
                    }
                }

                return sb.ToString();
            }
        }
    }
}