using System;
using TD.Api.Dtos;

namespace Fourplaces.Models
{
    public class SingletonLoginResult
    {
        private static LoginResult singletonLR;
        public SingletonLoginResult()
        {
        }

        public static LoginResult LR
        {

            get
            {
                return singletonLR;
            }
            set
            {
                if (singletonLR == null)
                {
                    singletonLR = value;
                }
            }
        }

        public static void destroyLR()
        {
            singletonLR = null;
        }
    }
}
