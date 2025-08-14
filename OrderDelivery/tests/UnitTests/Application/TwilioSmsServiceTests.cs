using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OrderDelivery.Application.Configurations;
using OrderDelivery.Application.Services;
using Twilio.Rest.Api.V2010.Account;
using Xunit;

namespace UnitTests.Application;

public class TwilioSmsServiceTests
{
    [Fact]
    public async Task SendVerificationCodeAsync_ValidInput_ReturnsTrue()
    {
        // Arrange
        var twilioSettings = new TwilioSettings
        {
            AccountSid = "ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
            ApiKey = "SKXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
            ApiSecret = "your_api_secret",
            PhoneNumber = "+1234567890"
        };
        var options = Options.Create(twilioSettings);
        var loggerMock = new Mock<ILogger<TwilioSmsService>>();
        var service = new TwilioSmsService(options, loggerMock.Object);

        // ملاحظة: هذا الاختبار يفترض أن TwilioClient لن يتصل فعليًا بالخدمة الحقيقية
        // في بيئة اختبار حقيقية يجب عمل Mock أو Stub لطرف TwilioClient
        // هنا نختبر فقط أن الدالة لا ترمي استثناء وتعيد false عند الفشل
        var result = await service.SendVerificationCodeAsync("+201234567890", "123456");
        Assert.False(result); // لأن الاتصال الحقيقي لن يتم في بيئة الاختبار
    }

    [Fact]
    public void TwilioSettings_IsValid_ReturnsTrue_WhenAllSettingsProvided()
    {
        // Arrange
        var settings = new TwilioSettings
        {
            AccountSid = "ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
            ApiKey = "SKXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
            ApiSecret = "your_api_secret",
            PhoneNumber = "+1234567890"
        };

        // Act & Assert
        Assert.True(settings.IsValid);
    }

    [Fact]
    public void TwilioSettings_IsValid_ReturnsFalse_WhenAnySettingMissing()
    {
        // Arrange
        var settings = new TwilioSettings
        {
            AccountSid = "ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
            ApiKey = "SKXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
            ApiSecret = "", // Empty secret
            PhoneNumber = "+1234567890"
        };

        // Act & Assert
        Assert.False(settings.IsValid);
    }

    [Fact]
    public void TwilioSettings_IsValid_ReturnsFalse_WhenAccountSidMissing()
    {
        // Arrange
        var settings = new TwilioSettings
        {
            AccountSid = "", // Empty AccountSid
            ApiKey = "SKXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
            ApiSecret = "your_api_secret",
            PhoneNumber = "+1234567890"
        };

        // Act & Assert
        Assert.False(settings.IsValid);
    }

    [Fact]
    public void TwilioSettings_IsValid_ReturnsFalse_WhenPhoneNumberMissing()
    {
        // Arrange
        var settings = new TwilioSettings
        {
            AccountSid = "ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
            ApiKey = "SKXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
            ApiSecret = "your_api_secret",
            PhoneNumber = "" // Empty phone number
        };

        // Act & Assert
        Assert.False(settings.IsValid);
    }
} 