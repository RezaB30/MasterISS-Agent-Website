using MasterISS_Agent_Website.ViewModels.Setup;
using MasterISS_Agent_Website_WebServices.SetupWebService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace MasterISS_Agent_Website
{
    public class SetupServiceWrapper
    {
        private readonly string Username;

        private readonly string Rand;

        private readonly string Culture;

        private readonly string KeyFragment;

        private readonly string Password;

        private CustomerSetupServiceClient Client { get; set; }

        public SetupServiceWrapper()
        {
            Username = AgentClaimInfo.SetupServiceUser();
            Culture = CultureInfo.CurrentCulture.ToString();
            KeyFragment = new CustomerSetupServiceClient().GetKeyFragment(AgentClaimInfo.SetupServiceUser());
            Rand = Guid.NewGuid().ToString("N");
            Password = AgentClaimInfo.SetupServiceHash();
            Client = new CustomerSetupServiceClient();
        }
        private string Hash()
        {
            var wrapper = new WebServiceWrapper();
            var hashAuthenticaiton = wrapper.CalculateHash<SHA256>(Username + Rand + Password + KeyFragment);
            return hashAuthenticaiton;
        }

        public GetTaskListResponse GetTaskList(GetTaskListViewModel getTaskListViewModel)
        {
            var request = new GetTaskListRequest
            {
                Culture = Culture,
                Hash = Hash(),
                Rand = Rand,
                Username = Username,
                DateSpan = new DateSpan
                {
                    StartDate = DateParseOperations.ConvertedDateTimeForGetTask(getTaskListViewModel.StartDate),
                    EndDate = DateParseOperations.ConvertedDateTimeForGetTask(getTaskListViewModel.EndDate)
                }
            };

            var response = Client.GetTaskList(request);

            return response;
        }

        public GetTaskDetailsResponse GetTaskDetails(long taskNo)
        {
            var response = Client.GetTaskDetails(TaskNoRequest(taskNo));

            return response;
        }
        private TaskNoRequest TaskNoRequest(long taskNo)
        {
            var request = new TaskNoRequest
            {
                Culture = Culture,
                Hash = Hash(),
                Rand = Rand,
                Username = Username,
                TaskNo = taskNo
            };

            return request;
        }

        public GetCustomerAttachmentsResponse GetCustomerAttachments(long taskNo)
        {
            var response = Client.GetCustomerAttachments(TaskNoRequest(taskNo));

            return response;
        }

        public GetCustomerContractResponse GetCustomerContract(long taskNo)
        {
            var response = Client.GetCustomerContract(TaskNoRequest(taskNo));

            return response;
        }

        public GetCustomerCredentialResponse GetCustomerCredentials(long taskNo)
        {
            var response = Client.GetCustomerCredentials(TaskNoRequest(taskNo));

            return response;
        }

        public GetCustomerLineDetailsResponse GetCustomerLineDetails(long taskNo)
        {
            var response = Client.GetCustomerLineDetails(TaskNoRequest(taskNo));

            return response;
        }

        public GetCustomerSessionInfoResponse GetCustomerSessionInfo(long taskNo)
        {
            var response = Client.GetCustomerSessionInfo(TaskNoRequest(taskNo));

            return response;
        }

        public AddCustomerAttachmentResponse AddCustomerAttachment(AddCustomerAttachmentViewModel uploadFileViewModel)
        {
            var request = new AddCustomerAttachmentRequest
            {
                Culture = Culture,
                Hash = Hash(),
                Rand = Rand,
                Username = Username,
                CustomerAttachment = new CustomerAttachment
                {
                    FileData = uploadFileViewModel.FileData,
                    FileType = uploadFileViewModel.Extension.Replace(".", ""),
                    TaskNo = uploadFileViewModel.TaskNo,
                    AttachmentType = (short)uploadFileViewModel.AttachmentType
                }
            };

            var response = Client.AddCustomerAttachment(request);

            return response;
        }

        public ParameterlessResponse AddTaskStatusUpdate(AddTaskStatusUpdateViewModel taskStatusUpdateViewModel)
        {
            var request = new AddTaskStatusUpdateRequest
            {
                Culture = Culture,
                Hash = Hash(),
                Rand = Rand,
                Username = Username,
                TaskUpdate = new TaskUpdate
                {
                    TaskNo = (long)taskStatusUpdateViewModel.TaskNo,
                    Description = taskStatusUpdateViewModel.Description,
                    FaultCode = (short)taskStatusUpdateViewModel.FaultCode,
                    ReservationDate = taskStatusUpdateViewModel.ReservationDate
                },
            };
            var response = Client.AddTaskStatusUpdate(request);

            return response;
        }

    }
}