using API.Models;
namespace API;

public class Validations
{
    public ValidationDisplay ValidationDisplayObj { get; set; }

    public Validations()
    {
        ValidationDisplayObj = new ValidationDisplay()
        {
            IsValid = true
        };

    }

    private void resetValidationDisplay()
    {
        ValidationDisplayObj = new ValidationDisplay()
        {
            IsValid = true
        };
    }
    public ValidationDisplay CheckIdValidations(School? currSchool)
    {
        resetValidationDisplay();
        if (currSchool == null)
        { 
            ValidationDisplayObj = new ValidationDisplay()
            {
                FieldName = "Id",
                ErrorMessage = "There are is no School with this ID!",
                IsValid = false
            };
        }

        else if (currSchool.ExpiresAt != null)
        {
            ValidationDisplayObj = new ValidationDisplay()
            {
                FieldName = "Id",
                ErrorMessage = "This school has been deleted!",
                IsValid = false
            };
        }

        return ValidationDisplayObj;
    }

    public ValidationDisplay CheckSchoolNameValidations(School? currSchool)
    {
        resetValidationDisplay();
        if (currSchool == null)
        {
            ValidationDisplayObj = new ValidationDisplay()
            {
                FieldName = "SchoolName",
                ErrorMessage = "There is no school with this name!",
                IsValid = false
            };
        }

        return ValidationDisplayObj;
    }
    
    
    public List<ValidationDisplay>? CheckNewSchool(School currSchool)
    {
        resetValidationDisplay();
        List<ValidationDisplay> validations = new List<ValidationDisplay>();
        if (currSchool.DistrictId <=0)
        {
            validations.Add(new ValidationDisplay()
            {
                FieldName = "District Id",
                ErrorMessage = "Id must be greater than 0!",
                IsValid = false
            });
        }

        if (currSchool.LicenseId <= 0)
        {
            validations.Add(new ValidationDisplay()
            {
                FieldName = "License Id",
                ErrorMessage = "Id must be greater than 0!",
                IsValid = false
            });
        }
        return validations.Count > 0 ? validations : null;
    }

    public ValidationDisplay CheckDeletedSchool(School? currSchool)
    {
        resetValidationDisplay();
        if (currSchool == null)
        {
            ValidationDisplayObj = new ValidationDisplay()
            {
                FieldName = "Id",
                ErrorMessage = "There is no school with this ID!",
                IsValid = false
            };
        }
        else if (currSchool.ExpiresAt != null)
        {
            ValidationDisplayObj = new ValidationDisplay()
            {
                FieldName = "ExpiredAt",
                ErrorMessage = "This school has already been deleted!",
                IsValid = false
            };
        }

        return ValidationDisplayObj;
    }

    public List<ValidationDisplay> CheckUpdatedSchool(School? currSchool)
    {
        resetValidationDisplay();
        List<ValidationDisplay> validations = new List<ValidationDisplay>();
        if (currSchool == null)
        {
            ValidationDisplayObj = new ValidationDisplay()
            {
                FieldName = "Id",
                ErrorMessage = "There is no school with this ID!",
                IsValid = false
            };
            validations.Add(ValidationDisplayObj);
        }
        else
        {
            if (currSchool.ExpiresAt != null)
            {
                ValidationDisplayObj = new ValidationDisplay()
                {
                    FieldName = "ExpiresAt",
                    ErrorMessage = "This school has already been deleted, you cant update it!",
                    IsValid = false
                };
                validations.Add(ValidationDisplayObj);
            }
            else
            {
                if (currSchool.DistrictId < 0)
                {
                    validations.Add(new ValidationDisplay()
                    {
                        FieldName = "District Id",
                        ErrorMessage = "Id must be greater than 0!",
                        IsValid = false
                    });
                }

                if (currSchool.LicenseId < 0)
                {
                    validations.Add(new ValidationDisplay()
                    {
                        FieldName = "License Id",
                        ErrorMessage = "Id must be greater than 0!",
                        IsValid = false
                    });
                }
            }
            
        }

        return validations;
    }
}