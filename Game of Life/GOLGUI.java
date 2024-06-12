package ui;

import java.awt.BorderLayout;
import java.awt.EventQueue;

import javax.swing.JFrame;
import javax.swing.JPanel;
import javax.swing.border.EmptyBorder;
import javax.swing.JLabel;
import java.awt.Font;
import javax.swing.SwingConstants;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.FocusEvent;
import java.awt.event.FocusListener;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.io.File;
import java.net.URISyntaxException;

import javax.swing.JButton;
import java.awt.Color;
import java.awt.Dimension;

import javax.swing.border.SoftBevelBorder;
import javax.swing.event.ChangeEvent;
import javax.swing.event.ChangeListener;
import javax.swing.border.BevelBorder;
import javax.swing.JSeparator;
import javax.swing.JSlider;
import javax.swing.JTextField;

import java.util.Timer;
import java.util.TimerTask;

public class GOLGUI extends JFrame
{
	private class PVT implements FocusListener
	{
		private String tipText;
		private JTextField jtf;
		
		public PVT(JTextField jtf)
		{
			this.tipText = jtf.getText();
			this.jtf = jtf;
		}

		@Override
		public void focusGained(FocusEvent e)
		{
			if (jtf.getText().equals(tipText))
			{
				jtf.setText("");
				jtf.setForeground(Color.black);
			}		
		}

		@Override
		public void focusLost(FocusEvent e)
		{
			if (jtf.getText().equals(tipText) || jtf.getText().isBlank())
			{
				jtf.setText(tipText);
				jtf.setForeground(Color.gray);
			}		
		}
	}	
	
	static
	{
		System.loadLibrary("GameOfLifeLogic");
		//System.load("C:\\Users\\ubfun\\Desktop\\New Conway\\GameOfLifeLogic.dll");
//		System.load("C:\\Users\\ubfun\\Desktop\\Conway\\Bridge.dll");
//		System.out.println("\n\n\nHASNT ERRORED YET!\n\n");
//		try
//		{
//			System.out.println(System.getProperty("java.library.path"));
//			String temp = new File(GOLGUI.class.getProtectionDomain().getCodeSource().getLocation().toURI()).getPath();
//			temp = temp.substring(0, temp.lastIndexOf("\\"));
////			String temp = "C:\\Users\\ubfun\\Desktop\\Conway";
//			System.load(temp + "\\Bridge.dll");
//			System.load(temp + "\\GOL.dll");
//			System.out.println("Loaded DLL");
//		}
//		catch (URISyntaxException e)
//		{
//			System.out.println("Error in loading the DLLs!");
//			e.printStackTrace(System.out);
//		}
	}

	private static native boolean[] GetSingle();

	private static native void Handshake(int width, int height);

	private static native void SetValue(int pos, boolean value);

	private static native void Step();

	private enum Direction
	{
		UP, DOWN, LEFT, RIGHT;
	}

	private JPanel contentPane;
	private JButton[] buttons;
	private int internalWidth = 40;
	private int internalHeight = 40;
	private int viewWidth = 20;
	private int viewHeight = 20;
	private int topLeftX;
	private int topLeftY;
	private int speedms = 500;
	private JTextField internalWidthTxt;
	private JTextField internalHeightTxt;
	private JTextField viewWidthTxt;
	private JTextField viewHeightTxt;
	private JPanel gamePanel;
	private JButton btnPlayPause;

