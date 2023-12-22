package othello;

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.EventQueue;

import javax.swing.ImageIcon;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JPanel;
import javax.swing.border.EmptyBorder;

import java.awt.GridLayout;
import java.awt.Image;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.SwingConstants;
import java.awt.Font;

/**
 * Othello board game GUI (main class). 
 * 
 * @author Sam Gardner & Glenna Williams
 *
 */
public class OthelloGUI extends JFrame
{

	private JPanel contentPane = new JPanel();
	private JPanel gamePane = new JPanel();
	private final JPanel inputPane = new JPanel();
	private final JButton btnSkipTurn = new JButton("Skip Turn");
	private final JButton btnSubmit = new JButton("Submit");
	private final JLabel lblTurn = new JLabel("White's Turn");
	private Board board = new Board();
	//stores the position of the most recently selected spot on the board
	private int i = -1;
	private int j = -1;
	JButton[][] gameBtns = new JButton[8][];
	// if true white's turn if false black's turn
	private static boolean turnToggle = false;
	private static int skipCounter = 0;
	private static ImageIcon whitePiece;
	private static ImageIcon blackPiece;
	private JLabel lblTitle = new JLabel("Othello");

	/**
	 * Launch the application.
	 */
	public static void main(String[] args)
	{
		whitePiece = new ImageIcon(new ImageIcon(OthelloGUI.class.getResource("/othello/images/White Piece.png"))
				.getImage().getScaledInstance(64, 64, Image.SCALE_SMOOTH));
		blackPiece = new ImageIcon(new ImageIcon(OthelloGUI.class.getResource("/othello/images/Black Piece.png"))
				.getImage().getScaledInstance(64, 64, Image.SCALE_SMOOTH));
		EventQueue.invokeLater(new Runnable()
		{
			public void run()
			{
				try
				{
					OthelloGUI frame = new OthelloGUI();
					frame.setVisible(true);
				}
				catch (Exception e)
				{
					e.printStackTrace();
				}
			}
		});

	}

	/**
	 * Constructor for the Othello GUI, creates the frame for the game.
	 */
	public OthelloGUI()
	{
		this.setResizable(false);
		createContentPane();
		gamePane.setLayout(new GridLayout(8, 8, 0, 0));
		createLabelTitle();
		createInputPane();
		addButtonFunctionality();
		createLabelTurn();
		createOthelloButtons();
		setTurn(turnToggle);
		paint();
	}

	/**
	 * Creates a container for user input buttons Skip Turn and Submit.
	 */
	private void createInputPane() {
		inputPane.setLayout(new BorderLayout(0, 0));
		inputPane.add(btnSkipTurn, BorderLayout.WEST);
		inputPane.add(btnSubmit, BorderLayout.EAST);
		inputPane.add(lblTurn, BorderLayout.CENTER);
	}

	/**
	 * Creates all the green tiles on the board. Adds the event listeners to the buttons
	 * where the text is set to pending and the "most recently pressed button" coordinates
	 * are updated to this button. 
	 */
	private void createOthelloButtons() {
		for (int x = 0; x < 8; x++)
		{
			gameBtns[x] = new JButton[8];
			for (int y = 0; y < 8; y++)
			{
				final int xx = x;
				final int yy = y;
				gameBtns[x][y] = new JButton("");
				gameBtns[x][y].setFont(new Font("Tahoma", Font.PLAIN, 10));
				gameBtns[x][y].setBackground(Color.green);
				gameBtns[x][y].addActionListener(new ActionListener()
				{
					//save this buttons coordinates in the anonymous class definition
					int ii = xx, jj = yy;

					@Override
					public void actionPerformed(ActionEvent e)
					{
						if (i != -1)
						{
							gameBtns[i][j].setText("");							
						}
						//set the "most recently pressed button" to be this button 
						i = ii;
						j = jj;
						if(gameBtns[i][j].getIcon() == null) {
							gameBtns[i][j].setText("Pending");	
						}
					}
				});
				gamePane.add(gameBtns[x][y]);
			}

		}
	}

	/**
	 * Styles the label that displays which player's turn it is. 
	 */
	private void createLabelTurn() {
		lblTurn.setOpaque(true);
		lblTurn.setHorizontalAlignment(SwingConstants.CENTER);
	}

	/**
	 * Adds event listeners to the Skip Turn button and Submit button 
	 * by calling on the corresponding methods. 
	 */
	private void addButtonFunctionality() {
		btnSkipTurn.addActionListener(new ActionListener()
		{
			@Override
			public void actionPerformed(ActionEvent e)
			{
				skip();
			}

		});
		btnSubmit.addActionListener(new ActionListener()
		{
			@Override
			public void actionPerformed(ActionEvent e)
			{
				submit();
			}

		});
	}

