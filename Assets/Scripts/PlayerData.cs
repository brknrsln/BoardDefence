public class PlayerData
{
    private static PlayerData _instance;
        
    public static PlayerData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerData();
                _instance.Load();
            }
            return _instance;
        }
    }
        
    public int Level { get; private set; } = 1;

    private void Save()
    {
        UnityEngine.PlayerPrefs.SetInt("PlayerLevel", Level);
        UnityEngine.PlayerPrefs.Save();
    }

    private void Load()
    {
        Level = UnityEngine.PlayerPrefs.GetInt("PlayerLevel", 1);
    }
        
    public void NextLevel()
    {
        Level++;
        Save();
    }
}