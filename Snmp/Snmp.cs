// <copyright file="Snmp.cs" company="Carl Zeiss"> 
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
    /// Snmp message
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    public class Snmp : Message
    {
        #region fields
        
        private int snmpVersion;
        private string community;
        private byte[] msg;

        private TrapV1 trap;
        
        #endregion

        #region construction/desctruction

        /// <summary>
        /// Initializes a new instance of the <see cref="Snmp"/> class.
        /// </summary>
        /// <param name="data">data stream</param>
        public Snmp(byte[] data)
        {
            int current = 0;
            byte b = GetByteAtOffset(data, ref current);

            if (b != TypeIdentifiers.SEQUENCE)
            {
                string error = string.Format("expected SEQUENCE (0x30) for snmp message, but it is - {0:X}", b);
                throw new Exception(error);
            }

            Length = GetLength(data, ref current);

            TotalLength = Length + current;

            int after_length = current;
            snmpVersion = GetVerion(data, ref current);
            community = GetCommunity(data, ref current);

            int traplength = Length - (current - after_length);

            msg = new byte[traplength];
            Array.Copy(data, current, msg, 0, traplength);

            trap = new TrapV1(msg);
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets snmp version
        /// </summary>
        public int SnmpVersion
        {
            get
            {
                return snmpVersion;
            }
        }

        /// <summary>
        /// Gets community value
        /// </summary>
        public string Community
        {
            get
            {
                return community;
            }
        }

        /// <summary>
        /// Gets Trap structure
        /// </summary>
        public TrapV1 Trap
        {
            get
            {
                return trap;
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Gets an SNMP version frin data strean
        /// </summary>
        /// <param name="data">data stream</param>
        /// <param name="offset">offset to version</param>
        /// <returns>snmp version value</returns>
        public int GetVerion(byte[] data, ref int offset)
        {
            byte x = GetByteAtOffset(data, ref offset);

            if (x != TypeIdentifiers.INTEGER)
            {
                string error = string.Format("expected integer for snmp version, but it is - {0:X}", x);
                throw new Exception(error);
            }

            int version = 0;
            version = GetByteAtOffset(data, ref offset);
            version |= GetByteAtOffset(data, ref offset) << 8;

            return version;
        }

        /// <summary>
        /// Parses community string from data stream
        /// </summary>
        /// <param name="data">data stream</param>
        /// <param name="offset">offset to community value</param>
        /// <returns>community string</returns>
        public string GetCommunity(byte[] data, ref int offset)
        {
            byte x = GetByteAtOffset(data, ref offset);

            if (x != TypeIdentifiers.OCTET_STRING)
            {
                string error = string.Format("expected octet string for community (0x04), but it is - {0:X}", x);
                throw new Exception(error);
            }

            int length = GetLength(data, ref offset);

            StringBuilder c = new StringBuilder();

            for (int ii = 0; ii < length; ii++)
            {
                x = GetByteAtOffset(data, ref offset);
                c.Append(Convert.ToChar(x));
            }

            return c.ToString();
        }

        #endregion
    }
}
