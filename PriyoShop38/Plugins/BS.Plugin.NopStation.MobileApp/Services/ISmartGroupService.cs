using System;
using System.Collections.Generic;
using Nop.Core;
using BS.Plugin.NopStation.MobileApp.Domain;
using BS.Plugin.NopStation.MobileApp.Models;

namespace BS.Plugin.NopStation.MobileApp.Services
{
    public partial interface ISmartGroupService
    {
        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all smart group.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<SmartGroup> GetAllSmartGroup(int pageIndex, int pageSize);


        IList<SmartGroup> GetAllSmartGroup();
        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Inserts the smart group.
        /// </summary>
        /// <param name="smartGroup">The smart group.</param>
        void InsertSmartGroup(SmartGroup smartGroup);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the contacts.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        IPagedList<SmartContactModel> GetContacts(int id, int pageIndex, int pageSize);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all contacts at once.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        IEnumerable<SmartContactModel> GetAllContactsAtOnce(int id);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the smart group by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        SmartGroup GetSmartGroupById(int id);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the name of the smart group by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        SmartGroup GetSmartGroupByName(string name);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Smarts the group auto complete.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IEnumerable<string> SmartGroupAutoComplete(string name);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Updates the smart group.
        /// </summary>
        /// <param name="smartGroup">The smart group.</param>
        void UpdateSmartGroup(SmartGroup smartGroup);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the customer info by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        Dictionary<string, string> GetCustomerInfoById(int id = 0);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes the smart group.
        /// </summary>
        /// <param name="smartGroup">The smart group.</param>
        void DeleteSmartGroup(SmartGroup smartGroup);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Groups the name is exist.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        bool GroupNameIsExist(string name, int id = 0);

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
        IPagedList<SmartContactModel> SmartGroupSP(int pageIndex, int pageSize, string customerWhere, string newsLetterWhere, string customerRoleWhere, string othersWhere);

        string GetGruopName(int id);
    }
}
