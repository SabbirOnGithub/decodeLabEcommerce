using System;

namespace BS.Plugin.NopStation.MobileWebApi.Model
{
    // Summary:
    //     Represents the optgroup HTML element and its attributes. In a select list,
    //     multiple groups with the same name are supported. They are compared with
    //     reference equality.
    public class SelectListGroup
    {
        //public SelectListGroup();

        // Summary:
        //     Gets or sets a value that indicates whether this System.Web.Mvc.SelectListGroup
        //     is disabled.
        public bool Disabled { get; set; }
        //
        // Summary:
        //     Represents the value of the optgroup's label.
        public string Name { get; set; }
    }
}
