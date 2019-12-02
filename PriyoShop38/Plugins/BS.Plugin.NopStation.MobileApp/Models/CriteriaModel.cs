using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BS.Plugin.NopStation.MobileApp.Validators;
using Nop.Web.Framework.Mvc;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Nop.Web.Framework;

namespace BS.Plugin.NopStation.MobileApp.Models
{
	[Validator(typeof(SmartGroupValidator))]

	public class CriteriaModel : BaseNopEntityModel
  {
		
		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[NopResourceDisplayName("Admin.Plugin.BsNotification.Groups.Fields.Name")]
		public string Name { get; set; }

		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the key word.
		/// </summary>
		/// <value>The key word.</value>
		public string KeyWord { get; set; }

		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the columns.
		/// </summary>
		/// <value>The columns.</value>
		public string Columns { get; set; }

		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the conditions.
		/// </summary>
		/// <value>The conditions.</value>
		public string Conditions { get; set; }

		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the and or.
		/// </summary>
		/// <value>The and or.</value>
		public string AndOr { get; set; }

		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the query.
		/// </summary>
		/// <value>The query.</value>
		public string Query { get; set; }


        
  }

}
