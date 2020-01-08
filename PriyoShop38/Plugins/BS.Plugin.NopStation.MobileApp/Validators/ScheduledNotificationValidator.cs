using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileApp.Models;

namespace BS.Plugin.NopStation.MobileApp.Validators
{
    public class ScheduledNotificationValidator : AbstractValidator<ScheduledNotificationModel>
    {
        public ScheduledNotificationValidator()
        {
            
        }
    }
}
