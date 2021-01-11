using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private List<IWeapon> _weaponStorage;
    private int _selectedWeapon;
    //private HelthIndicatorScript _helthIndicator;

    public int Helth { get; private set; }

    void Awake()
    {
        Helth = 100;
        _selectedWeapon = 0;
        _weaponStorage = new List<IWeapon>();
        //_weaponStorage.Add(new ArmHit());

        //_helthIndicator = FindObjectOfType<HelthIndicatorScript>();
    }

    private void Start()
    {
        //if (null != _helthIndicator)
        //    _helthIndicator.Refresh(Helth);
    }

    private void Update()
    {
    }

    public void ReactToHit(int hitAccount)
    {
        Helth -= hitAccount;
        //_helthIndicator.Refresh(Helth);

        Debug.Log("Player hit. " + Helth.ToString());
    }

    private void AddWeapon(IWeapon weapon)
    {
        IWeapon storageWeapon = 
            _weaponStorage.Find((IWeapon item) => weapon.Name == item.Name);

        if (null != storageWeapon)
            storageWeapon.AddCharge(weapon.Charge);
        else
            _weaponStorage.Add(weapon);
    }

    private IWeapon GetNextWeapon()
    {
        _selectedWeapon = (_selectedWeapon + 1) % _weaponStorage.Count;
        return _weaponStorage[_selectedWeapon];
    }

    public void AddHealth(int count)
    {
        Helth = Mathf.Max(100, Helth + count);
        //_helthIndicator.Refresh(Helth);

        Debug.Log("Player helth. " + Helth.ToString());
    }
}
