using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTCPServer
{
    class DetailedMessage
    {
        public string Message { get; }
        public string Name { get; }
        public int Length { get; }

        public DetailedMessage(string sender, string text)
        {
            Message = text;
            Name = text;
            Length = ToString().Length;
        }
        public override string ToString()
        {
            return Name + ": " + Message;
        }

        public static implicit operator DetailedMessage(string m)
        {
            //m = "name: message"
            string name = m.Split(':')[0];
            string text = m.Split(':')[1];
            text = text.Split(' ')[1];
            DetailedMessage dm = new DetailedMessage(name, text);
            return dm;
        }
        public static implicit operator byte[](DetailedMessage dm)
        {
            return Encoding.ASCII.GetBytes("-" + dm.ToString());
        }

        }
}
