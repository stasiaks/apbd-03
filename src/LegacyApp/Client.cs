namespace LegacyApp;

public record Client(int Id, string Name, string Email, string Address, ClientType Type);

public enum ClientType
{
    Regular,
    Important,
    VeryImportant,
}
