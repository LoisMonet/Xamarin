using System;
using Newtonsoft.Json;
using Storm.Mvvm;

namespace TD.Api.Dtos
{
    public class LoginResult : NotifierBase
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        private DateTime _dateExpiresTo;

        private int _expiresIn;

        [JsonProperty("expires_in")]
        public int ExpiresIn
        {
            get
            {
                return _expiresIn;
            }


            set
            {
                Console.WriteLine("Dev_expiredIn:"+value);
                SetProperty(ref _expiresIn, value);
                _dateExpiresTo = DateTime.Now.AddSeconds(value);

            }


        }
        
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        public bool IsExpired()
        {
            DateTime now = DateTime.Now;
            TimeSpan span = _dateExpiresTo.Subtract(now);
            if(span<= TimeSpan.Zero)
            {
                Console.WriteLine("Dev_expired:it's over");
                return true;
            }
            else
            {
                Console.WriteLine("Dev_staying:" + span.TotalSeconds);
                return false;
            }

        }
    }
}