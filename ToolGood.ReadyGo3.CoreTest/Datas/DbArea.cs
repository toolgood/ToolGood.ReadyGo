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

        public virtual int Id { get; set; }
        public virtual int ParentId { get; set; }
        public virtual string Path { get; set; }
        public virtual int Level { get; set; }
        public virtual string Name { get; set; }
        public virtual string NameEn { get; set; }
        public virtual string NamePinyin { get; set; }
        public virtual string Code { get; set; }
        public virtual DateTime AddingTime { get; set; }

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

        public virtual int Id { get; set; }
        public virtual int ParentId { get; set; }
        public virtual string Path { get; set; }
        public virtual int Level { get; set; }
        public virtual string Name { get; set; }
        public virtual string NameEn { get; set; }
        public virtual string NamePinyin { get; set; }
        public virtual string Code { get; set; }
        public virtual DateTime? AddingTime { get; set; }

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

        public virtual int Id { get; set; }
        public virtual int ParentId { get; set; }
        public virtual string Path { get; set; }
        public virtual int Level { get; set; }
        public virtual string Name { get; set; }
        public virtual string NameEn { get; set; }
        public virtual string NamePinyin { get; set; }
        public virtual string Code { get; set; }
        public virtual DateTime AddingTime { get; set; }

    }
}