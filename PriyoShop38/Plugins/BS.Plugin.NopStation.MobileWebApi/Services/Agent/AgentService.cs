using System;
using System.Linq;
using System.Data.Entity;

using Nop.Core.Data;
using Nop.Services.Events;

using BS.Plugin.NopStation.MobileWebApi.Data;
using BS.Plugin.NopStation.MobileWebApi.Domain.Agent;
using BS.Plugin.NopStation.MobileWebApi.Models.Agent;

using BS.Plugin.NopStation.MobileWebApi.Utility;

using System.Data.SqlClient;

namespace BS.Plugin.NopStation.MobileWebApi.Services.Agent
{
    public class AgentService : IAgentService
    {
        private readonly MobileWebApiObjectContext _context;
        private readonly IRepository<AgentMasterInformationTemp> _AgentMasterInformationTempRepository;
        private readonly IRepository<AgentImageInformationTemp> _AgentImageInformationTempRepository;
        private readonly IEventPublisher _eventPublisher;

        public AgentService(
            MobileWebApiObjectContext context,
            IRepository<AgentMasterInformationTemp> AgentMasterInformationTempRepository,
            IRepository<AgentImageInformationTemp> AgentImageInformationTempRepository,
            IEventPublisher eventPublisher
        )
        {
            _context = context;
            _AgentMasterInformationTempRepository = AgentMasterInformationTempRepository;
            _AgentImageInformationTempRepository = AgentImageInformationTempRepository;
            _eventPublisher = eventPublisher;
        }

        public void RegisterZeroAgent(ZeroAgentInfo ZeroAgentInfo, out ZeroAgentResponse ZeroAgentResponse)
        {
            ZeroAgentResponse = new ZeroAgentResponse();

            if (ZeroAgentInfo == null)
                throw new ArgumentNullException(nameof(ZeroAgentInfo));

            var EntryDate = DateTime.Now;
            var GeneratedOTP = OTP.GenerateOTP();

            AgentMasterInformationTemp AgentMasterInformationTemp =
                new AgentMasterInformationTemp
                {
                    AgentName = ZeroAgentInfo.AgentName,
                    AgentContactNo = ZeroAgentInfo.AgentContactNo,
                    AgentNID = ZeroAgentInfo.AgentNID,
                    AgentPassword = ZeroAgentInfo.AgentPassword,
                    AgentContactAddress = ZeroAgentInfo.AgentContactAddress,
                    AgentOrganizationName = ZeroAgentInfo.AgentOrganizationName,
                    OTP = GeneratedOTP,
                    OTPGenerationDate = EntryDate,
                    OTPExpireInMinute = 3,
                    EntryDate = EntryDate
                };

            var AgentImageInformationTemp =
                new AgentImageInformationTemp
                {
                    AgentImage = ZeroAgentInfo.AgentImage,
                    EntryDate = EntryDate
                };

            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                _context.Entry(AgentMasterInformationTemp);
                _context.Entry(AgentMasterInformationTemp).State = EntityState.Added;
                _context.SaveChanges();

                AgentImageInformationTemp.AgentMasterId = AgentMasterInformationTemp.Id;

                _context.Entry(AgentImageInformationTemp);
                _context.Entry(AgentImageInformationTemp).State = EntityState.Added;
                _context.SaveChanges();

                RegisterOTP2Send(ZeroAgentInfo.AgentContactNo, GeneratedOTP);

                dbContextTransaction.Commit();

                //event notification
                _eventPublisher.EntityInserted(AgentMasterInformationTemp);
                _eventPublisher.EntityInserted(AgentImageInformationTemp);

                // prepare response
                ZeroAgentResponse.Id = AgentMasterInformationTemp.Id;
                ZeroAgentResponse.Status = "success";
                ZeroAgentResponse.Message = "go to otp";
            }
        }

