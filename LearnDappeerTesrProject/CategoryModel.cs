using LearnDapperTestProject;

namespace LearnDappeerTesrProject
{
	public class CategoryModel
	{
		public int CategoryID { get; set; }

		public string CategoryName { get; set; }

		public ICollection<ProductModel> Products { get; set; }


	}
}
