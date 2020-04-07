namespace ArcadiaTeamsBot.ServiceDesk.Requests
{
    using System;
    using System.Collections.Generic;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;

    public class RequestTypeForUI
    {
        private readonly Dictionary<string, string> serviceDeskMappingConfiguration;

        public RequestTypeForUI(Dictionary<string, string> serviceDeskMappingConfiguration)
        {
            this.serviceDeskMappingConfiguration = serviceDeskMappingConfiguration;
            this.RequestTypeForUIFields = new List<RequestTypeForUIField>();
        }

        public List<RequestTypeForUIField> RequestTypeForUIFields { get; set; }

        public void LoadFieldsTypes(ServiceDeskRequestTypeDTO requestTypeDTO)
        {
            foreach (var fieldFromDTO in requestTypeDTO.RequestTypeFields)
            {
                var fieldName = fieldFromDTO.FieldName;

                RequestTypeForUIFieldType fieldType;

                if (this.serviceDeskMappingConfiguration.TryGetValue(fieldFromDTO.FieldName, out var fieldTypeNameFromConfig))
                {
                    Enum.TryParse(fieldTypeNameFromConfig, out fieldType);
                }
                else
                {
                    fieldType = RequestTypeForUIFieldType.String;
                }

                this.RequestTypeForUIFields.Add(new RequestTypeForUIField(fieldName, fieldType));
            }
        }
    }
}
