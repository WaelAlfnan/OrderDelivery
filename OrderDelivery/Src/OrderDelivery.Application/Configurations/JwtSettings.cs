namespace OrderDelivery.Application.Configurations
{
    /// <summary>
    /// Configuration class for JWT settings
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Secret key used to sign JWT tokens
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// JWT issuer (who creates the token)
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// JWT audience (who the token is intended for)
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Token expiration time in minutes
        /// </summary>
        public int ExpirationInMinutes { get; set; }

        /// <summary>
        /// Refresh token expiration time in days
        /// </summary>
        public int RefreshTokenExpirationInDays { get; set; }
    }
} 