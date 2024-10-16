﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;

namespace Recipe_Book
{
   class Program
   {
      private static List<Recipe> recipes = new List<Recipe>();
      static void Main(string[] args)
      {
         Console.SetIn(new StreamReader(Console.OpenStandardInput(8192)));
         LoadRecipes(ref recipes);
         Console.WindowWidth = 204;
         //Console.WriteLine(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Recipes.txt"));
         Console.WriteLine("Welcome to the Recipe Book v0.0.4! Please type either \"Read\" or \"Write\" to read your recipes, or write them respectively. Type \"Print\" to bring up the print interface. Type \"Exit\" at any time to quit.");
      chooseReadOrWriteRecipeStart:
         switch (Console.ReadLine().ToLower())
         {
            case "read":
               Console.WriteLine("Type \"List\" to get a list of your recipe titles in alphabetical order, \"Search\" to search for a recipe and read it, or \"Back\" to go back at any time.");
            startOfReadRecipe:
               switch (Console.ReadLine().ToLower().Trim(' '))
               {
                  case "list":
                     if (recipes.Count == 0)
                     {
                        Console.WriteLine("No entries.");
                        goto startOfReadRecipe;
                     }
                     string[] ss = new string[recipes.Count];
                     for (int i = 0; i < ss.Length; i++)
                     {
                        ss[i] = recipes[i].recipeName;
                     }
                     ss.ToList().OrderBy(s => s).ToArray();
                     Console.WriteLine(string.Join(Environment.NewLine, ss));
                     Console.WriteLine("Type \"List\" to get a list of your recipe titles in alphabetical order, \"Search\" to search for a recipe and read it, or \"Back\" to go back at any time.");
                     goto startOfReadRecipe;
                  case "search":
                     Console.WriteLine("Type the name of the recipe exactally as it is spelled.");
                     string key = Console.ReadLine().ToLower();
                     Recipe? temp = null;
                     foreach (Recipe thing in recipes)
                     {
                        if (thing.recipeName.ToLower() == key)
                        {
                           temp = thing;
                           break;
                        }
                     }
                     if (temp != null)
                     {
                        Console.WriteLine("Recipe found, it serves " + ((Recipe)temp).serveSize + " people by default. Type an integer multiplier to multiply the ingredients to serve more people.");
                     askMultForRead:
                        if (!int.TryParse(Console.ReadLine(), out int res))
                        {
                           Console.WriteLine("Error: response not recognized. Type an integer value.");
                           goto askMultForRead;
                        }
                        Console.WriteLine(DisplayRecipe((Recipe)temp, res));
                        Console.WriteLine("Type \"List\" to get a list of your recipe titles in alphabetical order, \"Search\" to search for a recipe and read it, or \"Back\" to go back at any time.");
                        goto startOfReadRecipe;
                     }
                     else
                     {
                        Console.WriteLine("Recipe not found. Type \"List\" to get a list of your recipe titles in alphabetical order, \"Search\" to search for a recipe and read it, or \"Back\" to go back at any time.");
                        goto startOfReadRecipe;
                     }
                  case "back":
                     Console.WriteLine("Please type either \"Read\" or \"Write\" to read your recipes, or write them respectively. Type \"Print\" to bring up the print interface. Type \"Exit\" at any time to quit.");
                     goto chooseReadOrWriteRecipeStart;
                  default:
                     Console.WriteLine("Error: response not recognized. Please type either \"Search\", \"Back\", or \"List\".");
                     goto startOfReadRecipe;
               }
            case "write":
               Recipe r = new Recipe();
               Console.WriteLine("Create a new unique name for your recipe:");
               {
                  string s = Console.ReadLine();
                  s = new Regex("[^a-zA-Z0-9 -]").Replace(s, "").Trim(' ').Replace(' ', '_');
                  r.recipeName = s;
               }
               Console.WriteLine("The name for your new recipe is \"" + r.recipeName + "\". Now type in an integer equal to the number of ingredient sets.");
               {
               askingIngredientCount:
                  if (!int.TryParse(Console.ReadLine().Trim(' '), out int i))
                  {
                     Console.WriteLine("Error: response not recognized. Please type an integer with no other characters.");
                     goto askingIngredientCount;
                  }
                  r.ingredientTitles = new string[i];
                  r.ingredientAmounts = new float[i];
                  r.ingredientUnits = new string[i];
               }
               for (int i = 1; i < r.ingredientTitles.Length + 1; i++)
               {
                  switch (i)
                  {
                     case 1:
                        {
                           Console.WriteLine("Type the name of your 1st ingredient:");
                           {
                              string s = Console.ReadLine();
                              s = new Regex("[^a-zA-Z0-9 -]").Replace(s, "").Trim(' ');
                              r.ingredientTitles[i - 1] = s;
                           }
                           Console.WriteLine("Type the unit your 1st ingredient is measured in:");
                           r.ingredientUnits[i - 1] = Console.ReadLine();
                           Console.WriteLine("Type the amount of the 1st ingredient there is. (in the unit specified above):");
                        askingIngredientAmount:
                           if (!float.TryParse(Console.ReadLine().Trim(' '), out float f))
                           {
                              Console.WriteLine("Error: response not recognized. Please type a number with no other characters.");
                              goto askingIngredientAmount;
                           }
                           r.ingredientAmounts[i - 1] = f;
                           if ((i) >= r.ingredientTitles.Length)
                           {
                              break;
                           }
                           Console.WriteLine("Moving on to the 2nd ingredient.");
                        }
                        break;
                     case 2:
                        {
                           Console.WriteLine("Type the name of your 2nd ingredient:");
                           {
                              string s = Console.ReadLine();
                              s = new Regex("[^a-zA-Z0-9 -]").Replace(s, "").Trim(' ');
                              r.ingredientTitles[i - 1] = s;
                           }
                           Console.WriteLine("Type the unit your 2nd ingredient is measured in:");
                           r.ingredientUnits[i - 1] = Console.ReadLine();
                           Console.WriteLine("Type the amount of the 2nd ingredient there is. (in the unit specified above):");
                        askingIngredientAmount:
                           if (!float.TryParse(Console.ReadLine().Trim(' '), out float f))
                           {
                              Console.WriteLine("Error: response not recognized. Please type a number with no other characters.");
                              goto askingIngredientAmount;
                           }
                           r.ingredientAmounts[i - 1] = f;
                           if ((i) >= r.ingredientTitles.Length)
                           {
                              break;
                           }
                           Console.WriteLine("Moving on to the 3rd ingredient.");
                        }
                        break;
                     case 3:
                        {
                           Console.WriteLine("Type the name of your 3rd ingredient:");
                           {
                              string s = Console.ReadLine();
                              s = new Regex("[^a-zA-Z0-9 -]").Replace(s, "").Trim(' ');
                              r.ingredientTitles[i - 1] = s;
                           }
                           Console.WriteLine("Type the unit your 3rd ingredient is measured in:");
                           r.ingredientUnits[i - 1] = Console.ReadLine();
                           Console.WriteLine("Type the amount of the 3rd ingredient there is. (in the unit specified above):");
                        askingIngredientAmount:
                           if (!float.TryParse(Console.ReadLine().Trim(' '), out float f))
                           {
                              Console.WriteLine("Error: response not recognized. Please type a number with no other characters.");
                              goto askingIngredientAmount;
                           }
                           r.ingredientAmounts[i - 1] = f;
                           if ((i) >= r.ingredientTitles.Length)
                           {
                              break;
                           }
                           Console.WriteLine("Moving on to the 4th ingredient.");
                        }
                        break;
                     default:
                        {
                           Console.WriteLine("Type the name of your " + i.ToString() + "th ingredient:");
                           {
                              string s = Console.ReadLine();
                              s = new Regex("[^a-zA-Z0-9 -]").Replace(s, "").Trim(' ');
                              r.ingredientTitles[i - 1] = s;
                           }
                           Console.WriteLine("Type the unit your " + i.ToString() + "th ingredient is measured in:");
                           r.ingredientUnits[i - 1] = Console.ReadLine();
                           Console.WriteLine("Type the amount of the " + i.ToString() + "th ingredient there is. (in the unit specified above):");
                        askingIngredientAmount:
                           if (!float.TryParse(Console.ReadLine().Trim(' '), out float f))
                           {
                              Console.WriteLine("Error: response not recognized. Please type a number with no other characters.");
                              goto askingIngredientAmount;
                           }
                           r.ingredientAmounts[i - 1] = f;
                           if ((i) >= r.ingredientTitles.Length)
                           {
                              break;
                           }
                           Console.WriteLine("Moving on to the " + (i + 1).ToString() + "th ingredient.");
                        }
                        break;
                  }
               }
               Console.WriteLine("Please type the instructions to accompany your recipe:");
               r.instructions = Regex.Escape(Console.ReadLine().Trim(' '));
               Console.WriteLine("Please type the amount of people this recipe serves.");
            askRecipeServSize:
               if (!int.TryParse(Console.ReadLine().Trim(' '), out int rs))
               {
                  Console.WriteLine("Error: response not recognized. Please type an integer.");
                  goto askRecipeServSize;
               }
               r.serveSize = rs;
               recipes.Add(r);
               WriteRecipes(recipes);
               LoadRecipes(ref recipes);
               Console.WriteLine("Very good. Your new recipe \"" + r.recipeName + "\" is now saved. Type \"Read\" to read recipes, \"Write\" to write another, \"Print\" to bring up the print interface, or \"Exit\" to quit.");
               goto chooseReadOrWriteRecipeStart;
            case "print":
               if (recipes.Count == 0)
               {
                  Console.WriteLine("No entries.");
                  goto startOfReadRecipe;
               }
               string[] sss = new string[recipes.Count];
               for (int i = 0; i < sss.Length; i++)
               {
                  sss[i] = recipes[i].recipeName;
               }
               sss.ToList().OrderBy(s => s).ToArray();
               Console.WriteLine(string.Join(Environment.NewLine, sss));
               Console.WriteLine("Above is a list of your recipes. To print one recipe, Type the name of the recipe exactally as it is spelled. To print multiple, type their names exactally as they are spelled seperated by a single space. To print all recipes, type \"ALL RECIPIES\". To go back, type \"GO BACK\".");
               string cout = Console.ReadLine();
               if (cout.Trim(' ') != "ALL RECIPIES")
               {
                  string[] keys = cout.ToLower().Trim(' ').Split(' ');
                  List<Recipe> tempp = new List<Recipe>();
                  foreach (string s in keys)
                  {
                     if (s != "")
                     {
                        foreach (Recipe reci in recipes)
                        {
                           if (s == reci.recipeName)
                           {
                              tempp.Add(reci);
                           }
                        }
                     }
                  }
                  string toPrint = string.Empty;
                  for (int i = 0; i < tempp.Count; i++)
                  {
                     toPrint += DisplayRecipe(tempp[i], 1) + Environment.NewLine + Environment.NewLine + Environment.NewLine;
                  }
                  File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Cooking Recipes.txt", toPrint);
                  ProcessStartInfo pi = new ProcessStartInfo(AppDomain.CurrentDomain.BaseDirectory + @"\Cooking Recipes.txt");
                  pi.UseShellExecute = true;
                  pi.Verb = "print";
                  Process process = Process.Start(pi);
                  Console.WriteLine("Print attempt made. Please type either \"Read\" or \"Write\" to read your recipes, or write them respectively. Type \"Print\" to bring up the print interface. Type \"Exit\" at any time to quit.");
                  goto chooseReadOrWriteRecipeStart;
               }
               else if (cout.Trim(' ') != "GO BACK")
               {
                  string toPrint = string.Empty;
                  for (int i = 0; i < recipes.Count; i++)
                  {
                     toPrint += DisplayRecipe(recipes[i], 1) + Environment.NewLine + Environment.NewLine + Environment.NewLine;
                  }
                  File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Cooking Recipes.txt", toPrint);
                  ProcessStartInfo pi = new ProcessStartInfo(AppDomain.CurrentDomain.BaseDirectory + @"\Cooking Recipes.txt");
                  pi.UseShellExecute = true;
                  pi.Verb = "print";
                  Process process = Process.Start(pi);
                  Console.WriteLine("Print attempt made. Please type either \"Read\" or \"Write\" to read your recipes, or write them respectively. Type \"Print\" to bring up the print interface. Type \"Exit\" at any time to quit.");
                  goto chooseReadOrWriteRecipeStart;
               }
               else
               {
                  Console.WriteLine("Please type either \"Read\" or \"Write\" to read your recipes, or write them respectively. Type \"Print\" to bring up the print interface. Type \"Exit\" at any time to quit.");
                  goto chooseReadOrWriteRecipeStart;
               }
            case "exit":
            case "quit":
               Environment.Exit(0);
               break;
            default:
               Console.WriteLine("Error: response not recognized. Please type either \"Read\", \"Write\", or \"Print\" to read your recipes, write them, or print them respectively.");
               goto chooseReadOrWriteRecipeStart;
         }
         Console.ReadKey();
      }
      private static void LoadRecipes(ref List<Recipe> recipes)
      {
         if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\Recipes.txt"))
         {
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Recipes.txt", "");
         }
         string[] rawData = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Recipes.txt").Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
         recipes = new List<Recipe>();
         if (rawData.Length == 1 && rawData[0] == "")
         {
            return;
         }
         for (int i = 0; i < rawData.Length; i++)
         {
            Recipe r = new Recipe();
            string[] working = rawData[i].Split('|');
            if (working.Length == 1)
            {
               break;
            }
            r.recipeName = working[0];
            List<string> ilist = new List<string>();
            int indexHolder = 0;
            for (int j = 2; j < working.Length; j++)
            {
               if (working[j] == "")
               {
                  indexHolder = j;
                  break;
               }
               else
               {
                  ilist.Add(working[j]);
               }
            }
            r.ingredientTitles = ilist.ToArray();
            ilist.Clear();
            for (int j = indexHolder + 1; j < working.Length; j++)
            {
               if (working[j] == "")
               {
                  indexHolder = j;
                  break;
               }
               else
               {
                  ilist.Add(working[j]);
               }
            }
            List<float> flist = new List<float>();
            ilist.ForEach(x => { flist.Add(float.Parse(x)); });
            r.ingredientAmounts = flist.ToArray();
            ilist.Clear();
            for (int j = indexHolder + 1; j < working.Length; j++)
            {
               if (working[j] == "")
               {
                  indexHolder = j;
                  break;
               }
               else
               {
                  ilist.Add(working[j]);
               }
            }
            r.ingredientUnits = ilist.ToArray();
            r.instructions = working[working.Length - 3];
            r.serveSize = int.Parse(working[working.Length - 1]);
            recipes.Add(r);
         }
      }
      private static void WriteRecipes(List<Recipe> recipes)
      {
         StringBuilder sb = new StringBuilder();
         foreach (Recipe r in recipes)
         {
            sb.Append(r.recipeName + "||");
            foreach (string s in r.ingredientTitles)
            {
               sb.Append(s + "|");
            }
            sb.Append("|");
            foreach (float f in r.ingredientAmounts)
            {
               sb.Append(f.ToString() + "|");
            }
            sb.Append("|");
            foreach (string s in r.ingredientUnits)
            {
               sb.Append(s + "|");
            }
            sb.Append("|" + r.instructions + "||" + r.serveSize);
            sb.Append(System.Environment.NewLine);
         }
         File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Recipes.txt", sb.ToString());
      }
      private static string DisplayRecipe(Recipe recipe, int serveMultiplier = 1)
      {
         string s = string.Empty;
         s += "---=====*" + recipe.recipeName + "*=====---" + Environment.NewLine;
         for (int i = 0; i < recipe.ingredientAmounts.Length; i++)
         {
            s += recipe.ingredientTitles[i] + " :: " + (recipe.ingredientAmounts[i] * serveMultiplier) + " - (" + recipe.ingredientUnits[i] + ")" + Environment.NewLine;
         }
         s += recipe.instructions + Environment.NewLine;
         switch (recipe.serveSize * serveMultiplier)
         {
            case 1:
               s += "This recipe serves 1 person." + Environment.NewLine;
               break;
            default:
               s += "This recipe serves " + (recipe.serveSize * serveMultiplier) + " people." + Environment.NewLine;
               break;
         }
         s += "---======";
         for (int i = 0; i < recipe.recipeName.Length; i++)
         {
            s += "=";
         }
         s += "======---";
         return s;
      }
   }

}
struct Recipe
{
   //recipe formatting:recipeName||ingredientTitles[if there are multiple, they are seperated by a single "|"]||ingredientAmounts[""]||ingredientUnits[""]||instructions
   public string recipeName;
   public string[] ingredientTitles;
   public float[] ingredientAmounts;
   public string[] ingredientUnits;
   public string instructions;
   public int serveSize;
   public Recipe(string recipName, string instr, string[] ingrTitles, float[] ingrAmounts, string[] ingrUnits, int srv)
   {
      recipeName = recipName;
      instructions = instr;
      ingredientTitles = ingrTitles;
      ingredientAmounts = ingrAmounts;
      ingredientUnits = ingrUnits;
      serveSize = srv;
   }
}

