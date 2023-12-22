//This is a direct conversion of my C# version of this game.

open System
open System.Text.RegularExpressions
open System.Text
open System.Linq
open System.Collections.Generic

let alphaAndSpaceReg : Regex = new Regex(@"^[a-zA-Z ]+$")
let alphaCharReg : Regex = new Regex(@"[a-zA-Z]")
let affirmativeReg : Regex = new Regex(@"^[Y|yE|eS|s]|[Y|y]")
let negativeReg : Regex = new Regex(@"^[N|nO|o]|[N|n]|[Q|qU|uI|iT|t]")

[<EntryPoint>]
let main (argv : string[]) =
    let mutable running : bool = true
    while (running) do
        Console.Clear() |> ignore
        let mutable gaming : bool = true
        let mutable validInput : bool = false
        let mutable fullWord : string = String.Empty
        let guessWord : StringBuilder = new StringBuilder()
        let guessedLetters : List<char> = new List<char>()
        let mutable livesLeft : int = 6
        Console.WriteLine "Welcome to hangman! Enter a word to start: (Can contain only letters and spaces.)"
        while (not(validInput)) do
            //Cleans the input.
            fullWord <- String.Join(" ", Console.ReadLine().Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToLower()
            if (alphaAndSpaceReg.IsMatch fullWord) then
                validInput <- true
            else
                Console.WriteLine("Sorry, that input isn't valid. You can only choose a word or phrase that contains letters and spaces.")
                Console.WriteLine("Enter a word to start: (Can contain only letters and spaces.) ")
        //We now have valid input.
        alphaCharReg.Replace(fullWord, "_") |> guessWord.Append |> ignore
        //Fresh start for printing UI.
        Console.Clear() |> ignore
        //Print status bar:
        Console.WriteLine("Lives Left: {0}.   Letters guessed: {1}\n", livesLeft, String.Join(", ", guessedLetters))
        //Print word:
        Console.WriteLine(guessWord.ToString())
        while (gaming) do
            //Print prompt:
            let mutable validGuess : bool = false
            let mutable guess : char = Char.MinValue
            while (not(validGuess)) do
                Console.WriteLine("\nGuess a letter: ")
                guess <- Console.ReadLine().ToLower().Trim().FirstOrDefault()
                if (guess = Char.MinValue) then
                    Console.WriteLine("Make sure to type a character. The first character entered will be your guess.")
                else if (guessedLetters.Contains(guess)) then
                    Console.WriteLine("You already guessed that letter!")
                else if (not(alphaCharReg.IsMatch(guess.ToString()))) then
                    Console.WriteLine("You can only guess English letters!")
                else
                    validGuess <- true
            guessedLetters.Add(guess)
            //Evaluate:
            let mutable didFindLetter : bool = false
            for i : int in 0..(fullWord.Length - 1) do
                if (fullWord.ToLower().[i] = guess) then
                    didFindLetter <- true
                    guessWord.Remove(i, 1) |> ignore
                    if (i = 0 || fullWord.[i - 1] = ' ') then
                        guessWord.Insert(i, guess.ToString().ToUpper()) |> ignore
                    else
                        guessWord.Insert(i, guess.ToString().ToLower()) |> ignore
            if (not(didFindLetter)) then
                livesLeft <- (livesLeft - 1)
            //Fresh start for printing UI.
            Console.Clear() |> ignore
            //Print status bar:
            Console.WriteLine("Lives Left: {0}.   Letters guessed: {1}\n", livesLeft, String.Join(", ", guessedLetters))
            //Print word:
            Console.WriteLine(guessWord.ToString())
            //Check for win/lose
            if (livesLeft = 0 || not(guessWord.ToString().Contains('_'))) then
                //you lose.
                if (livesLeft = 0) then
                    Console.WriteLine("Uh oh! Out of lives, you lose. The word/phrase was: \"{0}\" Play Again? (Y/N)", fullWord)
                else
                    Console.WriteLine("Congratulations! You Win! Play Again? (Y/N)")
                validInput <- false
                let mutable input : string = String.Empty
                while (not(validInput)) do
                    input <- Console.ReadLine()
                    if (affirmativeReg.IsMatch(input) || negativeReg.IsMatch(input)) then
                        validInput <- true
                    else
                        Console.WriteLine("Hey! That isn't a valid input, say \"Y\" or \"N\".")
                if (affirmativeReg.IsMatch(input)) then
                    livesLeft <- 6
                    guessedLetters.Clear()
                    guessWord.Clear() |> ignore
                    fullWord <- String.Empty
                    validInput <- false
                    gaming <- false
                    running <- true
                else if (negativeReg.IsMatch(input)) then
                    Console.WriteLine("Thanks for playing!")
                    Threading.Thread.Sleep(2000)
                    gaming <- false
                    running <- false
    0 // return an integer exit code