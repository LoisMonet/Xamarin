﻿using System;
namespace Fourplaces.Models.Exceptions
{
    public class NoConnectE:Exception
    {
        private String exceptionMess;
        public string urlSave;

        public NoConnectE(String message)
        {
            exceptionMess = message;
        }

        public NoConnectE(String url, float a) //no connect for cache
        {
            exceptionMess = "vous n'êtes pas connecté à internet";
            urlSave = url;


        }


        public NoConnectE(String url, String message) 
        {
            exceptionMess = message;
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
