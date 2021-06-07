using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using MasterISS_Agent_Website_WebServices.AgentWebService;
using MasterISS_Agent_Website.ViewModels;
using MasterISS_Agent_Website.ViewModels.Customer;
using RadiusR.DB.Enums;
using MasterISS_Agent_Website_Enums.Enums;

namespace MasterISS_Agent_Website
{
    public class WebServiceWrapper
    {
        private readonly string Username;

        private readonly string Rand;

        private readonly string Culture;

        private readonly string KeyFragment;

        private readonly string Password;

        private AgentWebServiceClient Client { get; set; }

        public WebServiceWrapper()
        {
            Client = new AgentWebServiceClient();
            Culture = CultureInfo.CurrentCulture.ToString();
            Username = Properties.Settings.Default.Username;
            Rand = Guid.NewGuid().ToString("N");
            Password = Properties.Settings.Default.Password;
            KeyFragment = new AgentWebServiceClient().GetKeyFragment(Properties.Settings.Default.Username);
        }
        public string CalculateHash<HAT>(string value) where HAT : HashAlgorithm
        {
            HAT algorithm = (HAT)HashAlgorithm.Create(typeof(HAT).Name);
            var calculatedHash = string.Join(string.Empty, algorithm.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(b => b.ToString("x2")));
            return calculatedHash;
        }
        public string Hash<HAT>() where HAT : HashAlgorithm
        {
            var hashAuthenticaiton = CalculateHash<HAT>(Username + Rand + CalculateHash<HAT>(Password) + KeyFragment);
            return hashAuthenticaiton;
        }

        public AgentServiceAuthenticationResponse Authenticate(SignInViewModel signInViewModel)
        {
            var request = new AgentServiceAuthenticationRequest
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                AuthenticationParameters = new AuthenticationRequest
                {
                    UserEmail = signInViewModel.Username,
                    PasswordHash = CalculateHash<SHA256>(signInViewModel.Password)
                }
            };

            var response = Client.Authenticate(request);

            return response;
        }

        public AgentServicePaymentResponse PayBills(long[] billsIds)
        {
            var request = new AgentServicePaymentRequest
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                PaymentRequest = new PaymentRequest
                {
                    BillIDs = billsIds,
                    UserEmail = AgentClaimInfo.UserEmail(),
                    PrePaidSubscription = null
                },
            };

            var response = Client.PayBills(request);