#region Codewars Interpreter
//private class LL
//{
//   private LL prev;
//   private LL next;
//   public bool val;

//   public LL GetPrev()
//   {
//      if (prev == null)
//      {
//         prev = new LL();
//         prev.next = this;
//      }
//      return prev;
//   }

//   public LL GetNext()
//   {
//      if (next == null)
//      {
//         next = new LL();
//         next.prev = this;
//      }
//      return next;
//   }

//   public bool HasPrev() => prev != null;
//   public bool HasNext() => next != null;
//}
//public static string interpret(string code, string input)
//{
//   StringBuilder resultBits = new StringBuilder();
//   int instrPtr = 0;
//   int inpPtr = (input.Length * 8) - 8;
//   LL datPtr = new LL();
//   while (instrPtr >= 0 && instrPtr < code.Length)
//   {
//      switch (code[instrPtr])
//      {
//         case '+':
//            datPtr.val = !datPtr.val;
//            instrPtr++;
//            break;
//         case ',':
//            if (inpPtr < 0)
//            {
//               datPtr.val = false;
//            }
//            else
//            {
//               byte b = (byte)input[(inpPtr / 8) + (inpPtr % 8 == 0 ? 0 : 1)];
//               b = (byte)(b << (inpPtr % 8));
//               datPtr.val = b > 127;
//               inpPtr--;
//            }
//            instrPtr++;
//            break;
//         case ';':
//            resultBits.Append(datPtr.val ? "1" : "0");
//            instrPtr++;
//            break;
//         case '<':
//            datPtr = datPtr.GetPrev();
//            instrPtr++;
//            break;
//         case '>':
//            datPtr = datPtr.GetNext();
//            instrPtr++;
//            break;
//         case '[':
//            if (!datPtr.val)
//            {
//               for (int i = instrPtr + 1, j = 0; ; i++)
//               {
//                  if (i > code.Length - 1)
//                  {
//                     instrPtr = -2;
//                     break;
//                  }
//                  if (code[i] == '[')
//                  {
//                     j++;
//                  }
//                  else if (code[i] == ']')
//                  {
//                     if (j > 0)
//                     {
//                        j--;
//                     }
//                     else
//                     {
//                        instrPtr = i;
//                        break;
//                     }
//                  }
//               }
//            }
//            instrPtr++;
//            break;
//         case ']':
//            if (datPtr.val)
//            {
//               for (int i = instrPtr - 1, j = 0; ; i--)
//               {
//                  if (i < 0)
//                  {
//                     instrPtr = -2;
//                     break;
//                  }
//                  if (code[i] == ']')
//                  {
//                     j++;
//                  }
//                  else if (code[i] == '[')
//                  {
//                     if (j > 0)
//                     {
//                        j--;
//                     }
//                     else
//                     {
//                        instrPtr = i;
//                        break;
//                     }
//                  }
//               }
//            }
//            instrPtr++;
//            break;
//         default:
//            instrPtr++;
//            break;
//      }
//   }
//   if (resultBits.Length % 8 != 0)
//   {
//      resultBits.Append(Enumerable.Repeat('0', 8 - resultBits.Length % 8).ToArray());
//   }
//   string rb = resultBits.ToString();
//   StringBuilder asciiResult = new StringBuilder();
//   for (int i = 0; i < rb.Length / 8; i++)
//   {
//      char[] charArray = rb.Substring(i * 8, 8).ToCharArray();
//      Array.Reverse(charArray);
//      int inp = Convert.ToInt32(new string(charArray), 2);
//      asciiResult.Append((char)inp);
//   }
//   return asciiResult.ToString();
//}

