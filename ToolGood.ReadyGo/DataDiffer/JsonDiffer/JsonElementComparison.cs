namespace ToolGood.ReadyGo.JsonDiffPatch
{
    /// <summary>
    /// Represents <see cref="System.Text.Json.JsonElement"/> comparison modes.
    /// </summary>
    public enum JsonElementComparison
    {
        /// <summary>
        /// Only compares raw text of two <see cref="System.Text.Json.JsonElement"/> instances.
        /// </summary>
        RawText,
        
        /// <summary>
        /// Deserializes both <see cref="System.Text.Json.JsonElement"/> instances into value object of the most significant type
        /// and compares the value objects.
        /// </summary>
        Semantic
    }
}