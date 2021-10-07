using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Signup.Presentation
{
    public class ResultModel
    {
        public bool Status { get; set; }
        public int Id { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public dynamic Data { get; set; }

        public string link { get; set; }

    }
}
