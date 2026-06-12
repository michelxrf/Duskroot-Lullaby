using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Analytics;
using static AudioCharacterGender;


public class AudioPunchPlayer : MonoBehaviour
{
    public enum Weapon { Melee, Sword, Slingshot, Hammer, Staff }

    [Header("Person")]
    public Weapon weapon;
    public bool isBear = false;

    [Header("FMOD")]
    [SerializeField] private EventReference punchEvent;
    [SerializeField] private EventReference swordEvent;
    [SerializeField] private EventReference slingEvent;
    [SerializeField] private EventReference hammerEvent;
    [SerializeField] private EventReference staffEvent;
    private PlayerSetup playerSetup;
    private CharacterGender gender;
    private void Start()
    {   if (!isBear)
        {
            playerSetup = GetComponent<PlayerSetup>();
            gender = playerSetup.GetGender();
        } else
        {
            gender = CharacterGender.Man;
        }
    }
    public void PlayPunch()
    {
        GetWeapon();
        if(playerSetup!=null)
           gender = playerSetup.GetGender();
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
            case Weapon.Hammer:
                selectedEvent = hammerEvent;
                break;
            case Weapon.Staff:
                selectedEvent = staffEvent;
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
            case "LongSword":
                weapon = Weapon.Sword;
                break;
            case "Sword":
                weapon = Weapon.Sword;
                break;
            case "Hammer":
                weapon = Weapon.Hammer;
                break;
            case "Staff":
                weapon = Weapon.Staff;
                break;

            default:
                weapon = Weapon.Melee;
                break;

        }
    }

}
