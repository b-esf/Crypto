using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACL_Demo
{
    //Rule class
    //keeps track of the ACL number, deny or permit, source IP, and mask
    class Rule
    {
        private string _aclNum;
        private string _permission;
        private string _source;
        private string _mask;

        public string aclNum { get { return _aclNum; } set { _aclNum = value; } }
        public string permission { get { return _permission; } set { _permission = value; } }
        public string source { get { return _source; } set { _source = value; } }
        public string mask { get { return _mask; } set { _mask = value; } }

        public Rule(string aclNum, string permission, string source, string mask)
        {
            this._aclNum = aclNum;
            this._permission = permission;
            this._source = source;
            this._mask = mask;
        }

    }
}
