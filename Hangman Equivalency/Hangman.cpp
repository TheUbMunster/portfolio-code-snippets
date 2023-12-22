#include <iostream>
#include <string>
#include <vector>
#include <iomanip>
#include <algorithm>
#include <regex>
#include <thread>
#include <chrono>

void clearConsole(void)
{
	 std::system("cls"); //I was told this was bad for security reasons. Oh well.
}

int main()
{
	 bool running = true;
	 while (running)
	 {
			clearConsole();
			bool gaming = true;
			bool validInput = false;
			std::string fullWord;
			std::string guessWord;
			std::vector<char> guessedLetters;
			int livesLeft = 6;
			std::cout << "Welcome to hangman! Enter a word to start: (Can contain only letters and spaces.) \n";
			while (!validInput)
			{
				 fullWord.clear();
				 std::getline(std::cin, fullWord);
				 // Cleans the input.
				 // Split on spaces, remove entry entries, trim whitespace of words, lowercaseify.
				 std::transform(fullWord.begin(), fullWord.end(), fullWord.begin(), [](unsigned char c) { return std::tolower(c); });
				 std::vector<std::string> temp;
				 for (size_t ind = 0; ind < fullWord.length(); )
				 {
						ind = fullWord.find_first_not_of(' ', ind);
						size_t end = fullWord.find(' ', ind);
						end = (end == ((size_t)-1)) ? fullWord.length() : end; //no -1 on len to correct for error on the last iteration
						size_t len = (end - ind);
						temp.push_back(fullWord.substr(ind, len));
						ind = end;
				 }
				 fullWord.clear();
				 for (auto& e : temp)
				 {
						fullWord += (e + " ");
				 }
				 if (fullWord.length() > 0)
				 {
						fullWord.erase(fullWord.length() - 1);
				 }
				 if (std::regex_match(fullWord.c_str(), std::regex("^[a-zA-Z ]+$")))
				 {
						validInput = true;
				 }
				 else
				 {
						std::cout << "Sorry, that input isn't valid. You can only choose a word or phrase that contains letters and spaces.\n";
						std::cout << "Enter a word to start: (Can contain only letters and spaces.) \n";
				 }
			}
			// We now have a valid input.
			guessWord.resize(fullWord.length(), ' ');
			std::transform(fullWord.begin(), fullWord.end(), guessWord.begin(), [](unsigned char c) { return ((c == ' ') ? ' ' : '_'); });
			clearConsole();
			//print status bar:
			std::cout << "Lives Left: " << std::to_string(livesLeft) << ".   Letters guessed: ";
			for (size_t i = 0; i < guessedLetters.size(); i++)
			{
				 std::cout << guessedLetters[i];
				 if (i + 1 != guessedLetters.size())
				 {
						std::cout << ", ";
				 }
			}
			std::cout << "\n\n";
			//print word:
			std::cout << guessWord << "\n";
			while (gaming)
			{
				 //print prompt:
				 bool validGuess = false;
				 char guess = '\0';
				 while (!validGuess)
				 {
						std::cout << "\nGuess a letter: \n";
						std::string temp;
						std::getline(std::cin, temp);
						guess = ((temp.length() > 0) ? temp[temp.find_first_not_of(' ')] : guess);
						guess = std::tolower(guess);
						if (guess == '\0')
						{
							 std::cout << "Make sure to type a character. The first character entered will be your guess.\n";
						}
						else if (std::count(guessedLetters.begin(), guessedLetters.end(), guess) > 0)
						{
							 std::cout << "You already guessed that letter!\n";
						}
						else if (!std::isalpha(guess))
						{
							 std::cout << "You can only guess English letters!\n";
						}
						else
						{
							 validGuess = true;
						}
				 }
				 guessedLetters.push_back(guess);
				 //evaluate:
				 bool didFindLetter = false;
				 for (size_t i = 0; i < fullWord.length(); i++)
				 {
						if (std::tolower(fullWord[i]) == guess)
						{
							 didFindLetter = true;
							 if (i == 0 || fullWord[i - 1] == ' ')
							 {
									guessWord[i] = std::toupper(guess);
							 }
							 else
							 {
									guessWord[i] = std::tolower(guess);
							 }
						}
				 }
				 if (!didFindLetter)
				 {
						livesLeft--;
				 }
				 //Clear for fresh start.
				 clearConsole();
				 //print status bar:
				 std::cout << "Lives Left: " << std::to_string(livesLeft) << ".   Letters guessed: ";
				 for (size_t i = 0; i < guessedLetters.size(); i++)
				 {
						std::cout << guessedLetters[i];
						if (i + 1 != guessedLetters.size())
						{
							 std::cout << ", ";
						}
				 }
				 std::cout << "\n\n";
				 //print word:
				 std::cout << guessWord << "\n";
				 //check for win/lose.
				 if (livesLeft == 0 || std::count(guessWord.begin(), guessWord.end(), '_') == 0)
				 {
						if (livesLeft == 0)
						{
							 //You lose.
							 std::cout << "Uh oh! Out of lives, you lose. The word/phrase was: \"" << fullWord << "\" Play Again? (Y/N)\n";
						}
						else
						{
							 //You win!
							 std::cout << "Congratulations! You win! Play Again? (Y/N)\n";
						}
						std::string input;
						validInput = false;
						while (!validInput)
						{
							 std::getline(std::cin, input);
							 if (std::regex_match(input.c_str(), std::regex("^[Y|yE|eS|s]|[Y|y]")) || std::regex_match(input.c_str(), std::regex("^[N|nO|o]|[N|n]|[Q|qU|uI|iT|t]")))
							 {
									validInput = true;
							 }
							 else
							 {
									std::cout << "Hey! That isn't a valid input, say \"Y\" or \"N\".\n";
							 }
						}
						if (std::regex_match(input.c_str(), std::regex("^[Y|yE|eS|s]|[Y|y]")))
						{
							 livesLeft = 6;
							 guessedLetters.clear();
							 guessWord.clear();
							 fullWord.clear();
							 gaming = false;
							 running = true;
						}
						else if (std::regex_match(input.c_str(), std::regex("^[N|nO|o]|[N|n]|[Q|qU|uI|iT|t]")))
						{
							 std::cout << "Thanks for playing!\n";
							 std::this_thread::sleep_for(std::chrono::seconds(2));
							 gaming = false;
							 running = false;
						}
				 }
			}
	 }
}