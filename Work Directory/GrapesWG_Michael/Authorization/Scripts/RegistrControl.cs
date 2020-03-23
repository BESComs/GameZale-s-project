using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RegistrControl : MonoBehaviour
{
    [SerializeField]private GameObject 
        Step0,
        Step1,
        Step2,
        Step3,
        StepPreviousButton;

    [SerializeField] private Button
        Step1Button,
        Step2Button,
        FBButton;

    public TextMeshProUGUI LabelPas, LabelCheckPas, LabelName, LabelSecondName, LabelMiddleName;
    public RegistrInfo Info;
    private int curStep = 0;
    private bool Sending, fbIdent;

    private void Update()
    {
        StepControl();
        InteractableButton();
        LabelOfPasAndCheckPas();
        LabelOfFullName();
    }

    void StepControl()
    {
        switch (curStep)
        {
            case 0:
                Step0.SetActive(true);
                Step1.SetActive(false);
                Step2.SetActive(false);
                Step3.SetActive(false);
                break;
            case 1:
                Step0.SetActive(false);
                Step1.SetActive(true);
                Step2.SetActive(false);
                break;
            case 2:
                Step1.SetActive(false);
                Step2.SetActive(true);
                Step3.SetActive(false);
                break;
            case 3:
                Step2.SetActive(false);
                Step3.SetActive(true);
                break;
            case 4:
                SendRequest();
                SetNextStep();
                break;
        }
        
    }

    async void SendRequest()
    {
        BackgroundToast.ActivateToast();
        Sending = true;
        var signUp = Info.GetData();
       
        Toaster.ShowMessage(RequestsKeys.WaitingResponse, Toaster.Position.Bottom, Toaster.Time.HalfSecond);
       
        var responseJson = await ServerRequests.AsyncSignUp(signUp);
        Toaster.ShowMessage(responseJson.message, Toaster.Position.Bottom, Toaster.Time.ThreeSecond);

        if (responseJson.status == RequestsKeys.StatusSuccess)
            await SceneManager.LoadSceneAsync("Launcher Scene 2019");
        
        curStep = 0;
        Sending = false;
        BackgroundToast.DeactivateToast();
    }
    
    void InteractableButton()
    {
        Step1Button.interactable = Info.HasLogPas() && !Sending &&
                                   !Info.HasForbiddenCharactersInPassword();
        FBButton.interactable = !Sending;
        Step2Button.interactable = Info.HasGender() && Info.HasFullName() && 
                                   !Info.HasForbiddenCharactersInFullName();
    }

    void LabelOfPasAndCheckPas()
    {
        LabelPas.text = Info.HasForbiddenCharactersInPassword() ? 
            "<color=#D95757>Такие символы нельзя использоать</color>" :Info.LengthOfPasChecked()
            ? "<color=#70D958>Достаточная длина</color>"
            : "Придумайте надёжный пароль и запомните его\n" + 
              "Длина пароля не менее <color=#D95757>6 </color> символов";
        LabelCheckPas.text = Info.HasSymbolInCheckPassword()
            ? Info.PasswordChecked() ? "<color=#70D958>Пароли совпадают</color>" 
            : "<color=#D95757>Кажется, они не совпадают</color>"
            : "Напишите тут пароль снова, чтобы\nубедиться в его верности";
    }

    void LabelOfFullName()
    {
        var color = new Color();
         if(ColorUtility.TryParseHtmlString(Info.HasForbiddenCharactersInName() 
             ? "#D95757" : "#ffffff", out color ) )
             LabelName.color = color;
         if(ColorUtility.TryParseHtmlString(Info.HasForbiddenCharactersInSecondName() 
             ? "#D95757" : "#ffffff", out color ) )
             LabelSecondName.color = color;
         if(ColorUtility.TryParseHtmlString(Info.HasForbiddenCharactersInMiddleName() 
             ? "#D95757" : "#ffffff", out color ) )
             LabelMiddleName.color = color;
    }
    

    public void SetNextStep()
    {
        curStep++;
    }
    public void SetPreviousStep()
    {
        curStep--;
        if (curStep == 0) ActiveStepPreviousButton(false);
    }

    public void ActiveStepPreviousButton(bool tf)
    {
        StepPreviousButton.SetActive(tf);
    }
}