//testEmpty();
//testSingleCommands();
//testHelloWorld();
//testBasic();
//void testEmpty()
//{
//    Console.WriteLine("" == interpret("", "") ? "Correct" : "Incorrect");
//}
//void testSingleCommands()
//{
//    Console.WriteLine("" == interpret("<", "") ? "Correct" : "Incorrect");
//    Console.WriteLine("" == interpret(">", "") ? "Correct" : "Incorrect");
//    Console.WriteLine("" == interpret("+", "") ? "Correct" : "Incorrect");
//    Console.WriteLine("" == interpret(".", "") ? "Correct" : "Incorrect");
//    Console.WriteLine("\u0000" == interpret(";", "") ? "Correct" : "Incorrect");
//}
//void testHelloWorld()
//{
//    Console.WriteLine("Hello, world!\n" == interpret(";;;+;+;;+;+;+;+;+;+;;+;;+;;;+;;+;+;;+;;;+;;+;+;;+;+;;;;+;+;;+;;;+;;+;+;+;;;;;;;+;+;;+;;;+;+;;;+;+;;;;+;+;;+;;+;+;;+;;;+;;;+;;+;+;;+;;;+;+;;+;;+;+;+;;;;+;+;;;+;+;+;", "") ? "Correct" : "Incorrect");
//}
//void testBasic()
//{
//    Console.WriteLine("Codewars" == interpret(">,>,>,>,>,>,>,>,<<<<<<<[>]+<[+<]>>>>>>>>>[+]+<<<<<<<<+[>+]<[<]>>>>>>>>>[+<<<<<<<<[>]+<[+<]>>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+]<<<<<<<<;>;>;>;>;>;>;>;<<<<<<<,>,>,>,>,>,>,>,<<<<<<<[>]+<[+<]>>>>>>>>>[+]+<<<<<<<<+[>+]<[<]>>>>>>>>>]<[+<]", "Codewars\u00ff") ? "Correct" : "Incorrect");
//    Console.WriteLine("Codewars" == interpret(">,>,>,>,>,>,>,>,>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+<<<<<<<<[>]+<[+<]>;>;>;>;>;>;>;>;>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+<<<<<<<<[>]+<[+<]>>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+]+<<<<<<<<+[>+]<[<]>>>>>>>>>]<[+<]>,>,>,>,>,>,>,>,>+<<<<<<<<+[>+]<[<]>>>>>>>>>]<[+<]", "Codewars") ? "Correct" : "Incorrect");
//    Console.WriteLine("\u0048" == interpret(">,>,>,>,>,>,>,>,>>,>,>,>,>,>,>,>,<<<<<<<<+<<<<<<<<+[>+]<[<]>>>>>>>>>[+<<<<<<<<[>]+<[+<]>>>>>>>>>>>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+<<<<<<<<[>]+<[+<]>>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+]>[>]+<[+<]>>>>>>>>>[+]>[>]+<[+<]>>>>>>>>>[+]<<<<<<<<<<<<<<<<<<+<<<<<<<<+[>+]<[<]>>>>>>>>>]<[+<]>>>>>>>>>>>>>>>>>>>>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+<<<<<<<<[>]+<[+<]>>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+]<<<<<<<<<<<<<<<<<<<<<<<<<<[>]+<[+<]>>>>>>>>>[+]>>>>>>>>>>>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>]<[+<]<<<<<<<<<<<<<<<<<<+<<<<<<<<+[>+]<[<]>>>>>>>>>[+]+<<<<<<<<+[>+]<[<]>>>>>>>>>]<[+<]>>>>>>>>>>>>>>>>>>>;>;>;>;>;>;>;>;<<<<<<<<", "\u0008\u0009") ? "Correct" : "Incorrect");
//}
#endregion