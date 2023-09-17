// <copyright file="TrapV1.cs" company="Carl Zeiss"> 
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
    /// TrapV1 class
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    public class TrapV1 : Message
    {
        #region fields
        private byte[] data;
        private string enterpriseOid;
        private string agentAddress;
        private int genericTrapType;
        private int specificTrapType;
        private string timeStamp;
        private int varbindLength;
        private List<VarBindSequence> sequences = new List<VarBindSequence>();

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TrapV1" /> class
        /// </summary>
        /// <param name="inp">byte stream</param>
        public TrapV1(byte[] inp)
        {
            data = inp;

            int current = 0;
            byte b = GetByteAtOffset(data, ref current);

            if (b != TypeIdentifiers.TRAPV1)
            {
                string error = string.Format("expected TRAPV1 (0xa4) for trap identifier, but it is - {0:X}", b);
                throw new Exception(error);
            }

            Length = GetLength(data, ref current);
            TotalLength = Length + current;

            enterpriseOid = ReadOid(data, ref current);
            agentAddress = ReadAddress(data, ref current);
            genericTrapType = ReadInteger(data, ref current);
            specificTrapType = ReadInteger(data, ref current);
            timeStamp = ReadTimeticks(data, ref current);

            if (IsByteEqual(data, ref current, 0x30))
            {
                varbindLength = GetLength(data, ref current);

                int current_mem = current;

                while ((current - current_mem) < varbindLength)
                {
                    VarBindSequence sequence = ReadVarbindSequence(data, ref current);
                    sequences.Add(sequence);
                }
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets enterprise object identifier
        /// </summary>
        public string EnterppriseOid
        {
            get
            {
                return enterpriseOid;
            }
        }

        /// <summary>
        /// Gets agent address
        /// </summary>
        public string AgentAddress
        {
            get
            {
                return agentAddress;
            }
        }

        /// <summary>
        /// Gets generic trap type
        /// </summary>
        public int GenericTrapType
        {
            get
            {
                return genericTrapType;
            }
        }

        /// <summary>
        /// Gets specific trap type
        /// </summary>
        public int SpecificTrapType
        {
            get
            {
                return specificTrapType;
            }
        }

        /// <summary>
        /// Gets time stamp
        /// </summary>
        public string TimeStamp
        {
            get
            {
                return timeStamp;
            }
        }

        /// <summary>
        /// Gets a list of variable/identifier sequences
        /// </summary>
        public List<VarBindSequence> Sequences
        {
            get
            {
                return sequences;
            }
        }

        #endregion
    }
}
