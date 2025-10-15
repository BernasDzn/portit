namespace Tests.Domain;

using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;

public class EncryptionTest
{
    private const string testKey = "213df0f9d43e0409a4bde0ec732d9bd0";

    [Fact]
    public void WhenEncryptingNull_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => EncryptionHelper.Encrypt(null!));
    }

    [Fact]
    public void WhenDecryptingNull_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => EncryptionHelper.Decrypt(null!));
    }

    [Theory]
    [InlineData("Test string")]
    [InlineData("Cow tools")]
    [InlineData("ランプ幻想")]
    [InlineData("Porque te havia eu de amar, oh Sol, se tu és o inimigo dos sonhos de imaginar; se tu nos chamas à realidade, e a realidade é tão triste")]
    public void WhenEncryptingString_StringMatches(string value)
    {
        EncryptionHelper.SetEncryptionKey(testKey);

        var encrypted = EncryptionHelper.Encrypt(value);
        var decrypted = EncryptionHelper.Decrypt(encrypted);

        Assert.Equal(value, decrypted);
    }

    [Theory]
    [InlineData("Test string")]
    [InlineData("The Philadelphia Air Quartet")]
    [InlineData("Destroying yourself is too accessible")]
    [InlineData("É perceber à custa de amarguras que o existir é padecer, o pensar descrer, o experimentar desenganar-se e a esperança nas causas da terra uma cruel mentira de nossos desejos.")]
    public void WhenEncryptingString_WrongStringDoesNotMatch(string value)
    {
        EncryptionHelper.SetEncryptionKey(testKey);

        var encrypted = EncryptionHelper.Encrypt(value);
        var decrypted = EncryptionHelper.Decrypt(encrypted);

        Assert.NotEqual(value + "x", decrypted);
    }
}