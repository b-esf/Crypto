using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACL_Demo
{
    //Packet class
    //keeps track of the packet's source and destination IPs
    class Packet
    {        
        private string _source; 
        private string _destination;
        public string source { get { return _source; } set { _source = value; } }
        public string destination { get { return _destination; } set { _destination = value; } }

        public Packet(string source, string destination)
        {
            this._source = source;
            this._destination = destination;
        }
    }
}
