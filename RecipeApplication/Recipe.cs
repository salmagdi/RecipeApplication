using System;
using Spectre.Console;

class Recipe
{
    public string Title { get; set; } = "";
    public List<string> Ingredients { get; set; } = new List<string>();
    public List<string> Instructions { get; set; } = new List<string>();
    public List<string> Categories { get; set; } = new List<string>();
    public Guid Id { get; set; }

    public Recipe(string title, List<string> ingredients, List<string> instructions, List<string> categories)
    {
        Title = title;
        Instructions = instructions;
        Categories = categories;
        Ingredients = ingredients;
        Id = Guid.NewGuid();
    }
   
}
