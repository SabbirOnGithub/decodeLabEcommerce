using System;
using System.Web.Http;
using System.Collections.Generic;

using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Models.Agent;
using BS.Plugin.NopStation.MobileWebApi.Services.Agent;


namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class ZeroAgentController : WebApiController
    {
        private readonly IAgentService _zeroAgentService;

        public ZeroAgentController(IAgentService zeroAgentService)
        {
            _zeroAgentService = zeroAgentService;
        }

        //protected string GetToken(long agentId)
        //{
        //    var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //    var now = Math.Round((DateTime.UtcNow.AddDays(180) - unixEpoch).TotalSeconds);

        //    var payload = new Dictionary<string, object>()
        //    {
        //        { Constant.AgentIdName, agentId },
        //        { "exp", now }
        //    };

        //    string secretKey = Constant.SecretKey;
        //    var token = JWT.JsonWebToken.Encode(payload, secretKey, JWT.JwtHashAlgorithm.HS256);

        //    return token;
        //}

        [HttpPost, Route("api/zeroagent/register")]
        public IHttpActionResult Register(ZeroAgentInfo ZeroAgentInfo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _zeroAgentService.RegisterZeroAgent(ZeroAgentInfo, out ZeroAgentResponse ZeroAgentResponse);
                    return Ok(ZeroAgentResponse);
                }

                return Ok(new ZeroAgentResponse());
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                ZeroAgentResponse ZeroAgentResponse= new ZeroAgentResponse
                {
                    Message = ex.Message
                };

                return Ok(ZeroAgentResponse);
            }
        }

        [HttpPost, Route("api/zeroagent/verifyotp")]
        public IHttpActionResult VerifyOTP(ZeroAgentOTP ZeroAgentOTP)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _zeroAgentService.VerifyOTP(ZeroAgentOTP, out ZeroAgentResponse ZeroAgentResponse);
                    return Ok(ZeroAgentResponse);
                }

                return Ok(new ZeroAgentResponse());
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                ZeroAgentResponse ZeroAgentResponse= new ZeroAgentResponse
                {
                    Message = ex.Message
                };

                return Ok(ZeroAgentResponse);
            }
        }
    }
}
