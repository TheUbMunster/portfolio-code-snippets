Imports System
Imports System.Linq
Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic
Imports System.Threading

Module Hangman
   Private ReadOnly alphaAndSpaceReg As Regex = New Regex("^[a-zA-Z ]+$")
   Private ReadOnly alphaCharReg As Regex = New Regex("[a-zA-Z]")
   Private ReadOnly affirmativeReg As Regex = New Regex("^[Y|yE|eS|s]|[Y|y]")
   Private ReadOnly negativeReg As Regex = New Regex("^[N|nO|o]|[N|n]|[Q|qU|uI|iT|t]")
   Sub Main(args As String())
      Dim running As Boolean = True
      While running
         Console.Clear()
         Dim gaming As Boolean = True
         Dim validInput As Boolean = False
         Dim fullWord As String = String.Empty
         Dim guessWord As StringBuilder = New StringBuilder(String.Empty)
         Dim guessedLetters As List(Of Char) = New List(Of Char)
         Dim livesLeft As Int32 = 6
         Console.WriteLine("Welcome to hangman! Enter a word to start: (Can contain only letters and spaces.) ")
         While Not validInput
            fullWord = String.Join(" ", Console.ReadLine().Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries)).ToLower()
            If alphaAndSpaceReg.IsMatch(fullWord) Then
               validInput = True
            Else
               Console.WriteLine("Sorry, that input isn't valid. You can only choose a word or phrase that contains letters and spaces.")
               Console.WriteLine("Enter a word to start: (Can contain only letters and spaces.) ")
            End If
         End While
         ' We Now have a valid input.
         guessWord.Append(alphaCharReg.Replace(fullWord, "_"))
         Console.Clear()
         ' print status bar:
         Console.WriteLine($"Lives Left: {livesLeft}.   Letters guessed: {String.Join(", ", guessedLetters)}{Environment.NewLine}")
         ' print word
         Console.WriteLine(guessWord.ToString())
         While gaming
            ' print prompt:
            Dim validGuess As Boolean = False
            Dim guess As Char = Char.MinValue
            While Not validGuess
               Console.WriteLine($"{Environment.NewLine}Guess a letter: ")
               guess = Console.ReadLine().ToLower().Trim().FirstOrDefault()
               If guess = Char.MinValue Then
                  Console.WriteLine("Make sure to type a character. The first character entered will be your guess.")
               ElseIf guessedLetters.Contains(guess) Then
                  Console.WriteLine("You already guessed that letter!")
               ElseIf Not alphaCharReg.IsMatch(guess.ToString()) Then
                  Console.WriteLine("You can only guess English letters!")
               Else
                  validGuess = True
               End If
            End While
            guessedLetters.Add(guess)
            ' evaluate:
            Dim didFindLetter As Boolean = False
            For index = 0 To fullWord.Length - 1
               If fullWord.ToLower()(index) = guess Then
                  didFindLetter = True
                  guessWord.Remove(index, 1)
                  If index = 0 Then
                     guessWord.Insert(index, guess.ToString().ToUpper())
                  Else
                     If fullWord(index - 1) = " " Then ' can't add this as an or to the above if line, vb seems to not break if statement evaluations if the first half of an or is true.
                        guessWord.Insert(index, guess.ToString().ToUpper())
                     Else
                        guessWord.Insert(index, guess.ToString().ToLower())
                     End If
                  End If
               End If
            Next index
            If Not didFindLetter Then
               livesLeft -= 1
            End If
            ' Clear for fresh start.
            Console.Clear()
            ' print status bar:
            Console.WriteLine($"Lives Left: {livesLeft}.   Letters guessed: {String.Join(", ", guessedLetters)}{Environment.NewLine}")
            ' print word:
            Console.WriteLine(guessWord.ToString())
            ' check for win/lose.
            If livesLeft = 0 Or (Not guessWord.ToString().Contains("_")) Then
               If livesLeft = 0 Then
                  ' You lose.
                  Console.WriteLine($"Uh oh! Out of lives, you lose. The word/phrase was: ""{fullWord}"" Play Again? (Y/N)")
               Else
                  ' You win!
                  Console.WriteLine("Congratulations! You win! Play Again? (Y/N)")
               End If
               Dim input As String = String.Empty
               validInput = False
               While Not validInput
                  input = Console.ReadLine()
                  If affirmativeReg.IsMatch(input) Or negativeReg.IsMatch(input) Then
                     validInput = True
                  Else
                     Console.WriteLine("Hey! That isn't a valid input, say ""Y"" or ""N"".")
                  End If
               End While
               If affirmativeReg.IsMatch(input) Then
                  livesLeft = 6
                  guessedLetters.Clear()
                  guessWord.Clear()
                  fullWord = String.Empty
                  gaming = False
                  running = True
               ElseIf negativeReg.IsMatch(input) Then
                  Console.WriteLine("Thanks for playing!")
                  Thread.Sleep(2000)
                  gaming = False
                  running = False
               End If
            End If
         End While
      End While
   End Sub
End Module