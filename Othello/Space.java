package othello;

/**
 * Stores the different colors the board can hold at a time.
 * 
 * @author Sam Gardner
 *
 */
public enum Space
{
	EMPTY(0),
	WHITE(1),
	BLACK(2);
	
	private int value;
	
	private Space(int value)
	{
		this.value = value;
	}
	
	public int getValue()
	{
		return value;
	}
}