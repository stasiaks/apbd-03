namespace LegacyApp.Tests;

public class UserServiceTests
{
    [Fact]
    public void Given_LegacyAppConsumerData_ReturnsTrue()
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

    private static UserService CreateSut() => new UserService();
}