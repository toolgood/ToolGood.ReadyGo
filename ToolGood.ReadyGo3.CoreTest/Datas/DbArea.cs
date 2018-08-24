using System;
using ToolGood.ReadyGo3.Attributes;


namespace ToolGood.ReadyGo3.Test.Datas
{
    /// <summary>
    /// </summary>
    [Table("Area")]
    [Serializable]
    public class DbArea
    {
        public DbArea() { }

        public DbArea(int id, int parentId, string path, int level, string name, string en, string pinyin, string code)
        {
            Id = id;
            ParentId = parentId;
            Path = path;
            Level = level;
            Name = name;
            NameEn = en;
            NamePinyin = pinyin;
            Code = code;
            AddingTime = DateTime.Now;
        }

        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Path { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string NamePinyin { get; set; }
        public string Code { get; set; }
        public DateTime AddingTime { get; set; }

    }
  

    [Table("Area")]
    [Serializable]
    public class DbArea2
    {
        public DbArea2() { }

        public DbArea2(int id, int parentId, string path, int level, string name, string en, string pinyin, string code)
        {
            Id = id;
            ParentId = parentId;
            Path = path;
            Level = level;
            Name = name;
            NameEn = en;
            NamePinyin = pinyin;
            Code = code;
            AddingTime = DateTime.Now;
        }

        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Path { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string NamePinyin { get; set; }
        public string Code { get; set; }
        public DateTime? AddingTime { get; set; }

    }


    /// <summary>
    /// </summary>
    [Table("Area3")]
    [Serializable]
    public class DbArea3
    {
        public DbArea3() { }

        public DbArea3(int id, int parentId, string path, int level, string name, string en, string pinyin, string code)
        {
            Id = id;
            ParentId = parentId;
            Path = path;
            Level = level;
            Name = name;
            NameEn = en;
            NamePinyin = pinyin;
            Code = code;
            AddingTime = DateTime.Now;
        }

        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Path { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string NamePinyin { get; set; }
        public string Code { get; set; }
        public DateTime AddingTime { get; set; }

    }
}