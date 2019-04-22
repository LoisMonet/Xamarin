using System;
namespace Fourplaces.Models.Exceptions
{
    public class ConnexionE :Exception
    {
        private String exceptionMess;
        public ConnexionE(String email, String mdp)
        {
            if (email == "")
            {
                exceptionMess += "le champ email est vide";

            }
            if (mdp == "")
            {
                exceptionMess += "\n le champ mot de passe est vide";
            }

        }

        public ConnexionE(String message)
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
