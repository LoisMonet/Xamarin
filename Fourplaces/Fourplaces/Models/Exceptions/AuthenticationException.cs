using System;
namespace Fourplaces.Models.Exceptions
{
    public class AuthenticationException : Exception
    {

        private String exceptionMess;


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

        public AuthenticationException(double lat, double longi)
        {
            if(lat.Equals(0) || longi.Equals(0))
            {
                exceptionMess = "\nla latitude et la longitude ne peuvent pas être égales à 0";
            }

            if (lat > 90 || lat < -90)
            {
                exceptionMess += "\nla latitude doit être comprise entre -90 et 90";

            }
            if (longi > 180 || longi < -180)
            {
                exceptionMess += "\nla longitude doit être comprise entre 180 et -180";

            }

        }

            public AuthenticationException(String message)
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
