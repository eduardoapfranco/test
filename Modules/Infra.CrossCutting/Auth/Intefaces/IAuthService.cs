namespace Infra.CrossCutting.Auth.Intefaces
{
    public interface IAuthService<T> where T : class
    {
        string GenerateToken(T user);
    }
}
