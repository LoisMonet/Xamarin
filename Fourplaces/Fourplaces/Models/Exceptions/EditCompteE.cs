using System;
namespace Fourplaces.Models.Exceptions
{
    public class EditCompteE :Exception
    {
        private String exceptionMess;

        public EditCompteE(String fName, String lName)
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

        public String ExceptionMess
        {
            get
            {
                return exceptionMess;
            }
        }
    }
}
