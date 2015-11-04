using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ACL_Demo
{
    class Program
    {
        //Main body of the program
        //It reads in the rules and packet info from file.
        //Then it determines whether the packet should be allowed permitted or denied.
        static void Main(string[] args)
        {
            ACL accessList = readACL();
            List<Packet> packetList = readPackets();

            Console.WriteLine("For ACL:");
            Console.WriteLine(File.ReadAllText(@"acl.txt"));
            Console.WriteLine();
            
            foreach (Packet pk in packetList)
            {
                accessList.VerifyPacket(pk);
            }
            Console.Read();
        }

        //read ACL from file and return an ACL object
        static ACL readACL()
        {
            string line;            
            List<Rule> tempACL = new List<Rule>();
            using (StreamReader acl = new StreamReader(@"acl.txt"))
            {
                while ((line = acl.ReadLine()) != null)
                {
                    if (line.Contains(@"access-list"))
                    {
                        List<string> tempList = line.Split(' ').ToList();
                        string aclNum = tempList[1];
                        string permission = tempList[2];
                        string source = tempList[3];
                        string mask = tempList[4];
                        Rule tempRule = new Rule(aclNum, permission, source, mask);
                        tempACL.Add(tempRule);
                    } 
                }
            }
            ACL accessList = new ACL(tempACL);
            return accessList;
        }

        //read packets from file and return a list of packet objects
        static List<Packet> readPackets()
        {
            string line;
            List<Packet> packets = new List<Packet>();
            using (StreamReader pack = new StreamReader(@"packets.txt"))
            {
                while ((line = pack.ReadLine()) != null)
                {
                    string source = line.Substring(0, line.IndexOf(" "));
                    string destination = line.Substring(line.IndexOf(" "));
                    Packet tempPacket = new Packet(source, destination);
                    packets.Add(tempPacket);
                }
            }

            return packets;
        }
    }
}
