using System;
namespace Fourplaces.Models.Exceptions
{
    public class PwdCompteE:Exception
    {
        private String exceptionMess;
        public PwdCompteE(String opw, String npw) //SEE AGAIN EXCEPTION
        {

            if (opw == null || opw == "")
            {
                exceptionMess += "le champ ancien mot de passe est vide";

            }
            if (npw == null || npw == "")
            {
                exceptionMess += "\n le champ nouveau mot de passe est vide";
            }

        }

        public PwdCompteE(String message)
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
