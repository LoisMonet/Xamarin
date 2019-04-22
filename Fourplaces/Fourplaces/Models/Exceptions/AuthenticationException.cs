using System;
namespace Fourplaces.Models.Exceptions
{
    public class AuthenticationException : Exception
    {

        private String exceptionMess;
        public string urlSave;

        //ConnexionE(String email, String mdp)
        public AuthenticationException(String email, String mdp) 
        {
            if (email == "")
            {
                exceptionMess += "le champ email est vide";

            }
            if(mdp == "")
            {
                exceptionMess += "\n le champ mot de passe est vide";
            }
            
        }

        //EditCompteE(String fName, String lName)
        public AuthenticationException(String fName, String lName,int useless) //SEE AGAIN EXCEPTION
        {


            if (fName == "")
            {
                exceptionMess += "le champ first name est vide";

            }
            if (lName == "")
            {
                exceptionMess += "\n le champ last name est vide";
            }

        }

        //PwdCompteE(String opw, String npw)
        public AuthenticationException(String opw, String npw, bool useless) //SEE AGAIN EXCEPTION
        {

            if (opw==null ||opw == "")
            {
                exceptionMess += "le champ ancien mot de passe est vide";

            }
            if (npw==null ||npw == "")
            {
                exceptionMess += "\n le champ nouveau mot de passe est vide";
            }

        }

        //RegisterE(String email, String mdp,String fName,String lName )
        public AuthenticationException(String email, String mdp,String fName,String lName )
        {

            if (email == "")
            {
                exceptionMess += "le champ email est vide";

            }
            if (mdp == "")
            {
                exceptionMess += "\n le champ mot de passe est vide";
            }
            if (fName == "")
            {
                exceptionMess += "\nle champ first name  est vide";

            }
            if (lName == "")
            {
                exceptionMess += "\n le champ last name est vide";
            }


        }

        //AddPlaceE(tring nLieu,String desc, String lat, String longi)

        //USE imageID field else I have currently two method with the same number of parameters
        public AuthenticationException(String nLieu,String desc, String lat, String longi,int useless) //SEE AGAIN EXCEPTION
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

        //LatLongValE(double lat, double longi)
        public AuthenticationException(double lat, double longi)
        {
            if(lat.Equals(0) || longi.Equals(0))
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

        //OtherE(String message)
        public AuthenticationException(String message)
        {
            exceptionMess = message;
        }

        //NoConnectCacheE(String url,float a)
        public AuthenticationException(String url,float a) //no connect for cache
        {
            exceptionMess = "vous n'êtes pas connecté à internet";
            urlSave = url;


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
