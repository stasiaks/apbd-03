namespace LegacyApp;

internal class UserDataAccessWrapper : IUserDataAccess
{
    public void AddUser(User user) => UserDataAccess.AddUser(user);
}
