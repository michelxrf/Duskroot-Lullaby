using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Singleton used for tracking user data along the game. This includes character progression, inventory, and other character related data.
/// It also handles saving and loading this data from both local storage and PlayFab cloud save.
/// </summary>
public class CharacterDataManager : MonoBehaviour
{
    public static CharacterDataManager Instance { get; private set; }
    public PlayerCharactersData Data { get; private set; }

    private bool _isDirty;
    [SerializeField] CharacterTemplate[] _charactersTemplates;

    // placeholder for level up threshold, should be replaced by a more complex system that can be configured per character and level
    int currentLevelThreshold = 100;

    private void Awake()
    {
        // Singleton initialization
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        SaveIfDirty();
    }

    /// <summary>
    /// Initializes the player character data
    /// </summary>
    /// <param name="loadedData"></param>
    public void Initialize(PlayerCharactersData loadedData)
    {
        if (loadedData == null)
        {
            Data = PlayerCharactersData.CreateDefault(_charactersTemplates);
            PlayFabCharacterSave.Save(Data);
        }
        else
            Data = loadedData;

        _isDirty = false;
    }

    /// <summary>
    /// Find and return character data by index
    /// </summary>
    public CharacterData GetCharacter(int index)
    {
        return Data.Characters[index];
    }

    /// <summary>
    /// Find and return character data by characterId
    /// </summary>
    public CharacterData GetCharacter(string characterId)
    {
        foreach (var character in Data.Characters)
        {
            if (character.CharacterId == characterId)
                return character;
        }
        return null;
    }

    public void AddExperience(int index, int xp)
    {
        var character = Data.Characters[index];

        character.Experience += xp;

        while (character.Experience >= currentLevelThreshold)
        {
            character.Experience -= currentLevelThreshold;
            character.Level++;
            currentLevelThreshold *= 2; // Example: each level requires double the XP of the previous level
        }

        _isDirty = true;
    }

    public void AddItem(int index, string itemId)
    {
        // TODO:
        // verify if item already exists then increment

        _isDirty = true;
    }

    public void RemoveItem(int index, string itemId)
    {
        // TODO:
        // verify if item exists then decrement or remove

        _isDirty = true;
    }

    public void SaveIfDirty()
    {
        if (!_isDirty)
            return;

        LocalCharacterSave.Save(Data);
        PlayFabCharacterSave.Save(Data);

        _isDirty = false;
    }
}