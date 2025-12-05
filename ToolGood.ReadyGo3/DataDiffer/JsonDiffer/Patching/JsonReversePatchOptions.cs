using ToolGood.ReadyGo3.JsonDiffPatch.Patching;

namespace ToolGood.ReadyGo3.JsonDiffPatch
{
    /// <summary>
    /// Represents options for patching JSON object.
    /// </summary>
    public struct JsonReversePatchOptions
    {
        /// <summary>
        /// Gets or sets the function to reverse long text patch.
        /// </summary>
        public TextPatch? ReverseTextPatchProvider { get; set; }
    }
}
