using System;
using System.Collections.Generic;
using System.Text;

namespace NBCAssembler
{
    enum NBCArgType
    {
        Constant,
        Address,
        Direct,
        Indirect,
        Label
    }
    class NBCCommand
    {

    }

    class NBCArg
    {
        int Value { set; get; }
        bool Long {get;set;}
        NBCArgType Type { set; get; }
        /// <summary>
        /// Attempt to parse an argument
        /// </summary>
        /// <param name="str">The string to attempt to parse</param>
        /// <returns>true if possible to parse, false if not</returns>
        static bool TryParse(string str)
        {
            str.Trim();
            if (str == "")
            {
                return false;
            }
            string[] words = str.Split();
            foreach (string i in words)
            {
                //Get the argument type
                switch (i.Substring(0,1))
                {
                    case "%":
                    case "$":
                    case "#":
                    case ":":
                        break;
                    default:
                        return false;
                }
                //Get the argument length
                switch (i.Substring(1,1).ToLower())
                {
                    case "s":
                    case "l":
                        break;
                    default:
                        return false;
                }
                int j;
                if (!int.TryParse(i.Substring(2), out j))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static NBCArg Parse(string str)
        {
            NBCArg tmp = new NBCArg();
            str.Trim();
            if (str == "")
            {
                throw new System.FormatException("String is in the incorrect format");
            }
            string[] words = str.Split();
            foreach (string i in words)
            {
                //Get the argument type
                switch (i.Substring(0,1))
                {
                    case "%":
                        tmp.Type = NBCArgType.Constant;
                        break;
                    case "$":
                        tmp.Type = NBCArgType.Address;
                        break;
                    case "#":
                        tmp.Type = NBCArgType.Indirect;
                        break;
                    //Note: Argument type Label returns a value of 0
                    case ":":
                        tmp.Type = NBCArgType.Label;
                        break;
                    default:
                        throw new System.FormatException("String is in the incorrect format");
                }
                //Get the argument length
                switch (i.Substring(1,1).ToLower())
                {
                    case "s":
                        tmp.Long = false;
                        break;
                    case "l":
                        tmp.Long = true;
                        break;
                    default:
                        throw new System.FormatException("String is in the incorrect format");
                }
                //Get the argument value
                if (tmp.Type == NBCArgType.Label)
                {
                    tmp.Value = 0;
                }
                else
                {
                    int j;
                    if (int.TryParse(i.Substring(2), out j))
                    {
                        tmp.Value = j;
                    }
                    else
                    {
                        throw new System.FormatException("String is in the incorrect format");
                    }
                }

            }
            return tmp;
        }
    }
}
