// <copyright file="Message.cs" company="Carl Zeiss"> 
// Copyright Carl Zeiss 2008, 2022 All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnmpParser
{
    /// <summary>
    /// Class that parses the SNMP V1 message
    /// </summary>
    public class Message
    {
        #region fields

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
        private int length;

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
        private int totalLength;

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets message length
        /// </summary>
        public int Length
        {
            get
            {
                return length;
            }

            set
            {
                if (value != length)
                {
                    length = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets message total length
        /// </summary>
        public int TotalLength
        {
            get
            {
                return totalLength;
            }

            set
            {
                if (value != length)
                {
                    totalLength = value;
                }
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Gets byte value at specific offset
        /// </summary>
        /// <param name="data">data stream</param>
        /// <param name="offset">offset to specific byte</param>
        /// <returns>byte value at specific offset</returns>
        public byte GetByteAtOffset(byte[] data, ref int offset)
        {
            return data[offset++];
        }

        /// <summary>
        /// Parses length from byte stream 
        /// </summary>
        /// <param name="data">data stream</param>
        /// <param name="offset">offset counter</param>
        /// <returns>returns length value</returns>
        public int GetLength(byte[] data, ref int offset)
        {
            byte next = GetByteAtOffset(data, ref offset);
            int length = 0;
            if ((next & 0x80) == 0)
            {
                length = (int)next;
            }
            else
            {
                int numberOfBytesInLength = (int)(next & 0x7F);

                for (int ii = 0; ii < numberOfBytesInLength; ii++)
                {
                    byte b = GetByteAtOffset(data, ref offset);
                    length = length << 8;
                    length |= b;
                }
            }

            return length;
        }

        /// <summary>
        /// Parses out object identifier 
        /// </summary>
        /// <param name="data">data stream</param>
        /// <param name="offset">offset to object identifier</param>
        /// <returns>object identifier</returns>
        public string ReadOid(byte[] data, ref int offset)
        {
            byte x = GetByteAtOffset(data, ref offset);

            if (x != 0x06)
            {
                string error = string.Format("expected value for OID is 0x06. but it is  - {0:X} ", x);
                throw new Exception(error);
            }

            int oidlength = GetLength(data, ref offset);

            x = GetByteAtOffset(data, ref offset);

            if (x != 0x2b)
            {
                string error = string.Format("expected value for beginning of oid is 0x2b. but it is {0:X}", x);
                throw new Exception(error);
            }

            List<int> enterprise_oid = new List<int>();

            enterprise_oid.Add(1);
            enterprise_oid.Add(3);

            for (int ii = 0; ii < oidlength - 1; ii++)
            {
                x = GetByteAtOffset(data, ref offset);

                if ((x & 0x80) == 0)
                {
                    enterprise_oid.Add(x);
                }
                else
                {
                    int val = x & 0x7F;

                    do
                    {
                        val = val << 7;
                        x = GetByteAtOffset(data, ref offset);
                        val |= x & 0x7F;
                        ii = ii + 1;
                    }
                    while ((x & 0x80) != 0);

                    enterprise_oid.Add(val);
                }
            }

            StringBuilder oid = new StringBuilder();

            if (enterprise_oid.Count > 0)
            {
                foreach (var ii in enterprise_oid)
                {
                    oid.Append(ii.ToString());
                    oid.Append('.');
                }

                oid.Remove(oid.Length - 1, 1);
            }

            return oid.ToString();
        }

        /// <summary>
        /// Parses out object identifier
        /// </summary>
        /// <param name="data">byte stream</param>
        /// <param name="offset">offset to object identifier</param>
        /// <returns>object identifier</returns>
        public string ReadOidString(byte[] data, ref int offset)
        {
            int oidlength = GetLength(data, ref offset);

            byte x = GetByteAtOffset(data, ref offset);

            if (x != 0x2b)
            {
                string error = string.Format("expected value for beginning of oid is 0x2b. but it is 0x {0:X}", x);
                throw new Exception(error);
            }

            List<int> enterprise_oid = new List<int>();

            enterprise_oid.Add(1);
            enterprise_oid.Add(3);

            for (int ii = 0; ii < oidlength - 1; ii++)
            {
                x = GetByteAtOffset(data, ref offset);

                if ((x & 0x80) == 0)
                {
                    enterprise_oid.Add((int)x);
                }
                else
                {
                    int val = x & 0x7F;

                    do
                    {
                        val = val << 7;
                        x = GetByteAtOffset(data, ref offset);
                        val |= x & 0x7F;
                        ii = ii + 1;
                    }
                    while ((x & 0x80) != 0);

                    enterprise_oid.Add(val);
                }
            }

            StringBuilder oid = new StringBuilder();

            if (enterprise_oid.Count > 0)
            {
                foreach (var ii in enterprise_oid)
                {
                    oid.Append(ii.ToString());
                    oid.Append('.');
                }

                oid.Remove(oid.Length - 1, 1);
            }

            return oid.ToString();
        }

        /// <summary>
        /// Parses address from byte stream
        /// </summary>
        /// <param name="data">data stream</param>
        /// <param name="offset">offset to address</param>
        /// <returns>address value</returns>
        public string ReadAddress(byte[] data, ref int offset)
        {
            byte x = GetByteAtOffset(data, ref offset);

            if (x != 0x40)
            {
                string error = string.Format("expected address is 0x40. but it is - 0x{0:X}", (int)x);
                throw new Exception(error);
            }

            StringBuilder address = new StringBuilder();
            int length = GetLength(data, ref offset);

            if (length > 0)
            {
                for (int ii = 0; ii < length; ii++)
                {
                    x = GetByteAtOffset(data, ref offset);

                    address.Append((int)x);
                    address.Append('.');
                }

                address.Remove(address.Length - 1, 1);
            }

            return address.ToString();
        }

        /// <summary>
        /// Parses integer from data stream
        /// </summary>
        /// <param name="data">data stream</param>
        /// <param name="offset">offset to integer</param>
        /// <returns>integer value</returns>
        public int ReadInteger(byte[] data, ref int offset)
        {
            byte x = GetByteAtOffset(data, ref offset);

            if (x != 0x2)
            {
                string error = string.Format("expected INTEGER is 0x2. but it is - {0:X}", x);
                throw new Exception(error);
            }

            int length = GetLength(data, ref offset);
            int value = 0;

            for (int ii = 0; ii < length; ii++)
            {
                x = GetByteAtOffset(data, ref offset);
                value |= x;

                if (ii < length - 1)
                {
                    value = value << 8;
                }
            }

            return value;
        }

        /// <summary>
        /// Parses time ticks from data stream
        /// </summary>
        /// <param name="data">data stream</param>
        /// <param name="offset">offset to time ticks</param>
        /// <returns>time ticks as string</returns>
        public string ReadTimeticks(byte[] data, ref int offset)
        {
            byte x = GetByteAtOffset(data, ref offset);

            if (x != 0x43)
            {
                string error = string.Format("expected TIME_STAMP is 0x43. but it is - {0:X}", x);
                throw new Exception(error);
            }

            int length = GetLength(data, ref offset);
            StringBuilder ticks = new StringBuilder();

            for (int ii = 0; ii < length; ii++)
            {
                x = GetByteAtOffset(data, ref offset);
                ticks.Append(x.ToString());
            }

            return ticks.ToString();
        }

        /// <summary>
        /// checks if byte at offset is equal to input byte value
        /// </summary>
        /// <param name="data">data stream</param>
        /// <param name="offset">offset to byte in data stream</param>
        /// <param name="input">input byte</param>
        /// <returns>True if bytes are equal</returns>
        public bool IsByteEqual(byte[] data, ref int offset, byte input)
        {
            byte x = GetByteAtOffset(data, ref offset);

            return x == input;
        }

        /// <summary>
        /// Parses integer as string from data stream
        /// </summary>
        /// <param name="data">data stream</param>
        /// <param name="offset">offset to integer</param>
        /// <returns>integer value as string</returns>
        public string ReadIntegerString(byte[] data, ref int offset)
        {
            int intLength = GetLength(data, ref offset);
            byte x = GetByteAtOffset(data, ref offset);
            int value = x;

            for (int ii = 1; ii < intLength; ii++)
            {
                value = value << 8;
                x = GetByteAtOffset(data, ref offset);
                value |= x;
            }

            string intvalue = x.ToString();

            return intvalue;
        }

        /// <summary>
        /// Parses "VarBind" sequence from data stream
        /// </summary>
        /// <param name="data">data stream</param>
        /// <param name="offset">offset to sequence of "varbind" structures</param>
        /// <returns>sequence of sturctures</returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
        public VarBindSequence ReadVarbindSequence(byte[] data, ref int offset)
        {
            byte x = GetByteAtOffset(data, ref offset);

            if (x != 0x30)
            {
                string error = string.Format("expected SEQUENCE is 0x30. but it is - {0:X}", x);
                throw new Exception(error);
            }

            int length = GetLength(data, ref offset);

            string oid = ReadOid(data, ref offset);

            x = GetByteAtOffset(data, ref offset);

            string val = string.Empty;

            if (x == 0x04)
            {
                val = GetOctetString(data, ref offset);
            }
            else if (x == 0x02)
            {
                val = ReadIntegerString(data, ref offset);
            }
            else if (x == 0x06)
            {
                val = ReadOidString(data, ref offset);
            }

            VarBindSequence sequence = new VarBindSequence();

            sequence.Set(oid, val);

            return sequence;
        }

        #endregion

        #region functions

        /// <summary>
        /// Gets octet as string from data stream
        /// </summary>
        /// <param name="data">data stream</param>
        /// <param name="offset">offset to octet string</param>
        /// <returns>octet as string</returns>
        private string GetOctetString(byte[] data, ref int offset)
        {
            int stringLength = GetLength(data, ref offset);

            byte x;

            StringBuilder ret = new StringBuilder();

            for (int ii = 0; ii < stringLength; ii++)
            {
                x = GetByteAtOffset(data, ref offset);

                ret.Append(Convert.ToChar(x));
            }

            return ret.ToString();
        }

        #endregion
    }
}
