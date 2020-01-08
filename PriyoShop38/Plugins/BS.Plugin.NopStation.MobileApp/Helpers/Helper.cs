using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using Nop.Admin.Models.Messages;
using System.Data;
using LumenWorks.Framework.IO.Csv;

namespace BS.Plugin.NopStation.MobileApp.Helpers
{
	public static class Helper
	{

		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the conditions.
		/// </summary>
		/// <param name="criteria">The criteria.</param>
		/// <returns></returns>
		public static string GetConditions(string criteria = "")
		{
			string[] criterias = criteria.Split(',');
			List<string> tempCustomerWhere = new List<string>();
			List<string> tempNewsLetterWhere = new List<string>();
			List<string> tempCustomerRoleWhere = new List<string>();
			List<string> tempOthersWhere = new List<string>();
			//newsLetterWhere;
			for(var i = 0; i < criterias.Length; i++)
			{
				string tableName = criterias[i].Split('.')[0];
				if(tableName == "Customer")
				{
					tempCustomerWhere.Add(criterias[i]);
				}
				else if(tableName == "NewsLetterSubscription")
				{
					tempNewsLetterWhere.Add(criterias[i]);
				}
				else if(tableName == "CustomerRole")
				{
					tempCustomerRoleWhere.Add(criterias[i]);
				}
				else if(tableName == "GenericAttribute")
				{
					tempOthersWhere.Add(criterias[i]);
				}
			}

			string customerWhere = "";
			string newsLetterWhere = "";
			string customerRoleWhere = "";
			string othersWhere = "";

			if(tempCustomerWhere.Count > 0)
			{
				foreach(var item in tempCustomerWhere)
				{
					customerWhere += ReplaceSqlSign(item) + " ";//customerWhere += ReplaceSqlSign(item.Substring(item.IndexOf('.') + 1)) + " ";
				}
			}

			if(tempNewsLetterWhere.Count > 0)
			{
				foreach(var item in tempNewsLetterWhere)
				{
					newsLetterWhere += ReplaceSqlSign(item) + " ";//newsLetterWhere += ReplaceSqlSign(item.Substring(item.IndexOf('.') + 1)) + " ";
				}
			}

			if(tempCustomerRoleWhere.Count > 0)
			{
				foreach(var item in tempCustomerRoleWhere)
				{
					customerRoleWhere += ReplaceSqlSign(item) + " ";//customerRoleWhere += ReplaceSqlSign(item.Substring(item.IndexOf('.') + 1)) + " ";
				}
			}

			if(tempOthersWhere.Count > 0)
			{
				foreach(var item in tempOthersWhere)
				{
					othersWhere += ReplaceSqlSign(item.Substring(item.IndexOf('.') + 1), true) + " ";
				}
			}

			//string test = customerWhere + " " + newsLetterWhere;


			newsLetterWhere = (string.IsNullOrEmpty(newsLetterWhere)) ? newsLetterWhere : newsLetterWhere.Remove(newsLetterWhere.Length - 4);

			customerRoleWhere = (string.IsNullOrEmpty(customerRoleWhere)) ? customerRoleWhere : customerWhere + customerRoleWhere.Remove(customerRoleWhere.Length - 4);
			othersWhere = (string.IsNullOrEmpty(othersWhere)) ? othersWhere : customerWhere + othersWhere.Remove(othersWhere.Length - 4);

			customerWhere = (string.IsNullOrEmpty(customerWhere)) ? customerWhere : customerWhere.Remove(customerWhere.Length - 4);

			string conditionString = String.Format("{0}^{1}^{2}^{3}", customerWhere, newsLetterWhere, customerRoleWhere, othersWhere);

			return conditionString;
		}

		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Replaces the SQL sign.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="genericAttribute">if set to <c>true</c> [generic attribute].</param>
		/// <returns></returns>
		private static string ReplaceSqlSign(string item, bool genericAttribute = false)
		{
			string filter = item.Split('^')[0];
			string condition = item.Split('^')[1];
			string keyword = item.Split('^')[2];
			string andOr = item.Split('^')[3];

			if(condition == "Is Equal To")
			{
				condition = " = ";
				keyword = "'" + keyword + "'";
			}
			else if(condition == "Begin With")
			{
				condition = " Like ";
				keyword = "'" + keyword + "%'";
			}
			else if(condition == "Contains")
			{
				condition = " Like ";
				keyword = "'%" + keyword + "%'";
			}
			else if(condition == "Does Not Contain")
			{
				condition = " != ";
				keyword = "'" + keyword + "'";
			}
			else if(condition == "Greater Than")
			{
				condition = " > ";
				keyword = "'" + keyword + "'";
			}
			else if(condition == "Less Than")
			{
				condition = " < ";
				keyword = "'" + keyword + "'";
			}

			if(filter.Contains("Created on"))
			{
				filter = filter.Split('.')[0].Equals("Customer") ? "Customer.CreatedOnUtc" : "CreatedOnUtc";
			}
			string finalString;
			if(!genericAttribute)
			{
				finalString = filter + " " + condition + " " + keyword + " " + andOr;
			}
			else
			{
				finalString = "[Key] = '" + filter + "' And Value " + condition + " " + keyword + " " + andOr;  //[Key] = 'JoinWineLoversClub' And Value = 'True' Or [Key] = 'City' And Value Like '%darien%'
			}

			return finalString;
		}


		private static DataTable ReadCsvFile(string file)
		{
			String[] csvData = File.ReadAllLines(file);
			DataTable csvDataTable = new DataTable();
			DataRow row;
			//string column = "";
			Dictionary<string, int> d = new Dictionary<string, int>();

			using(CsvReader csv = new CsvReader(new StreamReader(file), true))
			{
				int fieldCount = csvData[0].Split(',').Length;
				string[] headers = csvData[0].Replace("\"", "").Split(',');

				foreach(string str in headers)
				{
					csvDataTable.Columns.Add(str);
				}

				while(csv.ReadNextRecord())
				{
					string[] rowData = new string[fieldCount];
					for(int i = 0; i < fieldCount; i++)
					{
						rowData[i] = csv[i];
					}
					row = csvDataTable.NewRow();
					for(int i = 0; i < fieldCount; i++)
					{
						row[i] = rowData[i];
					}
					csvDataTable.Rows.Add(row);
				}
			}
			csvDataTable.AcceptChanges();
			return csvDataTable;
		}

		

	}
}