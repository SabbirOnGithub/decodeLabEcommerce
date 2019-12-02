using System.Collections.Generic;
using System.Web.Mvc;

namespace BS.Plugin.NopStation.MobileApp.Helpers
{
	/// <summary>
	/// All of these following classes are used for creating combobox list in add/edit smart groups
	/// </summary>
	public static class ComboList
	{
		public static IEnumerable<SelectListItem> ColumnList
		{
			get
			{
				return new List<SelectListItem>
				       {
				       	{new SelectListItem{Selected = true,Text = "Username", Value = "Customer.Username"}},
				       	{new SelectListItem{Selected = true,Text = "Email", Value = "Customer.Email"}},
								{new SelectListItem{Selected = true,Text = "Active", Value = "Customer.Active"}},
				       	{new SelectListItem{Selected = true,Text = "Customer Created on", Value = "Customer.CreatedOnUtc"}},
								{new SelectListItem{Selected = true,Text = "Last Login Date", Value = "Customer.LastLoginDateUtc"}},
				       	{new SelectListItem{Selected = true,Text = "Last Activity Date", Value = "Customer.LastActivityDateUtc"}},
								
								{new SelectListItem{Selected = true,Text = "Active", Value = "NewsLetterSubscription.Active"}},
				       	{new SelectListItem{Selected = true,Text = "NewsLetter Created on", Value = "NewsLetterSubscription.CreatedOnUtc"}},
								{new SelectListItem{Selected = true,Text = "NewsLetter Email", Value = "NewsLetterSubscription.Email"}},
								{new SelectListItem{Selected = true,Text = "Daily", Value = "NewsLetterSubscription.Daily"}},
				       	{new SelectListItem{Selected = true,Text = "Weekly", Value = "NewsLetterSubscription.Weekly"}},
								
								{new SelectListItem{Selected = true,Text = "Name", Value = "CustomerRole.Name"}},
								{new SelectListItem{Selected = true,Text = "FreeShipping", Value = "CustomerRole.FreeShipping"}},
								{new SelectListItem{Selected = true,Text = "TaxExempt", Value = "CustomerRole.TaxExempt"}},
								{new SelectListItem{Selected = true,Text = "Active", Value = "CustomerRole.Active"}},
								{new SelectListItem{Selected = true,Text = "IsSystemRole", Value = "CustomerRole.IsSystemRole"}},
								{new SelectListItem{Selected = true,Text = "SystemName", Value = "CustomerRole.SystemName"}},
								{new SelectListItem{Selected = true,Text = "StoreRole", Value = "CustomerRole.StoreRole"}},
								{new SelectListItem{Selected = true,Text = "StoreId", Value = "CustomerRole.StoreId"}},

								{new SelectListItem{Selected = true,Text = "City", Value = "GenericAttribute.City"}},
								{new SelectListItem{Selected = true,Text = "JoinWineLoversClub", Value = "GenericAttribute.JoinWineLoversClub"}},
								{new SelectListItem{Selected = true,Text = "Active", Value = "GenericAttribute.Active"}},

								/*{new SelectListItem{Selected = true,Text = "Role", Value = "Customer.Role"}},
				       	{new SelectListItem{Selected = true,Text = "Created on", Value = "Customer.Created on"}},*/
				       };
			}
		}

		public static IEnumerable<SelectListItem> ConditionList
		{
			get
			{
				return new List<SelectListItem>
				       {
				       	{new SelectListItem{Selected = true,Text = "Is Equal To", Value = "Is Equal To"}},
				       	{new SelectListItem{Selected = true,Text = "Begin With", Value = "Begin With"}},
								{new SelectListItem{Selected = true,Text = "Contains", Value = "Contains"}},
								{new SelectListItem{Selected = true,Text = "Does not Contain", Value = "Does not Contain"}},
								{new SelectListItem{Selected = true,Text = "Greater Than", Value = "Greater Than"}},
								{new SelectListItem{Selected = true,Text = "Less Than", Value = "Less Than"}},
								/*{new SelectListItem{Selected = true,Text = "Is Blank", Value = "Is Blank"}},
								{new SelectListItem{Selected = true,Text = "Is not Blank", Value = "Is not Blank"}},*/
				       };
			}
		}

		public static IEnumerable<SelectListItem> AndOrList
		{
			get
			{
				return new List<SelectListItem>
				       {
				       	{new SelectListItem{Selected = true,Text = "And", Value = "And"}},
				       	{new SelectListItem{Selected = true,Text = "Or", Value = "Or"}},
				       };
			}
		}

		public static IEnumerable<SelectListItem> ScheduleType
		{
			get
			{
				return new List<SelectListItem>
				       {
				       	{new SelectListItem{Selected = true,Text = "Once", Value = "Once"}},
				       	{new SelectListItem{Selected = false,Text = "Daily", Value = "Daily"}},
								{new SelectListItem{Selected = false,Text = "Weekly", Value = "Weekly"}},
				       };
			}
		}

	}

}
