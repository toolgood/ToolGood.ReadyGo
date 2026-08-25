using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Nodes;

namespace ToolGood.ReadyGo.JsonDiffPatch.Diffs
{
    /// <summary>
    /// The type of <see cref="JsonDiffDelta"/>.
    /// </summary>
    public enum DeltaKind
    {
        /// <summary>
        /// The delta is empty or of unknown type.
        /// </summary>
        None,
        /// <summary>
        /// The delta represents a newly added value.
        /// </summary>
        Added,
        /// <summary>
        /// The delta represents a modified value.
        /// </summary>
        Modified,
        /// <summary>
        /// The delta represents a deleted value.
        /// </summary>
        Deleted,
        /// <summary>
        /// The delta represents changes to array items.
        /// </summary>
        Array,
        /// <summary>
        /// The delta represents an array item move.
        /// </summary>
        ArrayMove,
        /// <summary>
        /// The delta represents changes to object properties.
        /// </summary>
        Object,
        /// <summary>
        /// The delta represents a text diff.
        /// </summary>
        Text
    }
    
    /// <summary>
    /// Implements JSON diff delta format described at <see href="https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md"/>.
    /// </summary>
    public struct JsonDiffDelta
    {
        internal const string InvalidPatchDocument = "Invalid patch document.";
        private const int OpTypeDeleted = 0;
        private const int OpTypeTextDiff = 2;
        private const int OpTypeArrayMoved = 3;

        private const string TypePropertyName = "_t";
        private const string ArrayType = "a";

        private JsonNode? _document;

        /// <summary>
        /// Initializes a new instance of <see cref="JsonDiffDelta"/> from the specified delta document.
        /// </summary>
        /// <param name="document">The delta document.</param>
        public JsonDiffDelta(JsonNode document)
        {
            _document = document;
            Kind = GetDeltaKind(document);
        }

        /// <summary>
        /// Gets the underlying delta document.
        /// </summary>
        public JsonNode? Document
        {
            get => _document;
            private set
            {
                _document = value;
                Kind = GetDeltaKind(value);
            }
        }

        /// <summary>
        /// Gets the kind of delta represented by <see cref="Document"/>.
        /// </summary>
        public DeltaKind Kind { get; private set; }

        private void CheckForKind(DeltaKind expectedKind)
        {
            var kind = Kind;
            if (Kind != expectedKind)
            {
                throw new InvalidOperationException($"Unable to get value from delta of type '{kind}'.");
            }
        }

        /// <summary>
        /// Gets the value that was added. The delta kind must be <see cref="DeltaKind.Added"/>.
        /// </summary>
        /// <returns>The added value.</returns>
        public JsonNode? GetAdded()
        {
            CheckForKind(DeltaKind.Added);
            return GetOrClone(Document!.AsArray()[0]);
        }

        /// <summary>
        /// Gets the value that was deleted. The delta kind must be <see cref="DeltaKind.Deleted"/>.
        /// </summary>
        /// <returns>The deleted value.</returns>
        public JsonNode? GetDeleted()
        {
            CheckForKind(DeltaKind.Deleted);
            return GetOrClone(Document!.AsArray()[0]);
        }

        /// <summary>
        /// Gets the new value of a modified delta. The delta kind must be <see cref="DeltaKind.Modified"/>.
        /// </summary>
        /// <returns>The new value.</returns>
        public JsonNode? GetNewValue()
        {
            CheckForKind(DeltaKind.Modified);
            return GetOrClone(Document!.AsArray()[1]);
        }

        /// <summary>
        /// Gets the old value of a modified delta. The delta kind must be <see cref="DeltaKind.Modified"/>.
        /// </summary>
        /// <returns>The old value.</returns>
        public JsonNode? GetOldValue()
        {
            CheckForKind(DeltaKind.Modified);
            return GetOrClone(Document!.AsArray()[0]);
        }

        /// <summary>
        /// Gets the new index of a moved array item. The delta kind must be <see cref="DeltaKind.ArrayMove"/>.
        /// </summary>
        /// <returns>The new index.</returns>
        public int GetNewIndex()
        {
            CheckForKind(DeltaKind.ArrayMove);
            return Document!.AsArray()[1]!.GetValue<int>();
        }

        /// <summary>
        /// Gets the text diff of a text delta. The delta kind must be <see cref="DeltaKind.Text"/>.
        /// </summary>
        /// <returns>The text diff.</returns>
        public string GetTextDiff()
        {
            CheckForKind(DeltaKind.Text);
            return Document!.AsArray()[0]!.GetValue<string>();
        }
        
