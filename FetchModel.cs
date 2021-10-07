using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Signup.Presentation
{
    public class FetchModel
    {
        public bool Status { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public dynamic Email { get; set; }

    }
}
