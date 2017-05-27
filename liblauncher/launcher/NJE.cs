using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace liblauncher.launcher
{
    class NJE:Exception
    {
        private readonly string _message;

        public override string Message
        {
            get { return "Can;t find Java"; }
        }

        public NJE()
        {
        }

        public NJE(string message)
        {
            _message = message;
        }
    }
}
