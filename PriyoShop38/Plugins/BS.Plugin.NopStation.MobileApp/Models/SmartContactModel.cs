using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Web.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BS.Plugin.NopStation.MobileApp.Models
{
    public class SmartContactModel : BaseEntity
  {
		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the Customer id.
		/// </summary>
		/// <value>The customer id.</value>
		public int CustomerId { get; set; }

		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		/// <value>The name of the user.</value>
		public string UserName { get; set; }

		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>The email.</value>
		public string Email { get; set; }

		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>The created on.</value>
        public DateTime CreatedOnUtc { get; set; }


        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the FullName.
        /// </summary>
        /// <value>The FullName.</value>
        public string FullName { get; set; }
		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the first name.
		/// </summary>
		/// <value>The first name.</value>
		public string FirstName { get; set; }

		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the last name.
		/// </summary>
		/// <value>The last name.</value>
		public string LastName { get; set; }
  }

}
