namespace PIM.Code.Common
{
    /// <summary>
    /// A Common class to process string and number
    /// </summary>
    public static class NumberCommon
    {
        /// <summary>
        /// Convert string to double
        /// </summary>
        /// <param name="stringNum">
        /// The string num.
        /// </param>
        /// <returns>
        /// A double vaule, if convert failed, 0 will be returned
        /// </returns>
        public static double StringToDouble(string stringNum)
        {
            double result;
            double.TryParse(stringNum, out result);
            return result;
        }

        /// <summary>
        /// Convert string to int
        /// </summary>
        /// <param name="stringNum">
        /// The string num.
        /// </param>
        /// <returns>
        /// An int value, if convert failed, 0 will be returned
        /// </returns>
        public static int StringToInt(string stringNum)
        {
            int result;
            int.TryParse(stringNum, out result);
            return result;
        }

        public static double StringToDouble(string stringNum, out bool isDouble)
        {
            double result;
            isDouble = double.TryParse(stringNum, out result);
            return result;
        }

        public static int StringToInt(string stringNum, out bool isDouble)
        {
            int result;
            isDouble = int.TryParse(stringNum, out result);
            return result;
        }
    }
}
