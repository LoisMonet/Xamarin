using System;
namespace Fourplaces.Models.Exceptions
{
    public class AddPlaceE :Exception
    {
        private String exceptionMess;

        public AddPlaceE(String nLieu, String desc, String lat, String longi)
        {

            if (nLieu == "")
            {
                exceptionMess += "le champ nom du lieu est vide";

            }
            if (desc == "")
            {
                exceptionMess += "\nle champ description est vide";

            }
            if (string.IsNullOrEmpty(lat))
            {
                exceptionMess += "\nle champ latitude est vide";

            }

            if (string.IsNullOrEmpty(longi))
            {
                exceptionMess += "\nle champ longitude est vide";

            }



        }

        public AddPlaceE(double lat, double longi)
        {
            if (lat.Equals(0) || longi.Equals(0))
            {
                exceptionMess = "\nla latitude et la longitude ne peuvent pas être égales à 0";
            }

            if (lat > 90 || lat < -90)
            {
                //Console.WriteLine("Dev_Exception:"+lat+"|"+(lat+10));
                exceptionMess += "\nla latitude doit être comprise entre -90 et 90";

            }
            if (longi > 180 || longi < -180)
            {
                //Console.WriteLine("Dev_Exception:" + longi + "|" + (lat + 10));
                exceptionMess += "\nla longitude doit être comprise entre 180 et -180";

            }

        }

        public AddPlaceE(String message)
        {
            exceptionMess = message;
        }

        public String ExceptionMess
        {
            get
            {
                return exceptionMess;
            }
        }
    }
}