	/**
	 * Launch the application.
	 */
	public static void main(String[] args)
	{
		EventQueue.invokeLater(new Runnable()
		{
			public void run()
			{
				try
				{
					GOLGUI frame = new GOLGUI();
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
	 * Create the frame.
	 */
	public GOLGUI()
	{
//		System.out.println("\n\n\nENTERED MAIN\n\n");
//		GetSingle();
//		System.out.println("\n\n\nCALLED GETSINGLE\n\n");

		//Handshake(internalWidth, internalHeight);
		this.setTitle("Conway's Game of Life");
		this.setResizable(false);
		setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		setBounds(100, 100, 600, 550);
		contentPane = new JPanel();
		contentPane.setBorder(new EmptyBorder(5, 5, 5, 5));
		contentPane.setLayout(new BorderLayout(0, 0));
		setContentPane(contentPane);

		JLabel lblGameTitle = new JLabel("Conway's Game of Life");
		lblGameTitle.setHorizontalAlignment(SwingConstants.CENTER);
		lblGameTitle.setFont(new Font("Tahoma", Font.PLAIN, 32));
		contentPane.add(lblGameTitle, BorderLayout.NORTH);

		gamePanel = new JPanel();
		contentPane.add(gamePanel, BorderLayout.CENTER);
		gamePanel.setLayout(new GridLayout(1, 1, 0, 0));

		JPanel inputPanel = new JPanel();
		inputPanel.setBorder(new EmptyBorder(10, 10, 10, 10));
		inputPanel.setBackground(Color.GRAY);
		contentPane.add(inputPanel, BorderLayout.WEST);
		inputPanel.setLayout(new GridLayout(4, 1, 0, 0));
		
		//===================================
		//Time panel stuff
		
		{
			JPanel timePanel = new JPanel();
			timePanel.setOpaque(false);
			inputPanel.add(timePanel);
			timePanel.setLayout(new BorderLayout(0, 0));
			
			JLabel lblTime = new JLabel("Time Controls");
			lblTime.setHorizontalAlignment(SwingConstants.CENTER);
			lblTime.setFont(new Font("Tahoma", Font.PLAIN, 18));
			timePanel.add(lblTime, BorderLayout.NORTH);
			
			JPanel inputTimeButtons = new JPanel();
			inputTimeButtons.setOpaque(false);
			timePanel.add(inputTimeButtons, BorderLayout.CENTER);
			inputTimeButtons.setLayout(new BorderLayout(0, 0));
			
			Timer timer = new Timer();
			
			btnPlayPause = new JButton(">");
			btnPlayPause.addActionListener(new ActionListener()
			{
				@Override
				public void actionPerformed(ActionEvent e)
				{
					if (btnPlayPause.getText() == ">")
					{
						btnPlayPause.setText("||");
						timer.schedule(createTimerTask(timer), (long)speedms);
					}
					else
					{
						btnPlayPause.setText(">");
					}
				}
			});
			btnPlayPause.setFont(new Font("Tahoma", Font.PLAIN, 11));
			inputTimeButtons.add(btnPlayPause, BorderLayout.WEST);
			
			JButton btnStep = new JButton(">|");
			btnStep.addActionListener(new ActionListener()
			{
				@Override
				public void actionPerformed(ActionEvent e)
				{
					if (btnPlayPause.getText() == ">")
					{
						step();
					}
				}
			});
			inputTimeButtons.add(btnStep, BorderLayout.EAST);
			
			JLabel lblSpeed = new JLabel("0.25/s");
			lblSpeed.setHorizontalAlignment(SwingConstants.CENTER);
			lblSpeed.setFont(new Font("Tahoma", Font.PLAIN, 12));
			inputTimeButtons.add(lblSpeed, BorderLayout.CENTER);
			
			JSlider slider = new JSlider();
			slider.setValue(1);
			slider.setPreferredSize(new Dimension(120, 11));
			slider.setMinimum(1);
			slider.setMaximum(16);
			slider.addChangeListener(new ChangeListener()
			{
				@Override
				public void stateChanged(ChangeEvent e)
				{
					speedms = (int)(4000f / (float)slider.getValue());
					lblSpeed.setText(String.format("%.2f" , (float)slider.getValue() / 4f) + "/s");
				}
			});
			inputTimeButtons.add(slider, BorderLayout.NORTH);
		}
		
		//==================================
		//Scroll panel stuff
		
		{
			JPanel scrollPanel = new JPanel();
			scrollPanel.setOpaque(false);
			inputPanel.add(scrollPanel);
			scrollPanel.setLayout(new BorderLayout(0, 0));
			
			JLabel lblScroll = new JLabel("Scroll Controls");
			lblScroll.setHorizontalAlignment(SwingConstants.CENTER);
			lblScroll.setFont(new Font("Tahoma", Font.PLAIN, 18));
			scrollPanel.add(lblScroll, BorderLayout.NORTH);
			
			JPanel inputScrollPanel = new JPanel();
			inputScrollPanel.setOpaque(false);
			scrollPanel.add(inputScrollPanel);
			inputScrollPanel.setLayout(new BorderLayout(0, 0));
			
			JButton btnUp = new JButton("^");
			btnUp.addActionListener(new ActionListener()
			{
				@Override
				public void actionPerformed(ActionEvent e)
				{
					scroll(Direction.UP);
				}		
			});
			inputScrollPanel.add(btnUp, BorderLayout.NORTH);
			
			JButton btnDown = new JButton("v");
			btnDown.addActionListener(new ActionListener()
			{
				@Override
				public void actionPerformed(ActionEvent e)
				{
					scroll(Direction.DOWN);
				}		
			});
			inputScrollPanel.add(btnDown, BorderLayout.SOUTH);
			
			JButton btnLeft = new JButton("<");
			btnLeft.addActionListener(new ActionListener()
			{
				@Override
				public void actionPerformed(ActionEvent e)
				{
					scroll(Direction.LEFT);
				}		
			});
			inputScrollPanel.add(btnLeft, BorderLayout.WEST);
			
			JButton btnRight = new JButton(">");
			btnRight.addActionListener(new ActionListener()
			{
				@Override
				public void actionPerformed(ActionEvent e)
				{
					scroll(Direction.RIGHT);
				}		
			});
			inputScrollPanel.add(btnRight, BorderLayout.EAST);
		}
		
		//=====================================
		//Board Panel stuff
		
		{
			JPanel boardPanel = new JPanel();
			boardPanel.setOpaque(false);
			inputPanel.add(boardPanel);
			boardPanel.setLayout(new BorderLayout(0, 0));
			
			JLabel lblBoard = new JLabel("Board Controls");
			lblBoard.setHorizontalAlignment(SwingConstants.CENTER);
			lblBoard.setFont(new Font("Tahoma", Font.PLAIN, 18));
			boardPanel.add(lblBoard, BorderLayout.NORTH);
			
			JPanel inputBoardButtons = new JPanel();
			inputBoardButtons.setOpaque(false);
			boardPanel.add(inputBoardButtons, BorderLayout.CENTER);
			inputBoardButtons.setLayout(new GridLayout(3, 2, 0, 0));
			
			internalWidthTxt = new JTextField();
			internalWidthTxt.setForeground(Color.GRAY);
			internalWidthTxt.setText("Game Width");
			inputBoardButtons.add(internalWidthTxt);
			internalWidthTxt.setColumns(3);
			internalWidthTxt.addFocusListener(new PVT(internalWidthTxt));
			
			internalHeightTxt = new JTextField();
			internalHeightTxt.setText("Game Height");
			internalHeightTxt.setForeground(Color.GRAY);
			inputBoardButtons.add(internalHeightTxt);
			internalHeightTxt.setColumns(3);
			internalHeightTxt.addFocusListener(new PVT(internalHeightTxt));
			
			viewWidthTxt = new JTextField();
			viewWidthTxt.setText("View Width");
			viewWidthTxt.setForeground(Color.GRAY);
			inputBoardButtons.add(viewWidthTxt);
			viewWidthTxt.setColumns(3);
			viewWidthTxt.addFocusListener(new PVT(viewWidthTxt));
			
			viewHeightTxt = new JTextField();
			viewHeightTxt.setForeground(Color.GRAY);
			viewHeightTxt.setText("View Height");
			inputBoardButtons.add(viewHeightTxt);
			viewHeightTxt.setColumns(3);
			viewHeightTxt.addFocusListener(new PVT(viewHeightTxt));
			
			JButton btnReset = new JButton("Reset");
			btnReset.setToolTipText("Sets all cells on the board to black.");
			btnReset.addActionListener(new ActionListener()
			{
				@Override
				public void actionPerformed(ActionEvent e)
				{
					for (int i = 0; i < internalWidth * internalHeight; i++)
					{
						SetValue(i, false);
					}
					draw();
				}
			});
			inputBoardButtons.add(btnReset);
			
			JButton btnConfirm = new JButton("Confirm");
			btnConfirm.addActionListener(new ActionListener()
			{
				@Override
				public void actionPerformed(ActionEvent e)
				{
					int vw = viewWidth, vh = viewHeight, gw = internalWidth, gh = internalHeight;
					try
					{
						vw = Integer.parseInt(viewWidthTxt.getText());
					}
					catch (NumberFormatException ee)
					{
						// do nothing
					}
					try 
					{
						vh = Integer.parseInt(viewHeightTxt.getText());
					}
					catch (NumberFormatException ee)
					{
						// do nothing
					}
					try 
					{
						gw = Integer.parseInt(internalWidthTxt.getText());
					}
					catch (NumberFormatException ee)
					{
						// do nothing
					}
					try 
					{
						gh = Integer.parseInt(internalHeightTxt.getText());
					}
					catch (NumberFormatException ee)
					{
						// do nothing
					}
					gw = gw < vw ? vw : gw;
					gh = gh < vh ? vh : gh;
					vw = vw < 5 ? 5 : vw;
					vh = vh < 5 ? 5 : vh;
					gw = gw < 5 ? 5 : gw;
					gh = gh < 5 ? 5 : gh;
					setup(vw, vh, gw, gh);
				}
			});
			inputBoardButtons.add(btnConfirm);
		}
		setup(15, 15, 30, 30);
	}

	private void setup(int vw, int vh, int gw, int gh)
	{
		if (gw < vw || gh < vh)
		{
			throw new IllegalArgumentException("Viewport size cannot be larger than the game grid in either dimenstion.");
		}
		
		viewWidth = vw;
		viewHeight = vh;
		internalWidth = gw;
		internalHeight = gh;
		topLeftX = (internalWidth / 2) - (viewWidth / 2);
		topLeftY = (internalHeight / 2) - (viewHeight / 2);
		
		Handshake(gw, gh);			
//		try 
//		{
//			System.out.println("About to handshake.");
//		}
//		catch (Exception e)
//		{
//			System.out.println("Exception thrown!");
//			e.printStackTrace(System.out);
//		}
		
		if (buttons != null)
		{
			for (int i = 0; i < buttons.length; i++)
			{
				gamePanel.remove(buttons[i]);
			}
		}
		
		buttons = new JButton[viewWidth * viewHeight];
		gamePanel.setLayout(new GridLayout(viewHeight, viewWidth, 0, 0));
		for (int i = 0; i < viewWidth * viewHeight; i++)
		{
			buttons[i] = new JButton();
			buttons[i].setBackground(Color.black);
			final int ii = i;
			buttons[i].addActionListener(new ActionListener()
			{
				private int c = ii;

				@Override
				public void actionPerformed(ActionEvent e)
				{
					if (btnPlayPause.getText() == "||")
					{
						return;
					}
					if (buttons[c].getBackground() != Color.white)
					{
						buttons[c].setBackground(Color.white);
						setData(c, true);
					}
					else
					{
						int x, y;
						if (false) //todo: make the edge of the game grid darkGrey
							buttons[c].setBackground(Color.darkGray);
						else
							buttons[c].setBackground(Color.black);
						setData(c, false);
					}
				}
			});
			gamePanel.add(buttons[i]);
		}
		gamePanel.revalidate();
		gamePanel.repaint();
	}
	
	private void scroll(Direction d)
	{
		switch(d)
		{
		case UP:
			topLeftY -= topLeftY > 0 ? 1 : 0;
			break;
		case DOWN:
			topLeftY += topLeftY + viewHeight < internalHeight ? 1 : 0;
			break;
		case LEFT:
			topLeftX -= topLeftX > 0 ? 1 : 0;
			break;
		case RIGHT:
			topLeftX += topLeftX + viewWidth < internalWidth ? 1 : 0;
			break;
		}
		draw();
	}
	
	private void step()
	{
		// call single iteration of GOL
		Step();
		draw();
		int nextCall = (int)(1000f / (float)speedms);
	}
	
	private void draw()
	{
		boolean[] arr = GetSingle();
		for (int i = 0; i < viewWidth; i++)
		{
			for (int j = 0; j < viewHeight; j++)
			{
				int viewOneD = (j * viewWidth) + i;
				int internalX = topLeftX + i;
				int internalY = topLeftY + j;
				int internalOneD = (internalY * internalWidth) + internalX;
				boolean cell = arr[internalOneD];
				buttons[viewOneD].setBackground(cell ? Color.white : Color.black);
			}
		}
	}

	private void setData(int v, boolean isWhite)
	{
		// change the cell data at a given location
		//transform v to internal
		int viewX = v % viewWidth;
		int viewY = (v - viewX) / viewHeight;
		int internalX = viewX + topLeftX;
		int internalY = viewY + topLeftY;
		int internal = (internalY * internalWidth) + internalX;
		SetValue(internal, isWhite);
	}
	
	private TimerTask createTimerTask(Timer timer)
	{
		return new TimerTask()		
		{
			@Override
			public void run() 
			{
				if (btnPlayPause.getText() == "||")
					timer.schedule(createTimerTask(timer), (long)speedms);
				step();
			}
		};
	}
}