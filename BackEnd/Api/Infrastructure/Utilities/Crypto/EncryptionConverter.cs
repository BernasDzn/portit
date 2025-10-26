using Api.Domain.ValueObjects;
using Api.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// For non-nullable strings
public class EncryptionConvertor : ValueConverter<string, string>
{
    public EncryptionConvertor()
        : this(null!) { }

    public EncryptionConvertor(ConverterMappingHints mappingHints = null!)
        : base(x => EncryptionHelper.Encrypt(x), x => EncryptionHelper.Decrypt(x), mappingHints)
    { }
}

// For unsigned integers
public class UIntEncryptionConvertor : ValueConverter<uint, string>
{
    public UIntEncryptionConvertor()
        : this(null!) { }

    public UIntEncryptionConvertor(ConverterMappingHints mappingHints = null!)
        : base(x => EncryptionHelper.Encrypt(x.ToString()), x => uint.Parse(EncryptionHelper.Decrypt(x)), mappingHints)
    { }
}

// For designations
public class DesignationEncryptionConverter : ValueConverter<Designation, string>
{
    public DesignationEncryptionConverter()
        : this(null!) { }

    public DesignationEncryptionConverter(ConverterMappingHints mappingHints = null!)
        : base(
            designation => EncryptionHelper.Encrypt(designation.Value),
            encrypted => new Designation { Value = EncryptionHelper.Decrypt(encrypted) },
            mappingHints)
    { }
}

// For emails
public class EmailEncryptionConverter : ValueConverter<Email, string>
{
    public EmailEncryptionConverter()
        : this(null!) { }

    public EmailEncryptionConverter(ConverterMappingHints mappingHints = null!)
        : base(
            email => EncryptionHelper.Encrypt(email.Value),
            encrypted => new Email { Value = EncryptionHelper.Decrypt(encrypted) },
            mappingHints)
    { }

    public static string Decrypt(string encryptedEmail)
    {
        return EncryptionHelper.Decrypt(encryptedEmail);
    }
}

// For phone numbers
public class PhoneNumberEncryptionConverter : ValueConverter<PhoneNumber, string>
{
    public PhoneNumberEncryptionConverter()
        : this(null!) { }

    public PhoneNumberEncryptionConverter(ConverterMappingHints mappingHints = null!)
        : base(
            phone => EncryptionHelper.Encrypt(phone.Value),
            encrypted => new PhoneNumber { Value = EncryptionHelper.Decrypt(encrypted) },
            mappingHints)
    { }
}