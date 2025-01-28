using CMS.DataEngine;
using CMS.FormEngine;

namespace XperienceCommunity.GeoLocation.Utilities
{
    internal static class ContactClassInfoHelper
    {
        public static readonly string ContactMappedColumnName = "XperienceGeoLocation_IsContactMapped";

        /// <summary>
        /// Ensures that the contact mapped field exists against the om.contact class info object.
        /// </summary>
        public static void EnsureContactMappedField()
        {
            var dci = DataClassInfoProvider.GetDataClassInfo("om.contact") ?? throw new Exception("The 'om.contact' class was not found.");

            var formInfo = new FormInfo(dci.ClassFormDefinition);

            if (formInfo.FieldExists(ContactMappedColumnName))
            {
                return;
            }

            // Field doesn't exist, create against om.contact
            var mappedField = new FormFieldInfo
            {
                Name = ContactMappedColumnName,
                Caption = "Indicates whether contact Geo location has been mapped",
                DataType = FieldDataType.Boolean,
                AllowEmpty = true
            };

            // Update form def
            formInfo.AddFormItem(mappedField);
            dci.ClassFormDefinition = formInfo.GetXmlDefinition();

            // Update class info / db table
            DataClassInfoProvider.SetDataClassInfo(dci);
        }
    }
}
