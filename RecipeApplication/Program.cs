using System;
using System.IO;
using System.Collections.Generic;
using Spectre.Console;
using System.Text.Json;
using System.Linq;

// Getting the json file path. 
string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
string file = System.IO.Path.Combine(currentDirectory, @"..\..\..\Text.json");
string filePath = Path.GetFullPath(file);

System.IO.File.WriteAllText(filePath, "[]");


while (true)
{
    // The title using FigletText.
    AnsiConsole.Write(
                new FigletText("Recipes app")
                    .Centered()
                    .Color(Color.DarkTurquoise));

    // A prompt to pick the functionality.
    List<string> operations = AnsiConsole.Prompt(
    new MultiSelectionPrompt<string>()
    .PageSize(10)
    .Title("[lightpink3]What do you want to do?[/]")
    .InstructionsText("[grey]use up and down arrows to toggle, press space then enter to accept[/]")
    .AddChoices(
        new[]{
                        "Add","Edit","List","Exit"
        }));

    string? firstChoice = operations.Count == 1 ? operations[0] : null;

    // Add new recipe.
    if (firstChoice == "Add")
    {
        var addChoice = AnsiConsole.Prompt(
        new MultiSelectionPrompt<string>()
        .PageSize(10)
        .Title("[lightpink3]Please pick what you want to edit:[/]")
        .InstructionsText("[grey]( use up and down arrows to toggle, press space then press enter to accept)[/]")
        .AddChoices(
                new[]
                {
                                "Categories","Recipes"
                }));

        string? secondChoice = addChoice.Count == 1 ? addChoice[0] : null;
        string jsonString = System.IO.File.ReadAllText(filePath);
        List<Recipe> menu = JsonSerializer.Deserialize<List<Recipe>>(jsonString);
        List<Category> categoriesMenu = JsonSerializer.Deserialize<List<Category>>(jsonString);
        if (secondChoice == "Recipes")
        {
            string recipeTitle = AnsiConsole.Ask<string>("1)Title: ");
            List<string> recipeIngredients = AnsiConsole.Ask<string>("2)Ingredients: [grey]seperate by - [/] ").Split("-").ToList();
            List<string> recipeInstructions = AnsiConsole.Ask<string>("3)Instructions: [grey]seperate by - [/] ").Split("-").ToList();
            List<string> recipeCategories = AnsiConsole.Ask<string>("4)Categories: [grey]seperate by - [/] ").Split("-").ToList();

            var recipe = new Recipe(recipeTitle, recipeIngredients, recipeInstructions, recipeCategories);
            var options = new JsonSerializerOptions { WriteIndented = true };
            jsonString = System.IO.File.ReadAllText(filePath);
            menu = JsonSerializer.Deserialize<List<Recipe>>(jsonString);
            if (menu != null)
            {
                menu.Add(recipe);
                System.IO.File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(menu, options));
            }

            AnsiConsole.Write("Successfully added the recipe!\n");
            bool mainMenu = AnsiConsole.Confirm("Do you want to return to main menu?");
            if (!mainMenu)
            {
                break;
            }
            AnsiConsole.Clear();
        }

        // Add new category.
        else if (secondChoice == "Categories")
        {
            string categoryName = AnsiConsole.Ask<string>("Category name: ");
            var category = new Category(categoryName);
            var options = new JsonSerializerOptions { WriteIndented = true };
            jsonString = System.IO.File.ReadAllText(filePath);
            menu = JsonSerializer.Deserialize<List<Recipe>>(jsonString);
            if (categoriesMenu != null)
            {
                categoriesMenu.Add(category);
                System.IO.File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(menu, options));
            }
            AnsiConsole.Write("Successfully added the category!)\n");
            bool mainMenu = AnsiConsole.Confirm("Do you want to return to main menu?");
            if (!mainMenu)
            {
                break;
            }
            AnsiConsole.Clear();
        }
    }

    // Editing a recipe.
    else if (firstChoice == "Edit")
    {
        var editChoice = AnsiConsole.Prompt(
        new MultiSelectionPrompt<string>()
        .PageSize(10)
        .Title("[lightpink3]Please pick what you want to edit:[/]")
        .InstructionsText("[grey]( use up and down arrows to toggle, press space then press enter to accept)[/]")
        .AddChoices(
                new[]
                {
                                "categories","Recipes"
                }));

        string? secondChoice = editChoice.Count == 1 ? editChoice[0] : null;
        string jsonString = System.IO.File.ReadAllText(filePath);
        List<Recipe> menu = JsonSerializer.Deserialize<List<Recipe>>(jsonString);

        if (secondChoice == "categories")
        {
            jsonString = System.IO.File.ReadAllText(filePath);
            menu = System.Text.Json.JsonSerializer.Deserialize<List<Recipe>>(jsonString);
            string category = AnsiConsole.Ask<string>("Enter the name of the category to edit:");
            string newCategory = AnsiConsole.Ask<string>("Enter the category's new name:");
            List<Recipe> beforeRename = menu.FindAll(r => r.Categories.Contains(category));
            if (beforeRename.Any())
            {
                foreach (Recipe r in beforeRename)
                {
                    r.Categories.Remove(category);
                    r.Categories.Add(newCategory);
                }
                // Updating the json file.
                var choices = new JsonSerializerOptions { WriteIndented = true };
                System.IO.File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(menu));
                AnsiConsole.Write("Successfully removed the category!\n");
            }
            else
            { 
                AnsiConsole.Write("This category was not found\n");
            }
            bool mainMenu2 = AnsiConsole.Confirm("Do you want to return to main menu?");
            if (!mainMenu2)
            {
                break;
            }
            AnsiConsole.Clear();
        }

        else if (secondChoice == "Recipes")
        {
            var table = new Table().Border(TableBorder.Ascii2);
            table.Expand();
            table.AddColumn("[yellow]Title[/]");
            table.AddColumn(new TableColumn("[yellow]Ingredients[/]").LeftAligned());
            table.AddColumn(new TableColumn("[yellow]Instructions[/]").LeftAligned());
            table.AddColumn(new TableColumn("[yellow]Categories[/]").LeftAligned());

            for (int i = 0; i < menu.Count; i++)
            {
                table.AddRow(
                     String.Join("\n", menu[i].Title),
                     String.Join("\n", menu[i].Ingredients.Select(x => $"- {x}")),
                     String.Join("\n", menu[i].Instructions.Select((x, n) => $"- {x}")),
                     String.Join("\n", menu[i].Categories.Select((x) => $"- {x}")));
                table.AddEmptyRow();
            }
            AnsiConsole.Write(table);

            int index = AnsiConsole.Ask<int>("Enter the number of the recipe to edit:");
            Recipe recipeEdit = menu[index - 1];
            var recipeChoice = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
            .PageSize(10)
            .Title("[lightpink3]Please pick what do you want to edit:[/]")
            .InstructionsText("[grey]( use up and down arrows to toggle, press space then press enter to accept)[/]")
            .AddChoices(
                new[]
                {
                                "instructions","ingredients","title","category"
                }));
            string? thirdChoice = recipeChoice.Count == 1 ? recipeChoice[0] : null;
            jsonString = System.IO.File.ReadAllText(filePath);
            menu = JsonSerializer.Deserialize<List<Recipe>>(jsonString);
            if (thirdChoice == "instructions")
            {
                List<string> newInstructions = AnsiConsole.Ask<string>("Enter new instructions: [gray]seperate by - .[/]?").Split("-").ToList();
                menu.Find(r => r.Id == recipeEdit.Id).Instructions = newInstructions;
            }
            else if (thirdChoice == "ingredients")
            {
                List<string> newIngredients = AnsiConsole.Ask<string>("Enter new ingredients: [gray]seperate by - .[/]?").Split("-").ToList();
                menu.Find(r => r.Id == recipeEdit.Id).Ingredients = newIngredients;
            }
            else if (thirdChoice == "title")
            {
                string newTitle = AnsiConsole.Ask<string>("Enter new title:");
                menu.Find(r => r.Id == recipeEdit.Id).Title = newTitle;
            }
            else if (thirdChoice == "category")
            {
                List<string> newCategory = AnsiConsole.Ask<string>("Enter new categories: [gray]seperate by - [/]?").Split("-").ToList();
                menu.Find(r => r.Id == recipeEdit.Id).Categories = newCategory;

            }

        }

        // Updating the json file.
        var options = new JsonSerializerOptions { WriteIndented = true };
        System.IO.File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(menu));
        AnsiConsole.Write("Successfully edited the recipe!\n");
        bool mainMenu = AnsiConsole.Confirm("Do you want to return to main menu?");
        if (!mainMenu)
        {
            break;
        }
        AnsiConsole.Clear();

    }
    // List a recipe
    else if (firstChoice == "List")
    {
        string jsonString = System.IO.File.ReadAllText(filePath);
        List<Recipe> menu = JsonSerializer.Deserialize<List<Recipe>>(jsonString);
        string listTitle = AnsiConsole.Ask<string>("Enter the name of the recipe to list:");
        List<Recipe> foundRecipe = menu.FindAll(r => r.Title == listTitle);

        // Recipe attributes displayed in a table.
        var table = new Table().Border(TableBorder.Ascii2);
        table.Expand();
        table.AddColumn("[yellow]Title[/]");
        table.AddColumn(new TableColumn("[yellow]Ingredients[/]").LeftAligned());
        table.AddColumn(new TableColumn("[yellow]Instructions[/]").LeftAligned());
        table.AddColumn(new TableColumn("[yellow]Categories[/]").LeftAligned());
        for (int i = 0; i < foundRecipe.Count; i++)
        {
            table.AddRow(
                 String.Join("\n", foundRecipe[i].Title),
                 String.Join("\n", foundRecipe[i].Ingredients.Select(x => $"- {x}")),
                 String.Join("\n", foundRecipe[i].Instructions.Select((x, n) => $"- {x}")),
                 String.Join("\n", foundRecipe[i].Categories.Select((x) => $"- {x}")));
            table.AddEmptyRow();
        }
        AnsiConsole.Write(table);
        AnsiConsole.Write("\n");
        bool mainMenu = AnsiConsole.Confirm("Do you want to return to main menu?");
        if (!mainMenu)
        {
            break;
        }
        AnsiConsole.Clear();
    }
}