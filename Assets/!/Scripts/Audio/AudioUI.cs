using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AudioUI : MonoBehaviour
{
    public static AudioUI instance;
    [Header("FMOD")]
    [SerializeField] private EventReference UI_Weapon;
    [SerializeField] private EventReference UI_Food;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
    }
    public void PlayUIWeapon()
    {
        EventInstance instanceUIWeapon = RuntimeManager.CreateInstance(UI_Weapon);
        instanceUIWeapon.start();
        instanceUIWeapon.release();
    }

    public void PlayUIFood() 
    {
        EventInstance instanceUIFood = RuntimeManager.CreateInstance(UI_Food);
        instanceUIFood.start();
        instanceUIFood.release();
    }

}
