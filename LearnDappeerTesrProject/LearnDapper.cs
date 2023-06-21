using Dapper;
using LearnDappeerTesrProject;
using System.Data;
using System.Data.SqlClient;
using Z.Dapper.Plus;

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

		// Reader Example
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

		// Reader Example with table
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

		// Calling Stored Procedure without params
		public static void CallingStoredProcedure()
		{
			using (SqlConnection connection =
				   new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				string command = "showAllProduct";

				var result = connection.Query<ProductModel>(command, commandType: CommandType.StoredProcedure);

				foreach (var item in result)
				{
					Console.WriteLine($"name : {item.Name} :: id : {item.ID} :: price : {item.Price}");
				}
			}
		}

		// Calling stored procedure
		public static void CallingStoredProcedureWithParams()
		{
			using (SqlConnection connection =
				   new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				string command = "updateProductName";

				DynamicParameters parameters = new DynamicParameters();
				parameters.Add("id", 1045);
				parameters.Add("name", "testChange");

				var result = connection.ExecuteScalar(command, parameters, commandType: CommandType.StoredProcedure);

				Console.WriteLine($"Updated rows  : {result}");
			}
		}

		// Adding Data using Table valued type!
		public static void TableValuedParamsExample()
		{
			// Add this in database for this Example to run
			//create type dbo.TVP_Cate
			//as table
			//(
			//	CategoryName varchar(max)
			//)

			// After use drop this type using Below Command
			// drop type dbo.TVP_Cate

			DataTable tbToAdd = new DataTable();
			//tbToAdd.Columns.Add("CategoryId", typeof(int));
			tbToAdd.Columns.Add("CategoryName", typeof(string));

			tbToAdd.Rows.Add("Category16");
			tbToAdd.Rows.Add("Category17");
			tbToAdd.Rows.Add("Category18");
			tbToAdd.Rows.Add("Category19");

			using (SqlConnection connection =
				   new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				var tvp = tbToAdd.AsTableValuedParameter("dbo.TVP_Cate");

				var result = connection.Execute("insert into Categories select * from @TVP_Cate;",
					param: new { TVP_People = tvp });

				Console.WriteLine($"Added Rows : {result}");

			}
		}

		// Join example first try
		public static void JoinExampleUsingSplitOnOption()
		{
			string sql = "select " +
						 "p.ID, p.Name, p.Price, c.CategoryId, c.CategoryName " +
						 "from " +
						 "Product p left join Categories c " +
						 "on p.CategoryID = c.CategoryId";

			using (SqlConnection connection =
				   new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				var productsWithData = connection.Query<CategoryModel, ProductModel, CategoryModel>(sql,
					(product, category) =>
					{
						product.CategoryID = category.CategoryId;
						//category.CategoryId = product.CategoryID;
						return product;
					},
				splitOn: "CategoryID").ToList();

				foreach (var item in productsWithData)
				{
					Console.WriteLine(item.CategoryID);
				}

				Console.WriteLine("Nothing");

			}

		}

		// Join Test 2
		public static void Test2OnJoin()
		{
			string sql = "select " +
						 "p.ID, p.Name, p.Price, c.CategoryId, c.CategoryName " +
						 "from " +
						 "Product p right join Categories c " +
						 "on p.CategoryID = c.CategoryId";

			using (SqlConnection connection =
				   new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				var result = connection.Query<ProductModel, CategoryModel, FullProductModel>(
					sql,
					(pro, cat) =>
						{
							pro.CategoryId = cat.CategoryID;
							return new FullProductModel()
							{
								ID = pro.ID,
								Name = pro.Name,
								Price = pro.Price,
								CategoryId = cat.CategoryID,
								CategoryName = cat.CategoryName
							};
						},
						splitOn: "CategoryID").ToList();

				int i = 1;
				foreach (var item in result)
				{
					Console.WriteLine(i++);
					Console.WriteLine($"ProductId : {item.ID} \t productName : {item.Name} \t CategoryID : {item.CategoryId} \t CategoryName : {item.CategoryName}");

				}

			}

		}

		// With parameter passing in join
		public static void Test3OnJoin()
		{
			string sql = "select p.ID, p.Name, p.Price, c.CategoryId, c.CategoryName from Product p left join Categories c on p.CategoryID = c.CategoryId where p.CategoryID = @id";


			var parameter = new
			{
				id = 10
			};

			using (SqlConnection connection =
				   new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				var result = connection.Query<ProductModel, CategoryModel, FullProductModel>(
					sql,
					(pro, cat) =>
					{
						pro.CategoryId = cat.CategoryID;
						return new FullProductModel()
						{
							ID = pro.ID,
							Name = pro.Name,
							Price = pro.Price,
							CategoryId = cat.CategoryID,
							CategoryName = cat.CategoryName
						};
					},
					parameter,
					splitOn: "CategoryID").ToList();

				int i = 1;
				foreach (var item in result)
				{
					Console.WriteLine(i++);
					Console.WriteLine(
						$"ProductId : {item.ID} \t productName : {item.Name} \t CategoryID : {item.CategoryId} \t CategoryName : {item.CategoryName}");

				}
			}
		}

		// Output Parameter from Query!
		public static void TakingOutputParamFromQuery()
		{
			using (SqlConnection connection =
				   new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				string sql = "select * from Product where CategoryID = @catID; select @id = @@TOTAL_READ;";

				var parameters = new DynamicParameters();
				parameters.Add("catID", 10);
				parameters.Add("id", DbType.Int32, direction: ParameterDirection.Output);

				connection.Execute(sql, parameters);

				int result = parameters.Get<int>("@id");

				Console.WriteLine($"result is : {result}");
			}

		}

		// Bulk insert Example 
		public static void BulkExample()
		{

			using (SqlConnection connection =
				   new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				List<CategoryModel> categories = GiveMeDataForAddOfCategoryModel();

				connection
					//.UseBulkOptions(op => op.InsertKeepIdentity = true)
					.BulkInsert<CategoryModel>(categories);


			}

			Console.WriteLine("Inserting Complete");
		}

		public static void BulkExampleDelete()
		{

			using (SqlConnection connection =
				   new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				List<CategoryModel> categories = GiveMeDataForAddOfCategoryModel();

				connection
					//.UseBulkOptions(op => op.InsertKeepIdentity = true)
					.BulkDelete<CategoryModel>(categories);


			}

			Console.WriteLine("Delete Complete");
		}

		public static void BulkExampleUpdate()
		{

			using (SqlConnection connection =
				   new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				List<CategoryModel> categories = GiveMeDataForAddOfCategoryModel();

				connection
					//.UseBulkOptions(op => op.InsertKeepIdentity = true)
					.BulkUpdate(categories);


			}

			Console.WriteLine("Update Complete");
		}

		public static void BulkExampleMerge()
		{

			using (SqlConnection connection =
				   new("Data Source=SF-CPU-523;Initial Catalog=Product_Management;User ID=sa;Password=Abhi@15042002;"))
			{
				List<CategoryModel> categories = GiveMeDataForAddOfCategoryModel();

				DapperPlusManager.Entity<CategoryModel>().Table("Categories");


				connection
					//.UseBulkOptions(op => op.InsertKeepIdentity = true)
					.BulkMerge(categories);


			}

			Console.WriteLine("Merge Complete");
		}

		private static List<CategoryModel> GiveMeDataForAddOfCategoryModel()
		{
			var list = new List<CategoryModel>();
			var random = new Random();

			for (int i = 3800; i < 5000; i++)
			{
				list.Add(new CategoryModel() { CategoryID = i + 1, CategoryName = $"TestingMerge_{(i * 100) + (110 * 1010)}" });
			}


			return list;
		}





	}


	#region Laptop Bulk

	// for bulk insert model name and table must be matched!
	public static void TestFromLaptopForBulkInsert()
	{
		using (SqlConnection connection =
			   new("Data Source=ABHIPATEL\\SQLEXPRESS;Initial Catalog=Product_Management;Integrated security = SSPI"))
		{
			List<CategoryModel> categories = GiveMeDataForAddOfCategoryModel();

			var result = connection.BulkInsert(categories);

			foreach (var item in categories)
			{
				Console.WriteLine($"CatID : {item.CategoryName}");
			}


		}
	}

	// for update you have to pass the id which are primary key in table!😎
	public static void TestFromLaptopForBulkUpdate()
	{
		using (SqlConnection connection =
			   new("Data Source=ABHIPATEL\\SQLEXPRESS;Initial Catalog=Product_Management;Integrated security = SSPI"))
		{
			List<CategoryModel> categories = GiveMeDataForAddOfCategoryModel();

			var result = connection.BulkUpdate(categories);

			foreach (var item in categories)
			{
				Console.WriteLine($"CatID : {item.CategoryName}");
			}

		}
	}

	public static void TestFormLaptopForBulkDelete()
	{
		using (SqlConnection connection =
			   new("Data Source=ABHIPATEL\\SQLEXPRESS;Initial Catalog=Product_Management;Integrated security = SSPI"))
		{
			List<CategoryModel> categories = GiveMeDataForAddOfCategoryModel();

			var result = connection.BulkDelete(categories);

			foreach (var item in categories)
			{
				Console.WriteLine($"CatID : {item.CategoryName}");
			}

		}
	}

	public static void TestFormLaptopForBulkMerge()
	{
		using (SqlConnection connection =
			   new("Data Source=ABHIPATEL\\SQLEXPRESS;Initial Catalog=Product_Management;Integrated security = SSPI"))
		{
			List<CategoryModel> categories = GiveMeDataForAddOfCategoryModel();

			var result = connection.BulkMerge(categories);

			foreach (var item in categories)
			{
				Console.WriteLine($"CatID : {item.CategoryName}");
			}

		}
	}

	#endregion
}
}
