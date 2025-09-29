namespace Domain.Model.Generic;

public class TaxNumber
{
    private string _value;
    public string Value { get => _value; set => _value = value; }

    //EF Core
    private TaxNumber() { }

    public TaxNumber(string value)
    {
        _value = value;
    }
}