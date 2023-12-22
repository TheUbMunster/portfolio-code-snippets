import re
import os
import time

def clearConsole(): 
   os.system('cls' if os.name == 'nt' else 'clear')
   #print("\n\n\n") #it's a royal pain to clear the console in python apparently so just add three newlines instead.

alphaAndSpaceReg = re.compile("^[a-zA-Z ]+$")
alphaCharReg = re.compile("[a-zA-Z]")
affirmativeReg = re.compile("^[Y|yE|eS|s]|[Y|y]")
negativeReg = re.compile("^[N|nO|o]|[N|n]|[Q|qU|uI|iT|t]")

running: bool = True
while running:
    clearConsole()
    gaming: bool = True
    validInput: bool = False
    fullWord: str = ""
    guessWord: str = ""
    guessedLetters = []
    livesLeft: int = 6
    print("Welcome to hangman! Enter a word to start: (Can contain only letters and spaces.) ")
    while not validInput:
        #cleans the input
        fullWord = " ".join(input().strip().lower().split())
        if alphaAndSpaceReg.fullmatch(fullWord):
            validInput = True
        else:
            print("Sorry, that input isn't valid. You can only choose a word or phrase that contains letters and spaces.")
            print("Enter a word to start: (Can contain only letters and spaces.) ")
    #we now have valid input
    for x in fullWord:
        if alphaCharReg.match(x):
            guessWord += "_"
        else:
            guessWord += " "
    clearConsole()
    #print status bar
    print("Lives Left: " + str(livesLeft) + ".   Letters guessed: " + ", ".join(guessedLetters) + "\n")
    #print word:
    print(guessWord)
    while gaming:
        #print prompt
        validGuess: bool = False
        guess = " "
        while not validGuess:
            print("\nGuess a letter: ")
            guess = input().strip().lower()
            guess = guess[0] if len(guess) > 0 else " "
            if guess == " ":
                print("Make sure to type a character. The first character entered will be your guess.")
            elif guess in guessedLetters:
                print("You already guessed that letter!")
            elif not alphaCharReg.fullmatch(guess):
                print("You can only guess English letters!")
            else:
                validGuess = True
        guessedLetters.append(guess)
        #evaluate
        didFindLetter: bool = False
        for i, val in enumerate(fullWord):
            if val.lower() == guess:
                didFindLetter = True
                if i == 0 or fullWord[i - 1] == " ":
                    guessWord = guessWord[:i] + guess.upper() + guessWord[i + 1:]
                else:
                    guessWord = guessWord[:i] + guess.lower() + guessWord[i + 1:]
        if not didFindLetter:
            livesLeft -= 1
        #Clear for fresh start
        clearConsole()
        #print status bar:
        print("Lives Left: " + str(livesLeft) + ".   Letters guessed: " + ", ".join(guessedLetters) + "\n")
        #print word
        print(guessWord)
        #check for win/lose
        if livesLeft == 0 or guessWord.find("_") == -1:
            if livesLeft == 0:
                print("Uh oh! Out of lives, you lose. The word/phrase was: \"" + fullWord + "\" Play Again? (Y/N)")
            else:
                print("Congratulations! You win! Play Again? (Y/N)")
            inputVal: str = ""
            validInput = False
            while not validInput:
                inputVal = input()
                if affirmativeReg.fullmatch(inputVal) or negativeReg.fullmatch(inputVal):
                    validInput = True
                else:
                    print("Hey! That isn't a valid input, say \"Y\" or \"N\".")
            if affirmativeReg.fullmatch(inputVal):
                livesLeft = 6
                guessedLetters.clear()
                guessWord = ""
                fullWord = ""
                gaming = False
                running = True
            elif negativeReg.fullmatch(inputVal):
                print("Thanks for playing!")
                time.sleep(2)
                gaming = False
                running = False