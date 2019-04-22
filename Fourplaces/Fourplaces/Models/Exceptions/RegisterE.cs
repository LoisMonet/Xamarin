using System;
namespace Fourplaces.Models.Exceptions
{
    public class RegisterE:Exception
    {
        private String exceptionMess;


        public RegisterE(String email, String mdp, String fName, String lName)
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

        public RegisterE(String message)
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
