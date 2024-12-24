using API.DTOs;
using CommonClasses.Models;
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
    public ValidationDisplay CheckNumericFieldValidations(int fieldData,string fieldName)
    {
        resetValidationDisplay();
        if (fieldData <= 0)
        { 
            ValidationDisplayObj = new ValidationDisplay()
            {
                FieldName = fieldName,
                ErrorMessage = $"{fieldName} must be greater than 0!",
                IsValid = false
            };
        }
        return ValidationDisplayObj;
    }
    
    public List<ValidationDisplay>? CheckNewSchool(SchoolDto currSchool)
    {
        resetValidationDisplay();
        List<ValidationDisplay> validations = new List<ValidationDisplay>();
        ValidationDisplay validationDisplay = CheckNumericFieldValidations(currSchool.DistrictId, nameof(currSchool.DistrictId));
        if (!validationDisplay.IsValid)
        {
            validations.Add(validationDisplay);
        }
        
        validationDisplay = CheckNumericFieldValidations(currSchool.LicenseId, nameof(currSchool.LicenseId));
        if (!validationDisplay.IsValid)
        {
            validations.Add(validationDisplay);
        }
        
        return validations.Count > 0 ? validations : null;
    }
    public List<ValidationDisplay> CheckUpdatedSchool(SchoolUpdateDto currSchool,int schoolId)
    {
        resetValidationDisplay();
        List<ValidationDisplay> validations = new List<ValidationDisplay>();
        ValidationDisplay validDisplay = CheckNumericFieldValidations(schoolId, "Id");
        if (validDisplay.IsValid)
        {
            if (currSchool.DistrictId < 0)
            {
                validations.Add(new ValidationDisplay()
                {
                    FieldName = nameof(currSchool.DistrictId),
                    ErrorMessage = "DistrictId must be greater than 0!",
                    IsValid = false
                });
            }

            if (currSchool.LicenseId < 0)
            {
                validations.Add(new ValidationDisplay()
                {
                    FieldName = nameof(currSchool.DistrictId),
                    ErrorMessage = "DistrictId must be greater than 0!",
                    IsValid = false
                });
            }
        }
        else
        {
            validations.Add(validDisplay);
        }
        
        return validations;
    }

    public void CheckEmptyFields(SchoolUpdateDto currUpdate, School prevSchool)
    {
        if (currUpdate.DistrictId == 0)
        {
            currUpdate.DistrictId = prevSchool.DistrictId;
        }

        if (currUpdate.LicenseId == 0)
        {
            currUpdate.LicenseId = prevSchool.LicenseId;
        }
    }
}