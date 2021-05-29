using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Agent_Website
{
    public static class AddressInfo
    {
        internal static SelectList ProvincesList(long? selectedValue)
        {
            var wrapper = new WebServiceWrapper();
            var provinceList = wrapper.GetProvinces();

            var list = new SelectList(provinceList.ValueNamePairList.Select(tck => new { Name = tck.Name, Value = tck.Value }), "Value", "Name", selectedValue);

            return list;
        }

        internal static SelectList DistrictList(long? provinceId, long? selectedValue)
        {
            if (provinceId.HasValue)
            {
                var wrapper = new WebServiceWrapper();
                var districtList = wrapper.GetDistricts((long)provinceId);
                var list = new SelectList(districtList.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }), "Value", "Name", selectedValue);
                return list;
            }
            return new SelectList("");

        }

        internal static SelectList RuralRegionsList(long? districtId, long? selectedValue)
        {
            if (districtId.HasValue)
            {
                var wrapper = new WebServiceWrapper();
                var ruralRegionsList = wrapper.GetRuralRegions((long)districtId);
                var list = new SelectList(ruralRegionsList.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }), "Value", "Name", selectedValue);
                return list;
            }
            return new SelectList("");
        }

        internal static SelectList NeighborhoodList(long? ruralRegionsId, long? selectedValue)
        {
            if (ruralRegionsId.HasValue)
            {
                var wrapper = new WebServiceWrapper();
                var neighborhoodList = wrapper.GetNeighbourhoods((long)ruralRegionsId);
                var list = new SelectList(neighborhoodList.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }), "Value", "Name", selectedValue);
                return list;
            }
            return new SelectList("");
        }

        internal static SelectList StreetList(long? neighborhoodId, long? selectedValue)
        {
            if (neighborhoodId.HasValue)
            {
                var wrapper = new WebServiceWrapper();
                var streetList = wrapper.GetStreets((long)neighborhoodId);
                var list = new SelectList(streetList.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }), "Value", "Name", selectedValue);
                return list;
            }
            return new SelectList("");
        }

        internal static SelectList BuildingList(long? streetId, long? selectedValue)
        {
            if (streetId.HasValue)
            {
                var wrapper = new WebServiceWrapper();
                var buildList = wrapper.GetBuildings((long)streetId);
                var list = new SelectList(buildList.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }), "Value", "Name", selectedValue);
                return list;
            }
            return new SelectList("");
        }

        internal static SelectList ApartmentList(long? buildingId, long? selectedValue)
        {
            if (buildingId.HasValue)
            {
                var wrapper = new WebServiceWrapper();
                var apartmentList = wrapper.GetApartments((long)buildingId);
                var list = new SelectList(apartmentList.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }), "Value", "Name");
                return list;
            }
            return new SelectList("");

        }
    }
}