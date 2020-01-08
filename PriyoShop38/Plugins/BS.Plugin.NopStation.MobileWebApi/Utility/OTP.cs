using System;

namespace BS.Plugin.NopStation.MobileWebApi.Utility
{
    public static class OTP
    {
        public static string GenerateOTP(int OTPLength = 6)
        {
            var OTP = new char[OTPLength];

            var allowableChars = "1234567890";

            var random = new Random();

            for (int i = 0; i < OTP.Length; i++)
            {
                OTP[i] = allowableChars[random.Next(allowableChars.Length)];
            }

            return new string(OTP);
        }
    }
}
