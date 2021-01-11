internal interface IWeapon
{
    string Name { get; }
    int Charge { get; }

    void AddCharge(int charge);
}