namespace OrderDelivery.Application.Configurations
{
    /// <summary>
    /// Configuration class for Twilio SMS settings
    /// </summary>
    public class TwilioSettings
    {
        /// <summary>
        /// Twilio Account SID
        /// </summary>
        public string AccountSid { get; set; } = string.Empty;

        /// <summary>
        /// Twilio API Key (Modern method)
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Twilio API Secret (Modern method)
        /// </summary>
        public string ApiSecret { get; set; } = string.Empty;

        /// <summary>
        /// Twilio phone number to send SMS from
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Validates that all required API Key settings are provided
        /// </summary>
        public bool IsValid => !string.IsNullOrEmpty(AccountSid) && 
                              !string.IsNullOrEmpty(ApiKey) && 
                              !string.IsNullOrEmpty(ApiSecret) && 
                              !string.IsNullOrEmpty(PhoneNumber);
    }
}