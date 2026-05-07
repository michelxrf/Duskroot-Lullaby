using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Analytics;
using static AudioCharacterGender;


public class AudioPunchPlayer : MonoBehaviour
{
    public enum Weapon { Melee, Sword, Slingshot }

    [Header("Person")]
    public Weapon weapon;

    [Header("FMOD")]
    [SerializeField] private EventReference punchEvent;
    [SerializeField] private EventReference swordEvent;
    [SerializeField] private EventReference slingEvent;

    private PlayerSetup playerSetup;
    private CharacterGender gender;
    private void Start()
    {
        playerSetup = GetComponent<PlayerSetup>();
        gender = playerSetup.GetGender();
    }
    public void PlayPunch()
    {
        GetWeapon();
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

            default:
                weapon = Weapon.Melee;
                break;

        }
    }

}
