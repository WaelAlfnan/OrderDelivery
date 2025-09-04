namespace OrderDelivery.Application.Interfaces
{
    /// <summary>
    /// Interface for SMS service operations
    /// </summary>
    public interface ISmsService
    {
        /// <summary>
        /// Sends a verification code to the specified phone number
        /// </summary>
        /// <param name="phoneNumber">Target phone number</param>
        /// <param name="verificationCode">6-digit verification code</param>
        /// <returns>True if SMS was sent successfully, false otherwise</returns>
        Task<bool> SendVerificationCodeAsync(string phoneNumber, string verificationCode);

        /// <summary>
        /// Sends a password reset code to the specified phone number
        /// </summary>
        /// <param name="phoneNumber">Target phone number</param>
        /// <param name="resetCode">6-digit reset code</param>
        /// <returns>True if SMS was sent successfully, false otherwise</returns>
        Task<bool> SendPasswordResetCodeAsync(string phoneNumber, string resetCode);

        /// <summary>
        /// Sends a notification message to the specified phone number
        /// </summary>
        /// <param name="phoneNumber">Target phone number</param>
        /// <param name="message">Message content</param>
        /// <returns>True if SMS was sent successfully, false otherwise</returns>
        Task<bool> SendNotificationAsync(string phoneNumber, string message);
    }
} 