using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using Nop.Core.Data;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using BS.Plugin.NopStation.MobileApp.Domain;
using Nop.Services.Events;
using BS.Plugin.NopStation.MobileApp.Models;
using Nop.Core.Domain.Common;
using BS.Plugin.NopStation.MobileApp.Data;
using System.Data;
using BS.Plugin.NopStation.MobileApp.Helpers;

namespace BS.Plugin.NopStation.MobileApp.Services
{
    public partial class SmartGroupService : ISmartGroupService
    {
       
        #region fields

        private readonly IRepository<SmartGroup> _smartGroupsRepository;
				private readonly IRepository<GenericAttribute> _gaRepository;
              private readonly IEventPublisher _eventPublisher;
				//private readonly IDbContext _dbContext;
				private readonly AdminAreaSettings _adminAreaSettings;
                private readonly MobileAppObjectContext _dbContext;
				private readonly CommonSettings _commonSettings;
				private readonly IDataProvider _dataProvider;

        #endregion

        #region ctor

				public SmartGroupService(IRepository<SmartGroup> smartGroupsRepository,
                    IEventPublisher eventPublisher,
                    AdminAreaSettings adminAreaSettings,
                    MobileAppObjectContext dbContext, 
                    CommonSettings commonSettings,
                    IDataProvider dataProvider,
                    IRepository<GenericAttribute> gaRepository)
        {
					this._smartGroupsRepository = smartGroupsRepository;
					this._eventPublisher = eventPublisher;
					this._adminAreaSettings = adminAreaSettings;
					this._dbContext = dbContext;
					this._commonSettings = commonSettings;
					this._dataProvider = dataProvider;
					this._gaRepository = gaRepository;
        }

        #endregion

        #region Implementation of ISmartGroupsService

				///--------------------------------------------------------------------------------------------
				/// <summary>
				/// Gets all smart group.
				/// </summary>
				/// <param name="pageIndex">Index of the page.</param>
				/// <param name="pageSize">Size of the page.</param>
				/// <returns></returns>
				/// --------------------------------------------------------------------------------------------
				public virtual IPagedList<SmartGroup> GetAllSmartGroup(int pageIndex, int pageSize)
				{
					var query = (from u in _smartGroupsRepository.Table
											 where !u.IsDeleted
											 orderby u.Id
											 select u);
					var smartGroups = new PagedList<SmartGroup>(query, pageIndex, pageSize);
					return smartGroups;
				}
                public virtual IList<SmartGroup> GetAllSmartGroup()
                {
                    var query = (from u in _smartGroupsRepository.Table
                                 where !u.IsDeleted
                                 orderby u.Id
                                 select u);

                    return query.ToList();
                }
				///--------------------------------------------------------------------------------------------
				/// <summary>
				/// Inserts the smart group.
				/// </summary>
				/// <param name="smartGroup">The smart group.</param>
				/// --------------------------------------------------------------------------------------------
				public virtual void InsertSmartGroup(SmartGroup smartGroup)
				{
					if(smartGroup == null)
						throw new ArgumentNullException("smartGroup");

					smartGroup.IsDeleted = false;
					_smartGroupsRepository.Insert(smartGroup);

					//event notification
					_eventPublisher.EntityInserted(smartGroup);
				}

				///--------------------------------------------------------------------------------------------
				/// <summary>
				/// Gets the smart group by id.
				/// </summary>
				/// <param name="id">The id.</param>
				/// <returns></returns>
				/// --------------------------------------------------------------------------------------------
				public SmartGroup GetSmartGroupById(int id)
				{
					var query = _smartGroupsRepository;
                    return query.GetById(id);
				}

				///--------------------------------------------------------------------------------------------
				/// <summary>
				/// Gets the name of the smart group by.
				/// </summary>
				/// <param name="name">The name.</param>
				/// <returns></returns>
				/// --------------------------------------------------------------------------------------------
				public virtual SmartGroup GetSmartGroupByName(string name)
				{
					if(string.IsNullOrWhiteSpace(name))
						return null;

					return _smartGroupsRepository.Table.SingleOrDefault(x => x.Name.Contains(name) && !x.IsDeleted);
				}

				///--------------------------------------------------------------------------------------------
				/// <summary>
				/// Smarts the group auto complete.
				/// </summary>
				/// <param name="name">The name.</param>
				/// <returns></returns>
				/// --------------------------------------------------------------------------------------------
				public virtual IEnumerable<string> SmartGroupAutoComplete(string name)
				{
					return
						from sg in _smartGroupsRepository.Table
						where sg.Name.Contains(name) && !sg.IsDeleted
						select sg.Name;
				}

