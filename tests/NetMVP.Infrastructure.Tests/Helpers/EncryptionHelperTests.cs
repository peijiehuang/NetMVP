using NetMVP.Infrastructure.Helpers;

namespace NetMVP.Infrastructure.Tests.Helpers;

public class EncryptionHelperTests
{
    [Fact]
    public void MD5Encrypt_ShouldReturnCorrectHash()
    {
        // Arrange
        var input = "test123";

        // Act
        var result = EncryptionHelper.MD5Encrypt(input);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(32, result.Length);
    }

    [Fact]
    public void SHA256Encrypt_ShouldReturnCorrectHash()
    {
        // Arrange
        var input = "test123";

        // Act
        var result = EncryptionHelper.SHA256Encrypt(input);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(64, result.Length);
    }

    [Fact]
    public void AESEncrypt_And_AESDecrypt_ShouldWork()
    {
        // Arrange
        var plainText = "Hello World";
        var key = "MySecretKey12345";

        // Act
        var encrypted = EncryptionHelper.AESEncrypt(plainText, key);
        var decrypted = EncryptionHelper.AESDecrypt(encrypted, key);

        // Assert
        Assert.NotEmpty(encrypted);
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void BCryptHash_And_BCryptVerify_ShouldWork()
    {
        // Arrange
        var password = "MyPassword123";

        // Act
        var hash = EncryptionHelper.BCryptHash(password);
        var isValid = EncryptionHelper.BCryptVerify(password, hash);
        var isInvalid = EncryptionHelper.BCryptVerify("WrongPassword", hash);

        // Assert
        Assert.NotEmpty(hash);
        Assert.True(isValid);
        Assert.False(isInvalid);
    }

    [Fact]
    public void GenerateRandomKey_ShouldReturnCorrectLength()
    {
        // Arrange
        var length = 16;

        // Act
        var key = EncryptionHelper.GenerateRandomKey(length);

        // Assert
        Assert.Equal(length, key.Length);
    }
}
