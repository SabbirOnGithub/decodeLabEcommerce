using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Events;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    /// <summary>
    /// Generic attribute service
    /// </summary>
    public partial class GenericAttributeServiceApi : GenericAttributeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : key group
        /// </remarks>
        private const string API_GENERICATTRIBUTE_KEY = "Nop.genericattribute.BsWebapi.{0}-{1}";
        private const string GENERICATTRIBUTE_KEY = "Nop.genericattribute.{0}-{1}";
        private const string GENERICATTRIBUTE_PATTERN_KEY = "Nop.genericattribute.";
        /// <summary>
        #endregion

        #region Fields
        private readonly HttpContextBase _httpContext;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="genericAttributeRepository">Generic attribute repository</param>
        /// <param name="eventPublisher">Event published</param>
        public GenericAttributeServiceApi(ICacheManager cacheManager,
            IRepository<GenericAttribute> genericAttributeRepository,
            IEventPublisher eventPublisher, HttpContextBase httpContext)
            : base(cacheManager, genericAttributeRepository, eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._genericAttributeRepository = genericAttributeRepository;
            this._eventPublisher = eventPublisher;
            this._httpContext = httpContext;
        }

        #endregion   
        
        #region Methods
        public override IList<GenericAttribute> GetAttributesForEntity(int entityId, string keyGroup)
        {
            string key = string.Format(GENERICATTRIBUTE_KEY, entityId, keyGroup);
            return _cacheManager.Get(key, () =>
            {
                var query = from ga in _genericAttributeRepository.Table
                            where ga.EntityId == entityId &&
                            ga.KeyGroup == keyGroup
                            select ga;
                var attributes = query.ToList();
                return attributes;
            });
        }
        /// <summary>
        /// Deletes an attribute
        /// </summary>
        /// <param name="attribute">Attribute</param>
        public override void DeleteAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");
            if (_httpContext != null && _httpContext.Request.RawUrl.StartsWith("/api/"))
            {
                var bsAttribute=_genericAttributeRepository.GetById(attribute.Id);
                _genericAttributeRepository.Delete(bsAttribute);
            }
            else
            {
                _genericAttributeRepository.Delete(attribute);
            }
           

            //cache
            _cacheManager.RemoveByPattern(GENERICATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(attribute);
        }

        /// <summary>
        /// Gets an attribute
        /// </summary>
        /// <param name="attributeId">Attribute identifier</param>
        /// <returns>An attribute</returns>
        

        /// <summary>
        /// Inserts an attribute
        /// </summary>
        /// <param name="attribute">attribute</param>
       
        /// <summary>
        /// Updates the attribute
        /// </summary>
        /// <param name="attribute">Attribute</param>
        public override void UpdateAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            if (_httpContext != null && _httpContext.Request.RawUrl.StartsWith("/api/"))
            {
                var bsAttribute = _genericAttributeRepository.GetById(attribute.Id);
                bsAttribute.EntityId = attribute.EntityId;
                bsAttribute.Key = attribute.Key;
                bsAttribute.KeyGroup = attribute.KeyGroup;
                bsAttribute.StoreId = attribute.StoreId;
                bsAttribute.Value = attribute.Value;
                _genericAttributeRepository.Update(attribute);
            }
            else
            {
                _genericAttributeRepository.Update(attribute);
            }
            _genericAttributeRepository.Update(attribute);

            //cache
            _cacheManager.RemoveByPattern(GENERICATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(attribute);
        }

        /// <summary>
        /// Save attribute value
        /// </summary>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="storeId">Store identifier; pass 0 if this attribute will be available for all stores</param>
        public override void SaveAttribute<TPropType>(BaseEntity entity, string key, TPropType value, int storeId = 0)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (key == null)
                throw new ArgumentNullException("key");

            string keyGroup = entity.GetUnproxiedEntityType().Name;

            var props = GetAttributesForEntity(entity.Id, keyGroup)
                .Where(x => x.StoreId == storeId)
                .ToList();
            var prop = props.FirstOrDefault(ga =>
                ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

            var valueStr = CommonHelper.To<string>(value);

            if (prop != null)
            {
                if (string.IsNullOrWhiteSpace(valueStr))
                {
                    //delete
                    DeleteAttribute(prop);
                }
                else
                {
                    //update
                    prop.Value = valueStr;
                    UpdateAttribute(prop);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(valueStr))
                {
                    //insert
                    prop = new GenericAttribute
                    {
                        EntityId = entity.Id,
                        Key = key,
                        KeyGroup = keyGroup,
                        Value = valueStr,
                        StoreId = storeId,
                        
                    };
                    InsertAttribute(prop);
                }
            }
        }

        #endregion
    }
}