namespace NBCAssembler
{
    public class NBCArg
    {
        public int Value { set; get; }
        public bool Long {get;set;}
        public NBCArgType Type { set; get; }
        /// <summary>
        /// Attempt to parse an argument
        /// </summary>
        /// <param name="str">The string to attempt to parse</param>
        /// <returns>true if possible to parse, false if not</returns>
        public static bool TryParse(string str)
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
                if (!int.TryParse(i.Substring(2), out int j))
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
        /// Parse a string
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <returns>NBCArg from the string passed</returns>
        public static NBCArg Parse(string str)
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
                    if (int.TryParse(i.Substring(2), out int j))
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
