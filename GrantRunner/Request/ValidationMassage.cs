using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Grant.WebApi.Request
{
    public class ValidationMassage
    {
        public string Message { get; set; }
    }



    public class ModeratorIdList
    {
        public List<long> Ids { get; set; }
    }
}