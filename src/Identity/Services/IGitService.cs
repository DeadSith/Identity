using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Services
{
    public interface IGitService
    {
        void Upload(); 
        void Pull();
        void Clone();
    }
}
