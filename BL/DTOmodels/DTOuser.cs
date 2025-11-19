using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DTOmodels.Interface;

namespace BL.DTOmodels
{
    public class DTOuser : IDTOmodels
    {
        public string Name { get; set; }
        public  string Email { get; set; }
    }
}
