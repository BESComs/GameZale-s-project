using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// ReSharper disable StringLiteralTypo

[Serializable]
public class ModelData
{
}

[Serializable]
public class StatisticData : ModelData
{
    public int playerId;
    public int exerciseId;
    public string createdDate;
    public int spentTime;
    public int score;
    public bool isRight;
}

[Serializable]
public class StatisticsList : ModelData
{
    public List<StatisticData> statistics;

    public StatisticsList(List<StatisticData> statistics)
    {
        this.statistics = new List<StatisticData>(statistics);
    }

    public string GetJsonString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("[");

        foreach (var stat in statistics)
        {
            sb.AppendLine(JsonUtility.ToJson(stat));
            sb.Append(",");
        }

        sb.Length -= 1;

        sb.AppendLine("]");
        return sb.ToString();
    }
}


[Serializable]
public class AuthData : ModelData
{
    public string login;
    public string password;
}

[Serializable]
public class AuthFacebookData : ModelData //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
{
    public string accessToken;
}

[Serializable]
public class RefreshData : ModelData
{
    public string refreshToken;
}

[Serializable]
public class ExerciseData : ModelData
{
    public int themeId;
    public string title;
    public int maxScore;
}

[Serializable]
public class SubjectData : ModelData
{
    public string name;
}


[Serializable]
public class ThemeData : ModelData
{
    public int subjectId;
    public string title;
}


[Serializable]
public class StudentData : ModelData
{
    public int id;
    public string firstName;
    public string lastName;
    public string middleName;
    public DateTime birthDay;
    public int gender;
    public string address;
    public string phoneMobile;
    public string phoneHome;
    public string email;
    public string photoPath;
}

[Serializable]
public class SignUpData : ModelData{
public string firstName;
public string lastName;
public string middleName;
public DateTime birthday;
public string gender;
public string phoneMobile;
public string email;
public string login;
public string password;
public string userRole;
}
[Serializable]
public class FBSignUpData : ModelData{
    public string accessToken;
    public string userRole;
}
[Serializable]
public class PlayerData : ModelData
{
    public int playerId;
    public int peopleId;
    public int userId;
    public string firstName;
    public string lastName;
    public string middleName;
    public DateTime birthDay;
    public string gender;
    public string phoneMobile;
    public bool phoneMobileConfirmed;
    public string email;
    public bool emailConfirmed;
    public string photoPath;
    public string login;
    public string facebookId;
    public DateTime lastLogin;
    public bool blocked;     
    public int coins;

}

[Serializable]
public class AuthResponseData : ModelData
{
    public string type;
    public string accessToken;
    public string expiresAt;
    public string refreshToken;
}

[Serializable]
public class PlayerStatisticData
{
    public long id;
    public string name;
    public int totalScore;
    public double totalSpentTime;
    public double averageScore;
    public double averageSpentTime;
    public int isRightCount;
    public int isNotRightCount;
}

[Serializable]
public class OutfitItemData
{
    public int id;
    public string name;
    public string price;
}

[Serializable, Obsolete("Номер учебного заведения больше не требуется")]
public class XEduResponseData : ModelData
{
    public string eduId;
    public string role;
}

[Serializable]
public class nullResponse : ModelData
{
}

[Serializable]
public class NoInternetError : ModelData
{
    public string status = "No Internet connection";
}