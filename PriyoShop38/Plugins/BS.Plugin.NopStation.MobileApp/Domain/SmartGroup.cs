using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileApp.Domain
{
    public class SmartGroup : BaseEntity
    {

        
        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        /// <value>
        /// The Name.
        /// </value>
        public virtual string Name { get; set; }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the key word.
        /// </summary>
        /// <value>The key word.</value>
        public virtual string KeyWord { get; set; }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public virtual string Columns { get; set; }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        /// <value>The conditions.</value>
        public virtual string Conditions { get; set; }


        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the and or.
        /// </summary>
        /// <value>The and or.</value>
        public virtual string AndOr { get; set; }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the Query.
        /// </summary>
        /// <value>
        /// The Query.
        /// </value>
        public virtual string Query { get; set; }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the is deleted.
        /// </summary>
        /// <value>The is deleted.</value>
        public virtual bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of Device creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// Gets or sets the date and time of Device update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }



    }
}
