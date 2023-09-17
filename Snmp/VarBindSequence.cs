using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnmpParser
{
    /// <summary>
    /// Variable / object identifier binding class
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    public class VarBindSequence
    {
        private string value;
        private string oid;

        #region properties

        /// <summary>
        /// Gets variable object identifier
        /// </summary>
        public string Oid
        {
            get
            {
                return oid;
            }
        }

        /// <summary>
        /// Gets object value
        /// </summary>
        public string Value
        {
            get
            {
                return value;
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Binds object identifier and value
        /// </summary>
        /// <param name="in_oid">object identifier</param>
        /// <param name="in_val">object value</param>
        public void Set(string in_oid, string in_val)
        {
            oid = in_oid;
            value = in_val;
        }

        #endregion
    }
}
