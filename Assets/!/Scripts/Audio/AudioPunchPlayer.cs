using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class AudioPunchPlayer : MonoBehaviour
{
    public enum Gender { Woman, Man, Girl, Boy }
    public enum Weapon { Melee, Sword, Slingshot }

    [Header("Person")]
    public Gender gender;
    public Weapon weapon;

    [Header("FMOD")]
    [SerializeField] private EventReference punchEvent;
    [SerializeField] private EventReference swordEvent;
    [SerializeField] private EventReference slingEvent;

    private PlayerSetup playerSetup;

    private void Start()
    {
        playerSetup = GetComponent<PlayerSetup>();
        GetGender();

    }
    public void PlayPunch()
    {
        GetWeapon();
        EventReference selectedEvent = punchEvent;
        switch (weapon)
        {
            case Weapon.Melee:
                selectedEvent = punchEvent;
                break;

            case Weapon.Sword:
                selectedEvent = swordEvent;
                break;

            case Weapon.Slingshot:
                selectedEvent = slingEvent;
                break;

            default:
                selectedEvent = punchEvent;
                break;
        }

        EventInstance punchInstance = RuntimeManager.CreateInstance(selectedEvent);
        punchInstance.setParameterByName("Gen", (float)gender);
        punchInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        punchInstance.start();
        punchInstance.release();
    }

    private void GetWeapon()
    {
        if (playerSetup == null) return;
        switch (playerSetup.currentWeapon)
        {
            case "Unarmed":
                weapon = Weapon.Melee;
                break;
            case "Sling":
                weapon = Weapon.Slingshot;
                break;
            case "Sword":
                weapon = Weapon.Sword;
                break;

            default:
                weapon = Weapon.Melee;
                break;

        }
    }

    private void GetGender()
    {
        if (playerSetup == null) return;
        switch (playerSetup.characterId)
        {
            case "Mage":
                gender = Gender.Boy;
                break;
            case "Ranger":
                gender = Gender.Girl;
                break;
            case "Tank":
                gender = Gender.Boy;
                break;
            case "Warrior":
                gender = Gender.Boy;
                break;

            default:
                gender = Gender.Boy;
                break;

        }
    }


}
