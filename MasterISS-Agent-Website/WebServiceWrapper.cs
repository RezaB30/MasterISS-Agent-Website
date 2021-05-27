using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using MasterISS_Agent_Website_WebServices.AgentWebService;
using MasterISS_Agent_Website.ViewModels;
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

        public AgentServiceSubscriptionsResponse GetAgentSubscriptions()
        {
            var request = new AgentServiceSubscriptionsRequest
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                SubscriptionsRequestParameters = new RequestBase
                {
                    UserEmail = AgentClaimInfo.UserEmail(),
                },
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
                Hash = Hash<SHA256>(),
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

        public CustomerServiceAddressDetailsResponse GetAddress(long BBK)
        {
            var request = new CustomerServiceAddressDetailsRequest
            {
                BBK = BBK,
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
            };

            var response = Client.GetApartmentAddress(request);

            return response;
        }




        CustomerServiceNameValuePairRequest CustomerServiceNameValuePairRequest(long itemCode)
        {
            var request = new CustomerServiceNameValuePairRequest
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
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
                Hash = Hash<SHA256>(),
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