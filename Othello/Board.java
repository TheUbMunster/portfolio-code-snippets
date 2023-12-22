package othello;

/**
 * Creates the rules for the game Othello and stores the state of the board. In addition 
 * creates the coordinate system for keeping track of the pieces and stores the coordinates 
 * for each move. Lastly keeps track of the black and white pieces on the board to determine a winner.
 *  
 * @author Sam Gardner & Glenna Williams
 *
 */
public class Board
{
	//creating the dimensions of the board.
	private static final int width = 8;
	private static final int height = 8;
	private Space[] boardData = new Space[width * height];
	
	/**
	 * Sets up the board with the 4 starting pieces in the center (2 white, 2 black), 
	 * with the remaining squares empty.  	 * 
	 */
	public Board()
	{
		//set up starting board.
		for (int i = 0; i < width * height; i++)
		{
			boardData[i] = Space.EMPTY;
		}
		boardData[convertCoordinates1D(3, 3)] = Space.WHITE;
		boardData[convertCoordinates1D(4, 4)] = Space.WHITE;
		boardData[convertCoordinates1D(3, 4)] = Space.BLACK;
		boardData[convertCoordinates1D(4, 3)] = Space.BLACK;
	}
	
	
	/**
	 * Takes a two dimensional coordinate and turns into a one dimensional coordinate (0,0 is the
	 * top left corner). 
	 * 
	 * @param i location of piece on the y-axis (the positive direction is down).
	 * @param j location of piece on the x-axis (the positive direction is right). 
	 * @return the one dimensional location of the piece on board. 
	 */
	public static int convertCoordinates1D(int i, int j)
	{
		//i is the row number (y-pos down)
		//j is the column number (x-pos right)
		return (i * width) + j;
	}
	
	/**
	 * Takes a one dimensional coordinate and turns into a two dimensional coordinate
	 *  
	 * @param inp the one dimensional coordinate.
	 * @return an int array, 0th index is i and the 1st index is j
	 */
	public static int[] convertCoordinates2D(int inp)
	{
		int[] arr = new int[2];
		arr[1] = inp % width; // j
		arr[0] = (inp - arr[1]) / height; // i
		return arr;
	}
	
	/**
	 * Given coordinates i and j, the return value is what value from Space
	 * is occupying the board, either empty, white, or black. 
	 * 
	 * @param b board
	 * @param i coordinate on the y axis
	 * @param j coordinate on the x axis
	 * @return color of the tile on the board
	 */
	public static Space getSpot(Board b, int i, int j)
	{
		return b.boardData[convertCoordinates1D(i, j)];
	}
	
