namespace ArcadiaTeamsBot.ServiceDesk.Requests.RequestTypeFactory
{
    using System.Collections.Generic;

    using ArcadiaTeamsBot.ServiceDesk.Abstractions.DTOs;
    using ArcadiaTeamsBot.ServiceDesk.Requests.RequestType;

    public class RequestTypeUIFactory : IRequestTypeUIFactory
    {
        private readonly RequestTypesMappingConfiguration requestTypesMappingConfiguration;

        public RequestTypeUIFactory(RequestTypesMappingConfiguration requestTypesMappingConfiguration)
        {
            this.requestTypesMappingConfiguration = requestTypesMappingConfiguration;
        }

        public IRequestTypeUI CreateRequestTypeUI(ServiceDeskRequestTypeDTO requestTypeDTO)
        {
            var requestTypeUIFields = new List<RequestTypeUIField>();

            this.requestTypesMappingConfiguration.TryGetValue(requestTypeDTO.Title, out var requestTypeConfig);

            foreach (var fieldDTO in requestTypeDTO.RequestTypeFields)
            {
                var fieldName = fieldDTO.FieldName;

                RequestTypeUIFieldType fieldType;

                if (requestTypeConfig?.ContainsKey(fieldName) == true)
                {
                    fieldType = requestTypeConfig[fieldName];
                }
                else
                {
                    fieldType = fieldDTO.Items != null ? RequestTypeUIFieldType.Select : RequestTypeUIFieldType.String;
                }

                requestTypeUIFields.Add(new RequestTypeUIField(fieldName, fieldType));
            }

            return new RequestTypeUI(requestTypeUIFields);
        }
    }
}
