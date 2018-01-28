using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ToolGood.ReadyGo3.Mvc.HtmlAgilityPack
{
    partial class HtmlNode
    {
        static HtmlNode()
        {
            ElementsFlags = new Dictionary<string, HtmlElementFlag>(StringComparer.OrdinalIgnoreCase);
            List<string> list = new List<string>() {
                "DOCTYPE",
                "html","body","head","meta","title","bgsound","script","style","noembed","noscript","noframes","embed","link","base",

                "var","ruby","rp","rt","rb","rtc","applet","marquee","param","slot",

                "keygen","legend","details","isindex","data","datalist","tt","source","track",

                "quote","blockquote","q","area","cite","main","summary","xmp","wbr","dir","listing",
                "aside","figcaption","figure","section","hgroup","plaintext","time","progress","meter","output",
                "mark","dfn","kbd","samp","abbr","bdi","bdo",

                 "nav","menu","menuitem","article", "header", "footer","code" , "address",
                 "a", "div","span", "nobr", "br","hr","img","image","audio","video",

                "dialog","object","canvas","template","map","picture",
                //frameset
                "frameset", "frame","iframe",
                //font
                "p","pre",
                "b","u","em","i","s","small","strong","sub","sup","font","ins","del","big","strike","center",
                "basefont",
                //
                "ol","ul","dl","li","dd","dt",
                // header
                "h1","h2","h3","h4","h5","h6",
                //form
                "form","input","fieldset","select","option","optgroup","button","textarea","label",
                // table
                "table","thead","tbody","tfoot","th","td","tr","col","colgroup","caption",
                // MathML tag
                "math", "mi", "mo", "mn", "ms","mtext","annotation-xml",
                //SVG Tags
                "svg", "foreignObject", "desc","circle",
                // xml Tags
                "xml",
            };

            foreach (var item in list) {
                ElementsFlags[item] = HtmlElementFlag.CanOverlap | HtmlElementFlag.Empty | HtmlElementFlag.Closed;
            }
            ElementsFlags["script"] = HtmlElementFlag.CData;
            ElementsFlags["noscript"] = HtmlElementFlag.CData;
            ElementsFlags["style"] = HtmlElementFlag.CData;
            ElementsFlags["noxhtml"] = HtmlElementFlag.CData;

            ElementsFlags["a"] = HtmlElementFlag.CanOverlap;
            ElementsFlags["table"] = HtmlElementFlag.CanOverlap;
            ElementsFlags["thead"] = HtmlElementFlag.CanOverlap;
            ElementsFlags["tbody"] = HtmlElementFlag.CanOverlap;
            ElementsFlags["tfoot"] = HtmlElementFlag.CanOverlap;
            ElementsFlags["th"] = HtmlElementFlag.CanOverlap;
            ElementsFlags["td"] = HtmlElementFlag.CanOverlap;
            ElementsFlags["tr"] = HtmlElementFlag.CanOverlap;



            ElementsFlags["html"] = HtmlElementFlag.CanOverlap;
            ElementsFlags["body"] = HtmlElementFlag.CanOverlap;
            ElementsFlags["p"] = HtmlElementFlag.CanOverlap;
            ElementsFlags["div"] = HtmlElementFlag.CanOverlap;
            ElementsFlags["span"] = HtmlElementFlag.CanOverlap;
            ElementsFlags["article"] = HtmlElementFlag.CanOverlap;

        }

    }
}
