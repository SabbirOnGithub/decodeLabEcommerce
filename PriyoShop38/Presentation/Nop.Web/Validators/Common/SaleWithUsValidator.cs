using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Common;

namespace Nop.Web.Validators.Common
{
    public partial class SaleWithUsValidator : BaseNopValidator<SaleWithUsModel>
    {
        public SaleWithUsValidator(ILocalizationService localizationService, CommonSettings commonSettings)
        {
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Enter your full name");
            RuleFor(x => x.CompanyName).NotEmpty().WithMessage("Enter your company name");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Enter email");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Wrong email");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Enter phone number");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Enter your address");            
        }
    }
}