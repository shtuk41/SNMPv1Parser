using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnmpParser
{
    /// <summary>
    /// Type identifiers struct
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    public struct TypeIdentifiers
    {
        public const byte BOOLEAN = 0x01;
        public const byte INTEGER = 0x02;
        public const byte BIT_STRING = 0x03;
        public const byte OCTET_STRING = 0x04;
        public const byte DISPLAYSTRING = 0x04;
        public const byte NULLTI = 0x05;
        public const byte OBJECT_IDENTIFIER = 0x06;
        public const byte SEQUENCE = 0x30;
        public const byte IP_ADDRESS = 0x40;
        public const byte COUNTER32 = 0x41;
        public const byte GAUGE32 = 0x42;
        public const byte TIME_TICKS = 0x43;
        public const byte OPAQUE = 0x44;
        public const byte NSAP_ADDRESS = 0x45;
        public const byte COUNTER64 = 0x46;
        public const byte UINTEGER32 = 0x47;
        public const byte TRAPV1 = 0xA4;
    }
}
