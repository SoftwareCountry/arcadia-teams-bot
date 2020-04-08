namespace ArcadiaTeamsBot.ServiceDesk.Requests.RequestTypeFactory
{
    using System;
    using System.Collections.Generic;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;
    using ArcadiaTeamsBot.ServiceDesk.Requests.RequestType;

    public class RequestTypeForUIFactory : IRequestTypeForUIFactory
    {
        private readonly RequestTypesMappingConfiguration requestTypesMappingConfiguration;

        public RequestTypeForUIFactory(RequestTypesMappingConfiguration requestTypesMappingConfiguration)
        {
            this.requestTypesMappingConfiguration = requestTypesMappingConfiguration;
        }

        public IRequestTypeForUI CreateRequestTypeForUI(ServiceDeskRequestTypeDTO requestTypeDTO)
        {
            var requestTypeForUIFields = new List<RequestTypeForUIField>();

            this.requestTypesMappingConfiguration.TryGetValue(requestTypeDTO.Title, out var requestTypeFromConfig);

            foreach (var fieldFromDTO in requestTypeDTO.RequestTypeFields)
            {
                var fieldName = fieldFromDTO.FieldName;

                if (requestTypeFromConfig == null ||
                    !requestTypeFromConfig.TryGetValue(fieldName, out var fieldTypeNameFromConfig) ||
                    !Enum.TryParse(fieldTypeNameFromConfig, out RequestTypeForUIFieldType fieldType))
                {
                    fieldType = fieldFromDTO.Items != null ? RequestTypeForUIFieldType.Select : RequestTypeForUIFieldType.String;
                }

                requestTypeForUIFields.Add(new RequestTypeForUIField(fieldName, fieldType));
            }

            return new RequestTypeForUI(requestTypeForUIFields);
        }
    }
}
