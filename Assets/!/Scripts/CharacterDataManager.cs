using System;
using Unity.VisualScripting;
using UnityEngine;
using ProgressionSystem;


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
    public string localPlayerCharacterId = string.Empty;

    public Action OnLevelUp;
    public Action<int> OnExpChanged;

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
            Data = new PlayerCharactersData(_charactersTemplates);
            PlayFabCharacterSave.Save(Data);
        }
        else
            Data = loadedData;

        _isDirty = false;
    }

    public void SetLocalCharacterId(string characterId)
    {
        localPlayerCharacterId = characterId;
    }

    public CharacterData GetCurrentPlayerCharacter()
    {
        return GetCharacter(localPlayerCharacterId);
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
            if (character.characterId == characterId)
                return character;
        }
        return null;
    }

    public void AddExperience(int xp)
    {
        var character = GetCurrentPlayerCharacter();

        character.experience += xp / RunnerBootstrap.Instance.Runner.SessionInfo.PlayerCount;
        if(character.experience >= character.experienceToNextLevel)
        {
            ExperienceCalculator.LevelUpCharacter(character, character.level + 1);
            Data.Characters[Array.FindIndex(Data.Characters, c => c.characterId == character.characterId)] = character;
            OnLevelUp?.Invoke();
        }

        OnExpChanged?.Invoke(character.experience);

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

    public Sprite GetCharacterPortrait(string character)
    {
        int index = Array.FindIndex(_charactersTemplates, template => template.CharacterId == character);
        return _charactersTemplates[index].characterPortrait;
    }
}