        /// <summary>
        /// Enumerates the array item changes. The delta kind must be <see cref="DeltaKind.Array"/>.
        /// </summary>
        /// <returns>The array item changes.</returns>
        public IEnumerable<ArrayChangeEntry> GetArrayChangeEnumerable()
        {
            CheckForKind(DeltaKind.Array);
            foreach (var kvp in Document!.AsObject())
            {
                if (IsTypeProperty(kvp.Key) || !TryGetArrayIndex(kvp.Key, out var index, out _))
                {
                    continue;
                }

                yield return new ArrayChangeEntry(index, kvp.Value!);
            }
        }

        /// <summary>
        /// Enumerates the array item changes in patchable order against the specified left array.
        /// The delta kind must be <see cref="DeltaKind.Array"/>.
        /// </summary>
        /// <param name="left">The left array the delta applies to.</param>
        /// <returns>The array item changes.</returns>
        public IEnumerable<ArrayChangeEntry> GetPatchableArrayChangeEnumerable(JsonArray left)
        {
            return GetPatchableArrayChangeEnumerable(left, false);
        }

        internal IEnumerable<ArrayChangeEntry> GetPatchableArrayChangeEnumerable(JsonArray left, bool isReversing)
        {
            _ = left ?? throw new ArgumentNullException(nameof(left));
            
            CheckForKind(DeltaKind.Array);

            var arrayPatch = Document!.AsObject();
            var deleteItems = new List<ArrayChangeEntry>(left.Count / 3);
            var addItems = new List<ArrayChangeEntry>(left.Count / 3);
            var patchItems = new List<ArrayChangeEntry>(left.Count / 3);

            // Return items in order:
            // 1. Items to delete
            // 2. Items to add
            // 3. Items to patch
            foreach (var prop in arrayPatch)
            {
                var propertyName = prop.Key;
                if (IsTypeProperty(propertyName))
                {
                    continue;
                }

                var innerPatch = prop.Value;
                if (innerPatch is null)
                {
                    continue;
                }

                if (!TryGetArrayIndex(propertyName, out var index, out var isLeft))
                {
                    throw new FormatException(InvalidPatchDocument);
                }

                var entry = new ArrayChangeEntry(index, innerPatch);
                var kind = entry.Diff.Kind;
                // The left array can only contain deleted or array move operations
                if (isLeft && kind is not DeltaKind.Deleted && kind is not DeltaKind.ArrayMove)
                {
                    throw new FormatException(InvalidPatchDocument);
                }

                if (kind == DeltaKind.Deleted)
                {
                    if (isReversing)
                    {
                        addItems.Add(entry);
                    }
                    else
                    {
                        deleteItems.Add(entry);
                    }
                }
                else if (kind == DeltaKind.ArrayMove)
                {
                    if (isReversing)
                    {
                        var newIndex = entry.Diff.GetNewIndex();
                        if (newIndex < 0 || newIndex >= left.Count)
                        {
                            throw new FormatException(InvalidPatchDocument);
                        }

                        // Delete the item at new index
                        deleteItems.Add(new(newIndex, CreateAdded(null)));
                        // Add it back later at old index
                        addItems.Add(new(index, CreateDeleted(left[newIndex])));
                    }
                    else
                    {
                        if (index < 0 || index >= left.Count)
                        {
                            throw new FormatException(InvalidPatchDocument);
                        }

                        // Delete the item at old index
                        deleteItems.Add(new(index, CreateDeleted(null)));
                        // Add it back later at new index
                        addItems.Add(new(entry.Diff.GetNewIndex(), CreateAdded(left[index])));
                    }
                }
                else if (kind == DeltaKind.Added)
                {
                    if (isReversing)
                    {
                        deleteItems.Add(entry);
                    }
                    else
                    {
                        addItems.Add(entry);
                    }
                }
                else
                {
                    patchItems.Add(entry);
                }
            }

            // Sort items to delete in descending order
            deleteItems.Sort(DescendingCompare);
            // Sort items to add in ascending order
            addItems.Sort(AscendingCompare);

            var enumerable = isReversing
                ? patchItems.Concat(deleteItems).Concat(addItems)
                : deleteItems.Concat(addItems).Concat(patchItems);

            foreach (var kvp in enumerable)
            {
                yield return kvp;
            }

            static int AscendingCompare(ArrayChangeEntry x, ArrayChangeEntry y)
            {
                return x.Index - y.Index;
            }

            static int DescendingCompare(ArrayChangeEntry x, ArrayChangeEntry y)
            {
                return y.Index - x.Index;
            }
        }

