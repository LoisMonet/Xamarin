using System;
namespace Fourplaces.Models
{
    public class SingletonRestService
    {
        private static RestService singletonRS;
        private SingletonRestService()
        {

        }

        public static RestService RS
        {
            get{

                if (singletonRS == null)
                {
                    singletonRS = new RestService();
                }
                return singletonRS;
            }
        }
    }
}
