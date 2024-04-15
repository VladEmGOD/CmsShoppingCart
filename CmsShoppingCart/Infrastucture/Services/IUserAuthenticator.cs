namespace CmsShoppingCart.WebApp.Infrastucture.Services
{
    public interface IUserAuthenticator
    {
        public void SingIn();

        public void LogIn();

        public void LogOut();
    }
}
