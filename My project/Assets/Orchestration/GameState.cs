public class GameState
{
    // for storing cross level stuff
    // store stuff that needs to be memorized cross level here
    public static GameState Instance = new GameState();
    public int LevelsCompleted;
    public bool IsCat; // true = cat is on Track B, false = cat is on Track A
    public bool cageDropped = false;
}