        /// <summary>
        /// Sets the added value of an <see cref="DeltaKind.Added"/> delta.
        /// </summary>
        /// <param name="newValue">The new value to add.</param>
        public void Added(JsonNode? newValue)
        {
            EnsureDeltaType(nameof(Added), count: 1);
            var arr = Document!.AsArray();
            arr[0] = newValue?.DeepClone();
        }

        /// <summary>
        /// Sets the old and new values of a <see cref="DeltaKind.Modified"/> delta.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public void Modified(JsonNode? oldValue, JsonNode? newValue)
        {
            EnsureDeltaType(nameof(Modified), count: 2);
            var arr = Document!.AsArray();
            arr[0] = oldValue?.DeepClone();
            arr[1] = newValue?.DeepClone();
        }

        /// <summary>
        /// Sets the deleted value of a <see cref="DeltaKind.Deleted"/> delta.
        /// </summary>
        /// <param name="oldValue">The value that was deleted.</param>
        public void Deleted(JsonNode? oldValue)
        {
            EnsureDeltaType(nameof(Deleted), count: 3, opType: OpTypeDeleted);
            var arr = Document!.AsArray();
            arr[0] = oldValue?.DeepClone();
            arr[1] = 0;
        }

        /// <summary>
        /// Marks the delta as an array item move from a deleted item to the specified new position.
        /// </summary>
        /// <param name="newPosition">The new position of the moved item.</param>
        public void ArrayMoveFromDeleted(int newPosition)
        {
            EnsureDeltaType(nameof(ArrayMoveFromDeleted), count: 3, opType: OpTypeDeleted);
            var arr = Document!.AsArray();
            arr[0] = "";
            arr[1] = newPosition;
            arr[2] = OpTypeArrayMoved;
        }
        
        internal void ArrayMoveFromDeleted(int index, int newPosition)
        {
            if (Document is not JsonObject obj)
            {
                return;
            }

            if (!obj.TryGetPropertyValue($"_{index:D}", out var itemDelta)
                || itemDelta is null)
            {
                return;
            }

            var newItemDelta = new JsonDiffDelta(itemDelta);
            newItemDelta.ArrayMoveFromDeleted(newPosition);
        }

        /// <summary>
        /// Adds an array item change to an <see cref="DeltaKind.Array"/> delta.
        /// </summary>
        /// <param name="index">The index of the changed array item.</param>
        /// <param name="isLeft">Whether the index refers to the left (original) array.</param>
        /// <param name="innerChange">The inner delta of the array item.</param>
        public void ArrayChange(int index, bool isLeft, JsonDiffDelta innerChange)
        {
            if (innerChange.Document is null)
            {
                return;
            }

            var result = innerChange.Document;
            Debug.Assert(result.Parent is null);

            if (result.Parent is not null)
            {
                // This can be very slow. We don't want this to happen but
                // in the meantime, we can't fail the operation due to this
                result = result.DeepClone();
            }

            EnsureDeltaType(nameof(ArrayChange), isArrayChange: true);
            var obj = Document!.AsObject();
            obj.Add(isLeft ? $"_{index:D}" : $"{index:D}", result);
        }

        /// <summary>
        /// Adds an object property change to an <see cref="DeltaKind.Object"/> delta.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        /// <param name="innerChange">The inner delta of the property.</param>
        public void ObjectChange(string propertyName, JsonDiffDelta innerChange)
        {
            if (innerChange.Document is null)
            {
                return;
            }

            var result = innerChange.Document;
            Debug.Assert(result.Parent is null);

            if (result.Parent is not null)
            {
                // This can be very slow. We don't want this to happen but
                // in the meantime, we can't fail the operation due to this
                result = result.DeepClone();
            }

            EnsureDeltaType(nameof(ObjectChange));
            var obj = Document!.AsObject();
            obj.Add(propertyName, result);
        }

        /// <summary>
        /// Sets the text diff of a <see cref="DeltaKind.Text"/> delta.
        /// </summary>
        /// <param name="diff">The text diff.</param>
        public void Text(string diff)
        {
            EnsureDeltaType(nameof(Text), count: 3, opType: OpTypeTextDiff);
            var arr = Document!.AsArray();
            arr[0] = diff;
            arr[1] = 0;
            arr[2] = OpTypeTextDiff;
        }

