using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.Gadget.Internals
{
    /// <summary>
    /// 表名管理
    /// </summary>
    public class TableNameManager
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUsed = false;

        /// <summary>
        /// 默认配置
        /// </summary>
        public TableNameSetting DefaultSetting = new TableNameSetting();

        /// <summary>
        /// 配置文档
        /// </summary>
        public Dictionary<string, TableNameSetting> Settings = new Dictionary<string, TableNameSetting>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TableNameSetting this[string name] {
            get {
                return Settings[name];
            }
        }

        /// <summary>
        /// 尝试获取配置信息，若无，返回默认
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TableNameSetting TryGetSetting(string name)
        {
            TableNameSetting setting;
            if (Settings.TryGetValue(name, out setting)) {
                return setting;
            }
            return DefaultSetting;
        }


        /// <summary>
        /// 设置表名 并设置启用
        /// </summary>
        /// <param name="settingType"></param>
        /// <param name="text"></param>
        public void SetTableName(TableNameSettingType settingType, string text)
        {
            if (IsUsed == false) IsUsed = true;
            setSetting(DefaultSetting, settingType, text);
        }
        /// <summary>
        /// 设置表名 并设置启用
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="settingType"></param>
        /// <param name="text"></param>
        public void SetTableName(string settingName, TableNameSettingType settingType, string text)
        {
            if (IsUsed == false) IsUsed = true;
            if (string.IsNullOrEmpty(settingName)) {
                setSetting(DefaultSetting, settingType, text);
            } else {
                TableNameSetting setting;
                if (Settings.TryGetValue(settingName, out setting) == false) {
                    setting = new TableNameSetting();
                    Settings[settingName] = setting;
                }
                setSetting(setting, settingType, text);
            }
        }


        private void setSetting(TableNameSetting setting, TableNameSettingType settingType, string text)
        {
            switch (settingType) {
                case TableNameSettingType.DatabaseNameNullText: setting.DatabaseNameNullText = text; break;
                case TableNameSettingType.DatabaseNamePrefixText: setting.DatabaseNamePrefixText = text; break;
                case TableNameSettingType.DatabaseNameSuffixText: setting.DatabaseNameSuffixText = text; break;
                case TableNameSettingType.SchemaNameNullText: setting.SchemaNameNullText = text; break;
                case TableNameSettingType.SchemaNamePrefixText: setting.SchemaNamePrefixText = text; break;
                case TableNameSettingType.SchemaNameSuffixText: setting.SchemaNameSuffixText = text; break;
                case TableNameSettingType.TableNamePrefixText: setting.TableNamePrefixText = text; break;
                case TableNameSettingType.TableNameSuffixText: setting.TableNameSuffixText = text; break;
                default:
                    break;
            }
        }

    }
}
