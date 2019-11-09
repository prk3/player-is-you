
/**
 * This is the biggest antipattern in software development,
 * but unity doesn't provide any way of passing data to Scenes.
 */
public class GameStore
{
    public static int Level = 0;

    /**
     * This level should be selected when entering LevelSelectMenu.
     * Use -1 to select last unlocked level.
     */
    public static int SelectedLevel = -1;
}