        private void EnsureDeltaType(string opName, int count = 0, int opType = 0,
            bool isArrayChange = false)
        {
            if (count == 0)
            {
                // Object delta, i.e. object and array

                if (Document is null)
                {
                    Document = isArrayChange
                        ? new JsonObject {{TypePropertyName, ArrayType}}
                        : new JsonObject();
                    return;
                }

                if (Document is JsonObject deltaObject)
                {
                    // Check delta object is for array
                    string? deltaType = null;
                    deltaObject.TryGetPropertyValue(TypePropertyName, out var typeNode);
                    // Perf: this is fine we shouldn't have a node backed by JsonElement here
                    typeNode?.AsValue().TryGetValue(out deltaType);

                    if (string.Equals(deltaType, "a") == isArrayChange)
                    {
                        return;
                    }
                }
            }
            else
            {
                // Value delta
                if (Document is null)
                {
                    var newDeltaArray = new JsonArray();
                    for (var i = 0; i < count; i++)
                    {
                        if (i == 2)
                        {
                            newDeltaArray.Add(opType);
                        }
                        else
                        {
                            newDeltaArray.Add(null);
                        }
                    }

                    Document = newDeltaArray;
                    return;
                }

                if (Document is JsonArray deltaArray && deltaArray.Count == count)
                {
                    if (count < 3)
                    {
                        return;
                    }

                    if (deltaArray[count - 1]?.AsValue().GetValue<int>() == opType)
                    {
                        return;
                    }
                }
            }

            throw new InvalidOperationException(
                $"Operation '{opName}' cannot be performed on current delta result.");
        }

        private static DeltaKind GetDeltaKind(JsonNode? delta)
        {
            return delta switch
            {
                JsonArray arr => arr.Count switch
                {
                    1 => DeltaKind.Added,
                    2 => DeltaKind.Modified,
                    3 when arr[2] is JsonValue opType => GetDeltaKindFromOpType(opType),
                    _ => DeltaKind.None
                },
                JsonObject obj => GetDeltaKindFromJsonObject(obj),
                _ => DeltaKind.None
            };

            static DeltaKind GetDeltaKindFromJsonObject(JsonObject obj)
            {
                if (obj.TryGetPropertyValue(TypePropertyName, out var typeParam) &&
                    typeParam is JsonValue typeParamValue &&
                    typeParamValue.TryGetValue<string>(out var typeParamValueStr) &&
                    string.Equals(ArrayType, typeParamValueStr, StringComparison.Ordinal))
                {
                    return DeltaKind.Array;
                }

                return DeltaKind.Object;
            }

            static DeltaKind GetDeltaKindFromOpType(JsonValue opType)
            {
                if (!opType.TryGetValue<int>(out var opTypeValue))
                {
                    return DeltaKind.None;
                }

                return opTypeValue switch
                {
                    OpTypeDeleted => DeltaKind.Deleted,
                    OpTypeArrayMoved => DeltaKind.ArrayMove,
                    OpTypeTextDiff => DeltaKind.Text,
                    _ => DeltaKind.None
                };
            }
        }

        private static JsonNode? GetOrClone(JsonNode? value)
        {
            return value?.Parent is null ? value : value.DeepClone();
        }

        internal static JsonDiffDelta CreateAdded(JsonNode? newValue)
        {
            var delta = new JsonDiffDelta();
            delta.Added(newValue);
            return delta;
        }

        internal static JsonDiffDelta CreateDeleted(JsonNode? oldValue)
        {
            var delta = new JsonDiffDelta();
            delta.Deleted(oldValue);
            return delta;
        }
        
        internal static bool TryGetArrayIndex(string propertyName, out int index, out bool isLeft)
        {
            isLeft = propertyName.StartsWith("_");
            if (int.TryParse(isLeft ? propertyName.Substring(1) : propertyName, out index))
            {
                return true;
            }

            isLeft = false;
            index = 0;
            return false;
        }

        internal static bool IsTypeProperty(string propertyName)
        {
            return string.Equals(TypePropertyName, propertyName);
        }
        
        /// <summary>
        /// Represents a single array item change.
        /// </summary>
        public readonly struct ArrayChangeEntry
        {
            internal ArrayChangeEntry(int index, JsonNode diff)
            {
                Index = index;
                Diff = new JsonDiffDelta(diff);
            }
            
            internal ArrayChangeEntry(int index, JsonDiffDelta diff)
            {
                Index = index;
                Diff = diff;
            }
            
            /// <summary>
            /// Gets the index of the changed array item.
            /// </summary>
            public int Index { get; }
            /// <summary>
            /// Gets the delta of the changed array item.
            /// </summary>
            public JsonDiffDelta Diff { get; }
        }
    }
}