	/**
	 * Creates the content pane which acts as a container for all 
	 * the components of the Othello game.
	 */
	private void createContentPane() {
		setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		setBounds(100, 100, 650, 700);
		contentPane.setBorder(new EmptyBorder(5, 5, 5, 5));
		setContentPane(contentPane);
		contentPane.setLayout(new BorderLayout(0, 0));
		contentPane.add(lblTitle, BorderLayout.NORTH);
		contentPane.add(inputPane, BorderLayout.SOUTH);
		contentPane.add(gamePane);
	}

	/**
	 * Styles the title for the game Othello.
	 */
	private void createLabelTitle() {
		lblTitle.setFont(new Font("Tahoma", Font.PLAIN, 30));
		lblTitle.setHorizontalAlignment(SwingConstants.CENTER);
	}

	/**
	 * First checks if a spot has been selected on board and if the move is valid the move is committed and
	 * the turn is over. If the move is invalid a dialog box will notify player the move is invalid and to try
	 * again.  If the board is full or the white/black player has no pieces left, the
	 * gameOver() method is called.
	 */
	private void submit()
	{
		if (i != -1)
		{
			boolean valid = board.submitMove(i, j, turnToggle);
			// if move valid, other persons turn, if invalid still your turn
			if (valid)
			{
				gameBtns[i][j].setText("");
				skipCounter = 0;
				paint();
				turnToggle = !turnToggle;
				setTurn(turnToggle);
				int[] arr = board.countPieces();
				if (arr[0] + arr[1] == 64 || arr[0] == 0 || arr[1] == 0)
				{
					// board is full or either of player runs out of pieces. The game is over.
					gameOver();
				}
				i = j = -1;
			}
			else
			{
				JOptionPane.showMessageDialog(null, "Invalid Move!");
			}
		}
	}

	/**
	 * Increments skip counter to make other opponents turn. Checks if both players skip
	 * their turn 2 times in a row, if both do the game is over.
	 */
	private void skip()
	{
		turnToggle = !turnToggle;
		skipCounter++;
		if (skipCounter >= 2)
		{
			// if both players skip their turn consecutively, assume there are no valid moves and the game ends
			gameOver();
		}
		setTurn(turnToggle);
	}

	/**
	 * Checks the winner when game is over.  Gives an option dialog to play again which will
	 * make a new board and set it to blacks turn.  If quit is selected then the game will exit.
	 */
	private void gameOver()
	{
		int[] arr = board.countPieces();
		String winner = "";
		if (arr[0] > arr[1])
		{
			// white won.
			winner = "White won!\nWhite had " + arr[0] + " pieces, and black had " + arr[1] + " pieces.";
		}
		else if (arr[0] < arr[1])
		{
			// black won.
			winner = "Black won!\nBlack had " + arr[1] + " pieces, and white had " + arr[0] + " pieces.";
		}
		else
		{
			// tie.
			winner = "It was a tie...";
		}
		if (JOptionPane.showOptionDialog(null, winner, "Game over", JOptionPane.YES_NO_OPTION,
				JOptionPane.QUESTION_MESSAGE, null, new String[]
				{ "Play again", "Quit" }, null) == 0)
		{
			// play again
			board = new Board();
			turnToggle = false;
			setTurn(turnToggle);
			paint();
		}
		else
		{
			// quit.
			System.exit(0);
		}
	}

	/**
	 * Loops through all the spots in the board and gets the spot, if the space is white 
	 * the white piece will be placed on the button, if the space is black a black piece will be
	 * placed on the button, if neither then the spot is set to empty.
	 */
	private void paint()
	{
		for (int x = 0; x < 8; x++)
		{
			for (int y = 0; y < 8; y++)
			{
				if (Board.getSpot(board, x, y) == Space.WHITE)
				{
					gameBtns[x][y].setIcon(whitePiece);
				}
				else if (Board.getSpot(board, x, y) == Space.BLACK)
				{
					gameBtns[x][y].setIcon(blackPiece);
				}
				else
				{
					gameBtns[x][y].setIcon(null);					
				}
			}
		}
	}

	/**
	 * Sets the text for whose turn it is on the label Turn.  If white's turn the 
	 * text color changes to black and the background changes to white.  If black's turn the 
	 * text changes to white and the background changes to black. 
	 * 
	 * @param isWhite true if it is white's turn, false if it is black's turn
	 */
	private void setTurn(boolean isWhite)
	{
		if (isWhite)
		{
			lblTurn.setText("White\'s Turn");
			lblTurn.setForeground(Color.BLACK);
			lblTurn.setBackground(Color.WHITE);
		}
		else
		{
			lblTurn.setText("Black\'s Turn");
			lblTurn.setForeground(Color.WHITE);
			lblTurn.setBackground(Color.BLACK);
		}
	}
}