using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderDelivery.Application.Configurations;
using OrderDelivery.Application.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace OrderDelivery.Application.Services
{
    /// <summary>
    /// Twilio SMS service implementation using API Key authentication
    /// </summary>
    public class TwilioSmsService : ISmsService
    {
        private readonly TwilioSettings _twilioSettings;
        private readonly ILogger<TwilioSmsService> _logger;

        public TwilioSmsService(IOptions<TwilioSettings> twilioSettings, ILogger<TwilioSmsService> logger)
        {
            _twilioSettings = twilioSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Sends a verification code to the specified phone number
        /// </summary>
        public async Task<bool> SendVerificationCodeAsync(string phoneNumber, string verificationCode)
        {
            try
            {
                var message = $"Your OrderDelivery verification code is: {verificationCode}. Valid for 10 minutes.";
                return await SendSmsAsync(phoneNumber, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending verification code to {PhoneNumber}", phoneNumber);
                return false;
            }
        }

        /// <summary>
        /// Sends a password reset code to the specified phone number
        /// </summary>
        public async Task<bool> SendPasswordResetCodeAsync(string phoneNumber, string resetCode)
        {
            try
            {
                var message = $"Your OrderDelivery password reset code is: {resetCode}. Valid for 10 minutes.";
                return await SendSmsAsync(phoneNumber, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset code to {PhoneNumber}", phoneNumber);
                return false;
            }
        }

        /// <summary>
        /// Sends a notification message to the specified phone number
        /// </summary>
        public async Task<bool> SendNotificationAsync(string phoneNumber, string message)
        {
            try
            {
                return await SendSmsAsync(phoneNumber, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to {PhoneNumber}", phoneNumber);
                return false;
            }
        }

        /// <summary>
        /// Sends SMS using Twilio API with API Key authentication
        /// </summary>
        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                // Validate Twilio settings
                if (!_twilioSettings.IsValid)
                {
                    _logger.LogError("Invalid Twilio settings. Please check AccountSid, ApiKey, ApiSecret, and PhoneNumber");
                    return false;
                }

                // Initialize Twilio client with API Key authentication
                TwilioClient.Init(_twilioSettings.ApiKey, _twilioSettings.ApiSecret, _twilioSettings.AccountSid);
                _logger.LogInformation("Initialized Twilio client with API Key authentication");

                // Send message
                var messageResource = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(_twilioSettings.PhoneNumber),
                    to: new PhoneNumber(phoneNumber)
                );

                _logger.LogInformation("SMS sent successfully to {PhoneNumber}. SID: {MessageSid}",
                    phoneNumber, messageResource.Sid);

                return messageResource.Status != MessageResource.StatusEnum.Failed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS to {PhoneNumber}", phoneNumber);
                return false;
            }
        }
    }
}