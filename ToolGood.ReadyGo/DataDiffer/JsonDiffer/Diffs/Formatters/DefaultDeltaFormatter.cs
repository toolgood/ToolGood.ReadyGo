using System;
using System.Linq;
using System.Text.Json.Nodes;

namespace ToolGood.ReadyGo.JsonDiffPatch.Diffs.Formatters
{
    /// <summary>
    /// Provides a default implementation for formatting <see cref="JsonDiffDelta"/> into a result value.
    /// </summary>
    /// <typeparam name="TResult">The type of the formatting result.</typeparam>
    public abstract class DefaultDeltaFormatter<TResult> : IJsonDiffDeltaFormatter<TResult>
    {
        private readonly bool _usePatchableArrayChangeEnumerable;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDeltaFormatter{TResult}"/> class.
        /// </summary>
        protected DefaultDeltaFormatter()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDeltaFormatter{TResult}"/> class.
        /// </summary>
        /// <param name="usePatchableArrayChangeEnumerable">
        /// Whether array changes should be enumerated in patchable order.
        /// </param>
        protected DefaultDeltaFormatter(bool usePatchableArrayChangeEnumerable)
        {
            _usePatchableArrayChangeEnumerable = usePatchableArrayChangeEnumerable;
        }

        /// <summary>
        /// Formats the specified delta against the left value into a result value.
        /// </summary>
        /// <param name="delta">The delta to format.</param>
        /// <param name="left">The left value the delta applies to.</param>
        /// <returns>The formatted result.</returns>
        public virtual TResult? Format(ref JsonDiffDelta delta, JsonNode? left)
        {
            var value = CreateDefault();
            return FormatJsonDiffDelta(ref delta, left, value);
        }

        /// <summary>
        /// Creates the default result value.
        /// </summary>
        /// <returns>The default result value.</returns>
        protected virtual TResult? CreateDefault()
        {
            return default;
        }

        /// <summary>
        /// Formats a <see cref="JsonDiffDelta"/> according to its <see cref="DeltaKind"/>.
        /// </summary>
        /// <param name="delta">The delta to format.</param>
        /// <param name="left">The left value the delta applies to.</param>
        /// <param name="existingValue">The existing result value to append to.</param>
        /// <returns>The formatted result.</returns>
        protected virtual TResult? FormatJsonDiffDelta(ref JsonDiffDelta delta, JsonNode? left, TResult? existingValue)
        {
            switch (delta.Kind)
            {
                case DeltaKind.Added:
                    existingValue = FormatAdded(ref delta, existingValue);
                    break;
                case DeltaKind.Modified:
                    existingValue = FormatModified(ref delta, left, existingValue);
                    break;
                case DeltaKind.Deleted:
                    existingValue = FormatDeleted(ref delta, left, existingValue);
                    break;
                case DeltaKind.ArrayMove:
                    existingValue = FormatArrayMove(ref delta, left, existingValue);
                    break;
                case DeltaKind.Text:
                    existingValue = FormatTextDiff(ref delta, CheckType<JsonValue>(left), existingValue);
                    break;
                case DeltaKind.Array:
                    existingValue = FormatArray(ref delta, CheckType<JsonArray>(left), existingValue);
                    break;
                case DeltaKind.Object:
                    existingValue = FormatObject(ref delta, CheckType<JsonObject>(left), existingValue);
                    break;
            }

            return existingValue;

            static T CheckType<T>(JsonNode? node)
            {
                return node switch
                {
                    T returnValue => returnValue,
                    _ => throw new FormatException(JsonDiffDelta.InvalidPatchDocument)
                };
            }
        }

        /// <summary>
        /// Formats an <see cref="DeltaKind.Added"/> delta.
        /// </summary>
        /// <param name="delta">The delta to format.</param>
        /// <param name="existingValue">The existing result value to append to.</param>
        /// <returns>The formatted result.</returns>
        protected abstract TResult? FormatAdded(ref JsonDiffDelta delta, TResult? existingValue);

        /// <summary>
        /// Formats a <see cref="DeltaKind.Modified"/> delta.
        /// </summary>
        /// <param name="delta">The delta to format.</param>
        /// <param name="left">The left value the delta applies to.</param>
        /// <param name="existingValue">The existing result value to append to.</param>
        /// <returns>The formatted result.</returns>
        protected abstract TResult? FormatModified(ref JsonDiffDelta delta, JsonNode? left, TResult? existingValue);

