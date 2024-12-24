namespace ms_db;

public class Validations
{
    public bool CheckIfFieldIsEmpty(int field)
    {
        return field == 0 ? true : false;
    }

    public bool CheckIfFieldIsEmpty(string? field)
    {
        return field == null ? true: false;;
    }
    
}