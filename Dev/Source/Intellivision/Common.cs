using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Intellivision
{
    public class Common
    {
        public static UInt16 ConvertBitArrayToUInt16(BitArray bitArray)
        {
            Int32[] result = new Int32[1];

            //ReverseBitArray(ref bitArray);
            bitArray.CopyTo(result, 0);

            return (UInt16)result[0];
        }

        public static UInt16 GetNumberFromBits(UInt16 number, int startIndex, int length)
        {
            UInt16 result;

            BitArray bits = Common.ConvertUInt16ToBitArray(number);
            BitArray bitsOfInterest = new BitArray(length);
            int a = 0;
            for (int i = startIndex; i < startIndex + length; i++)
            {
                bitsOfInterest[a] = bits[i];
                a += 1;
            }

            result = Common.ConvertBitArrayToUInt16(bitsOfInterest);

            return result;
        }

        public static BitArray ConvertUInt16ToBitArray(UInt16 value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            BitArray bits = new BitArray(bytes);

            //ReverseBitArray(ref bits);

            return bits;
        }
    }
}