        public bool VerifyOTP(ZeroAgentOTP ZeroAgentOTP, out ZeroAgentResponse ZeroAgentResponse)
        {
            ZeroAgentResponse = new ZeroAgentResponse();

            var AgentMasterInformationTemp = _AgentMasterInformationTempRepository.GetById(ZeroAgentOTP.Id);

            if (AgentMasterInformationTemp.OTP == ZeroAgentOTP.OTP && AgentMasterInformationTemp.OTPExpireInMinute * 60 >= (DateTime.Now - AgentMasterInformationTemp.OTPGenerationDate).TotalSeconds)
            {
                var AgentImageInformationTemp = GetAgentImageInformationTempByAgentMasterId(AgentMasterInformationTemp.Id);

                var EntryDate = DateTime.Now;

                using (var dbContextTransaction = _context.Database.BeginTransaction())
                {
                    var AgentMasterInformation = new AgentMasterInformation
                    {
                        AgentId = _context.SqlQuery<long>("[SysInfo].[spGenerateSystemId] @ProcessName, @YearNo", new SqlParameter("@ProcessName", "AgentMasterInformation"), new SqlParameter("@YearNo", EntryDate.Year)).FirstOrDefault(),                                        // generated from signature
                        AgentContactNo = AgentMasterInformationTemp.AgentContactNo,
                        AgentName = AgentMasterInformationTemp.AgentName,
                        AgentOrganizationName = AgentMasterInformationTemp.AgentOrganizationName,
                        AgentContactAddress = AgentMasterInformationTemp.AgentContactAddress,
                        AgentNID = AgentMasterInformationTemp.AgentNID,
                        AgentPassword = AgentMasterInformationTemp.AgentPassword,
                        EntryDate = EntryDate
                    };

                    _context.Entry(AgentMasterInformation);
                    _context.Entry(AgentMasterInformation).State = EntityState.Added;
                    _context.SaveChanges();

                    var AgentImageInformation = new AgentImageInformation
                    {
                        AgentImageId = _context.SqlQuery<long>("[SysInfo].[spGenerateSystemId] @ProcessName, @YearNo", new SqlParameter("@ProcessName", "AgentImageInformation"), new SqlParameter("@YearNo", EntryDate.Year)).FirstOrDefault(),                                        // generated from signature
                        AgentImage = AgentImageInformationTemp.AgentImage,
                        AgentId = AgentMasterInformation.AgentId,
                        EntryDate = EntryDate
                    };

                    _context.Entry(AgentImageInformation);
                    _context.Entry(AgentImageInformation).State = EntityState.Added;
                    _context.SaveChanges();

                    dbContextTransaction.Commit();

                    //event notification
                    _eventPublisher.EntityInserted(AgentMasterInformation);
                    _eventPublisher.EntityInserted(AgentImageInformation);

                    // prepare response
                    ZeroAgentResponse.Id = AgentMasterInformation.AgentId;
                    ZeroAgentResponse.Status = "success";
                    ZeroAgentResponse.Message = "otp successfully verified";
                    return true;
                }
            }

            return false;
        }

        #region private methods
        private AgentImageInformationTemp GetAgentImageInformationTempByAgentMasterId(int AgentMasterId)
        {

            var query = from AgentImageInformationTemp in _AgentImageInformationTempRepository.TableNoTracking
                        where AgentMasterId.Equals(AgentImageInformationTemp.AgentMasterId)
                        select AgentImageInformationTemp;

            return query.ToList().FirstOrDefault();
        }

        private void RegisterOTP2Send(int RecipientNumber, string MessageBody)
        {
            MessageBody = "Required OTP for agent registration is " + MessageBody;
            _context.ExecuteSqlCommand("[PRIYOERP].[SysInfo].[SendSMSInformation] @RecipientNumber, @SentMessage", false, null, new SqlParameter("@RecipientNumber", RecipientNumber), new SqlParameter("@SentMessage", MessageBody));
        }

        #endregion
    }
}
