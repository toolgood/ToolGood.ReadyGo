using DuckDB.NET.Data;
using PetaTest;

namespace ToolGood.ReadyGo3.Test
{
	internal class Program
	{
		static void Main(string[] args)
		{
			using(var helper = SqlHelperFactory.OpenDuckDbFile("13.db")) {

				var dbs = helper.Select<testArray2>(@"SELECT 
    id,
    name,
    embedding,
    array_distance(embedding, [0.0, 1.0, 0.0]::FLOAT[3]) AS distance
FROM testArray
ORDER BY distance
LIMIT 3;");

			}

			using(var helper = SqlHelperFactory.OpenDuckDbFile("17.db")) {
				helper.Execute("INSTALL vss;LOAD vss;");
				helper.Execute("SET hnsw_enable_experimental_persistence = true;");

				helper._TableHelper.CreateTable(typeof(testArray));
				//helper.Execute("CREATE TABLE testArray ( id INTEGER, name VARCHAR, embedding FLOAT[3] );");
				helper.Execute(@"INSERT INTO testArray VALUES
	(1, '红色苹果', [1.0, 0.1, 0.2]::FLOAT[3]),
    (2, '青色苹果', [0.9, 0.15, 0.25]::FLOAT[3]),
    (3, '橙色橙子', [0.2, 0.8, 0.1]::FLOAT[3]),
    (4, '黄色香蕉', [0.3, 0.7, 0.05]::FLOAT[3]),
    (5, '紫色葡萄', [0.6, 0.2, 0.7]::FLOAT[3]),
    (6, '红色草莓', [0.95, 0.08, 0.18]::FLOAT[3]),
    (7, '绿色西瓜', [0.4, 0.3, 0.5]::FLOAT[3]),
    (8, '蓝色蓝莓', [0.5, 0.25, 0.6]::FLOAT[3]),
    (9, '黄色柠檬', [0.35, 0.75, 0.08]::FLOAT[3]),
    (10, '粉色火龙果', [0.55, 0.45, 0.4]::FLOAT[3]);");
				//helper.Insert(new testArray { id = 11, name = "测试", embedding = new float[] { 0.1f, 0.2f, 0.3f } });

				helper.Execute("CREATE INDEX idx_items_cosine ON testArray USING HNSW(embedding) WITH(metric = 'cosine');");

				var dbs = helper.Select<testArray2>(@"SELECT 
    id,
    name,
    embedding,
    array_distance(embedding, [1.0, 0.0, 0.0]::FLOAT[3]) AS distance
FROM testArray
ORDER BY distance
LIMIT 3;");

			}


		}
	}
	public class testArray
	{
		public int id { get; set; }
		public string name { get; set; }
		public float[] embedding { get; set; }
		//public double[] doubleArray { get; set; }
	}
	public class testArray2
	{
		public int id { get; set; }
		public string name { get; set; }

		public List<float> embedding { get; set; }
		public float distance { get; set; }
		//public double[] doubleArray { get; set; }
	}
}