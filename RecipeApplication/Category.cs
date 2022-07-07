using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 class Category
	{
	public string CategoryName { get; set; } = "";
	public Guid Id { get; set; }
    public Category(string categoryName) 
    {
        CategoryName = categoryName;
        Id = Guid.NewGuid();
    }

}