	/**
	 * Stores the coordinates selected during players turn and will place the 
	 * colored tile in that location if the move is valid.  Validation of the move
	 * is checked when move is submitted by checking the board in all directions
	 * for possible plays. If move is invalid, the board will not be modified. 
	 * 
	 * @param ix coordinate of the piece being placed 
	 * @param jy coordinate of the piece being placed
	 * @param isWhite true if the piece is white, false if the piece is black
	 * @return validity of move
	 */
	public boolean submitMove(int ix, int jy, boolean isWhite)
	{
		boolean isValidMove = false;
		Space col;
		if (isWhite)
		{
			col = Space.WHITE;
		}
		else
		{
			col = Space.BLACK;
		}
		//returns true if this move was a valid move.
		if (boardData[convertCoordinates1D(ix ,jy)] != Space.EMPTY)
		{
			//this spot is occupied already!
			return false;
		}
		else
		{		
			for (int ind = ix - 1; ind >= 0; ind--) //upwards check.
			{
				//ind represents i, and changes as we iterate.
				//j coordinate never changes during this loop.
				if (boardData[convertCoordinates1D(ind, jy)] == col) // we hit another piece of our color
				{
					if (ind + 1 == ix)
					{
						//this piece was DIRECTLY adjacent the piece we placed, therefore is invalid move.
						break;
					}
					else
					{
						//there is some pieces between the two pieces.
						isValidMove = true;
						//flip over the pieces.
						for (int start = ix, end = ind; start > end; start--)
						{
							boardData[convertCoordinates1D(start, jy)] = col;
						}
						break;
					}
				}
				else if (boardData[convertCoordinates1D(ind, jy)] == Space.EMPTY)
				{
					//this square MUST be empty.
					break;
				}
				else
				{
					//we continue because it's the other player's color
					continue;
				}
			}
			
			for (int iind = ix - 1, jind = jy + 1; iind >= 0 && jind <= width - 1; iind--, jind++) //up-right check.
			{
				//iind represents i, and changes as we iterate.
				//jind represents j, and changes as we iterate.
				if (boardData[convertCoordinates1D(iind, jind)] == col) // we hit another piece of our color
				{
					if (iind + 1 == ix)
					{
						//this piece was DIRECTLY adjacent to the piece we placed, therefore is invalid move.
						break;
					}
					else
					{
						//there is some pieces between the two pieces.
						isValidMove = true;
						//flip over the pieces.
						for (int starti = ix, startj = jy, end = iind; starti > end; starti--, startj++)
						{
							boardData[convertCoordinates1D(starti, startj)] = col;
						}
						break;
					}
				}
				else if (boardData[convertCoordinates1D(iind, jind)] == Space.EMPTY)
				{
					//this square MUST be empty.
					break;
				}
				else
				{
					//we continue because it's the other player's color
					continue;
				}
			}
			
			for (int ind = jy + 1; ind <= width - 1; ind++) //right check.
			{
				//i coordinate never changes during this loop.
				//ind represents j, and changes as we iterate.
				if (boardData[convertCoordinates1D(ix, ind)] == col) // we hit another piece of our color
				{
					if (ind - 1 == jy)
					{
						//this piece was DIRECTLY adjacent the piece we placed, therefore is invalid move.
						break;
					}
					else
					{
						//there is some pieces between the two pieces.
						isValidMove = true;
						//flip over the pieces.
						for (int start = jy, end = ind; start < end; start++)
						{
							boardData[convertCoordinates1D(ix, start)] = col;
						}
						break;
					}
				}
				else if (boardData[convertCoordinates1D(ix, ind)] == Space.EMPTY)
				{
					//this square MUST be empty.
					break;
				}
				else
				{
					//we continue because it's the other player's color
					continue;
				}
			}
			
			for (int iind = ix + 1, jind = jy + 1; iind <= height - 1 && jind <= width - 1; iind++, jind++) //bottom-right check.
			{
				//iind represents i, and changes as we iterate.
				//jind represents j, and changes as we iterate.
				if (boardData[convertCoordinates1D(iind, jind)] == col) // we hit another piece of our color
				{
					if (iind - 1 == ix)
					{
						//this piece was DIRECTLY adjacent to the piece we placed, therefore is invalid move.
						break;
					}
					else
					{
						//there is some pieces between the two pieces.
						isValidMove = true;
						//flip over the pieces.
						for (int starti = ix, startj = jy, end = iind; starti < end; starti++, startj++)
						{
							boardData[convertCoordinates1D(starti, startj)] = col;
						}
						break;
					}
				}
				else if (boardData[convertCoordinates1D(iind, jind)] == Space.EMPTY)
				{
					//this square MUST be empty.
					break;
				}
				else
				{
					//we continue because it's the other player's color
					continue;
				}
			}
			
			for (int ind = ix + 1; ind <= height - 1; ind++) //downwards check.
			{
				//ind represents i, and changes as we iterate.
				//j coordinate never changes during this loop.
				if (boardData[convertCoordinates1D(ind, jy)] == col) // we hit another piece of our color
				{
					if (ind - 1 == ix)
					{
						//this piece was DIRECTLY adjacent the piece we placed, therefore is invalid move.
						break;
					}
					else
					{
						//there is some pieces between the two pieces.
						isValidMove = true;
						//flip over the pieces.
						for (int start = ix, end = ind; start < end; start++)
						{
							boardData[convertCoordinates1D(start, jy)] = col;
						}
						break;
					}
				}
				else if (boardData[convertCoordinates1D(ind, jy)] == Space.EMPTY)
				{
					//this square MUST be empty.
					break;
				}
				else
				{
					//we continue because it's the other player's color
					continue;
				}
			}
			
			for (int iind = ix + 1, jind = jy - 1; iind <= height - 1 && jind >= 0; iind++, jind--) //bottom-left check.
			{
				//iind represents i, and changes as we iterate.
				//jind represents j, and changes as we iterate.
				if (boardData[convertCoordinates1D(iind, jind)] == col) // we hit another piece of our color
				{
					if (iind - 1 == ix)
					{
						//this piece was DIRECTLY adjacent to the piece we placed, therefore is invalid move.
						break;
					}
					else
					{
						//there is some pieces between the two pieces.
						isValidMove = true;
						//flip over the pieces.
						for (int starti = ix, startj = jy, end = iind; starti < end; starti++, startj--)
						{
							boardData[convertCoordinates1D(starti, startj)] = col;
						}
						break;
					}
				}
				else if (boardData[convertCoordinates1D(iind, jind)] == Space.EMPTY)
				{
					//this square MUST be empty.
					break;
				}
				else
				{
					//we continue because it's the other player's color
					continue;
				}
			}
			
			for (int ind = jy - 1; ind >= 0; ind--) //left check.
			{
				//i coordinate never changes during this loop.
				//ind represents j, and changes as we iterate.
				if (boardData[convertCoordinates1D(ix, ind)] == col) // we hit another piece of our color
				{
					if (ind + 1 == jy)
					{
						//this piece was DIRECTLY adjacent the piece we placed, therefore is invalid move.
						break;
					}
					else
					{
						//there is some pieces between the two pieces.
						isValidMove = true;
						//flip over the pieces.
						for (int start = jy, end = ind; start > end; start--)
						{
							boardData[convertCoordinates1D(ix, start)] = col;
						}
						break;
					}
				}
				else if (boardData[convertCoordinates1D(ix, ind)] == Space.EMPTY)
				{
					//this square MUST be empty.
					break;
				}
				else
				{
					//we continue because it's the other player's color
					continue;
				}
			}
			
			for (int iind = ix - 1, jind = jy - 1; iind >= 0 && jind >= 0; iind--, jind--) //top-left check.
			{
				//iind represents i, and changes as we iterate.
				//jind represents j, and changes as we iterate.
				if (boardData[convertCoordinates1D(iind, jind)] == col) // we hit another piece of our color
				{
					if (iind + 1 == ix)
					{
						//this piece was DIRECTLY adjacent to the piece we placed, therefore is invalid move.
						break;
					}
					else
					{
						//there is some pieces between the two pieces.
						isValidMove = true;
						//flip over the pieces.
						for (int starti = ix, startj = jy, end = iind; starti > end; starti--, startj--)
						{
							boardData[convertCoordinates1D(starti, startj)] = col;
						}
						break;
					}
				}
				else if (boardData[convertCoordinates1D(iind, jind)] == Space.EMPTY)
				{
					//this square MUST be empty.
					break;
				}
				else
				{
					//we continue because it's the other player's color
					continue;
				}
			}
			
			if (isValidMove)
			{
				//*some* pieces must have been flipped so commit the move
				boardData[convertCoordinates1D(ix, jy)] = col;
			}
			
			return isValidMove;
		}
	}
	
	/**
	 * Goes through the entire board and counts the number of white
	 * pieces and the number of black pieces. Used to tally up the pieces
	 * to see who has won.
	 * 
	 * @return the number of each color of pieces on the board,
	 */
	public int[] countPieces()
	{
		//index 0 is white pieces, index 1 is black pieces.
		int[] arr = new int[2];
		for (int i = 0; i < boardData.length; i++)
		{
			if (boardData[i] == Space.WHITE)
			{
				arr[0]++;
			}
			else if (boardData[i] == Space.BLACK)
			{
				arr[1]++;
			}
			else
			{
				//spot must be empty, do nothing.
			}
		}
		return arr;
	}
}