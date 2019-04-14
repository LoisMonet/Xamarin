using System;
namespace Fourplaces.Models.Exceptions
{
    public class AuthenticationException : Exception
    {

        private String exceptionMess;


        public AuthenticationException(String login, String mdp)
        {
            
            if (login == "")
            {
                exceptionMess += "le champ email est vide";

            }
            if(mdp == "")
            {
                exceptionMess += "\n le champ mot de passe est vide";
            }
            
        }

        public AuthenticationException()
        {
            exceptionMess = "mauvais email ou mot de passe";
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
