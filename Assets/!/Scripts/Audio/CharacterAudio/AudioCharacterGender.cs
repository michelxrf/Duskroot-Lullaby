using UnityEngine;
public enum CharacterGender { Woman, Man, Girl, Boy }
public static class AudioCharacterGender
{
    public static CharacterGender GetGender(this PlayerSetup setup)
    {
        if (setup == null)
        {
            //Debug.LogWarning("AudioCharacterGender: PlayerSetup enviado é nulo!");
            return CharacterGender.Boy;
        }

        return setup.characterId switch
        {
            "Mage" => CharacterGender.Boy,
            "Ranger" => CharacterGender.Girl,
            "Tank" => CharacterGender.Boy,
            "Warrior" => CharacterGender.Boy,
            _ => CharacterGender.Boy
        };
    }
}
