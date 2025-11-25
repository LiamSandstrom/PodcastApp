using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DTOmodels.Interface;

namespace BL.DTOmodels
{
    public class DTOcategory : IDTOmodels
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
