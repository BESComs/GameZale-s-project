using System.Threading.Tasks;

public interface IConfirmableRequest
{
    Task<bool> RequestConfirmationAsync();
}