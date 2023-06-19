using System.Data;
using Dapper;
using LearnDappeerTesrProject;
using System.Data.SqlClient;

namespace LearnDapperTestProject
{
	public static class LearnDapper
	{

		// used in mapping with model
		public static void MappingToModel()
		{
			using (SqlConnection connection = new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				// query will convert to model which are passed in generic format!
				var result = connection.Query<ProductModel>("select * from Product");

				foreach (var item in result)
				{
					Console.WriteLine($"name : {item.Name} :: id : {item.ID} :: price : {item.Price}");
				}
			}
		}

		// use for single value to be return
		public static void ExecuteScalarExample()
		{
			using (SqlConnection connection = new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				// normal returns object 
				//var result = connection.ExecuteScalar("select count(*) from Product");

				// return a int because used a generics
				var result = connection.ExecuteScalar<int>("select count(*) from Product");

				Console.WriteLine($"Total Product : {result}");
			}
		}

		// used when only one expected!
		public static void QuerySingleExample()
		{
			using (SqlConnection connection = new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				// explicit type cast
				//var result = connection.QuerySingle<ProductModel>("select * from Product where ID = 1003");

				// dynamic return type
				//var result = connection.QuerySingle("select * from Product where ID = 1003");

				// nothing is fetched because no data is related to 1001 id
				//var result = connection.QuerySingleOrDefault<ProductModel>("select * from Product where ID = 1001");

				// query first return the first element when multiple is fetched
				//var result = connection.QueryFirst<ProductModel>("select * from Product");

				// query will not return a null when nothing is there in table
				var result = connection.QueryFirstOrDefault<ProductModel>("select * from Product where ID = 1001");

				if (result != null)
				{
					Console.WriteLine($"name : {result.Name} :: id : {result.ID} :: price : {result.Price}");
				}
				else
				{
					Console.WriteLine("nothing is fetched!");
				}
			}
		}

		// Multiple Tables read at one time!
		public static void QueryMultipleExample()
		{
			using (SqlConnection connection = new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				string command = "select * from Product; select * from Categories;";

				var result = connection.QueryMultiple(command);


				var test = result.Read<ProductModel>();
				var test1 = result.Read<CategoryModel>();

				foreach (var item in test)
				{
					Console.WriteLine($"name : {item.Name} :: id : {item.ID} :: price : {item.Price}");
				}

				foreach (var item in test1)
				{
					Console.WriteLine($"id : {item.CategoryID} :: category : {item.CategoryName}");
				}


				//Console.WriteLine("-----------------------");

				//ProductModel model = result.ReadFirst<ProductModel>();

				//Console.WriteLine(model.ID); Console.WriteLine(model.Name);
			}
		}

		// Insert using Dapper .Execute() method
		public static void InsertData()
		{
			string command = "insert into Product values (@name,@price,@catID)";
			object[] param = { new { name = "product", price = 10001, catID = 2 } };

			using (SqlConnection connection = new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				var result = connection.Execute(command, param);

				Console.WriteLine(result);
			}
		}

		// Multiple insert because the data cleared unintentionally
		public static void InsertProductData()
		{
			using (SqlConnection connection = new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				for (int i = 1; i <= 10; i++)
				{
					string command = "insert into Product values (@name,@price,@catID)";
					object[] param = { new { name = $"product{i}", price = 10001 + (i * 10), catID = i } };

					var result = connection.Execute(command, param);

					if (result == 1)
					{
						Console.WriteLine("Added!");
					}
					else
					{
						Console.WriteLine("not added");
					}
				}
			}
		}

		// Update data using dapper 
		public static void UpdateData()
		{
			string command = "update Product set Name = @name where ID = @id";
			object[] param = { new { name = "Changed!", id = 1003 } };

			using (SqlConnection connection = new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				var result = connection.Execute(command, param);

				Console.WriteLine(result);
			}
		}

		// Delete data using dapper
		public static void DeleteData()
		{
			string command = "delete from Product where ID = @id";
			object[] param = { new { id = 1026 } };

			using (SqlConnection connection = new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				var result = connection.Execute(command, param);

				Console.WriteLine(result);
			}
		}

		public static void ExecuteReaderExample()
		{
			using (SqlConnection connection =
			       new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				string command = "select * from Product";

				var reader = connection.ExecuteReader(command);

				while (reader.Read())
				{
					Console.WriteLine($"id : {reader.GetInt32(0)} :: name : {reader.GetString(1)}");
				}

			}
		}

		public static void ExecuteReaderExampleUsingDataTable()
		{
			using (SqlConnection connection =
			       new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				string command = "select * from Product";

				var reader = connection.ExecuteReader(command);

				DataTable table = new DataTable();

				table.Load(reader);

				foreach (DataRow dataRow in table.Rows)
				{
					Console.WriteLine($"id : {dataRow[0]} :: name : {dataRow[1]}");
				}
			}
		}

		public static void CallingStoredProcedure()
		{
			using (SqlConnection connection =
			       new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				string command = "showAllProduct";

				var result = connection.Query<ProductModel>(command,commandType: CommandType.StoredProcedure);

				foreach (var item in result)
				{
					Console.WriteLine($"name : {item.Name} :: id : {item.ID} :: price : {item.Price}");
				}
			}
		}
	}
}