				///--------------------------------------------------------------------------------------------
				/// <summary>
				/// Updates the smart group.
				/// </summary>
				/// <param name="smartGroup">The smart group.</param>
				/// --------------------------------------------------------------------------------------------
				public void UpdateSmartGroup(SmartGroup smartGroup)
				{
					if(smartGroup == null)
						throw new ArgumentNullException("smartGroup");

					_smartGroupsRepository.Update(smartGroup);
					//event notification
					_eventPublisher.EntityUpdated(smartGroup);
					
				}

				///--------------------------------------------------------------------------------------------
				/// <summary>
				/// Deletes the smart group.
				/// </summary>
				/// <param name="smartGroup">The smart group.</param>
				/// --------------------------------------------------------------------------------------------
				public void DeleteSmartGroup(SmartGroup smartGroup)
				{
					_smartGroupsRepository.Delete(smartGroup);
					_eventPublisher.EntityDeleted(smartGroup);
				}

				///--------------------------------------------------------------------------------------------
				/// <summary>
				/// Groups the name is exist.
				/// </summary>
				/// <param name="name">The name.</param>
				/// <param name="id">The id.</param>
				/// <returns></returns>
				/// --------------------------------------------------------------------------------------------
				public bool GroupNameIsExist(string name, int id = 0)
				{
					return
						_smartGroupsRepository.Table.Any(sg => sg.Name.Equals(name) && sg.Id != id && !sg.IsDeleted);
				}

				public Dictionary<string, string> GetCustomerInfoById(int id = 0)
				{
					var personName = new Dictionary<string, string>();

					var persons = (from ga in _gaRepository.Table
											 where ga.KeyGroup.Equals("Customer") && ga.EntityId == id && (ga.Key.Equals("FirstName") || ga.Key.Equals("LastName"))
											 select ga);

					foreach(var person in persons)
					{
						if(!personName.ContainsKey(person.Key))
							personName.Add(person.Key, person.Value);
					}

											 

					return personName;
					
				}

				///--------------------------------------------------------------------------------------------
				/// <summary>
				/// Gets the contacts.
				/// </summary>
				/// <param name="id">The id.</param>
				/// <param name="pageIndex"></param>
				/// <param name="pageSize"></param>
				/// <returns></returns>
				/// --------------------------------------------------------------------------------------------
				public IPagedList<SmartContactModel> GetContacts(int id, int pageIndex, int pageSize)
				{
					var criteria = this.GetSmartGroupById(id).Query;

					string[] whereConditions = Helper.GetConditions(criteria).Split('^');
					string customerWhereCondition = whereConditions[0];
					string newsLetterWhereCondition = whereConditions[1];
					string roleWhereCondition = whereConditions[2];
					string othersWhereCondition = whereConditions[3];

					var contacts = this.SmartGroupSP(pageIndex, pageSize, customerWhereCondition, newsLetterWhereCondition, roleWhereCondition, othersWhereCondition);

                    //foreach(var contact in contacts)
                    //{
                    //    int customerId = contact.CustomerId;

                    //    contact.FirstName = this.GetCustomerInfoById(customerId).ContainsKey("FirstName") ? this.GetCustomerInfoById(customerId)["FirstName"] : "";
                    //    contact.LastName = this.GetCustomerInfoById(customerId).ContainsKey("LastName") ? this.GetCustomerInfoById(customerId)["LastName"] : "";
                    //}

					return contacts;
				}

