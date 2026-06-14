namespace CSharpApp.Core.Interfaces;

public interface ITokenService
{
    Task<string> GetAccessToken();
}