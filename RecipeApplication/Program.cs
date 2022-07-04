using System;
using System.IO;
using System.Collections.Generic;
using Spectre.Console;
using System.Text.Json;
using System.Linq;

// Getting the json file path. 
string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
string File = System.IO.Path.Combine(CurrentDirectory, @"..\..\..\Text.json");
string FilePath = Path.GetFullPath(File);

System.IO.File.WriteAllText(FilePath, "[]");

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

    // Adding a new recipe.
    if (firstChoice == "Add")
    {
        string recipeTitle = AnsiConsole.Ask<string>("1)Title: ");
        List<string> recipeIngredients = AnsiConsole.Ask<string>("2)Ingredients: [grey]seperate by - [/] ").Split("-").ToList();
        List<string> recipeInstructions = AnsiConsole.Ask<string>("3)Instructions: [grey]seperate by - [/] ").Split("-").ToList();
        List<string> recipeCategories = AnsiConsole.Ask<string>("4)Categories: [grey]seperate by - [/] ").Split("-").ToList();

        var recipe = new Recipe(recipeTitle, recipeIngredients, recipeInstructions, recipeCategories);
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = System.IO.File.ReadAllText(FilePath);
        List<Recipe> menu = System.Text.Json.JsonSerializer.Deserialize<List<Recipe>>(jsonString);
        menu.Add(recipe);
        System.IO.File.WriteAllText(FilePath, System.Text.Json.JsonSerializer.Serialize(menu, options));

        AnsiConsole.Write("Successfully added the recipe!\n");
        bool mainMenu = AnsiConsole.Confirm("Do you want to return to main menu?");
        if (!mainMenu)
        {
            break;
        }
        AnsiConsole.Clear();
    }

    // Editing a recipe.
    else if (firstChoice == "Edit")
    {
        var attributeToEdit = AnsiConsole.Prompt(
        new MultiSelectionPrompt<string>()
        .PageSize(10)
        .Title("[lightpink3]Please pick what you want to edit:[/]")
        .InstructionsText("[grey]( use up and down arrows to toggle, press space then press enter to accept)[/]")
        .AddChoices(
                new[]
                {
                                "categories","Recipes"
                }));
        string title = AnsiConsole.Ask<string>("What do you want to edit?");
        
        string? secondChoice = attributeToEdit.Count == 1 ? attributeToEdit[0] : null;
        string? thirdChoice = attributeToEdit.Count == 1 ? attributeToEdit[0] : null;
        string jsonString = System.IO.File.ReadAllText(FilePath);
        List<Recipe> menu = System.Text.Json.JsonSerializer.Deserialize<List<Recipe>>(jsonString);

        // Checking which element the user would like to edit. 
        if (secondChoice == "categories")
        {
            List<string> newCategory = AnsiConsole.Ask<string>("Enter new categories: [gray]seperate by - [/]?").Split("-").ToList();
            menu.Find(r => r.Title == title).Categories = newCategory;
        }
        else if (secondChoice=="Recipes")
        {
                string secondTitle = AnsiConsole.Ask<string>("Enter the title of the recipe to edit:");
                var secondAttributeToEdit = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                .PageSize(10)
                .Title("[lightpink3]Please pick what you want to edit:[/]")
                .InstructionsText("[grey]( use up and down arrows to toggle, press space then press enter to accept)[/]")
                .AddChoices(
                    new[]
                    {
                                "instructions","ingredients","title"
                    }));
            if (thirdChoice == "instructions")
            {
                List<string> newInstructions = AnsiConsole.Ask<string>("Enter new instructions: [gray]seperate by - .[/]?").Split("-").ToList();
                menu.Find(r => r.Title == title).Instructions = newInstructions;
            }
            else if (thirdChoice == "ingredients")
            {
                List<string> newIngredients = AnsiConsole.Ask<string>("Enter new ingredients: [green]seperate by - .[/]?").Split("-").ToList();
                menu.Find(r => r.Title == title).Ingredients = newIngredients;
            }
            else
            {
                string newTitle = AnsiConsole.Ask<string>("Enter new title:");
                menu.Find(r => r.Title == title).Title = newTitle;
            }
        }
        

        // Updating the json file.
        var options = new JsonSerializerOptions { WriteIndented = true };
        System.IO.File.WriteAllText(FilePath, System.Text.Json.JsonSerializer.Serialize(menu));
        AnsiConsole.Write("Successfully edited the recipe!\n");
        bool mainMenu = AnsiConsole.Confirm("Do you want to return to main menu?");
        if (!mainMenu)
        {
            break;
        }
        AnsiConsole.Clear();

    }
    // Listing a recipe.
    else if (firstChoice == "List")
    {
        string jsonString = System.IO.File.ReadAllText(FilePath);
        List<Recipe> menu = System.Text.Json.JsonSerializer.Deserialize<List<Recipe>>(jsonString);
        string listTitle = AnsiConsole.Ask<string>("Enter the title of the recipe to list:");
        Recipe foundRecipe = menu.Find(r => r.Title == listTitle);

        AnsiConsole.MarkupLine("[green]Ingredients: [/]");
        for (int i = 0; i < foundRecipe.Ingredients.Count; i++)
            AnsiConsole.Write(foundRecipe.Ingredients[i] + " ");

        AnsiConsole.Write("\n");
        AnsiConsole.MarkupLine("[green]Instructions: [/]");
        for (int i = 0; i < foundRecipe.Instructions.Count; i++)
            AnsiConsole.Write(foundRecipe.Instructions[i]);

        AnsiConsole.Write("\n");
        AnsiConsole.MarkupLine("[green]Categories: [/]");
        for (int i = 0; i < foundRecipe.Categories.Count; i++)
            AnsiConsole.Write(foundRecipe.Categories[i]);

        bool mainMenu = AnsiConsole.Confirm("Do you want to return to main menu?");
        if (!mainMenu)
        {
            break;
        }
        AnsiConsole.Clear();
    }
    // Exiting.
    else
    {
        break;
    }
}