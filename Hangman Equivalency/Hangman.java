package hangman;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Scanner;

/**
 * @author Samuel Gardner
 */
public class Hangman
{
	public static void clearConsole()
	{
		//Copied from: https://stackoverflow.com/questions/2979383/java-clear-the-console
		if (System.getProperty("os.name").contains("Windows"))
		{
			try
			{
				new ProcessBuilder("cmd", "/c", "cls").inheritIO().start().waitFor();
			}
			catch (InterruptedException | IOException e)
			{
				e.printStackTrace();
			}
		}
		else
		{
			System.out.println("\n\n"); //for mac/linux
		}
	}

	public static void main(String[] args)
	{
		final String alphaAndSpaceReg = "^[a-zA-Z ]+$";
		final String alphaCharReg = "[a-zA-Z]";
		final String affirmativeReg = "^[Y|yE|eS|s]|[Y|y]";
		final String negativeReg = "^[N|nO|o]|[N|n]|[Q|qU|uI|iT|t]";
		try (Scanner sc = new Scanner(System.in))
		{
			boolean running = true;
			while (running)
			{
				clearConsole();
				boolean gaming = true;
				boolean validInput = false;
				String fullWord = "";
				StringBuilder guessWord = new StringBuilder();
				ArrayList<String> guessedLetters = new ArrayList<String>();
				int livesLeft = 6;
				System.out.println("Welcome to hangman! Enter a word to start: (Can contain only letters and spaces.) ");
				while (!validInput)
				{
					// Cleans the input.
					List<String> l = Arrays.asList(sc.nextLine().strip().split(" "));
					l.removeIf(x -> x.isBlank());
					fullWord = String.join(" ", l);
					if (fullWord.matches(alphaAndSpaceReg))
					{
						validInput = true;
					}
					else
					{
						System.out.println("Sorry, that input isn't valid. You can only choose a word or phrase that contains letters and spaces.");
						System.out.println("Enter a word to start: (Can contain only letters and spaces.) ");
					}
				}
				// We now have a valid input.
				guessWord.append(fullWord.replaceAll(alphaCharReg, "_"));
				clearConsole();
				// print status bar:
				System.out.println("Lives Left: " + livesLeft + ".   Letters guessed: " + String.join(", ", guessedLetters) + "\n");
				// print word:
				System.out.println(guessWord.toString());
				while (gaming)
				{
					// print prompt:
					boolean validGuess = false;
					char guess = '\0';
					while (!validGuess)
					{
						System.out.println("\nGuess a letter: ");
						String s = sc.nextLine().toLowerCase().strip();
						guess = s.length() > 0 ? s.charAt(0) : '\0';
						if (guess == '\0')
						{
							System.out.println("Make sure to type a character. The first character entered will be your guess.");
						}
						else if (guessedLetters.contains(Character.toString(guess)))
						{
							System.out.println("You already guessed that letter!");
						}
						else if (!Character.toString(guess).matches(alphaCharReg))
						{
							System.out.println("You can only guess English letters!");
						}
						else
						{
							validGuess = true;
						}
					}
					guessedLetters.add(Character.toString(guess));
					// evaluate:
					boolean didFindLetter = false;
					for (int i = 0; i < fullWord.length(); i++)
					{
						if (fullWord.toLowerCase().charAt(i) == guess)
						{
							didFindLetter = true;
							guessWord.replace(i, i + 1, "");
							if (i == 0 || fullWord.charAt(i - 1) == ' ')
							{
								guessWord.insert(i, Character.toString(guess).toUpperCase());
							}
							else
							{
								guessWord.insert(i, Character.toString(guess).toLowerCase());
							}
						}
					}
					if (!didFindLetter)
					{
						livesLeft--;
					}
					// Clear for fresh start.
					clearConsole();
					// print status bar:
					System.out.println("Lives Left: " + livesLeft + ".   Letters guessed: " + String.join(", ", guessedLetters) + "\n");
					// print word:
					System.out.println(guessWord.toString());
					// check for win/lose.
					if (livesLeft == 0 || !guessWord.toString().contains("_"))
					{
						if (livesLeft == 0)
						{
							//You lose.
							System.out.println("Uh oh! Out of lives, you lose. The word/phrase was: \"" + fullWord + "\" Play Again? (Y/N)");
						}
						else
						{
							//You win!
							System.out.println("Congratulations! You win! Play Again? (Y/N)");
						}
						String input = "";
						validInput = false;
						while (!validInput)
						{
							input = sc.nextLine();
							if (input.matches(affirmativeReg) || input.matches(negativeReg))
							{
								validInput = true;
							}
							else
							{
								System.out.println("Hey! That isn't a valid input, say \"Y\" or \"N\".");
							}
						}
						if (input.matches(affirmativeReg))
						{
							livesLeft = 6;
							guessedLetters.clear();
							guessWord.setLength(0);
							fullWord = "";
							gaming = false;
							running = true;
						}
						else if (input.matches(negativeReg))
						{
							System.out.println("Thanks for playing!");
							try
							{
								Thread.sleep(2000);
							}
							catch (InterruptedException e)
							{
								e.printStackTrace();
							}
							gaming = false;
							running = false;
						}
					}
				}
			}
		}
	}
}


