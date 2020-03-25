using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegistrInfo : MonoBehaviour
{
    [SerializeField]private TMP_InputField 
        Name,
        SecondName,
        MiddleName,
        Email,
        Login,
        Password,
        CheckPassword,
        Phone;

    [SerializeField] private Toggle 
        GenderM,
        GenderW;

    [SerializeField]private BirthDayScript DateBirth;
    
    public SignUpData GetData() => new SignUpData
    {
        email = Email.text,
        firstName = Name.text,
        lastName = SecondName.text,
        middleName = MiddleName.text,
        phoneMobile = (Phone.text.Length > 0 ? "998" + Phone.text :"99890000000"),
        login = Login.text,
        password = Password.text,
        gender = GenderDefine(),
        userRole = RoleDefine(),
        birthday = DateBirth.CurrentDate
    };

    public void Reset()
    {
        Email.text = "";
        Name.text = "";
        SecondName.text = "";
        MiddleName.text = "";
        Phone.text = "";
        Login.text = "";
        Password.text = "";
        GenderM.SetIsOnWithoutNotify(false);
        GenderW.SetIsOnWithoutNotify(false);
        DateBirth.InitStart();
    }
    public string RoleDefine() =>"Player";
    
    public string GenderDefine()
    {
        if(GenderM.isOn)
            return "Male";
        if(GenderW.isOn)
            return "Female";
        return "";
    }
    public bool HasSymbolInCheckPassword() => CheckPassword.text.Length > 0;
    public bool PasswordChecked() => CheckPassword.text == Password.text;
    public bool HasGender() => GenderM.isOn || GenderW.isOn;
    public bool HasFullName() => Name.text.Length > 0 && SecondName.text.Length > 0;
    public bool HasLogPas() => Login.text.Length > 0 && LengthOfPasChecked() && PasswordChecked();
    public bool LengthOfPasChecked() => Password.text.Length > 5;
    private bool HasForbiddenCharacters(TMP_InputField text, string format) => Regex.IsMatch(text.text, format);
    public bool HasForbiddenCharactersInName() => HasForbiddenCharacters(Name, @"[\W^+^_]");
    public bool HasForbiddenCharactersInSecondName() => HasForbiddenCharacters(SecondName, @"[\W^+^_]");
    public bool HasForbiddenCharactersInMiddleName() => HasForbiddenCharacters(MiddleName, @"[\W^+^_]");
    public bool HasForbiddenCharactersInPassword() => HasForbiddenCharacters(Password, @"[\W^+^_]");
    public bool HasForbiddenCharactersInFullName() => HasForbiddenCharacters(Name, @"[\W^+^_]") ||
                                                      HasForbiddenCharacters(SecondName, @"[\W^+^_]") ||
                                                      HasForbiddenCharacters(MiddleName, @"[\W^+^_]") ; 

}

