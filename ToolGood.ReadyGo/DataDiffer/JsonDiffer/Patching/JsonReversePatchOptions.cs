using ToolGood.ReadyGo.JsonDiffPatch.Patching;

namespace ToolGood.ReadyGo.JsonDiffPatch
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
