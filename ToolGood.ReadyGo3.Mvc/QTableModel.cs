using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Text.RegularExpressions;

using ToolGood.ReadyGo3.Mvc.AntiXSS;



namespace ToolGood.ReadyGo3.DataCentxt

{

    /// <summary>
    /// 适用于MVC 中间值的转值类
    /// 前端传值 =》 转成MVC Tb类（基类：QTableModel） =》 通过GetChange方法 传值给 QTable =》 转成SQL语句 =》 保存到数据库
    /// </summary>

    public abstract class QTableModel

    {

        private Dictionary<string, object> _dictionary = new Dictionary<string, object>();



        protected void SetValue(string txt, object value) { _dictionary[txt] = value; }

        protected T GetValue<T>(string txt) { return _dictionary.ContainsKey(txt) ? (T)_dictionary[txt] : default(T); }

        protected void SetString(string txt, string html)

        {

            if (string.IsNullOrEmpty(html)) return;

            html = Regex.Replace(html, @"(<![^>]*>)", "", RegexOptions.IgnoreCase);

            html = Regex.Replace(html, @"<style[^>]*?>[\s\S]*?</style>", "", RegexOptions.IgnoreCase);

            html = Regex.Replace(html, @"<script([^>])*>(\w|\W)*?</script([^>])*>", "", RegexOptions.IgnoreCase);

            html = Regex.Replace(html, "</?[a-zA-Z][^>]*>", "");

            html = Regex.Replace(html, @"\s+", " ");



            _dictionary[txt] = html;

        }

        protected void SetHtml(string txt, string html)

        {

            if (string.IsNullOrEmpty(html)) return;

            _dictionary[txt] = HtmlSanitizer.Sanitize(html);

        }

        public Dictionary<string, object> GetChange() { return _dictionary; }

 

    }

}
