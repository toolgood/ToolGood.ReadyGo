using System.Text.Json.Nodes;
using ToolGood.ReadyGo.JsonDiffPatch.Diffs;

namespace ToolGood.ReadyGo.JsonDiffPatch
{
    static partial class JsonDiffPatcher
    {
        // Object diff:
        // https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#object-with-inner-changes
        private static void DiffObject(
            ref JsonDiffDelta delta, 
            JsonObject left, 
            JsonObject right,
            JsonDiffOptions? options)
        {
            JsonDiffContext? diffContext = null;
            var propertyFilter = options?.PropertyFilter;
            if (propertyFilter is not null)
            {
                diffContext = new JsonDiffContext(left, right);
            }

            foreach (var kvp in left)
            {
                var prop = kvp.Key;
                var leftValue = kvp.Value;

                if (propertyFilter is not null && !propertyFilter(prop, diffContext!))
                {
                    continue;
                }

                if (!right.TryGetPropertyValue(prop, out var rightValue))
                {
                    // Deleted: https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#deleted
                    delta.ObjectChange(prop, JsonDiffDelta.CreateDeleted(leftValue));
                }
                else
                {
                    // Modified: https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#modified
                    var valueDiff = new JsonDiffDelta();
                    DiffInternal(ref valueDiff, leftValue, rightValue, options);
                    if (valueDiff.Document is not null)
                    {
                        delta.ObjectChange(prop, valueDiff);
                    }
                }
            }

            foreach (var kvp in right)
            {
                var prop = kvp.Key;
                var rightValue = kvp.Value;

                if (propertyFilter is not null && !propertyFilter(prop, diffContext!))
                {
                    continue;
                }

                if (!left.ContainsKey(prop))
                {
                    // Added: https://github.com/benjamine/jsondiffpatch/blob/master/docs/deltas.md#added
                    delta.ObjectChange(prop, JsonDiffDelta.CreateAdded(rightValue));
                }
            }
        }
    }
}
