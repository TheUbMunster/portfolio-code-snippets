using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;
using System.Runtime.InteropServices.ComTypes;

namespace A01___Hangman
{
   class Hangman
   {
      readonly static Regex alphaAndSpaceReg = new Regex(@"^[a-zA-Z ]+$");
      readonly static Regex alphaCharReg = new Regex(@"[a-zA-Z]");
      readonly static Regex affirmativeReg = new Regex(@"^[Y|yE|eS|s]|[Y|y]");
      readonly static Regex negativeReg = new Regex(@"^[N|nO|o]|[N|n]|[Q|qU|uI|iT|t]");
      static void Main(string[] args)
      {
         //int[,] values = new int[4, 5]
         //    {
         //        { 1, 2, 3, 4, 5 },
         //        { 6, 7, 8, 9, 10 },
         //        { 11, 12, 13, 14, 15 },
         //        { 16, 17, 18, 19, 20 },
         //    };
         //for (int i = 0, j = 0; !(i == values.GetLength(0)); i += j == values.GetLength(1) - 1 ? 1 : 0, j += j < values.GetLength(1) - 1 ? 1 : -j)
         //{
         //    Console.Write(values[i, j] + " ");
         //}
         //Console.ReadLine();
         //return;
         bool running = true;
         while (running)
         {
            Console.Clear();
            bool gaming = true;
            bool validInput = false;
            string fullWord = string.Empty;
            StringBuilder guessWord = new StringBuilder(string.Empty);
            List<char> guessedLetters = new List<char>();
            int livesLeft = 6;
            Console.WriteLine("Welcome to hangman! Enter a word to start: (Can contain only letters and spaces.) ");
            while (!validInput)
            {
               // Cleans the input.
               fullWord = string.Join(" ", Console.ReadLine().Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).ToLower();
               if (alphaAndSpaceReg.IsMatch(fullWord))
               {
                  validInput = true;
               }
               else
               {
                  Console.WriteLine("Sorry, that input isn't valid. You can only choose a word or phrase that contains letters and spaces.");
                  Console.WriteLine("Enter a word to start: (Can contain only letters and spaces.) ");
               }
            }
            // We now have a valid input.            
            guessWord.Append(alphaCharReg.Replace(fullWord, "_"));
            Console.Clear();
            //print status bar:
            Console.WriteLine($"Lives Left: {livesLeft}.   Letters guessed: {string.Join(", ", guessedLetters)}\n");
            //print word:
            Console.WriteLine(guessWord.ToString());
            while (gaming)
            {
               //print prompt:
               bool validGuess = false;
               char guess = '\0';
               while (!validGuess)
               {
                  Console.WriteLine("\nGuess a letter: ");
                  guess = Console.ReadLine().ToLower().Trim().FirstOrDefault();
                  if (guess == '\0')
                  {
                     Console.WriteLine("Make sure to type a character. The first character entered will be your guess.");
                  }
                  else if (guessedLetters.Contains(guess))
                  {
                     Console.WriteLine("You already guessed that letter!");
                  }
                  else if (!alphaCharReg.IsMatch(guess.ToString()))
                  {
                     Console.WriteLine("You can only guess English letters!");
                  }
                  else
                  {
                     validGuess = true;
                  }
               }
               guessedLetters.Add(guess);
               //evaluate:
               bool didFindLetter = false;
               for (int i = 0; i < fullWord.Length; i++)
               {
                  if (fullWord.ToLower()[i] == guess)
                  {
                     didFindLetter = true;
                     guessWord.Remove(i, 1);
                     if (i == 0 || fullWord[i - 1] == ' ')
                     {
                        guessWord.Insert(i, guess.ToString().ToUpper());
                     }
                     else
                     {
                        guessWord.Insert(i, guess.ToString().ToLower());
                     }
                  }
               }
               if (!didFindLetter)
               {
                  livesLeft--;
               }
               //Clear for fresh start.
               Console.Clear();
               //print status bar:
               Console.WriteLine($"Lives Left: {livesLeft}.   Letters guessed: {string.Join(", ", guessedLetters)}\n");
               //print word:
               Console.WriteLine(guessWord.ToString());
               //check for win/lose.
               if (livesLeft == 0 || !guessWord.ToString().Contains('_'))
               {
                  if (livesLeft == 0)
                  {
                     //You lose.
                     Console.WriteLine($"Uh oh! Out of lives, you lose. The word/phrase was: \"{fullWord}\" Play Again? (Y/N)");
                  }
                  else
                  {
                     //You win!
                     Console.WriteLine("Congratulations! You win! Play Again? (Y/N)");
                  }
                  string input = string.Empty;
                  validInput = false;
                  while (!validInput)
                  {
                     input = Console.ReadLine();
                     if (affirmativeReg.IsMatch(input) || negativeReg.IsMatch(input))
                     {
                        validInput = true;
                     }
                     else
                     {
                        Console.WriteLine("Hey! That isn't a valid input, say \"Y\" or \"N\".");
                     }
                  }
                  if (affirmativeReg.IsMatch(input))
                  {
                     livesLeft = 6;
                     guessedLetters.Clear();
                     guessWord.Clear();
                     fullWord = string.Empty;
                     gaming = false;
                     running = true;
                  }
                  else if (negativeReg.IsMatch(input))
                  {
                     Console.WriteLine("Thanks for playing!");
                     Thread.Sleep(2000);
                     gaming = false;
                     running = false;
                  }
               }
            }
         }
      }
   }
}