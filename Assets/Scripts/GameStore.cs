
/**
 * A collection of statics/globals shared across game scenes.
 * This is the biggest antipattern in software development world,
 * but unity doesn't provide any way of passing data to Scenes, so ¯\_(ツ)_/¯
 */
public class GameStore
{
    /**
     * Level to be opened in Gameplay scene.
     */
    public static int Level = 0;

    /**
     * This level should be selected when entering LevelSelectMenu.
     * Use -1 to select last unlocked level.
     */
    public static int SelectedLevel = -1;
}
