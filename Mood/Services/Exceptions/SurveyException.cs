using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mood.Services.Exceptions
{
    public class SurveyException : Exception
    {
        public SurveyException(string message) : base(message)
        {

        }
    }
}