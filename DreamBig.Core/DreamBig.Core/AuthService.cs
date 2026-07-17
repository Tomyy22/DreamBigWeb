public class AuthService
{
    public bool IsAuthenticated { get; private set; }
    public void Login() => IsAuthenticated = true;
}