        /// <summary>
        /// Formats a <see cref="DeltaKind.Deleted"/> delta.
        /// </summary>
        /// <param name="delta">The delta to format.</param>
        /// <param name="left">The left value the delta applies to.</param>
        /// <param name="existingValue">The existing result value to append to.</param>
        /// <returns>The formatted result.</returns>
        protected abstract TResult? FormatDeleted(ref JsonDiffDelta delta, JsonNode? left, TResult? existingValue);

        /// <summary>
        /// Formats an <see cref="DeltaKind.ArrayMove"/> delta.
        /// </summary>
        /// <param name="delta">The delta to format.</param>
        /// <param name="left">The left value the delta applies to.</param>
        /// <param name="existingValue">The existing result value to append to.</param>
        /// <returns>The formatted result.</returns>
        protected abstract TResult? FormatArrayMove(ref JsonDiffDelta delta, JsonNode? left, TResult? existingValue);

        /// <summary>
        /// Formats a <see cref="DeltaKind.Text"/> delta.
        /// </summary>
        /// <param name="delta">The delta to format.</param>
        /// <param name="left">The left text value the delta applies to.</param>
        /// <param name="existingValue">The existing result value to append to.</param>
        /// <returns>The formatted result.</returns>
        protected abstract TResult? FormatTextDiff(ref JsonDiffDelta delta, JsonValue? left, TResult? existingValue);

        /// <summary>
        /// Formats an <see cref="DeltaKind.Array"/> delta.
        /// </summary>
        /// <param name="delta">The delta to format.</param>
        /// <param name="left">The left array the delta applies to.</param>
        /// <param name="existingValue">The existing result value to append to.</param>
        /// <returns>The formatted result.</returns>
        protected virtual TResult? FormatArray(ref JsonDiffDelta delta, JsonArray left, TResult? existingValue)
        {
            var arrayChangeEnumerable = _usePatchableArrayChangeEnumerable
                ? delta.GetPatchableArrayChangeEnumerable(left)
                : delta.GetArrayChangeEnumerable();

            return arrayChangeEnumerable
                .Aggregate(existingValue, (current, entry) =>
                {
                    var elementDelta = entry.Diff;
                    var leftValue = elementDelta.Kind switch
                    {
                        DeltaKind.Added or DeltaKind.None => null,
                        _ => entry.Index < 0 || entry.Index >= left.Count
                            ? throw new FormatException(JsonDiffDelta.InvalidPatchDocument)
                            : left[entry.Index]
                    };
                    return FormatArrayElement(entry, leftValue, current);
                });
        }

        /// <summary>
        /// Formats a single array item change.
        /// </summary>
        /// <param name="arrayChange">The array item change to format.</param>
        /// <param name="left">The left value of the array item.</param>
        /// <param name="existingValue">The existing result value to append to.</param>
        /// <returns>The formatted result.</returns>
        protected virtual TResult? FormatArrayElement(in JsonDiffDelta.ArrayChangeEntry arrayChange, JsonNode? left, TResult? existingValue)
        {
            var delta = arrayChange.Diff;
            return FormatJsonDiffDelta(ref delta, left, existingValue);
        }

        /// <summary>
        /// Formats an <see cref="DeltaKind.Object"/> delta.
        /// </summary>
        /// <param name="delta">The delta to format.</param>
        /// <param name="left">The left object the delta applies to.</param>
        /// <param name="existingValue">The existing result value to append to.</param>
        /// <returns>The formatted result.</returns>
        protected virtual TResult? FormatObject(ref JsonDiffDelta delta, JsonObject left, TResult? existingValue)
        {
            var deltaDocument = delta.Document!.AsObject();
            foreach (var prop in deltaDocument)
            {
                var propDelta = new JsonDiffDelta(prop.Value!);
                left.TryGetPropertyValue(prop.Key, out var leftValue);
                existingValue = FormatObjectProperty(ref propDelta, leftValue, prop.Key, existingValue);
            }

            return existingValue;
        }

        /// <summary>
        /// Formats a single object property change.
        /// </summary>
        /// <param name="delta">The property delta to format.</param>
        /// <param name="left">The left value of the property.</param>
        /// <param name="propertyName">The name of the changed property.</param>
        /// <param name="existingValue">The existing result value to append to.</param>
        /// <returns>The formatted result.</returns>
        protected virtual TResult? FormatObjectProperty(ref JsonDiffDelta delta, JsonNode? left, string propertyName, TResult? existingValue)
        {
            return FormatJsonDiffDelta(ref delta, left, existingValue);
        }
    }
}