				///--------------------------------------------------------------------------------------------
				/// <summary>
				/// Smarts the group SP.
				/// </summary>
				/// <param name="pageIndex">Index of the page.</param>
				/// <param name="pageSize">Size of the page.</param>
				/// <param name="customerWhere">The customer where.</param>
				/// <param name="newsLetterWhere">The news letter where.</param>
				/// <param name="customerRoleWhere">The customer role where.</param>
				/// <param name="othersWhere">The others where.</param>
				/// <returns></returns>
				/// --------------------------------------------------------------------------------------------
				public IPagedList<SmartContactModel> SmartGroupSP(int pageIndex, int pageSize, string customerWhere, string newsLetterWhere, string customerRoleWhere, string othersWhere)
				{
					if(_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
					{
						//stored procedures are enabled and supported by the database.
						var pCustomerWhere = _dataProvider.GetParameter();
						pCustomerWhere.ParameterName = "CustomerWhere";
						pCustomerWhere.Value = customerWhere;
						pCustomerWhere.DbType = DbType.AnsiStringFixedLength;

						var pNewLetterWhere = _dataProvider.GetParameter();
						pNewLetterWhere.ParameterName = "NewsLetterWhere";
						pNewLetterWhere.Value = newsLetterWhere;
						pNewLetterWhere.DbType = DbType.AnsiStringFixedLength;

						var pCustomerRoleWhere = _dataProvider.GetParameter();
						pCustomerRoleWhere.ParameterName = "CustomerRoleWhere";
						pCustomerRoleWhere.Value = customerRoleWhere;
						pCustomerRoleWhere.DbType = DbType.AnsiStringFixedLength;
						
						var pOthersWhere = _dataProvider.GetParameter();
						pOthersWhere.ParameterName = "OthersWhere";
						pOthersWhere.Value = othersWhere;
						pOthersWhere.DbType = DbType.AnsiStringFixedLength;

						//long-running query. specify timeout (600 seconds)
                        var smartContactModel = _dbContext.SqlQuery<SmartContactModel>("EXEC [SmartGroup_BsNotification] @CustomerWhere, @NewsLetterWhere, @CustomerRoleWhere, @OthersWhere", pCustomerWhere, pNewLetterWhere, pCustomerRoleWhere, pOthersWhere).ToList();
                        //invoke stored procedure
                        //var smartContactModel = _dbContext.ExecuteStoredProcedureList<SmartContactModel>("SmartGroup_BsNotification", pCustomerWhere, pNewLetterWhere, pCustomerRoleWhere, pOthersWhere);
						var smartGroups = new PagedList<SmartContactModel>(smartContactModel, pageIndex, pageSize);
						return smartGroups;
					}
					else
					{
						throw new ArgumentNullException("smartContactModel");
					}
				}

				///--------------------------------------------------------------------------------------------
				/// <summary>
				/// Gets all contacts at once.
				/// </summary>
				/// <param name="id">The id.</param>
				/// <returns></returns>
				/// --------------------------------------------------------------------------------------------
				public IEnumerable<SmartContactModel> GetAllContactsAtOnce(int id)
				{
					var criteria = this.GetSmartGroupById(id).Query;

					string[] whereConditions = Helper.GetConditions(criteria).Split('^');
					string customerWhereCondition = whereConditions[0];
					string newsLetterWhereCondition = whereConditions[1];
					string roleWhereCondition = whereConditions[2];
					string othersWhereCondition = whereConditions[3];

					var contacts = this.SmartGroupSPForAllContacts(customerWhereCondition, newsLetterWhereCondition, roleWhereCondition, othersWhereCondition);

                    //foreach(var contact in contacts)
                    //{
                    //    int customerId = contact.CustomerId;

                    //    contact.FirstName = this.GetCustomerInfoById(customerId).ContainsKey("FirstName") ? this.GetCustomerInfoById(customerId)["FirstName"] : "";
                    //    contact.LastName = this.GetCustomerInfoById(customerId).ContainsKey("LastName") ? this.GetCustomerInfoById(customerId)["LastName"] : "";	
                    //}

					return contacts;
				}

				///--------------------------------------------------------------------------------------------
				/// <summary>
				/// Smarts the group SP for all contacts.
				/// </summary>
				/// <param name="customerWhere">The customer where.</param>
				/// <param name="newsLetterWhere">The news letter where.</param>
				/// <param name="customerRoleWhere">The customer role where.</param>
				/// <param name="othersWhere">The others where.</param>
				/// <returns></returns>
				public IEnumerable<SmartContactModel> SmartGroupSPForAllContacts(string customerWhere, string newsLetterWhere, string customerRoleWhere, string othersWhere)
				{
					if(_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
					{
						//stored procedures are enabled and supported by the database.
						var pCustomerWhere = _dataProvider.GetParameter();
						pCustomerWhere.ParameterName = "CustomerWhere";
						pCustomerWhere.Value = customerWhere;
						pCustomerWhere.DbType = DbType.AnsiStringFixedLength;

						var pNewLetterWhere = _dataProvider.GetParameter();
						pNewLetterWhere.ParameterName = "NewsLetterWhere";
						pNewLetterWhere.Value = newsLetterWhere;
						pNewLetterWhere.DbType = DbType.AnsiStringFixedLength;

						var pCustomerRoleWhere = _dataProvider.GetParameter();
						pCustomerRoleWhere.ParameterName = "CustomerRoleWhere";
						pCustomerRoleWhere.Value = customerRoleWhere;
						pCustomerRoleWhere.DbType = DbType.AnsiStringFixedLength;

						var pOthersWhere = _dataProvider.GetParameter();
						pOthersWhere.ParameterName = "OthersWhere";
						pOthersWhere.Value = othersWhere;
						pOthersWhere.DbType = DbType.AnsiStringFixedLength;

						//long-running query. specify timeout (600 seconds)
                        var allContacts = _dbContext.SqlQuery<SmartContactModel>("EXEC [SmartGroup_BsNotification] @CustomerWhere, @NewsLetterWhere, @CustomerRoleWhere, @OthersWhere", pCustomerWhere, pNewLetterWhere, pCustomerRoleWhere, pOthersWhere).ToList();
						return allContacts;
					}
					else
					{
						throw new ArgumentNullException("smartContactModel");
					}
				}
                public string GetGruopName(int id)
                {
                    if(id==0)return null;
                    var group = _smartGroupsRepository.GetById(id);
                    
                    return group != null?group.Name:null;
                }
			
        #endregion


               
    }
}
