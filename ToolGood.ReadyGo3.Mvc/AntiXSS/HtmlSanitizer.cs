using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ToolGood.ReadyGo3.Mvc.HtmlAgilityPack;
using System.Web;

namespace ToolGood.ReadyGo3.Mvc.AntiXSS
{
    public class HtmlSanitizer
    {
        /// <summary>
        /// The default allowed URI schemes.
        /// </summary>
        public static readonly ISet<string> DefaultAllowedSchemes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "http", "https","mailto" };

        /// <summary>
        /// The default allowed HTML tag names.
        /// </summary>
        public static readonly ISet<string> DefaultAllowedTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            // https://developer.mozilla.org/en/docs/Web/Guide/HTML/HTML5/HTML5_element_list
            "a", "abbr", "acronym", "address", "area", "b",
            "big", "blockquote", "br", "button", "caption", "center", "cite",
            "code", "col", "colgroup", "dd", "del", "dfn", "dir", "div", "dl", "dt",
            "em", "fieldset", "font", "form", "h1", "h2", "h3", "h4", "h5", "h6",
            "hr", "i", "img", "input", "ins", "kbd", "label", "legend", "li", "map",
            "menu", "ol", "optgroup", "option", "p", "pre", "q", "s", "samp",
            "select", "small", "span", "strike", "strong", "sub", "sup", "table",
            "tbody", "td", "textarea", "tfoot", "th", "thead", "tr", "tt", "u",
            "ul", "var",
            // HTML5
            // Sections
            "section", "nav", "article", "aside", "header", "footer", "main",
            // Grouping content
            "figure", "figcaption",
            // Text-level semantics
            "data", "time", "mark", "ruby", "rt", "rp", "bdi", "wbr",
            // Forms
            "datalist", "keygen", "output", "progress", "meter",
            // Interactive elements
            "details", "summary", "menuitem",
            // document elements
            "html", /*"head",*/ "body"
        };

        /// <summary>
        /// The default allowed HTML attributes.
        /// </summary>
        public static readonly ISet<string> DefaultAllowedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            // https://developer.mozilla.org/en-US/docs/Web/HTML/Attributes
            "abbr", "accept", "accept-charset", "accesskey",
            "action", "align", "alt", "axis", "bgcolor", "border", "cellpadding",
            "cellspacing", "char", "charoff", "charset", "checked", "cite",  "class",
            "clear", "cols", "colspan", "color", "compact", "coords", "datetime",
            "dir", "disabled", "enctype", "for", "frame", "headers", "height",
            "href", "hreflang", "hspace", /* "id", */ "ismap", "label", "lang",
            "longdesc", "maxlength", "media", "method", "multiple", "name",
            "nohref", "noshade", "nowrap", "prompt", "readonly", "rel", "rev",
            "rows", "rowspan", "rules", "scope", "selected", "shape", "size",
            "span", "src", "start", "style", "summary", "tabindex", "target", "title",
            "type", "usemap", "valign", "value", "vspace", "width",
            // HTML5
            "high", // <meter>
            "keytype", // <keygen>
            "list", // <input>
            "low", // <meter>
            "max", // <input>, <meter>, <progress>
            "min", // <input>, <meter>
            "novalidate", // <form>
            "open", // <details>
            "optimum", // <meter>
            "pattern", // <input>
            "placeholder", // <input>, <textarea>
            "pubdate", // <time>
            "radiogroup", // <menuitem>
            "required", // <input>, <select>, <textarea>
            "reversed", // <ol>
            "spellcheck", // Global attribute
            "step", // <input>
            "wrap", // <textarea>
            "challenge", // <keygen>
            "contenteditable", // Global attribute
            "draggable", // Global attribute
            "dropzone", // Global attribute
            "autocomplete", // <form>, <input>
            "autosave", // <input>
            "action", "background", "dynsrc", "href", "lowsrc", "src"
        };

        /// <summary>
        /// The default URI attributes.
        /// </summary>
        public static readonly ISet<string> DefaultUriAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "action", "background", "dynsrc", "href", "lowsrc", "src" };

        /// <summary>
        /// The default allowed CSS properties.
        /// </summary>
        public static readonly ISet<string> DefaultAllowedCssProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            // CSS 3 properties <http://www.w3.org/TR/CSS/#properties>
            "background", "background-attachment", "background-color",
            "background-image", "background-position", "background-repeat",
            "border", "border-bottom", "border-bottom-color",
            "border-bottom-style", "border-bottom-width", "border-collapse",
            "border-color", "border-left", "border-left-color",
            "border-left-style", "border-left-width", "border-right",
            "border-right-color", "border-right-style", "border-right-width",
            "border-spacing", "border-style", "border-top", "border-top-color",
            "border-top-style", "border-top-width", "border-width", "bottom",
            "caption-side", "clear", "clip", "color", "content",
            "counter-increment", "counter-reset", "cursor", "direction", "display",
            "empty-cells", "float", "font", "font-family", "font-size",
            "font-style", "font-variant", "font-weight", "height", "left",
            "letter-spacing", "line-height", "list-style", "list-style-image",
            "list-style-position", "list-style-type", "margin", "margin-bottom",
            "margin-left", "margin-right", "margin-top", "max-height", "max-width",
            "min-height", "min-width", "opacity", "orphans", "outline",
            "outline-color", "outline-style", "outline-width", "overflow",
            "padding", "padding-bottom", "padding-left", "padding-right",
            "padding-top", "page-break-after", "page-break-before",
            "page-break-inside", "quotes", "right", "table-layout",
            "text-align", "text-decoration", "text-indent", "text-transform",
            "top", "unicode-bidi", "vertical-align", "visibility", "white-space",
            "widows", "width", "word-spacing", "z-index" };

        // from http://genshi.edgewall.org/
        private static readonly Regex CssUnicodeEscapes = new Regex(@"\\([0-9a-fA-F]{1,6})\s?|\\([^\r\n\f0-9a-fA-F'""{};:()#*])", RegexOptions.Compiled);
        private static readonly Regex CssComments = new Regex(@"/\*.*?\*/", RegexOptions.Compiled);
        // IE6 <http://heideri.ch/jso/#80>
        private static readonly Regex CssExpression = new Regex(@"[eE\uFF25\uFF45][xX\uFF38\uFF58][pP\uFF30\uFF50][rR\u0280\uFF32\uFF52][eE\uFF25\uFF45][sS\uFF33\uFF53]{2}[iI\u026A\uFF29\uFF49][oO\uFF2F\uFF4F][nN\u0274\uFF2E\uFF4E]", RegexOptions.Compiled);
        private static readonly Regex CssUrl = new Regex(@"[Uu][Rr\u0280][Ll\u029F]\s*\(\s*(['""]?)\s*([^'"")\s]+)\s*(['""]?)\s*", RegexOptions.Compiled);


        public static string Sanitize(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            TryRemoveTag(document.DocumentNode);
            html = GetBody(document.DocumentNode);
            document = null;
            return html;
        }
        private static string GetBody(HtmlNode node)
        {
            if (node.HasChildNodes) {
                if (node.FirstChild.Name == "html") {
                    foreach (var nd in node.FirstChild.ChildNodes) {
                        if (nd.Name == "body") {
                            return nd.InnerHtml;
                        }
                    }
                    return "";
                }
            }
            return node.InnerHtml;
        }

        private static void TryRemoveTag(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Comment) { node.Remove(); return; }
            if (node.NodeType == HtmlNodeType.Element) {
                if (DefaultAllowedTags.Contains(node.Name) == false) { node.Remove(); return; }
                TryRemoveTagAttribute(node.Attributes);

                for (int i = node.ChildNodes.Count - 1; i >= 0; i--) {
                    var cn = node.ChildNodes[i];
                    TryRemoveTag(cn);
                }
            }
            if (node.NodeType == HtmlNodeType.Text) {
                var s = HttpUtility.HtmlDecode(node.InnerHtml);
                var temp = new StringBuilder();
                for (var i = 0; i < s.Length; i++) {
                    switch (s[i]) {
                        case '&': temp.Append("&amp;"); break;
                        case '\u00a0': temp.Append("&nbsp;"); break;
                        //case '"': temp.Append("&quot;"); break;
                        case '<': temp.Append("&lt;"); break;
                        case '>': temp.Append("&gt;"); break;
                        default: temp.Append(s[i]); break;
                    }
                }
                node.InnerHtml = temp.ToString();
            }
            if (node.NodeType == HtmlNodeType.Document) {
                for (int i = node.ChildNodes.Count - 1; i >= 0; i--) {
                    var cn = node.ChildNodes[i];
                    TryRemoveTag(cn);
                }
            }


        }
        private static void TryRemoveTagAttribute(HtmlAttributeCollection atts)
        {
            for (int i = atts.Count - 1; i >= 0; i--) {
                var item = atts[i];
                if (DefaultAllowedAttributes.Contains(item.Name) == false) { item.Remove(); continue; }
                if (item.Value.Contains("&{")) { item.Remove(); continue; }
                if (DefaultUriAttributes.Contains(item.Name)) {
                    if (TryRemoveSchemes(item)) continue;
                }
                if (item.Name.ToLower() == "style") {
                    TryRemoveStyle(item);
                }
            }
        }
        private static bool TryRemoveSchemes(HtmlAttribute attribute)
        {
            var v = attribute.Value;
            if (string.IsNullOrEmpty(v) == false) {
                v = HttpUtility.HtmlDecode(v);
                if (v.Contains("&#") == false) {
                    var url = SanitizeUrl(v);
                    if (string.IsNullOrEmpty(url) == false) {
                        attribute.Value = url;
                        return false;
                    }
                }
            }
            attribute.Remove();
            return true;
        }


        private static void TryRemoveStyle(HtmlAttribute attribute)
        {
            var css = attribute.Value;
            if (string.IsNullOrWhiteSpace(css)) {
                attribute.Remove();
                return;
            }
            var isChange = false;
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            css = HttpUtility.HtmlDecode(css);

            var sp = css.Split(';');


            foreach (var s in sp) {
                var index = s.IndexOf(':');
                if (index == -1) { isChange = true; continue; }
                var key = DecodeCss(s.Substring(0, index)).Trim();
                var val = DecodeCss(s.Substring(index + 1)).Trim();
                if (val.Contains("<")) { isChange = true; continue; }
                if (!DefaultAllowedCssProperties.Contains(key)) { isChange = true; continue; }
                if (CssExpression.IsMatch(val)) { isChange = true; continue; }

                var urls = CssUrl.Matches(val);

                if (urls.Count > 0) {
                    if (urls.Cast<Match>().Any(m => SanitizeUrl(m.Groups[2].Value) == null)) { isChange = true; continue; }
                    var t = CssUrl.Replace(val, m => ("url(&quot;" + SanitizeUrl(m.Groups[2].Value) + "&quot;"));
                    isChange = true;
                    dictionary[key] = t;
                } else {
                    if (val.Contains("\\")) { isChange = true; continue; }
                    if (val.Contains("/")) { isChange = true; continue; }
                    dictionary[key] = val;
                }
            }

            if (isChange == false) return;
            if (dictionary.Count == 0) {
                attribute.Remove();
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var item in dictionary) {
                sb.Append(item.Key);
                sb.Append(":");
                sb.Append(item.Value);
                sb.Append(";");
            }
            sb.Remove(sb.Length - 1, 1);
            attribute.Value = sb.ToString();
        }

        protected static string DecodeCss(string css)
        {
            var r = CssUnicodeEscapes.Replace(css, m => {
                if (m.Groups[1].Success)
                    return ((char)int.Parse(m.Groups[1].Value, NumberStyles.HexNumber)).ToString();
                var t = m.Groups[2].Value;
                return t == "\\" ? @"\\" : t;
            });

            r = CssComments.Replace(r, m => "");

            return r;
        }



        #region SanitizeUrl
        /// <summary>
        /// Sanitizes a URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="baseUrl">The base URL relative URLs are resolved against (empty or null for no resolution).</param>
        /// <returns>The sanitized URL or null if no safe URL can be created.</returns>
        private static string SanitizeUrl(string url, string baseUrl = "")
        {
            var iri = GetSafeIri(url);

            if (iri == null) return null;

            if (!iri.IsAbsolute && !string.IsNullOrEmpty(baseUrl)) {
                // resolve relative uri
                if (Uri.TryCreate(baseUrl, UriKind.Absolute, out Uri baseUri)) {
                    try {
                        return new Uri(baseUri, iri.Value).AbsoluteUri;
                    } catch (UriFormatException) {
                        return null;
                    }
                } else return null;
            }

            return EncodeUrl(iri.Value);
        }
        private static string EncodeUrl(string url)
        {
            url = url.Replace("&quot;", "");
            var hasHash = url.Contains('#');
            if (url.Contains('?') == false && hasHash == false) return url;
            string sharp = null;
            if (hasHash) {
                var index = url.LastIndexOf("#");
                if (url.Length > index) {
                    sharp = url.Substring(index + 1);
                }
                url = url.Substring(0, index);
            }

            if (url.Contains('?')) {
                var sb = new StringBuilder();
                var index = url.IndexOf('?');
                sb.Append(url.Substring(0, index + 1));
                var sp = url.Substring(index + 1).Split('&');
                foreach (var item in sp) {
                    if (string.IsNullOrEmpty(item)) continue;
                    index = item.IndexOf('=');
                    if (index == -1) {
                        sb.Append(HttpUtility.UrlEncode(item));
                        sb.Append('=');
                    } else {
                        sb.Append(HttpUtility.UrlEncode(item.Substring(0, index)));
                        sb.Append('=');
                        sb.Append(HttpUtility.UrlEncode(item.Substring(index + 1)));
                    }
                    sb.Append('&');
                }
                sb.Remove(sb.Length - 1, 1);
                url = sb.ToString();
            }

            if (hasHash) {
                return url + "#" + sharp;
            }
            return url;
        }



        private static readonly Regex SchemeRegex = new Regex(@"^\s*([^\/#]*?)(?:\:|&#0*58|&#x0*3a)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Tries to create a safe <see cref="Iri"/> object from a string.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The <see cref="Iri"/> object or null if no safe <see cref="Iri"/> can be created.</returns>
        private static Iri GetSafeIri(string url)
        {
            var schemeMatch = SchemeRegex.Match(url);

            if (schemeMatch.Success) {
                var scheme = schemeMatch.Groups[1].Value;
                return DefaultAllowedSchemes.Contains(scheme, StringComparer.OrdinalIgnoreCase) ? new Iri { Value = url, Scheme = scheme } : null;
            }

            return new Iri { Value = url };
        }
        #endregion


    }
}