            return response;
        }

        public AgentServicePaymentResponse PayBillsPrePaid(string subscriberNo)
        {
            var request = new AgentServicePaymentRequest
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                PaymentRequest = new PaymentRequest
                {
                    BillIDs = null,
                    UserEmail = AgentClaimInfo.UserEmail(),
                    PrePaidSubscription = subscriberNo
                },
            };

            var response = Client.PayBills(request);

            return response;
        }


        public AgentServiceBillListResponse GetBills(string customerCode)
        {
            var request = new AgentServiceBillListRequest
            {
                Culture = Culture,
                Rand = Rand,
                Hash = Hash<SHA256>(),
                Username = Username,
                BillListRequest = new BillListRequest
                {
                    CustomerCode = customerCode,
                    UserEmail = AgentClaimInfo.UserEmail(),
                },
            };

            var response = Client.GetBills(request);
            return response;
        }

        public AgentServiceNewCustomerRegisterResponse NewCustomerRegister(NewCustomerViewModel addCustomerViewModel)
        {
            var request = new AgentServiceNewCustomerRegisterRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                CustomerRegisterParameters = new NewCustomerRegisterRequest
                {
                    UserEmail = AgentClaimInfo.UserEmail(),
                    CorporateCustomerInfo = addCustomerViewModel.GeneralInfo.CustomerTypeId == (int)CustomerType.Individual ? null : new CorporateCustomerInfo
                    {
                        CentralSystemNo = addCustomerViewModel.CorporateInfo.CentralSystemNo,
                        ExecutiveBirthPlace = addCustomerViewModel.CorporateInfo.ExecutiveBirthPlace,
                        ExecutiveFathersName = addCustomerViewModel.CorporateInfo.ExecutiveFathersName,
                        ExecutiveMothersMaidenName = addCustomerViewModel.CorporateInfo.ExecutiveMothersMaidenName,
                        ExecutiveMothersName = addCustomerViewModel.CorporateInfo.ExecutiveMothersName,
                        ExecutiveNationality = addCustomerViewModel.CorporateInfo.ExecutiveNationalityId,
                        ExecutiveProfession = addCustomerViewModel.CorporateInfo.ExecutiveProfessionId,
                        ExecutiveSex = addCustomerViewModel.CorporateInfo.ExecutiveSexId,
                        TaxNo = addCustomerViewModel.CorporateInfo.TaxNo,
                        TaxOffice = addCustomerViewModel.CorporateInfo.TaxOffice,
                        Title = addCustomerViewModel.CorporateInfo.Title,
                        TradeRegistrationNo = addCustomerViewModel.CorporateInfo.TradeRegistrationNo,
                        ExecutiveResidencyAddress = NewAddressInfo(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.NewCustomerAddressInfoRequest, (int)addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.PostalCode, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.Floor),
                        CompanyAddress = NewAddressInfo(addCustomerViewModel.CorporateInfo.CompanyAddress.NewCustomerAddressInfoRequest, (int)addCustomerViewModel.CorporateInfo.CompanyAddress.PostalCode, addCustomerViewModel.CorporateInfo.CompanyAddress.Floor)
                    },

                    CustomerGeneralInfo = new CustomerGeneralInfo()
                    {
                        BillingAddress = NewAddressInfo(addCustomerViewModel.GeneralInfo.BillingAddress.NewCustomerAddressInfoRequest, (int)addCustomerViewModel.GeneralInfo.BillingAddress.PostalCode, addCustomerViewModel.GeneralInfo.BillingAddress.Floor),
                        ContactPhoneNo = addCustomerViewModel.GeneralInfo.ContactPhoneNo,
                        Email = addCustomerViewModel.GeneralInfo.Email,
                        CustomerType = addCustomerViewModel.GeneralInfo.CustomerTypeId,
                        Culture = addCustomerViewModel.GeneralInfo.Culture,
                        OtherPhoneNos = addCustomerViewModel.GeneralInfo.OtherPhoneNos.Select(phoneNo => new PhoneNoListItem()
                        {
                            Number = phoneNo
                        }).ToArray()
                    },

                    IDCardInfo = new IDCardInfo()
                    {
                        BirthDate = DateParseOperations.ConvertDatetimeByWebService(addCustomerViewModel.IDCard.BirthDate),
                        CardType = addCustomerViewModel.IDCard.CardTypeId,
                        DateOfIssue = DateOfIssueValue((int)addCustomerViewModel.IDCard.CardTypeId, addCustomerViewModel.IDCard),
                        District = addCustomerViewModel.IDCard.TCBirthCertificate?.District,
                        FirstName = addCustomerViewModel.IDCard.FirstName,
                        LastName = addCustomerViewModel.IDCard.LastName,
                        Neighbourhood = addCustomerViewModel.IDCard.TCBirthCertificate?.Neighbourhood,
                        PageNo = addCustomerViewModel.IDCard.TCBirthCertificate?.PageNo,
                        PassportNo = addCustomerViewModel.IDCard.PassportNo,
                        PlaceOfIssue = addCustomerViewModel.IDCard.TCBirthCertificate?.PlaceOfIssue,
                        Province = addCustomerViewModel.IDCard.TCBirthCertificate?.Province,
                        RowNo = addCustomerViewModel.IDCard.TCBirthCertificate?.RowNo,
                        SerialNo = addCustomerViewModel.IDCard.SerialNo,
                        TCKNo = addCustomerViewModel.IDCard.TCKNo,
                        VolumeNo = addCustomerViewModel.IDCard.TCBirthCertificate?.VolumeNo,
                    },

                    IndividualCustomerInfo = addCustomerViewModel.GeneralInfo.CustomerTypeId != (int)CustomerType.Individual ? null : new IndividualCustomerInfo()
                    {
                        BirthPlace = addCustomerViewModel.Individual.BirthPlace,
                        FathersName = addCustomerViewModel.Individual.FathersName,
                        MothersMaidenName = addCustomerViewModel.Individual.MothersMaidenName,
                        MothersName = addCustomerViewModel.Individual.MothersName,
                        Nationality = addCustomerViewModel.Individual.NationalityId,
                        Profession = addCustomerViewModel.Individual.ProfessionId,
                        Sex = addCustomerViewModel.Individual.SexId,
                        ResidencyAddress = NewAddressInfo(addCustomerViewModel.Individual.ResidencyAddress.NewCustomerAddressInfoRequest, (int)addCustomerViewModel.Individual.ResidencyAddress.PostalCode, addCustomerViewModel.Individual.ResidencyAddress.Floor)
                    },

                    SubscriptionInfo = new SubscriptionRegistrationInfo()
                    {
                        BillingPeriod = addCustomerViewModel.SubscriptionInfo.BillingPeriodId,
                        ServiceID = addCustomerViewModel.SubscriptionInfo.PartnerTariffID,
                        SetupAddress = NewAddressInfo(addCustomerViewModel.SubscriptionInfo.SetupAddress.NewCustomerAddressInfoRequest, (int)addCustomerViewModel.SubscriptionInfo.SetupAddress.PostalCode, addCustomerViewModel.SubscriptionInfo.SetupAddress.Floor)
                    },
                    ExtraInfo = new ExtraInfo()
                    {
                        ApplicationType = addCustomerViewModel.ExtraInfo.SubscriptionRegistrationTypeId,
                        PSTN = addCustomerViewModel.ExtraInfo.PSTN,
                        XDSLNo = addCustomerViewModel.ExtraInfo.XDSLNo
                    }
                },
            };

            var response = Client.NewCustomerRegister(request);

            return response;
        }
        private string DateOfIssueValue(int cardTypeId, IDCardViewModel IDCardViewModel)
        {

            if (cardTypeId == (int)CardType.TCBirthCertificate)
            {
                var dateOfIssue = DateParseOperations.ConvertDatetimeByWebService(IDCardViewModel.TCBirthCertificate.DateOfIssue);
                return dateOfIssue;
            }
            var expiryDate = DateParseOperations.ConvertDatetimeByExpiryDate(IDCardViewModel.TCIDCardWithChip.ExpiryDate);
            return expiryDate;
        }

        private MasterISS_Agent_Website_WebServices.AgentWebService.AddressInfo NewAddressInfo(NewCustomerAddressInfoRequest addressInfoRequest, int postalCode, string floor)
        {
            var request = new MasterISS_Agent_Website_WebServices.AgentWebService.AddressInfo()
            {
                AddressNo = addressInfoRequest.AddressNo,
                AddressText = addressInfoRequest.AddressText,
                ApartmentID = addressInfoRequest.ApartmentId,
                ApartmentNo = addressInfoRequest.ApartmentNo,
                DistrictID = addressInfoRequest.DistrictId,
                DistrictName = addressInfoRequest.DistrictName,
                DoorID = addressInfoRequest.DoorId,
                DoorNo = addressInfoRequest.DoorNo,
                NeighbourhoodID = addressInfoRequest.NeighbourhoodID,
                NeighbourhoodName = addressInfoRequest.NeighbourhoodName,
                ProvinceID = addressInfoRequest.ProvinceId,
                ProvinceName = addressInfoRequest.ProvinceName,
                RuralCode = addressInfoRequest.RuralCode,
                StreetID = addressInfoRequest.StreetId,
                StreetName = addressInfoRequest.StreetName,
                PostalCode = postalCode,
                Floor = floor
            };
            return request;
        }

        public AgentServiceSubscriptionsResponse GetAgentSubscriptions(int pageNo,int pageSize)
        {
            var request = new AgentServiceSubscriptionsRequest
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                SubscriptionsRequestParameters = new AgentSubscriptionsRequest
                {
                    UserEmail = AgentClaimInfo.UserEmail(),
                    Pagination = new PaginationRequest
                    {
                        ItemPerPage = pageSize,
                        PageNo = pageNo - 1
                    }
                }
            };

            var response = Client.GetAgentSubscriptions(request);

            return response;
        }

        public AgentServiceKeyValueListResponse GetAgentTariffs()
        {
            var response = Client.GetAgentTariffs(AgentServiceParameterlessRequest());

            return response;
        }

        public AgentServiceKeyValueListResponse GetCultures()
        {
            var response = Client.GetCultures(AgentServiceParameterlessRequest());

            return response;
        }

        public AgentServiceKeyValueListResponse GetCustomerTypes()
        {
            var response = Client.GetCustomerTypes(AgentServiceParameterlessRequest());

            return response;
        }

        public AgentServiceKeyValueListResponse GetNationalities()
        {
            var response = Client.GetNationalities(AgentServiceParameterlessRequest());

            return response;
        }

        public AgentServiceServiceOperatorsResponse ServiceOperators(long subsciptionId)
        {
            var request = new AgentServiceServiceOperatorsRequest
            {
                Username = Username,
                Culture = Culture,
                Rand = Rand,
                Hash = Hash<SHA256>(),
                ServiceOperatorsParameters = new ServiceOperatorsRequest
                {
                    UserEmail = AgentClaimInfo.UserEmail(),
                    SubscriptionId = subsciptionId
                }
            };

            var response = Client.ServiceOperators(request);

            return response;
        }

        public AgentServiceAddWorkOrderResponse AddWorkOrder(AddWorkOrderViewModel addWorkOrderViewModel)
        {
            var request = new AgentServiceAddWorkOrderRequest
            {
                Hash = Hash<SHA256>(),
                Culture = Culture,
                Rand = Rand,
                Username = Username,
                AddWorkOrder = new AddWorkOrderRequest
                {
                    Description = addWorkOrderViewModel.Description,
                    SubscriptionId = addWorkOrderViewModel.SubscriptionId,
                    XDSLType = (short)addWorkOrderViewModel.XDSLTypes,
                    TaskType = (short)addWorkOrderViewModel.TaskTypes,
                    SetupUserId = addWorkOrderViewModel.SetupUserId,
                    ModemName = addWorkOrderViewModel.ModemName,
                    HasModem = addWorkOrderViewModel.HasModem,
                    UserEmail = AgentClaimInfo.UserEmail()
                }
            };

            var response = Client.AddWorkOrder(request);

            return response;
        }

        public AgentServiceClientFormsResponse GetAgentClientForms(int formTypeId, long subscriptionId)
        {
            var request = new AgentServiceClientFormsRequest
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                ClientFormsParameters = new AgentClientFormsRequest
                {
                    UserEmail = AgentClaimInfo.UserEmail(),
                    FormType = formTypeId,
                    SubscriptionId = subscriptionId
                }
            };

            var response = Client.GetAgentClientForms(request);

            return response;
        }

        public AgentServiceSaveClientAttachmentResponse SaveClientAttachment(AddClientAttachmentViewModel addClientAttachmentViewModel)
        {
            var request = new AgentServiceSaveClientAttachmentRequest
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                SaveClientAttachmentParameters = new SaveAgentClientAttachmentRequest
                {
                    AttachmentType = addClientAttachmentViewModel.AttachmentId,
                    FileContent = addClientAttachmentViewModel.FileContect,
                    FileExtention = addClientAttachmentViewModel.FileExtention,
                    SubscriptionId = addClientAttachmentViewModel.SubscriptionId,
                    UserEmail = AgentClaimInfo.UserEmail()
                }
            };

            var response = Client.SaveClientAttachment(request);

            return response;
        }

        public AgentServiceBillReceiptResponse GetBillReceipt(long billId)
        {
            var request = new AgentServiceBillReceiptRequest
            {
                Username = Username,
                Culture = Culture,
                Hash = Hash<SHA256>(),
                BillReceiptParameters = new BillReceiptRequest
                {
                    UserEmail = AgentClaimInfo.UserEmail(),
                    BillId = billId,
                },
                Rand = Rand
            };
            var response = Client.GetBillReceipt(request);

            return response;
        }

        public AgentServiceRelatedPaymentsResponse GetRelatedPayments(int pageNo,int pageSize)
        {
            var request = new AgentServiceRelatedPaymentsRequest
            {
                Username = Username,
                Rand = Rand,
                Culture = Culture,
                Hash = Hash<SHA256>(),
                RelatedPaymentsParameters = new RelatedPaymentsRequest
                {
                    UserEmail = AgentClaimInfo.UserEmail(),
                    Pagination = new PaginationRequest
                    {
                        ItemPerPage = pageSize,
                        PageNo = pageNo - 1,
                    }
                }
            };
            var response = Client.GetRelatedPayments(request);

            return response;
        }

        public AgentServiceCustomerSetupTaskResponse GetCustomerTasks(long subscriptionId)
        {
            var request = new AgentServiceCustomerSetupTaskRequest
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                CustomerTaskParameters = new CustomerSetupTaskRequest
                {
                    UserEmail = AgentClaimInfo.UserEmail(),
                    SubscriptionId = subscriptionId
                }
            };

            var response = Client.GetCustomerTasks(request);

            return response;
        }

        public AgentServiceKeyValueListResponse GetPaymentDays(long Id)
        {
            var request = new AgentServiceListFromIDRequest
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                ListFromIDRequest = new ListFromIDRequest
                {
                    UserEmail = AgentClaimInfo.UserEmail(),
                    ID = Id
                }
            };
            var response = Client.GetPaymentDays(request);

            return response;
        }

        public AgentServiceKeyValueListResponse GetProfessions()
        {
            var response = Client.GetProfessions(AgentServiceParameterlessRequest());

            return response;
        }

        public AgentServiceKeyValueListResponse GetSexes()
        {
            var response = Client.GetSexes(AgentServiceParameterlessRequest());

            return response;
        }

        public AgentServiceKeyValueListResponse GetTCKTypes()
        {
            var response = Client.GetTCKTypes(AgentServiceParameterlessRequest());

            return response;
        }

        public CustomerServiceNameValuePair GetProvinces()
        {
            var request = new CustomerServiceProvincesRequest
            {
                Username = Username,
                Culture = Culture,
                Rand = Rand,
                Hash = Hash<SHA1>(),
            };
            var response = Client.GetProvinces(request);

            return response;
        }

        public CustomerServiceNameValuePair GetDistricts(long provinceId)
        {
            var response = Client.GetProvinceDistricts(CustomerServiceNameValuePairRequest(provinceId));

            return response;
        }

        public CustomerServiceNameValuePair GetRuralRegions(long districtId)
        {
            var response = Client.GetDistrictRuralRegions(CustomerServiceNameValuePairRequest(districtId));

            return response;
        }

        public CustomerServiceNameValuePair GetNeighbourhoods(long ruralId)
        {
            var response = Client.GetRuralRegionNeighbourhoods(CustomerServiceNameValuePairRequest(ruralId));

            return response;
        }

        public CustomerServiceNameValuePair GetStreets(long neighbourhoodId)
        {
            var response = Client.GetNeighbourhoodStreets(CustomerServiceNameValuePairRequest(neighbourhoodId));

            return response;
        }

        public CustomerServiceNameValuePair GetBuildings(long streetId)
        {
            var response = Client.GetStreetBuildings(CustomerServiceNameValuePairRequest(streetId));

            return response;
        }

        public CustomerServiceNameValuePair GetApartments(long buildingId)
        {
            var response = Client.GetBuildingApartments(CustomerServiceNameValuePairRequest(buildingId));

            return response;
        }

        public AgentServiceSMSCodeResponse SendConfirmationSMS(string phoneNo)
        {
            var request = new AgentServiceSMSCodeRequest
            {
                Culture = Culture,
                SMSCodeRequest = new SMSCodeRequest
                {
                    PhoneNo = phoneNo,
                    UserEmail = AgentClaimInfo.UserEmail()
                },
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
            };

            var response = Client.SendConfirmationSMS(request);

            return response;
        }

        public CustomerServiceAddressDetailsResponse GetApartmentAddress(long BBK)
        {
            var request = new CustomerServiceAddressDetailsRequest
            {
                BBK = BBK,
                Culture = Culture,
                Hash = Hash<SHA1>(),
                Rand = Rand,
                Username = Username,
            };

            var response = Client.GetApartmentAddress(request);

            return response;
        }

        public AgentServiceIDCardValidationResponse IDCardValidation(IDCardValidationViewModel IDCardValidationModel)
        {
            var request = new AgentServiceIDCardValidationRequest
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                IDCardValidationRequest = new IDCardValidationRequest
                {
                    BirthDate = IDCardValidationModel.BirtDate,
                    FirstName = IDCardValidationModel.FirstName,
                    IDCardType = IDCardValidationModel.IdCardType,
                    LastName = IDCardValidationModel.LastName,
                    RegistirationNo = IDCardValidationModel.RegistirationNo,
                    TCKNo = IDCardValidationModel.TCKNo,
                },
            };
            var response = Client.IDCardValidation(request);

            return response;
        }


        CustomerServiceNameValuePairRequest CustomerServiceNameValuePairRequest(long itemCode)
        {
            var request = new CustomerServiceNameValuePairRequest
            {
                Culture = Culture,
                Hash = Hash<SHA1>(),
                ItemCode = itemCode,
                Rand = Rand,
                Username = Username,
            };

            return request;
        }

        public AgentServiceNewCustomerRegisterResponse NewCustomerRegister()
        {
            var request = new AgentServiceNewCustomerRegisterRequest
            {
                Culture = Culture,
                Hash = Hash<SHA1>(),
                Rand = Rand,
                Username = Username,
                CustomerRegisterParameters = new NewCustomerRegisterRequest
                {
                    UserEmail = AgentClaimInfo.UserEmail(),
                }
            };

            var response = Client.NewCustomerRegister(request);

            return response;
        }

        private AgentServiceParameterlessRequest AgentServiceParameterlessRequest()
        {
            var request = new AgentServiceParameterlessRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                ParameterlessRequest = new ParameterlessRequest()
                {
                    UserEmail = AgentClaimInfo.UserEmail(),
                }
            };
            return request;
        }
    }
}