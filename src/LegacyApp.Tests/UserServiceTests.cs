namespace LegacyApp.Tests;

public class UserServiceTests
{
    [Fact]
    public void GivenLegacyAppConsumerData_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.AddUser(
            "John",
            "Doe",
            "johndoe@gmail.com",
            DateTime.Parse("1982-03-21"),
            1
        );

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("Kowalski", "kowalski@example.com", "1996-07-13", 2)]
    [AutoData]
    public void GivenEmptyFirstName_ReturnsFalse(
        string lastName,
        string email,
        DateTime dateOfBirth,
        int clientId
    )
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.AddUser(string.Empty, lastName, email, dateOfBirth, clientId);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("Jan", "kowalski@example.com", "1996-07-13", 2)]
    [AutoData]
    public void GivenEmptyLastName_ReturnsFalse(
        string firstName,
        string email,
        DateTime dateOfBirth,
        int clientId
    )
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.AddUser(firstName, string.Empty, email, dateOfBirth, clientId);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineAutoData("Jan", "Kowalski", "1996-07-13", 2)]
    [AutoData]
    public void GivenInvalidEmail_ReturnsFalse(
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        int clientId,
        string emailBase
    )
    {
        // Arrange
        var email = emailBase.Replace('@', 'A').Replace('.', 'B');
        var sut = CreateSut();

        // Act
        var result = sut.AddUser(firstName, lastName, email, dateOfBirth, clientId);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("Jan", "Kowalski", "kowalski@example.com", 2)]
    [AutoData]
    public void GivenUnder21_ReturnsFalse(
        string firstName,
        string lastName,
        string email,
        int clientId
    )
    {
        // Arrange
        var dateOfBirth = DateTime.Now.AddYears(-20);
        var sut = CreateSut();

        // Act
        var result = sut.AddUser(firstName, lastName, email, dateOfBirth, clientId);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("Jan", "Kowalski", "kowalski@example.com", "1996-06-12")]
    public void GivenNonexistingUser_ThrowsException(
        string firstName,
        string lastName,
        string email,
        DateTime dateOfBirth
    )
    {
        // Arrange
        var sut = CreateSut();

        // Act && Assert
        void resultAction() => sut.AddUser(firstName, lastName, email, dateOfBirth, 543);

        Assert.Throws<ArgumentException>(resultAction);
    }

    private static UserService CreateSut() => new();
}
