using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileApp.Models;
using Nop.Services.Localization;
using BS.Plugin.NopStation.MobileApp.Services;

namespace BS.Plugin.NopStation.MobileApp.Validators
{
    public class SmartGroupValidator : AbstractValidator<CriteriaModel>
    {
        public SmartGroupValidator(ILocalizationService localizationService, ISmartGroupService smartGroupsService)
        {
            RuleFor(x => x.Name)
                    .NotNull()
                    .WithMessage(localizationService.GetResource("Admin.Other.EmailMarketing.Groups.Fields.Name.Required"))
                    .Must((x, name) => !smartGroupsService.GroupNameIsExist(name, x.Id))
                    .WithMessage("This Group Name allready exist");
        }
    }
}
