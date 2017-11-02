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
        Indirect
    }
    class NBCCommand
    {

    }

    class NBCArg
    {
        int Value { set; get; }
        NBCArgType Type { set; get; }
        /// <summary>
        /// Attempt to parse an argument
        /// </summary>
        /// <param name="str">The string to attempt to parse</param>
        /// <returns>true if possible to parse, false if not</returns>
        static bool TryParse(string str)
        {
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static NBCArg Parse(string str)
        {
            str.Trim();
            if (str == "")
            {
                throw new System.FormatException("String is in the incorrect format");
            }
            string[] words = str.Split();
            foreach (string i in words)
            {
                if (!i.StartsWith("")) ;
            }
            return null;
        }
    }
}
