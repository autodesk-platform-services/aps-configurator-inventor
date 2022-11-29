using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodesk.Forge.Controllers
{
    class SignedDto
    {
        public string signedUrl { get; set; }
        public long expiration { get; set; }
        public bool singleUse { get; set; }
    }
}
