
[System.Serializable]
public class SaveState
{
    /// <summary>
    ///  Last beaten level
    /// </summary>
    public int LevelProgress;
    
    public SaveState(int levelProgress)
    {
        LevelProgress = levelProgress;
    }
    
    static public SaveState Defaults()
    {
        return new SaveState(
            levelProgress: 0
        );
    }

    public SaveState CopyWith(int? levelProgress = null)
    {
        return new SaveState(
            levelProgress: levelProgress ?? LevelProgress
        );
    }
}
