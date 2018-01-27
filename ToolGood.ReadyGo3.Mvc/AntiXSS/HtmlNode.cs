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
                "html","body","head","meta","title","bgsound","script",
                "style","noembed","noscript","noframes","menu","menuitem",
                "var","ruby","sub","sup","rp","rt","rb","rtc","applet","embed",
                "marquee","param","object","canvas","font","ins","del","template",
                "slot","caption","col","colgroup",
                "table","thead","tbody","tfoot","th","td","tr",
                "input","keygen","textarea","p","span","dialog",
                "fieldset","legend","label","details","form","isindex",
                "pre","data","datalist","ol","ul","dl","li","dd","dt",
                "b","big","strike","code","em","i","s","small","strong",
                "u","tt","nobr","select","option","optgroup","link","frameset",
                "frame","iframe","audio","video","source","track",
                "h1","h2","h3","h4","h5","h6","div",
                "quote","blockquote","q","base","basefont",
                "a","area","button","cite","main","summary",
                "xmp","br","wbr","hr","dir","center","listing",
                "img","image","nav","address","article","aside","figcaption","figure","section",
                "footer","header","hgroup","plaintext","time","progress","meter","output","map","picture",
                "mark","dfn","kbd","samp","abbr","bdi","bdo",
            };
            foreach (var item in list) {
                ElementsFlags[item] = HtmlElementFlag.CanOverlap | HtmlElementFlag.Empty | HtmlElementFlag.Closed;
            }
            ElementsFlags["script"] = HtmlElementFlag.CData;
            ElementsFlags["style"] = HtmlElementFlag.CData;
            ElementsFlags["noxhtml"] = HtmlElementFlag.CData;

            ElementsFlags["a"] = HtmlElementFlag.CanOverlap ;
            ElementsFlags["table"] = HtmlElementFlag.CanOverlap ;
            ElementsFlags["thead"] = HtmlElementFlag.CanOverlap ;
            ElementsFlags["tbody"] = HtmlElementFlag.CanOverlap ;
            ElementsFlags["tfoot"] = HtmlElementFlag.CanOverlap ;
            ElementsFlags["th"] = HtmlElementFlag.CanOverlap ;
            ElementsFlags["td"] = HtmlElementFlag.CanOverlap ;
            ElementsFlags["tr"] = HtmlElementFlag.CanOverlap ;

            

            ElementsFlags["html"] = HtmlElementFlag.CanOverlap ;
            ElementsFlags["body"] = HtmlElementFlag.CanOverlap ;
            ElementsFlags["p"] = HtmlElementFlag.CanOverlap ;
            ElementsFlags["div"] = HtmlElementFlag.CanOverlap;
            ElementsFlags["span"] = HtmlElementFlag.CanOverlap;
            ElementsFlags["article"] = HtmlElementFlag.CanOverlap;

        }
 
    }
}
