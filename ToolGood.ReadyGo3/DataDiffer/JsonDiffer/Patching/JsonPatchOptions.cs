using ToolGood.ReadyGo3.JsonDiffPatch.Patching;

namespace ToolGood.ReadyGo3.JsonDiffPatch
{
    /// <summary>
    /// Represents options for patching JSON object.
    /// </summary>
    public struct JsonPatchOptions
    {
        /// <summary>
        /// Gets or sets the function to patch long text.
        /// </summary>
        public TextPatch? TextPatchProvider { get; set; }
    }
}
