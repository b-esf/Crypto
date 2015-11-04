using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ACL_Demo
{
    class ACL
    {
        private List<Rule> access_list;

        public ACL(List<Rule> acl)
        {
            access_list = acl;
        }

        //Verify each packet against the ACL
        public void VerifyPacket(Packet packet)
        {
            bool packetPermitted = false;
            foreach (Rule tempRule in access_list)
            {
                if (addrIsMatch(tempRule.source, packet.source, tempRule.mask))
                {
                    //see if rule is for permit or deny
                    bool access = false;
                    if (tempRule.permission == "deny")
                        access = false;
                    else if (tempRule.permission == "permit")
                        access = true;
                    else
                        throw new Exception("Invalid rule permission - must be \'Permit\' or \'Deny\'");

                    if (access)
                    {
                        packetPermitted = true;
                    }
                    else
                        break;
                }
            }            
            Console.WriteLine("Source: " + packet.source + " Destination: " + packet.destination + " Access: " + (packetPermitted ? "Permit" : "Deny"));
        }

        //helper method that compares the source addresses of the rule and the packet using the rule's mask
        public bool addrIsMatch(string addr1, string addr2, string mask)
        {
            //rule source IP
            string[] arr1 = addr1.Split('.');
            //packet source IP
            string[] arr2 = addr2.Split('.');
            //rule mask
            string[] sourceMask = mask.Split('.');

            //run through each byte of the IP and compare
            bool match = false;
            for (int i = 0; i < arr1.Length; i++)
            {
                //if it's 0, then it must match exactly
                //else it can match anything
                if (sourceMask[i] == "0")
                {
                    match = (arr1[i] == arr2[i]);
                    if (!match)
                        break;
                }
                else
                {
                    match = true;
                }
            }
            return match;
        }
    }
}
