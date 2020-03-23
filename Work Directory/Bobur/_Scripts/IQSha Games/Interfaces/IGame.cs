namespace Work_Directory.Bobur._Scripts.IQSha_Games.Interfaces
{
    public interface IGame : IAnswerCheckable, ISceneInOut
    {
        void ChangeStateAfterAnswer();
    }
}