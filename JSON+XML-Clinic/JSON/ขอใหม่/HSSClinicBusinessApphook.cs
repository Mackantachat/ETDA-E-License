using BizPortal.DAL.MongoDB;
using BizPortal.Utils;
using BizPortal.Utils.Extensions;
using BizPortal.ViewModels;
using BizPortal.ViewModels.Apps;
using BizPortal.ViewModels.Apps.SRATStandard;
using BizPortal.ViewModels.Apps.HSSStandard;
using BizPortal.ViewModels.SingleForm;
using BizPortal.ViewModels.V2;
using Mapster;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BizPortal.Utils.Helpers;
using EGA.WS;
using System.Net;
using System.Text;
using RestSharp;

namespace BizPortal.AppsHook
{
    public class HSSClinicBusinessApphook : StoreBaseAppHook
    {
        public override decimal? CalculateFee(List<ISectionData> sectionData)
        {
            return 1000;
        }

        public override bool IsEnabledChat()
        {
            return true;
        }
        public override InvokeResult Invoke(AppsStage stage, ApplicationRequestViewModel model, AppHookInfo appHookInfo, ref ApplicationRequestEntity request)
        {
            InvokeResult result = new InvokeResult();
            result.Success = true;
            result.SendToEmail = model.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("GENERAL_EMAIL").DefaultString();


            // คำร้องใหม่
            if (stage == AppsStage.UserCreate)
            {
                // ใส่ข้อมูลพื้นที่เจ้าของคำร้องจากที่อยู่ร้านค้า
                if (AppSystemNameTextConst.ALL_APP_FORM_HAS_PROVINCE_ONLY.Contains(request.AppSysName))
                {
                    if (request.AppSysName == AppSystemNameTextConst.APP_TAX || request.AppSysName == AppSystemNameTextConst.APP_TAX_RENEW)
                    {
                        var storeInfo = request.Data["INFORMATION_BOARD_TAX_AGENT_AREA_ADDRESS"].Data;
                        if (storeInfo.ContainsKey("ADDRESS_PROVINCE_INFORMATION_BOARD_TAX_AGENT_AREA_ADDRESS"))
                        {
                            request.ProvinceID = int.Parse(storeInfo["ADDRESS_PROVINCE_INFORMATION_BOARD_TAX_AGENT_AREA_ADDRESS"]);
                            request.AmphurID = int.Parse(storeInfo["ADDRESS_AMPHUR_INFORMATION_BOARD_TAX_AGENT_AREA_ADDRESS"]);
                            request.TumbolID = int.Parse(storeInfo["ADDRESS_TUMBOL_INFORMATION_BOARD_TAX_AGENT_AREA_ADDRESS"]);

                            request.Province = (storeInfo["ADDRESS_PROVINCE_INFORMATION_BOARD_TAX_AGENT_AREA_ADDRESS_TEXT"]);
                            request.Amphur = (storeInfo["ADDRESS_AMPHUR_INFORMATION_BOARD_TAX_AGENT_AREA_ADDRESS_TEXT"]);
                            request.Tumbol = (storeInfo["ADDRESS_TUMBOL_INFORMATION_BOARD_TAX_AGENT_AREA_ADDRESS_TEXT"]);
                        }
                    }
                    else if (request.AppSysName == AppSystemNameTextConst.APP_TAX_EDIT || request.AppSysName == AppSystemNameTextConst.APP_TAX_CANCEL)
                    {
                        var storeInfo = request.Data["INFORMATION_STORE_TAX_PROVINCE_ONLY"].Data;
                        if (storeInfo.ContainsKey("ADDRESS_PROVINCE_INFORMATION_STORE_ADDRESS_TAX_PROVINCE_ONLY"))
                        {
                            request.ProvinceID = int.Parse(storeInfo["ADDRESS_PROVINCE_INFORMATION_STORE_ADDRESS_TAX_PROVINCE_ONLY"]);
                            request.AmphurID = int.Parse(storeInfo["ADDRESS_AMPHUR_INFORMATION_STORE_ADDRESS_TAX_PROVINCE_ONLY"]);
                            request.TumbolID = int.Parse(storeInfo["ADDRESS_TUMBOL_INFORMATION_STORE_ADDRESS_TAX_PROVINCE_ONLY"]);

                            request.Province = (storeInfo["ADDRESS_PROVINCE_INFORMATION_STORE_ADDRESS_TAX_PROVINCE_ONLY_TEXT"]);
                            request.Amphur = (storeInfo["ADDRESS_AMPHUR_INFORMATION_STORE_ADDRESS_TAX_PROVINCE_ONLY_TEXT"]);
                            request.Tumbol = (storeInfo["ADDRESS_TUMBOL_INFORMATION_STORE_ADDRESS_TAX_PROVINCE_ONLY_TEXT"]);
                        }
                    }
                    else
                    {
                        var storeInfo = request.Data["INFORMATION_STORE_HAS_PROVINCE_ONLY"].Data;
                        if (storeInfo.ContainsKey("ADDRESS_PROVINCE_INFORMATION_STORE_ADDRESS_PROVINCE_ONLY"))
                        {
                            request.ProvinceID = int.Parse(storeInfo["ADDRESS_PROVINCE_INFORMATION_STORE_ADDRESS_PROVINCE_ONLY"]);
                            request.AmphurID = int.Parse(storeInfo["ADDRESS_AMPHUR_INFORMATION_STORE_ADDRESS_PROVINCE_ONLY"]);
                            request.TumbolID = int.Parse(storeInfo["ADDRESS_TUMBOL_INFORMATION_STORE_ADDRESS_PROVINCE_ONLY"]);

                            request.Province = (storeInfo["ADDRESS_PROVINCE_INFORMATION_STORE_ADDRESS_PROVINCE_ONLY_TEXT"]);
                            request.Amphur = (storeInfo["ADDRESS_AMPHUR_INFORMATION_STORE_ADDRESS_PROVINCE_ONLY_TEXT"]);
                            request.Tumbol = (storeInfo["ADDRESS_TUMBOL_INFORMATION_STORE_ADDRESS_PROVINCE_ONLY_TEXT"]);
                        }
                    }
                }
                else
                {
                    if (request.Data.ContainsKey("INFORMATION_STORE"))
                    {
                        var storeInfo = request.Data["INFORMATION_STORE"].Data;
                        if (storeInfo.ContainsKey("ADDRESS_PROVINCE_INFORMATION_STORE__ADDRESS"))
                        {
                            request.ProvinceID = int.Parse(storeInfo["ADDRESS_PROVINCE_INFORMATION_STORE__ADDRESS"]);
                            request.AmphurID = int.Parse(storeInfo["ADDRESS_AMPHUR_INFORMATION_STORE__ADDRESS"]);
                            request.TumbolID = int.Parse(storeInfo["ADDRESS_TUMBOL_INFORMATION_STORE__ADDRESS"]);

                            request.Province = (storeInfo["ADDRESS_PROVINCE_INFORMATION_STORE__ADDRESS_TEXT"]);
                            request.Amphur = (storeInfo["ADDRESS_AMPHUR_INFORMATION_STORE__ADDRESS_TEXT"]);
                            request.Tumbol = (storeInfo["ADDRESS_TUMBOL_INFORMATION_STORE__ADDRESS_TEXT"]);
                        }
                    }
                }
            }

            //#if TEST_SERVER
            //send to srat
            try
            {

                switch (stage)
                {
                    case AppsStage.UserCreate:
                        {
                            var requestTypes_Dic = new Dictionary<string, string>() {
                            {RequestorInformationValueConst.REQUEST_TYPE_OWNER, RequestorInformationTextConst.REQUEST_TYPE_OWNER},
                            {RequestorInformationValueConst.REQUEST_TYPE_NOMINEE, RequestorInformationTextConst.REQUEST_TYPE_NOMINEE}
                        };
                            var buildingTypes_Dic = new Dictionary<string, string>() {
                            {StoreInformationBuildingTypeOptionValueConst.OWNED, StoreInformationBuildingTypeOptionTextConst.OWNED},
                            {StoreInformationBuildingTypeOptionValueConst.RENT, StoreInformationBuildingTypeOptionTextConst.RENT},
                            {StoreInformationBuildingTypeOptionValueConst.RENT_FREE, StoreInformationBuildingTypeOptionTextConst.RENT_FREE},
                        };
                            var buildingTypesOption_Dic = new Dictionary<string, string>() {
                            {StoreInformationBuildingRentingOwnerTypeOptionValueConst.JURISTIC, StoreInformationBuildingRentingOwnerTypeOptionTextConst.JURISTIC},
                            {StoreInformationBuildingRentingOwnerTypeOptionValueConst.CITIZEN, StoreInformationBuildingRentingOwnerTypeOptionTextConst.CITIZEN},
                            {StoreInformationBuildingRentingOwnerTypeOptionValueConst.Government, StoreInformationBuildingRentingOwnerTypeOptionTextConst.Government},
                            {StoreInformationBuildingRentingOwnerTypeOptionValueConst.Royal, StoreInformationBuildingRentingOwnerTypeOptionTextConst.Royal},
                        };
                         
                            

                            string requestType = string.Empty;
                            string ownerType = string.Empty;

                            var post = new HSSClinicBusinessRequest();
                            post.Id = model.ApplicationRequestID.ToString();
                            post.ApplicationNo = model.ApplicationRequestNumber;
                            post.SubmitDate = DateTime.Now;
                            // post.WroteAt = "Biz Portal";
                            // post.BizId = model.ApplicationRequestNumber;
                            //  post.BizGuid = model.ApplicationRequestID.ToString();
                            string applicantEmail = model.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("GENERAL_EMAIL");
                            string applicantTelephone = null;
                            var contactsAddress = new List<Contact>();
                            var contactsCurrent = new List<Contact>();
                            #region [Contact Current]
                            /// <summary>
                            /// Telephone = 1,
                            /// Mobile = 2,
                            /// Email = 3,
                            /// Fax = 4,
                            /// Facebook = 5,
                            /// Line = 6,
                            /// Website = 7,
                            /// </summary>

                            //tel
                            var currentTel = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_TELEPHONE_CURRENT_INFORMATION__ADDRESS");
                            var currentExt = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_TELEPHONE_EXT_CURRENT_INFORMATION__ADDRESS");
                            var currentTelExt = string.IsNullOrEmpty(currentExt) ? currentTel : currentTel + " ext." + currentExt;
                            contactsCurrent.Add(new Contact()
                            {
                                ContactType = 1,
                                ContactDetail = currentTelExt
                            });
                            // mobile
                            var currentMobile = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_MOBILE_CURRENT_INFORMATION__ADDRESS");
                            if (!string.IsNullOrEmpty(currentMobile))
                            {
                                contactsCurrent.Add(new Contact()
                                {
                                    ContactType = 2,
                                    ContactDetail = currentMobile
                                });
                            }
                            // email
                            var currentEmail = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_EMAIL_CURRENT_INFORMATION__ADDRESS");
                            if (!string.IsNullOrEmpty(currentEmail))
                            {
                                contactsCurrent.Add(new Contact()
                                {
                                    ContactType = 3,
                                    ContactDetail = currentEmail
                                });
                            }
                            // fax
                            var currentFax = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_FAX_CURRENT_INFORMATION__ADDRESS");
                            if (!string.IsNullOrEmpty(currentFax))
                            {
                                contactsCurrent.Add(new Contact()
                                {
                                    ContactType = 4,
                                    ContactDetail = currentFax
                                });
                            }

                            #endregion
                            var currentAddress = new BizPortal.ViewModels.Apps.SRATStandard.Address()
                            {
                                AddressNo = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_CURRENT_INFORMATION__ADDRESS"),
                                VillageNo = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_MOO_CURRENT_INFORMATION__ADDRESS"),
                                Soi = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_SOI_CURRENT_INFORMATION__ADDRESS"),
                                BuildingName = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_BUILDING_CURRENT_INFORMATION__ADDRESS"),
                                RoomNo = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_ROOMNO_CURRENT_INFORMATION__ADDRESS"),
                                FloorNo = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_FLOOR_CURRENT_INFORMATION__ADDRESS"),
                                Road = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_ROAD_CURRENT_INFORMATION__ADDRESS"),
                                SubDistrict = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_TUMBOL_CURRENT_INFORMATION__ADDRESS_TEXT"),
                                District = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_AMPHUR_CURRENT_INFORMATION__ADDRESS_TEXT"),
                                Province = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_PROVINCE_CURRENT_INFORMATION__ADDRESS_TEXT"),
                                PostCode = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_POSTCODE_CURRENT_INFORMATION__ADDRESS"),
                                GeoCode = model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_PROVINCE_CURRENT_INFORMATION__ADDRESS") +
                                                model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_AMPHUR_CURRENT_INFORMATION__ADDRESS") +
                                                model.Data.TryGetData("CURRENT_ADDRESS").ThenGetStringData("ADDRESS_TUMBOL_CURRENT_INFORMATION__ADDRESS"),
                                Latitude = null,
                                Longitude = null,
                                Contacts = contactsCurrent.ToArray()
                            };
                            var contactsInfo = new List<Contact>();
                            #region [Contact Info]
                            /// <summary>
                            /// Telephone = 1,
                            /// Mobile = 2,
                            /// Email = 3,
                            /// Fax = 4,
                            /// Facebook = 5,
                            /// Line = 6,
                            /// Website = 7,
                            /// </summary>

                            //tel
                            var infoTel = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_TELEPHONE_INFORMATION_STORE__ADDRESS");
                            var infoExt = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_TELEPHONE_EXT_INFORMATION_STORE__ADDRESS");
                            var infoTelExt = string.IsNullOrEmpty(infoExt) ? infoTel : infoTel + " ext." + infoExt;
                            contactsInfo.Add(new Contact()
                            {
                                ContactType = 1,
                                ContactDetail = infoTelExt
                            });
                            // mobile
                            var infoMobile = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_MOBILE_INFORMATION_STORE__ADDRESS");
                            if (!string.IsNullOrEmpty(infoMobile))
                            {
                                contactsInfo.Add(new Contact()
                                {
                                    ContactType = 2,
                                    ContactDetail = infoMobile
                                });
                            }
                            // email
                            var infoEmail = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_EMAIL_INFORMATION_STORE__ADDRESS");
                            if (!string.IsNullOrEmpty(infoEmail))
                            {
                                contactsInfo.Add(new Contact()
                                {
                                    ContactType = 3,
                                    ContactDetail = infoEmail
                                });
                            }
                            // fax
                            var infoFax = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_FAX_INFORMATION_STORE__ADDRESS");
                            if (!string.IsNullOrEmpty(infoFax))
                            {
                                contactsInfo.Add(new Contact()
                                {
                                    ContactType = 4,
                                    ContactDetail = infoFax
                                });
                            }

                            #endregion
                            if (model.IdentityType == UserTypeEnum.Citizen)
                            {

                                //การขออนุญาตครั้งนี้ ตรงกับข้อใด *
                                requestType = model.Data.TryGetData("REQUESTOR_INFORMATION__HEADER").ThenGetStringData("CITIZEN_REQUESTOR_INFORMATION__REQUEST_TYPE_OPTION");
                                //อาคารที่ตั้งร้าน/สถานประกอบการของคุณมีลักษณะกรรมสิทธิ์ตามข้อใด
                                ownerType = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("CITIZEN_INFORMATION_STORE_BUILDING_TYPE_OPTION");
                                #region [Contact Address]
                                /// <summary>
                                /// Telephone = 1,
                                /// Mobile = 2,
                                /// Email = 3,
                                /// Fax = 4,
                                /// Facebook = 5,
                                /// Line = 6,
                                /// Website = 7,
                                /// </summary>

                                //tel
                                var contactTel = model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_TELEPHONE_CITIZEN_ADDRESS");
                                var contactExt = model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_TELEPHONE_EXT_CITIZEN_ADDRESS");
                                var contactTelExt = string.IsNullOrEmpty(contactExt) ? contactTel : contactTel + " ext." + contactExt;
                                contactsAddress.Add(new Contact()
                                {
                                    ContactType = 1,
                                    ContactDetail = contactTelExt
                                });
                                // mobile
                                //var contactMobile = model.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("GENERAL_EMAIL");
                                //if (!string.IsNullOrEmpty(contactMobile))
                                //{
                                //    contacts.Add(new Contact()
                                //    {
                                //        ContactType = 2,
                                //        Detail = contactMobile
                                //    });
                                //}
                                //// email
                                //var contactEmail = model.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("GENERAL_EMAIL");
                                //if (!string.IsNullOrEmpty(contactEmail))
                                //{
                                //    contacts.Add(new Contact()
                                //    {
                                //        ContactType = 3,
                                //        Detail = contactEmail
                                //    });
                                //}
                                // fax
                                var contactFax = model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_FAX_CITIZEN_ADDRESS");
                                if (!string.IsNullOrEmpty(contactFax))
                                {
                                    contactsAddress.Add(new Contact()
                                    {
                                        ContactType = 4,
                                        ContactDetail = contactFax
                                    });
                                }

                                #endregion
                           
                                
                                #region Applicant
                                post.Applicant = new PersonApplicant()
                                {
                                    //Type = "Citizen",
                                    CitizenId = model.IdentityID,
                                    Title = model.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("DROPDOWN_CITIZEN_TITLE_TEXT"),
                                    FirstName = model.Data.TryGetData("OPENID").ThenGetStringData("FIRSTNAME_TH"),
                                    LastName = model.Data.TryGetData("OPENID").ThenGetStringData("LASTNAME_TH"),
                                    Nationality = model.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("DROPDOWN_GENERAL_INFORMATION__CITIZEN_NATIONALITY_TEXT"),
                                    Age = model.Data.TryGetData("GENERAL_INFORMATION").ThenGetIntData("GENERAL_INFORMATION__CITIZEN_AGE"),
                                    // Contacts = contacts.ToArray(),
                                    Telephone = applicantTelephone,
                                    Email = applicantEmail,
                                    Address = new BizPortal.ViewModels.Apps.SRATStandard.Address()
                                    {
                                        AddressNo = model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_CITIZEN_ADDRESS"),
                                        VillageNo = model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_MOO_CITIZEN_ADDRESS"),
                                        Soi = model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_SOI_CITIZEN_ADDRESS"),
                                        BuildingName = model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_BUILDING_CITIZEN_ADDRESS"),
                                        RoomNo = model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_ROOMNO_CITIZEN_ADDRESS"),
                                        FloorNo = model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_FLOOR_CITIZEN_ADDRESS"),
                                        Road = model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_ROAD_CITIZEN_ADDRESS"),
                                        SubDistrict = model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_TUMBOL_CITIZEN_ADDRESS_TEXT"),
                                        District = model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_AMPHUR_CITIZEN_ADDRESS_TEXT"),
                                        Province = model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_PROVINCE_CITIZEN_ADDRESS_TEXT"),
                                        PostCode = model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_POSTCODE_CITIZEN_ADDRESS"),
                                        GeoCode = model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_PROVINCE_CITIZEN_ADDRESS") +
                                                model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_AMPHUR_CITIZEN_ADDRESS") +
                                                model.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_TUMBOL_CITIZEN_ADDRESS"),
                                        Latitude = null,
                                        Longitude = null,
                                        Contacts = contactsAddress.ToArray()


                                    },
                                    ContactAddress = currentAddress
                                };


                                #endregion
                            }
                            else if ( model.IdentityType == UserTypeEnum.Juristic)
                            {
                                #region [Juristic Titles]
                                Dictionary<int, string> jurTitles = new Dictionary<int, string>();
                                jurTitles.Add(1, "ห้างหุ้นส่วนจำกัด");
                                jurTitles.Add(2, "บริษัทจำกัด");
                                jurTitles.Add(3, "ห้างหุ้นส่วนสามัญ");
                                jurTitles.Add(4, "สมาคม");
                                jurTitles.Add(5, "มูลนิธิ");
                                #endregion
                                DateTime regDateTime = DateTime.ParseExact(model.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("REGISTER_DATE"), "dd/MM/yyyy", new System.Globalization.CultureInfo("th"));
                                //การขออนุญาตครั้งนี้ ตรงกับข้อใด *
                                requestType = model.Data.TryGetData("REQUESTOR_INFORMATION__HEADER").ThenGetStringData("REQUESTOR_INFORMATION__REQUEST_TYPE_OPTION");
                                //อาคารที่ตั้งร้าน/สถานประกอบการของคุณมีลักษณะกรรมสิทธิ์ตามข้อใด
                                ownerType = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("INFORMATION_STORE_BUILDING_TYPE_OPTION");
                                post.Applicant = new JuristicPersonApplicant()
                                {
                                    //Id = model.IdentityID,
                                    //Type = "Juristic",
                                    JuristicType = model.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("GENERAL_INFORMATION__JURISTIC_TYPE"),
                                    JuristicId = model.IdentityID,
                                    Name = model.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("COMPANY_NAME_TH"),
                                    NameEN = model.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("COMPANY_NAME_EN"),
                                    RegisterDate = regDateTime.ToString("yyyy/MM/dd", new System.Globalization.CultureInfo("en")),
                                    RegisterCapital = model.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("REGISTER_CAPITAL"),
                                    RegisterCapitalPaid = model.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("REGISTER_CAPITAL_PAID"),
                                    Address = new BizPortal.ViewModels.Apps.SRATStandard.Address()
                                    {
                                        AddressNo = model.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_JURISTIC_HQ_ADDRESS"),
                                        VillageNo = model.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_MOO_JURISTIC_HQ_ADDRESS"),
                                        VillageName = model.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_VILLAGE_JURISTIC_HQ_ADDRESS"),
                                        BuildingName = model.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_BUILDING_JURISTIC_HQ_ADDRESS"),
                                        RoomNo = model.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_ROOMNO_JURISTIC_HQ_ADDRESS"),
                                        FloorNo = model.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_FLOOR_JURISTIC_HQ_ADDRESS"),
                                        Soi = model.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_SOI_JURISTIC_HQ_ADDRESS"),
                                        Road = model.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_ROAD_JURISTIC_HQ_ADDRESS"),
                                        SubDistrict = model.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_TUMBOL_JURISTIC_HQ_ADDRESS_TEXT"),
                                        District = model.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_AMPHUR_JURISTIC_HQ_ADDRESS_TEXT"),
                                        Province = model.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_PROVINCE_JURISTIC_HQ_ADDRESS_TEXT"),
                                        PostCode = model.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_POSTCODE_JURISTIC_HQ_ADDRESS"),
                                        GeoCode = model.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_PROVINCE_JURISTIC_HQ_ADDRESS") +
                                               model.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_AMPHUR_JURISTIC_HQ_ADDRESS") +
                                               model.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_TUMBOL_JURISTIC_HQ_ADDRESS"),
                                        Latitude = null,
                                        Longitude = null
                                    },         
                                    ContactAddress = currentAddress,
                                    Telephone = applicantTelephone,
                                    Email = applicantEmail
                                };
                                #region CommitteeData

                                var commiteeTotal = int.Parse(model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("COMMITTEE_INFORMATION_TOTAL"));
                                if (commiteeTotal > 0)
                                {
                                    var committeeList = new List<Committees>();
                                 
                                    for (int i = 0; i < commiteeTotal; i++)
                                    {
                                        var itemId = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ARR_IDX_" + i);

                                        string title = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("JURISTIC_COMMITTEE_TITLE_" + i);
                                        string firstname = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("JURISTIC_COMMITTEE_NAME_" + i);
                                        string lastname = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("JURISTIC_COMMITTEE_LASTNAME_" + i);

                                        var isAuthorizeValue = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("JURISTIC_COMMITTEE_IS_AUTHORIZED_OPTION_" + i);
                                        var isAuthorized = (isAuthorizeValue == "yes") ? true : false;
                                        var isLawyerValue = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("JURISTIC_COMMITTEE_IS_LAWYER_OPTION_" + i);
                                        var isLawyer = (isLawyerValue == "yes") ? true : false;
                                        committeeList.Add(new Committees
                                        {
                                            Order = int.Parse(model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("JURISTIC_COMMITTEE_NUMBER_" + i)),
                                            Title = title,
                                            FirstName = firstname,
                                            LastName = lastname,
                                            CitizenId = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("JURISTIC_COMMITTEE_CITIZEN_ID_" + i),
                                            CommitteeId = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("JURISTIC_COMMITTEE_CITIZEN_ID_" + i),
                                            Nationality = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("JURISTIC_COMMITTEE_NATIONALITY_OPTION__RADIO_TEXT_" + i),
                                            Age = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetIntData("JURISTIC_COMMITTEE_AGE_" + i),
                                            Address = new BizPortal.ViewModels.Apps.SRATStandard.Address
                                            {
                                                AddressNo = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ADDRESS_JURISTIC_COMMITTEE_ADDRESS_" + i),
                                                VillageNo = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ADDRESS_MOO_JURISTIC_COMMITTEE_ADDRESS_" + i),
                                                VillageName = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ADDRESS_VILLAGE_JURISTIC_COMMITTEE_ADDRESS_" + i),
                                                BuildingName = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ADDRESS_BUILDING_JURISTIC_COMMITTEE_ADDRESS_" + i),
                                                RoomNo = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ADDRESS_ROOMNO_JURISTIC_COMMITTEE_ADDRESS_" + i),
                                                FloorNo = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ADDRESS_FLOOR_JURISTIC_COMMITTEE_ADDRESS_" + i),
                                                Soi = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ADDRESS_SOI_JURISTIC_COMMITTEE_ADDRESS_" + i),
                                                Road = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ADDRESS_ROAD_JURISTIC_COMMITTEE_ADDRESS_" + i),
                                                SubDistrict = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ADDRESS_TUMBOL_JURISTIC_COMMITTEE_ADDRESS_TEXT_" + i),
                                                District = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ADDRESS_AMPHUR_JURISTIC_COMMITTEE_ADDRESS_TEXT_" + i),
                                                Province = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ADDRESS_PROVINCE_JURISTIC_COMMITTEE_ADDRESS_TEXT_" + i),
                                                PostCode = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ADDRESS_POSTCODE_JURISTIC_COMMITTEE_ADDRESS_" + i),
                                                GeoCode = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ADDRESS_PROVINCE_JURISTIC_COMMITTEE_ADDRESS_" + i) +
                                                            model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ADDRESS_AMPHUR_JURISTIC_COMMITTEE_ADDRESS_" + i) +
                                                            model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("ADDRESS_TUMBOL_JURISTIC_COMMITTEE_ADDRESS_" + i),

                                            },
                                            CanSigned = isAuthorized,
                                            IsAuthorizedText = model.Data.TryGetData("COMMITTEE_INFORMATION").ThenGetStringData("JURISTIC_COMMITTEE_IS_AUTHORIZED_OPTION__RADIO_TEXT_" + i),
                                            IsLawyer = isLawyer,
                                        });

                                       
                                    }
                                    ((JuristicPersonApplicant)post.Applicant).Committees = committeeList.ToArray();
                                }
                                #endregion
                            }





                            var ownerType_Txt = (buildingTypes_Dic.ContainsKey(ownerType)) ? buildingTypes_Dic.Where(e => e.Key == ownerType).FirstOrDefault().Value : "";
                            var ownerOption = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("INFORMATION_STORE_BUILDING_RENTING_OWNER_TYPE_OPTION");
                            var ownerOption_Txt = (buildingTypesOption_Dic.ContainsKey(ownerOption)) ? buildingTypesOption_Dic.Where(e => e.Key == ownerOption).FirstOrDefault().Value : "";
                            var ownerShipType = (ownerType == "INFORMATION_STORE_BUILDING_TYPE_OPTION__OWNED") ? ownerType_Txt : ownerType_Txt + " (ผู้ให้เช่า/ให้ใช้สถานที่ : " + ownerOption_Txt + ")";
                            var informationstoreAddress = new BizPortal.ViewModels.Apps.SRATStandard.Address()
                            {
                                AddressNo = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_INFORMATION_STORE__ADDRESS"),
                                VillageNo = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_MOO_INFORMATION_STORE__ADDRESS"),
                                Soi = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_SOI_INFORMATION_STORE__ADDRESS"),
                                BuildingName = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_BUILDING_INFORMATION_STORE__ADDRESS"),
                                RoomNo = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_ROOMNO_INFORMATION_STORE__ADDRESS"),
                                FloorNo = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_FLOOR_INFORMATION_STORE__ADDRESS"),
                                Road = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_ROAD_INFORMATION_STORE__ADDRESS"),
                                SubDistrict = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_TUMBOL_INFORMATION_STORE__ADDRESS_TEXT"),
                                District = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_AMPHUR_INFORMATION_STORE__ADDRESS_TEXT"),
                                Province = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_PROVINCE_INFORMATION_STORE__ADDRESS_TEXT"),
                                PostCode = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_POSTCODE_INFORMATION_STORE__ADDRESS"),
                                GeoCode = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_PROVINCE_INFORMATION_STORE__ADDRESS") +
                                           model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_AMPHUR_INFORMATION_STORE__ADDRESS") +
                                           model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_TUMBOL_INFORMATION_STORE__ADDRESS"),
                                Latitude = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_LAT_INFORMATION_STORE__ADDRESS"),
                                Longitude = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_LAT_INFORMATION_STORE__ADDRESS"),
                                Contacts = contactsInfo.ToArray()

                           };
                            post.Address = informationstoreAddress;
                            post.General = new General();
                            //การขออนุญาตครั้งนี้ ตรงกับข้อใด *
                            var requestType_Txt = (requestTypes_Dic.ContainsKey(requestType)) ? requestTypes_Dic.Where(e => e.Key == requestType).FirstOrDefault().Value : "";
                            post.General.ApplicantInfo = new ApplicantInfo
                            {
                                ApplicantType = requestType_Txt
                            };
                            post.General.INFORMATION_STORE = new INFORMATIONSTORE
                            {
                                INFORMATION_STORE_NAME_TH = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("INFORMATION_STORE_NAME_TH"),
                                INFORMATION_STORE_NAME_EN = model.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("INFORMATION_STORE_NAME_EN"),
                                INFORMATION_STORE_ADDRESS = informationstoreAddress,
                                INFORMATION_STORE_BUILDING_TYPE_OPTION = ownerShipType
                            };

                            #region APP_ORG
                            post.APP_CLINIC_BUSINESS = new APPCLINICBUSINESS();
                            post.APP_CLINIC_BUSINESS.APP_CLINIC_INFO_SECTION = new APPCLINICINFOSECTION
                            {

                                APP_CLINIC_INFO_SECTION_TYPE = model.Data.TryGetData("APP_CLINIC_INFO_SECTION").ThenGetStringData("DROPDOWN_APP_CLINIC_INFO_SECTION_TYPE_TEXT"),

                            };

                            post.APP_CLINIC_BUSINESS.APP_CLINIC_PLAN_INFO_SECTION = new APPCLINICPLANINFOSECTION
                            {

                                //APP_CLINIC_PLAN_INFO_SECTION_SERVICES = model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetStringData("DROPDOWN_APP_CLINIC_INFO_SECTION_TYPE_TEXT"),
                                // APP_CLINIC_PLAN_INFO_SECTION_TYPE_OPTION = model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_TYPE_OPTION"),
                                APP_CLINIC_PLAN_INFO_SECTION_BOOTHS = model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_BOOTHS"),
                                APP_CLINIC_PLAN_INFO_SECTION_FLOORS = model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_FLOORS"),
                                APP_CLINIC_PLAN_INFO_SECTION_AREA = model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_AREA"),
                                APP_CLINIC_PLAN_INFO_SECTION_WIDTH = model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_WIDTH"),
                                APP_CLINIC_PLAN_INFO_SECTION_LENGTH = model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_LENGTH"),
                                APP_CLINIC_PLAN_INFO_SECTION_HIGH = model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_HIGH"),
                                APP_CLINIC_PLAN_INFO_SECTION_PROFESSIONALS = model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_PROFESSIONALS"),

                            };
                            var dictTypeOption = new Dictionary<string, string>()
                            {
                                {"HOSPITAL_BUILDINGS", "อาคารสถานพยาบาลโดยเฉพาะ"},
                                {"RESIDENTIAL_BUILDING", "อาคารที่อยู่อาศัย"},
                                {"HOUSE", "ห้องแถว"},
                                {"TENEMENT_HOUSE", "ตึกแถว"},
                                {"ROW_HOUSE", "บ้านแถว"},
                                {"TWIN_HOUSE", "บ้านแฝด"},
                                {"COMMERCIAL_BUILDING", "อาคารพาณิชย์"},
                                {"OFFICE_BUILDING", "อาคารสำนักงาน"},
                                {"SHOPPING_CENTER", "ตั้งอยู่ในศูนย์การค้า"},
                                {"OTHER", "อื่นๆ"}
                            };
                            string TYPE_OPTION = model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_TYPE_OPTION");
                            if (dictTypeOption.ContainsKey(TYPE_OPTION))
                            {
                                if (TYPE_OPTION == "OTHER")
                                {
                                    TYPE_OPTION = model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_TYPE_OTHER");
                                }
                                else
                                {
                                    TYPE_OPTION = dictTypeOption[TYPE_OPTION];
                                }
                            }

                            post.APP_CLINIC_BUSINESS.APP_CLINIC_PLAN_INFO_SECTION.APP_CLINIC_PLAN_INFO_SECTION_TYPE_OPTION = TYPE_OPTION;
                            var app_clinic_plan_info_services = new List<string>();
                            if (model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetBooleanData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_XRAY_ROOM"))
                            {
                                app_clinic_plan_info_services.Add("ห้องเอกซเรย์");
                            }
                            if (model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetBooleanData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_ARTIFICIAL_ROOM"))
                            {
                                app_clinic_plan_info_services.Add("ห้องไตเทียม");
                            }
                            if (model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetBooleanData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_SMALL_ROOM"))
                            {
                                app_clinic_plan_info_services.Add("ห้องผ่าตัดเล็ก");
                            }
                            if (model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetBooleanData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_MAJOR_ROOM"))
                            {
                                app_clinic_plan_info_services.Add("ห้องผ่าตัดใหญ่");
                            }
                            if (model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetBooleanData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_ACUPUNCTURE_ROOM"))
                            {
                                app_clinic_plan_info_services.Add("ห้องฝังเข็ม");
                            }
                            if (model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetBooleanData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_OTHER"))
                            {
                                string other_text = model.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_OTHER_TEXT");
                                app_clinic_plan_info_services.Add(other_text);
                            }
                            post.APP_CLINIC_BUSINESS.APP_CLINIC_PLAN_INFO_SECTION.APP_CLINIC_PLAN_INFO_SECTION_SERVICES = app_clinic_plan_info_services;
                            post.APP_CLINIC_BUSINESS.APP_CLINIC_LICENSE_INFO_SECTION = new APPCLINICLICENSEINFOSECTION
                            {

                                APP_CLINIC_LICENSE_INFO_SECTION_TYPE = model.Data.TryGetData("APP_CLINIC_LICENSE_INFO_SECTION").ThenGetStringData("APP_CLINIC_LICENSE_INFO_SECTION_TYPE"),
                                APP_CLINIC_LICENSE_OLD_INFO_SECTION_TYPE = model.Data.TryGetData("APP_CLINIC_LICENSE_INFO_SECTION").ThenGetStringData("APP_CLINIC_LICENSE_OLD_INFO_SECTION_TYPE"),


                            };

                            var app_clinic_license_details = new List<string>();
                            var app_clinic_license_details_total = int.Parse(model.Data.TryGetData("APP_CLINIC_LICENSE_DETAIL_SECTION").ThenGetStringData("APP_CLINIC_LICENSE_DETAIL_SECTION_TOTAL"));
                            if (app_clinic_license_details_total > 0)
                            {
                                for (int i = 0; i < app_clinic_license_details_total; i++)
                                {
                                    string DAY_TEXT = model.Data.TryGetData("APP_CLINIC_LICENSE_DETAIL_SECTION").ThenGetStringData("DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_DAY_TEXT_" + i);
                                    string START_TIME_TEXT = model.Data.TryGetData("APP_CLINIC_LICENSE_DETAIL_SECTION").ThenGetStringData("DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_START_TIME_TEXT_" + i);
                                    string TIME_OUT_TEXT = model.Data.TryGetData("APP_CLINIC_LICENSE_DETAIL_SECTION").ThenGetStringData("DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_TIME_OUT_TEXT_" + i);
                                    string app_clinic_license_detail = string.Format("วัน {0} เวลาเริ่มงาน {1} เวลาเลิกงาน {2}.", DAY_TEXT, START_TIME_TEXT, TIME_OUT_TEXT);
                                    app_clinic_license_details.Add(app_clinic_license_detail);
                                }
                            }
                            post.APP_CLINIC_BUSINESS.APP_CLINIC_LICENSE_INFO_SECTION.APP_CLINIC_LICENSE_DETAIL_SECTION = app_clinic_license_details;
                            #endregion

                            #region file attachments
                            List<FileMetaData> fileAttachments = new List<FileMetaData>();

                            foreach (FileGroup group in model.UploadedFiles)
                            {
                                foreach (var item in group.Files)
                                {
                                    var description = item.Extras.ContainsKey("FILETYPENAME") ? item.Extras["FILETYPENAME"].ToString() : string.Empty;

                                    var filetype = item.FileTypeCode.Replace("-", "_");
                                    fileAttachments.Add(new FileMetaData()
                                    {
                                        FileId = item.FileID,
                                        DocName = description,
                                        ContentType = item.ContentType,
                                        FileSize = item.FileSize,
                                        Name = item.FileName,
                                        FileTypeCode = filetype
                                    });
                                }

                            }
                            post.ApplicationAttachments = fileAttachments.ToArray();
                            #endregion
                            string regisUrl = "http://srat-api.testapp2.dga.or.th/api/Application?serviceNo=2564011121070000000002&Version=23";
                            var jsonPost = JsonConvert.SerializeObject(post,
                                    Newtonsoft.Json.Formatting.None,
                                    new JsonSerializerSettings
                                    {
                                        NullValueHandling = NullValueHandling.Ignore
                                    }); // Serialize model data to JSON

                            // API Exception
                            Api.OnCheckingApplicationError += (ex) =>
                            {
                                result.Exception = ex;
                                var egaEx = ex as EGAWSAPIException;
                                if (egaEx != null)
                                {
                                    var message = string.Format("{0}: {1}", (int)egaEx.HttpStatusCode, egaEx.ResponseData["Message"].ToString());
                                    result.Data = GenerateAppsHookData(AppsHookResult.Failed, stage, message, egaEx.ResponseData.ToString(), jsonPost);
                                    result.Message = egaEx.ResponseData["Message"].ToString();
                                }
                                else
                                {
                                    result.Data = GenerateAppsHookData(AppsHookResult.Failed, stage, ex.Message, ex.StackTrace, jsonPost);
                                    result.Message = ex.Message;
                                }
                            };

                            var client = new RestClient(regisUrl);
                            var request_ = new RestRequest(Method.POST);
                            request_.RequestFormat = DataFormat.Json;
                            request_.AddHeader("Consumer-Key", Api.ConsumerKey);
                            request_.AddHeader("Content-Type", "application/json");
                            request_.AddParameter("application/json", jsonPost, ParameterType.RequestBody);
                            // request_.AddBody(post);

                            IRestResponse resp = client.Execute(request_);
                            if (resp.StatusCode == HttpStatusCode.OK)
                            {
                                result.Success = true;
                                result.Data = GenerateAppsHookData(AppsHookResult.Created, stage, "", resp.StatusCode.ToString(), jsonPost);

                            }
                            else
                            {
                                string error = "Unable to request to " + regisUrl + ".";
                                result.Data = GenerateAppsHookData(AppsHookResult.Failed, stage, error, null, jsonPost, true);
                                throw new Exception(error);
                            }
                        }
                        break;

                    case AppsStage.UserUpdate:
                        if (model.Status == ApplicationStatusV2Enum.CHECK || model.Status == ApplicationStatusV2Enum.PENDING)
                        {
                            List<FileMetaData> updateFiles = new List<FileMetaData>();
                            #region [POST DATA]
                            var requestedFiles = model.UploadedFiles.Where(o => o.Description == "REQUESTED_FILE").OrderByDescending(o => o.CreatedDate).FirstOrDefault();
                            if (requestedFiles != null && requestedFiles.Files != null && requestedFiles.Files.Count > 0)
                            {
                                foreach (var file in requestedFiles.Files)
                                {
                                    var fileTypeCode = file.FileTypeCode;
                                    var fileTypeName = file.Extras.ContainsKey("FILETYPENAME") ? file.Extras["FILETYPENAME"].ToString() : string.Empty;
                                    var fileId = file.Extras.ContainsKey("FILEID") ? file.Extras["FILEID"].ToString() : string.Empty;

                                    Dictionary<string, object> extras = new Dictionary<string, object>();                                 

                                    if (!string.IsNullOrEmpty(fileId))
                                    {
                                        extras.Add("FileId", fileId);
                                    }

                                    updateFiles.Add(new FileMetaData()
                                    {
                                        FileId = file.FileID,
                                        DocName = fileTypeName,
                                        ContentType = file.ContentType,
                                        FileSize = file.FileSize,
                                        Name = file.FileName,
                                        FileTypeCode = file.FileTypeCode,
                                        Extras = extras
                                    });
                                }

                            }
                            #endregion

                            var jsonPost = JsonConvert.SerializeObject(updateFiles.ToArray());

                            Api.OnCheckingApplicationError += (ex) =>
                            {
                                result.Exception = ex;
                                var egaEx = ex as EGAWSAPIException;
                                if (egaEx != null)
                                {
                                    var message = string.Format("{0}: {1}", (int)egaEx.HttpStatusCode, egaEx.ResponseData["Message"].ToString());
                                    result.Data = GenerateAppsHookData(AppsHookResult.Failed, stage, message, egaEx.ResponseData.ToString(), jsonPost);
                                    result.Message = egaEx.ResponseData["Message"].ToString();
                                }
                                else
                                {
                                    result.Data = GenerateAppsHookData(AppsHookResult.Failed, stage, ex.Message, ex.StackTrace, jsonPost);
                                    result.Message = ex.Message;
                                }
                            };

                            string url = string.Format("http://srat-api.testapp2.dga.or.th/api/Application/{0}/Modify", model.ApplicationRequestID.ToString());
                           
                            var client = new RestClient(url);
                            var request_ = new RestRequest(Method.PUT);
                            request_.RequestFormat = DataFormat.Json;
                            request_.AddHeader("Consumer-Key", Api.ConsumerKey);
                            request_.AddHeader("Content-Type", "application/json");
                            request_.AddParameter("application/json", jsonPost, ParameterType.RequestBody);
                            // request_.AddBody(post);

                            IRestResponse resp = client.Execute(request_);
                            if (resp.StatusCode == HttpStatusCode.OK)
                            {
                                result.Success = true;
                                //result.Data = GenerateAppsHookData(AppsHookResult.Created, stage, "", resp.StatusCode.ToString(), jsonPost);

                            }
                            else
                            {
                                string error = "Unable to request to " + url + ".";
                                result.Data = GenerateAppsHookData(AppsHookResult.Failed, stage, error, null, jsonPost, true);
                                throw new Exception(error);
                            }

                        }
                        else if (model.Status == ApplicationStatusV2Enum.APPROVED_WAITING_PAY_FEE)
                        {

                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Exception = ex;
                result.Success = false;
            }
            //#endif
            return result;
        }
        public override bool IsEnabledExportData(ApplicationStatusV2Enum status)
        {
            var canExportStatus = new ApplicationStatusV2Enum[]
            {
                ApplicationStatusV2Enum.COMPLETED
            };

            return canExportStatus.Contains(status);
        }

        public override string GenerateRequestData(Guid applicationrequestId)
        {
            var request = ApplicationRequestEntity.Get(applicationrequestId);
            var exportRequest = new HSSExportData();
            exportRequest.FormRevisionCode = request.FormRevisionCode;
            exportRequest.FormRevisionName = request.FormRevisionName;
            exportRequest.ApplicationRequestVersion = request.ApplicationRequestVersion;
            exportRequest.IdentityID = request.IdentityID;
            exportRequest.IdentityName = request.IdentityName;
            exportRequest.IdentityType = request.IdentityType.ToString();
            exportRequest.ApplicationID = request.ApplicationID;
            exportRequest.RequestBatchID = request.RequestBatchID.ToString();
            exportRequest.Fee = request.Fee;
            exportRequest.EMSFee = request.EMSFee.ToString();
            exportRequest.DueDateForPayFee = request.DueDateForPayFee;
            exportRequest.Duration = request.Duration;
            exportRequest.ProvinceID = request.ProvinceID;
            exportRequest.AmphurID = request.AmphurID;
            exportRequest.TumbolID = request.TumbolID;
            exportRequest.Province = request.Province;
            exportRequest.Amphur = request.Amphur;
            exportRequest.Tumbol = request.Tumbol;
            exportRequest.SourceIPAddress = request.SourceIPAddress;
            exportRequest.OrgCode = request.OrgCode;
            exportRequest.OrgNameTH = request.OrgNameTH;
            exportRequest.OrgAddress = request.OrgAddress;
            exportRequest.PermitName = request.PermitName;
            exportRequest.BusinessId = request.BusinessId;
            exportRequest.BusinessName = request.BusinessName;
            exportRequest.AppSysName = request.AppSysName;
            exportRequest.CreatedDate = request.CreatedDate;
            exportRequest.UpdatedDate = request.UpdatedDate;
            exportRequest.ExpectSLADate = request.ExpectSLADate.ToString();
            exportRequest.UpdatedDateByRequestor = request.UpdatedDateByRequestor;
            exportRequest.UpdatedDateByAgent = request.UpdatedDateByAgent;
            exportRequest.UpdatedByAgent = request.UpdatedByAgent;
            exportRequest.LastRequestorUpdateEmail = request.LastRequestorUpdateEmail;
            exportRequest.isViewed = request.isViewed;
            exportRequest.Status = request.Status.ToString();
            exportRequest.StatusOther = request.StatusOther;
            exportRequest.StatusRemark = request.StatusRemark;
            exportRequest.IsAgentCheckUserCorrection = request.IsAgentCheckUserCorrection;
            exportRequest.StatusBeforeCancel = request.StatusBeforeCancel;
            exportRequest.ApplicationRequestNumberAgent = request.ApplicationRequestNumberAgent;
            exportRequest.ActionReply = request.ActionReply;
            exportRequest.PermitDeliveryAddress = request.PermitDeliveryAddress;
            exportRequest.PermitDeliveryType = request.PermitDeliveryType;
            exportRequest.EMSFeePaymentType = request.EMSFeePaymentType;
            exportRequest.PaymentMethod = request.PaymentMethod;
            exportRequest.PaymentMethodEnabledChoice = request.PaymentMethodEnabledChoice;
            exportRequest.PaymentMethodOrgDetail = request.PaymentMethodOrgDetail;
            exportRequest.PaymentMethodOrgAddress = request.PaymentMethodOrgAddress;
            exportRequest.PaymentMethodOrgTel = request.PaymentMethodOrgTel;
            exportRequest.BillPaymentTypeForPaymentMethod = request.BillPaymentTypeForPaymentMethod;
            exportRequest.PermitDeliveryTypeEnabledChoice = request.PermitDeliveryTypeEnabledChoice;
            exportRequest.PermitDeliveryOrgDetail = request.PermitDeliveryOrgDetail;
            exportRequest.PermitDeliveryOrgAddress = request.PermitDeliveryOrgAddress;
            exportRequest.PermitDeliveryOrgTel = request.PermitDeliveryOrgTel;
            exportRequest.EMSTrackingNumber = request.EMSTrackingNumber;
            exportRequest.WaitingApproveDateTime = request.WaitingApproveDateTime;
            exportRequest.CheckApproveDateTime = request.CheckApproveDateTime;
            exportRequest.PendingApproveDateTime = request.PendingApproveDateTime;
            exportRequest.PaidFeeApproveDateTime = request.PaidFeeApproveDateTime;
            exportRequest.CreateLicenseApproveDateTime = request.CreateLicenseApproveDateTime;
            exportRequest.RejectDateTime = request.RejectDateTime;
            exportRequest.NoDocument = request.NoDocument;
            exportRequest.TransactionCode = request.TransactionCode;
            exportRequest.TransactionDate = request.TransactionDate;
            exportRequest.DataFiltered = request.DataFiltered;
            exportRequest.DataExcluded = request.DataExcluded;
            exportRequest.Remark = request.Remark;
            exportRequest.RequestedFiles = request.RequestedFiles;
            // public List<object> GovFiles = request.
            exportRequest.RequestedFiles = request.RequestedFiles;
            exportRequest.EPermitFiles = request.EPermitFiles;
            //  public List<object> BillPaymentFiles = request.
            exportRequest.PermitCanBeDeliveredOnPayment = request.PermitCanBeDeliveredOnPayment;
            exportRequest.UserCanGetAppDate = request.UserCanGetAppDate;
            exportRequest.UserCanGetAppDateEnd = request.UserCanGetAppDateEnd;
            exportRequest.ExpectedFinishDate = request.ExpectedFinishDate;
            exportRequest.LastUpdatedFrom = request.LastUpdatedFrom;
            exportRequest.isDone = request.isDone;
            exportRequest.ApplicationRequestID = request.ApplicationRequestID.ToString();
            exportRequest.ApplicationRequestNumber = request.ApplicationRequestNumber;
            exportRequest.ApplicationRequestRunningNumber = request.ApplicationRequestRunningNumber;
            exportRequest.Chats = request.Chats;
            var generalInfo = request.Data.TryGetData("GENERAL_INFORMATION");
            var juristicAddressInfo = request.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION");
            var commiteeInfo = request.Data.TryGetData("COMMITTEE_INFORMATION");
            var citizenAddressInfo = request.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION");
            var currentAddress = request.Data.TryGetData("CURRENT_ADDRESS");
            var requestorInfo = request.Data.TryGetData("REQUESTOR_INFORMATION__HEADER");
            var infoStore = request.Data.TryGetData("INFORMATION_STORE");
            var clinicOwnedOparetor = request.Data.TryGetData("APP_CLINIC_OWNED_OPARETOR_SECTION");
            var clinicOwnedConfirm = request.Data.TryGetData("APP_CLINIC_OWNED_CONFIRM_SECTION");
            var clinicLicenseInfo = request.Data.TryGetData("APP_CLINIC_LICENSE_INFO_SECTION");
            var clinicLicenseDetail = request.Data.TryGetData("APP_CLINIC_LICENSE_DETAIL_SECTION");
            var clinicLicenseInfo2 = request.Data.TryGetData("APP_CLINIC_LICENSE_INFO_SECTION_2");
            var clinicPlanInfo = request.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION");
            var clinicInfo = request.Data.TryGetData("APP_CLINIC_INFO_SECTION");



            #region GENERAL_INFORMATION
            exportRequest.Data = new HSSAppData();
            exportRequest.Data.GENERAL_INFORMATION = new GENERAL_INFORMATION();
            exportRequest.Data.GENERAL_INFORMATION.Data = new GENERAL_INFORMATION_DATA();
            exportRequest.Data.GENERAL_INFORMATION.GroupName = generalInfo.GroupName;
            exportRequest.Data.GENERAL_INFORMATION.Visible = generalInfo.Visible;
            exportRequest.Data.GENERAL_INFORMATION.Data.INFORMATION_HEADER__REQUEST_DATE = generalInfo.ThenGetStringData("INFORMATION_HEADER__REQUEST_DATE");
            exportRequest.Data.GENERAL_INFORMATION.Data.INFORMATION_HEADER__REQUEST_AT = generalInfo.ThenGetStringData("INFORMATION_HEADER__REQUEST_AT");
            exportRequest.Data.GENERAL_INFORMATION.Data.INFORMATION__REQUEST_AS_OPTION = generalInfo.ThenGetStringData("INFORMATION__REQUEST_AS_OPTION");
            exportRequest.Data.GENERAL_INFORMATION.Data.COMPANY_NAME_TH = generalInfo.ThenGetStringData("COMPANY_NAME_TH");
            exportRequest.Data.GENERAL_INFORMATION.Data.COMPANY_NAME_EN = generalInfo.ThenGetStringData("COMPANY_NAME_EN");
            exportRequest.Data.GENERAL_INFORMATION.Data.GENERAL_INFORMATION__JURISTIC_TYPE = generalInfo.ThenGetStringData("GENERAL_INFORMATION__JURISTIC_TYPE");
            exportRequest.Data.GENERAL_INFORMATION.Data.REGISTER_DATE = generalInfo.ThenGetStringData("REGISTER_DATE");
            exportRequest.Data.GENERAL_INFORMATION.Data.CHECKBOX_SHOW_COMMITTEE_INFORMATION = generalInfo.ThenGetStringData("CHECKBOX_SHOW_COMMITTEE_INFORMATION");
            exportRequest.Data.GENERAL_INFORMATION.Data.DROPDOWN_CITIZEN_TITLE = generalInfo.ThenGetStringData("DROPDOWN_CITIZEN_TITLE");
            exportRequest.Data.GENERAL_INFORMATION.Data.CITIZEN_NAME = generalInfo.ThenGetStringData("CITIZEN_NAME");
            exportRequest.Data.GENERAL_INFORMATION.Data.CITIZEN_LASTNAME = generalInfo.ThenGetStringData("CITIZEN_LASTNAME");
            exportRequest.Data.GENERAL_INFORMATION.Data.GENERAL_INFORMATION__CITIZEN_AGE = generalInfo.ThenGetStringData("GENERAL_INFORMATION__CITIZEN_AGE");
            exportRequest.Data.GENERAL_INFORMATION.Data.DROPDOWN_GENERAL_INFORMATION__CITIZEN_NATIONALITY = generalInfo.ThenGetStringData("DROPDOWN_GENERAL_INFORMATION__CITIZEN_NATIONALITY");
            exportRequest.Data.GENERAL_INFORMATION.Data.IDENTITY_ID = generalInfo.ThenGetStringData("IDENTITY_ID");
            exportRequest.Data.GENERAL_INFORMATION.Data.GENERAL_EMAIL = generalInfo.ThenGetStringData("GENERAL_EMAIL");
            exportRequest.Data.GENERAL_INFORMATION.Data.DROPDOWN_CITIZEN_TITLE_TEXT = generalInfo.ThenGetStringData("DROPDOWN_CITIZEN_TITLE_TEXT");
            exportRequest.Data.GENERAL_INFORMATION.Data.DROPDOWN_GENERAL_INFORMATION__CITIZEN_NATIONALITY_TEXT = generalInfo.ThenGetStringData("DROPDOWN_GENERAL_INFORMATION__CITIZEN_NATIONALITY_TEXT");
            exportRequest.Data.GENERAL_INFORMATION.Data.AJAX_DROPDOWN_GENERAL_INFORMATION__CITIZEN_IDCARD_ISSUE_PROVINCE_DDL = generalInfo.ThenGetStringData("AJAX_DROPDOWN_GENERAL_INFORMATION__CITIZEN_IDCARD_ISSUE_PROVINCE_DDL");
            exportRequest.Data.GENERAL_INFORMATION.Data.AJAX_DROPDOWN_GENERAL_INFORMATION__CITIZEN_IDCARD_ISSUE_AMPHUR_DDL = generalInfo.ThenGetStringData("AJAX_DROPDOWN_GENERAL_INFORMATION__CITIZEN_IDCARD_ISSUE_AMPHUR_DDL");
            exportRequest.Data.GENERAL_INFORMATION.Data.AJAX_DROPDOWN_GENERAL_INFORMATION__CITIZEN_IDCARD_ISSUE_PROVINCE_DDL_TEXT = generalInfo.ThenGetStringData("AJAX_DROPDOWN_GENERAL_INFORMATION__CITIZEN_IDCARD_ISSUE_PROVINCE_DDL_TEXT");
            exportRequest.Data.GENERAL_INFORMATION.Data.AJAX_DROPDOWN_GENERAL_INFORMATION__CITIZEN_IDCARD_ISSUE_AMPHUR_DDL_TEXT = generalInfo.ThenGetStringData("AJAX_DROPDOWN_GENERAL_INFORMATION__CITIZEN_IDCARD_ISSUE_AMPHUR_DDL_TEXT");
            exportRequest.Data.GENERAL_INFORMATION.Data.AJAX_DROPDOWN_CITIZEN_TITLE = generalInfo.ThenGetStringData("AJAX_DROPDOWN_CITIZEN_TITLE");
            exportRequest.Data.GENERAL_INFORMATION.Data.AJAX_DROPDOWN_CITIZEN_TITLE_EN = generalInfo.ThenGetStringData("AJAX_DROPDOWN_CITIZEN_TITLE_EN");
            exportRequest.Data.GENERAL_INFORMATION.Data.CITIZEN_FIRSTNAME_EN = generalInfo.ThenGetStringData("CITIZEN_FIRSTNAME_EN");
            exportRequest.Data.GENERAL_INFORMATION.Data.CITIZEN_LASTNAME_EN = generalInfo.ThenGetStringData("CITIZEN_LASTNAME_EN");
            exportRequest.Data.GENERAL_INFORMATION.Data.DROPDOWN_GENERAL_INFORMATION__CITIZEN_RACE = generalInfo.ThenGetStringData("DROPDOWN_GENERAL_INFORMATION__CITIZEN_RACE");
            exportRequest.Data.GENERAL_INFORMATION.Data.BIRTH_DATE = generalInfo.ThenGetStringData("BIRTH_DATE");
            exportRequest.Data.GENERAL_INFORMATION.Data.BIRTH_DATE_AGE = generalInfo.ThenGetStringData("BIRTH_DATE_AGE");
            exportRequest.Data.GENERAL_INFORMATION.Data.AJAX_DROPDOWN_CITIZEN_TITLE_TEXT = generalInfo.ThenGetStringData("AJAX_DROPDOWN_CITIZEN_TITLE_TEXT");
            exportRequest.Data.GENERAL_INFORMATION.Data.AJAX_DROPDOWN_CITIZEN_TITLE_EN_TEXT = generalInfo.ThenGetStringData("AJAX_DROPDOWN_CITIZEN_TITLE_EN_TEXT");
            exportRequest.Data.GENERAL_INFORMATION.Data.DROPDOWN_GENERAL_INFORMATION__CITIZEN_RACE_TEXT = generalInfo.ThenGetStringData("DROPDOWN_GENERAL_INFORMATION__CITIZEN_RACE_TEXT");
            exportRequest.Data.GENERAL_INFORMATION.Data.DROPDOWN_CITIZEN_TITLE_EN = generalInfo.ThenGetStringData("DROPDOWN_CITIZEN_TITLE_EN");
            exportRequest.Data.GENERAL_INFORMATION.Data.DROPDOWN_CITIZEN_TITLE_EN_TEXT = generalInfo.ThenGetStringData("DROPDOWN_CITIZEN_TITLE_EN_TEXT");

            #endregion
            var ownerType = HSSUtility.GetOwnerType().FirstOrDefault(x => x.Value == exportRequest.Data.GENERAL_INFORMATION.Data.INFORMATION__REQUEST_AS_OPTION).Key.ToString();


            if (ownerType == "1")
            {
                #region CITIZEN_ADDRESS_INFORMATION
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION = new CITIZEN_ADDRESS_INFORMATION();
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data = new CITIZEN_ADDRESS_INFORMATION_DATA();
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.GroupName = citizenAddressInfo.GroupName;
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Visible = citizenAddressInfo.Visible;
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_MOO_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_MOO_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_SOI_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_SOI_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_BUILDING_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_BUILDING_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_ROOMNO_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_ROOMNO_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_FLOOR_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_FLOOR_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_ROAD_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_ROAD_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_PROVINCE_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_PROVINCE_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_AMPHUR_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_AMPHUR_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_TUMBOL_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_TUMBOL_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_POSTCODE_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_POSTCODE_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_TELEPHONE_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_TELEPHONE_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_TELEPHONE_EXT_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_TELEPHONE_EXT_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_FAX_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_FAX_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_PROVINCE_CITIZEN_ADDRESS_TEXT = citizenAddressInfo.ThenGetStringData("ADDRESS_PROVINCE_CITIZEN_ADDRESS_TEXT");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_AMPHUR_CITIZEN_ADDRESS_TEXT = citizenAddressInfo.ThenGetStringData("ADDRESS_AMPHUR_CITIZEN_ADDRESS_TEXT");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_TUMBOL_CITIZEN_ADDRESS_TEXT = citizenAddressInfo.ThenGetStringData("ADDRESS_TUMBOL_CITIZEN_ADDRESS_TEXT");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.CHECKBOX_SHOW_CITIZEN_ADDRESS_INFORMATION = citizenAddressInfo.ThenGetStringData("CHECKBOX_SHOW_CITIZEN_ADDRESS_INFORMATION");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_MOBILE_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_MOBILE_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.ADDRESS_EMAIL_CITIZEN_ADDRESS = citizenAddressInfo.ThenGetStringData("ADDRESS_EMAIL_CITIZEN_ADDRESS");
                exportRequest.Data.CITIZEN_ADDRESS_INFORMATION.Data.CITIZEN_EMAIL = citizenAddressInfo.ThenGetStringData("CITIZEN_EMAIL");

                #endregion
            }
            else
            {
                #region JURISTIC_ADDRESS_INFORMATION
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION = new JURISTIC_ADDRESS_INFORMATION();
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data = new JURISTIC_ADDRESS_INFORMATION_DATA();
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.GroupName = juristicAddressInfo.GroupName;
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Visible = juristicAddressInfo.Visible;
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_MOO_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_MOO_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_VILLAGE_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_VILLAGE_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_SOI_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_SOI_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_BUILDING_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_BUILDING_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_ROOMNO_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_ROOMNO_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_FLOOR_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_FLOOR_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_ROAD_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_ROAD_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_PROVINCE_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_PROVINCE_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_PROVINCE_JURISTIC_HQ_ADDRESS_TEXT = juristicAddressInfo.ThenGetStringData("ADDRESS_PROVINCE_JURISTIC_HQ_ADDRESS_TEXT");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_AMPHUR_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_AMPHUR_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_AMPHUR_JURISTIC_HQ_ADDRESS_TEXT = juristicAddressInfo.ThenGetStringData("ADDRESS_AMPHUR_JURISTIC_HQ_ADDRESS_TEXT");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_TUMBOL_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_TUMBOL_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_TUMBOL_JURISTIC_HQ_ADDRESS_TEXT = juristicAddressInfo.ThenGetStringData("ADDRESS_TUMBOL_JURISTIC_HQ_ADDRESS_TEXT");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_POSTCODE_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_POSTCODE_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_TELEPHONE_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_TELEPHONE_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_TELEPHONE_EXT_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_TELEPHONE_EXT_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_FAX_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_FAX_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_MOBILE_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_MOBILE_JURISTIC_HQ_ADDRESS");
                exportRequest.Data.JURISTIC_ADDRESS_INFORMATION.Data.ADDRESS_EMAIL_JURISTIC_HQ_ADDRESS = juristicAddressInfo.ThenGetStringData("ADDRESS_EMAIL_JURISTIC_HQ_ADDRESS");

                #endregion
                #region COMMITTEE_INFORMATION
                exportRequest.Data.COMMITTEE_INFORMATION = new COMMITTEE_INFORMATION();
                exportRequest.Data.COMMITTEE_INFORMATION.Data = new COMMITTEE_INFORMATION_DATA();
                exportRequest.Data.COMMITTEE_INFORMATION.GroupName = commiteeInfo.GroupName;
                exportRequest.Data.COMMITTEE_INFORMATION.Visible = commiteeInfo.Visible;
                int commiteeTotal = commiteeInfo.ThenGetIntData("COMMITTEE_INFORMATION_TOTAL");
                exportRequest.Data.COMMITTEE_INFORMATION.Data.COMMITTEE_INFORMATION_TOTAL = commiteeTotal;
                if (commiteeTotal > 0)
                {
                    var commiteeList = new List<COMMITTEE>();
                    for (int i = 0; i < commiteeTotal; i++)
                    {
                        var commitee = new COMMITTEE()
                        {
                            ARR_IDX = commiteeInfo.ThenGetStringData("ARR_IDX_" + i),
                            JURISTIC_COMMITTEE_NUMBER = commiteeInfo.ThenGetStringData("JURISTIC_COMMITTEE_NUMBER_" + i),
                            JURISTIC_COMMITTEE_TITLE = commiteeInfo.ThenGetStringData("JURISTIC_COMMITTEE_TITLE_" + i),
                            JURISTIC_COMMITTEE_NAME = commiteeInfo.ThenGetStringData("JURISTIC_COMMITTEE_NAME_" + i),
                            JURISTIC_COMMITTEE_LASTNAME = commiteeInfo.ThenGetStringData("JURISTIC_COMMITTEE_LASTNAME_" + i),
                            JURISTIC_COMMITTEE_CITIZEN_ID = commiteeInfo.ThenGetStringData("JURISTIC_COMMITTEE_CITIZEN_ID_" + i),
                            COMMITTEE_INFORMATION_CITIZEN_ID = commiteeInfo.ThenGetStringData("COMMITTEE_INFORMATION_CITIZEN_ID_" + i),
                            JURISTIC_COMMITTEE_NATIONALITY_OPTION = commiteeInfo.ThenGetStringData("JURISTIC_COMMITTEE_NATIONALITY_OPTION_" + i),
                            DROPDOWN_JURISTIC_COMMITTEE_TITLE_TEXT = commiteeInfo.ThenGetStringData("DROPDOWN_JURISTIC_COMMITTEE_TITLE_TEXT_" + i),
                            JURISTIC_COMMITTEE_IS_AUTHORIZED_OPTION = commiteeInfo.ThenGetStringData("JURISTIC_COMMITTEE_IS_AUTHORIZED_OPTION_" + i),
                            JURISTIC_COMMITTEE_IS_LAWYER_LICENSE_DUEDATE = commiteeInfo.ThenGetStringData("JURISTIC_COMMITTEE_IS_LAWYER_LICENSE_DUEDATE_" + i),
                            DROPDOWN_JURISTIC_COMMITTEE_ACCOUNTING_TYPE = commiteeInfo.ThenGetStringData("DROPDOWN_JURISTIC_COMMITTEE_ACCOUNTING_TYPE_" + i),
                            JURISTIC_COMMITTEE_ACCOUNTING_LICENSE_ID = commiteeInfo.ThenGetStringData("JURISTIC_COMMITTEE_ACCOUNTING_LICENSE_ID_" + i),
                            JURISTIC_COMMITTEE_ACCOUNTING_DUE_DATE = commiteeInfo.ThenGetStringData("JURISTIC_COMMITTEE_ACCOUNTING_DUE_DATE_" + i),
                            COMMITTEE_INFORMATION_PASSPORT_NUMBER = commiteeInfo.ThenGetStringData("COMMITTEE_INFORMATION_PASSPORT_NUMBER-" + i),
                            IS_EDIT = commiteeInfo.ThenGetStringData("IS_EDIT_" + i),
                            CUSREQ = commiteeInfo.ThenGetStringData("CUSREQ_" + i),
                            JURISTIC_COMMITTEE_IS_AUTHORIZED_OPTION__RADIO_TEXT = commiteeInfo.ThenGetStringData("JURISTIC_COMMITTEE_IS_AUTHORIZED_OPTION__RADIO_TEXT_" + i),
                            JURISTIC_COMMITTEE_NATIONALITY_OPTION__RADIO_TEXT = commiteeInfo.ThenGetStringData("JURISTIC_COMMITTEE_NATIONALITY_OPTION__RADIO_TEXT_" + i),
                            DROPDOWN_JURISTIC_COMMITTEE_ACCOUNTING_TYPE_TEXT = commiteeInfo.ThenGetStringData("DROPDOWN_JURISTIC_COMMITTEE_ACCOUNTING_TYPE_TEXT_" + i),
                            AJAX_DROPDOWN_COMMITTEE_INFORMATION_CITIZEN_IDCARD_ISSUE_PROVINCE_DDL = commiteeInfo.ThenGetStringData("AJAX_DROPDOWN_COMMITTEE_INFORMATION_CITIZEN_IDCARD_ISSUE_PROVINCE_DDL_" + i),
                            AJAX_DROPDOWN_COMMITTEE_INFORMATION_CITIZEN_IDCARD_ISSUE_AMPHUR_DDL = commiteeInfo.ThenGetStringData("AJAX_DROPDOWN_COMMITTEE_INFORMATION_CITIZEN_IDCARD_ISSUE_AMPHUR_DDL_" + i),
                            AJAX_DROPDOWN_COMMITTEE_INFORMATION_CITIZEN_IDCARD_ISSUE_PROVINCE_DDL_TEXT = commiteeInfo.ThenGetStringData("AJAX_DROPDOWN_COMMITTEE_INFORMATION_CITIZEN_IDCARD_ISSUE_PROVINCE_DDL_TEXT_" + i),
                            AJAX_DROPDOWN_COMMITTEE_INFORMATION_CITIZEN_IDCARD_ISSUE_AMPHUR_DDL_TEXT = commiteeInfo.ThenGetStringData("AJAX_DROPDOWN_COMMITTEE_INFORMATION_CITIZEN_IDCARD_ISSUE_AMPHUR_DDL_TEXT_" + i),

                        };
                        commiteeList.Add(commitee);
                    }
                    exportRequest.Data.COMMITTEE_INFORMATION.Data.Commitees = commiteeList;
                }
                #endregion
            }


            #region CURRENT_ADDRESS
            exportRequest.Data.CURRENT_ADDRESS = new CURRENT_ADDRESS();
            exportRequest.Data.CURRENT_ADDRESS.Data = new CURRENT_ADDRESS_DATA();
            exportRequest.Data.CURRENT_ADDRESS.GroupName = currentAddress.GroupName;
            exportRequest.Data.CURRENT_ADDRESS.Visible = currentAddress.Visible;
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_MOO_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_MOO_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_SOI_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_SOI_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_BUILDING_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_BUILDING_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_ROOMNO_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_ROOMNO_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_FLOOR_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_FLOOR_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_ROAD_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_ROAD_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_PROVINCE_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_PROVINCE_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_AMPHUR_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_AMPHUR_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_TUMBOL_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_TUMBOL_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_POSTCODE_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_POSTCODE_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_TELEPHONE_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_TELEPHONE_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_TELEPHONE_EXT_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_TELEPHONE_EXT_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_FAX_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_FAX_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_MOBILE_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_MOBILE_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_EMAIL_CURRENT_INFORMATION__ADDRESS = currentAddress.ThenGetStringData("ADDRESS_EMAIL_CURRENT_INFORMATION__ADDRESS");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_PROVINCE_CURRENT_INFORMATION__ADDRESS_TEXT = currentAddress.ThenGetStringData("ADDRESS_PROVINCE_CURRENT_INFORMATION__ADDRESS_TEXT");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_AMPHUR_CURRENT_INFORMATION__ADDRESS_TEXT = currentAddress.ThenGetStringData("ADDRESS_AMPHUR_CURRENT_INFORMATION__ADDRESS_TEXT");
            exportRequest.Data.CURRENT_ADDRESS.Data.ADDRESS_TUMBOL_CURRENT_INFORMATION__ADDRESS_TEXT = currentAddress.ThenGetStringData("ADDRESS_TUMBOL_CURRENT_INFORMATION__ADDRESS_TEXT");
            exportRequest.Data.CURRENT_ADDRESS.Data.CURRENT_INFORMATION_STORE__USE_CITIZEN_ADDRESS_CURRENT_INFORMATION_STORE__USE_CITIZEN_ADDRESS__TRUE = currentAddress.ThenGetStringData("CURRENT_INFORMATION_STORE__USE_CITIZEN_ADDRESS_CURRENT_INFORMATION_STORE__USE_CITIZEN_ADDRESS__TRUE");

            #endregion

            exportRequest.Data.REQUESTOR_INFORMATION__HEADER = new REQUESTOR_INFORMATION__HEADER();
            exportRequest.Data.REQUESTOR_INFORMATION__HEADER.GroupName = requestorInfo.GroupName;
            exportRequest.Data.REQUESTOR_INFORMATION__HEADER.Visible = requestorInfo.Visible;
            exportRequest.Data.REQUESTOR_INFORMATION__HEADER.CITIZEN_REQUESTOR_INFORMATION__REQUEST_TYPE_OPTION = requestorInfo.ThenGetStringData("CITIZEN_REQUESTOR_INFORMATION__REQUEST_TYPE_OPTION");
            #region INFORMATION_STORE
            exportRequest.Data.INFORMATION_STORE = new INFORMATION_STORE();
            exportRequest.Data.INFORMATION_STORE.Data = new INFORMATION_STORE_DATA();
            exportRequest.Data.INFORMATION_STORE.GroupName = infoStore.GroupName;
            exportRequest.Data.INFORMATION_STORE.Visible = infoStore.Visible;
            exportRequest.Data.INFORMATION_STORE.Data.INFORMATION_STORE_NAME_TH = infoStore.ThenGetStringData("INFORMATION_STORE_NAME_TH");
            exportRequest.Data.INFORMATION_STORE.Data.INFORMATION_STORE_NAME_EN = infoStore.ThenGetStringData("INFORMATION_STORE_NAME_EN");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_MOO_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_MOO_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_SOI_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_SOI_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_BUILDING_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_BUILDING_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_ROOMNO_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_ROOMNO_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_FLOOR_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_FLOOR_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_ROAD_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_ROAD_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_PROVINCE_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_PROVINCE_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_AMPHUR_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_AMPHUR_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_TUMBOL_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_TUMBOL_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_POSTCODE_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_POSTCODE_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_TELEPHONE_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_TELEPHONE_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_TELEPHONE_EXT_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_TELEPHONE_EXT_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_FAX_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_FAX_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_MOBILE_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_MOBILE_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_EMAIL_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_EMAIL_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_LAT_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_LAT_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_LNG_INFORMATION_STORE__ADDRESS = infoStore.ThenGetStringData("ADDRESS_LNG_INFORMATION_STORE__ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.SEARCH_TEXT_GOOGLE_MAP = infoStore.ThenGetStringData("SEARCH_TEXT_GOOGLE_MAP");
            exportRequest.Data.INFORMATION_STORE.Data.CITIZEN_INFORMATION_STORE_BUILDING_TYPE_OPTION = infoStore.ThenGetStringData("CITIZEN_INFORMATION_STORE_BUILDING_TYPE_OPTION");
            exportRequest.Data.INFORMATION_STORE.Data.INFORMATION_STORE_HEALTH_OTHER = infoStore.ThenGetStringData("INFORMATION_STORE_HEALTH_OTHER");
            exportRequest.Data.INFORMATION_STORE.Data.INFORMATION_STORE__USE_CITIZEN_ADDRESS_INFORMATION_STORE__USE_CITIZEN_ADDRESS__TRUE = infoStore.ThenGetStringData("INFORMATION_STORE__USE_CITIZEN_ADDRESS_INFORMATION_STORE__USE_CITIZEN_ADDRESS__TRUE");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_PROVINCE_INFORMATION_STORE__ADDRESS_TEXT = infoStore.ThenGetStringData("ADDRESS_PROVINCE_INFORMATION_STORE__ADDRESS_TEXT");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_AMPHUR_INFORMATION_STORE__ADDRESS_TEXT = infoStore.ThenGetStringData("ADDRESS_AMPHUR_INFORMATION_STORE__ADDRESS_TEXT");
            exportRequest.Data.INFORMATION_STORE.Data.ADDRESS_TUMBOL_INFORMATION_STORE__ADDRESS_TEXT = infoStore.ThenGetStringData("ADDRESS_TUMBOL_INFORMATION_STORE__ADDRESS_TEXT");
            exportRequest.Data.INFORMATION_STORE.Data.INFORMATION_STORE_TSICID = infoStore.ThenGetStringData("INFORMATION_STORE_TSICID");
            exportRequest.Data.INFORMATION_STORE.Data.AJAX_DROPDOWN_INFORMATION_STORE_OFFICE_CODE = infoStore.ThenGetStringData("AJAX_DROPDOWN_INFORMATION_STORE_OFFICE_CODE");
            exportRequest.Data.INFORMATION_STORE.Data.CHECKBOX_SHOW_INFORMATION_STORE_NAME = infoStore.ThenGetStringData("CHECKBOX_SHOW_INFORMATION_STORE_NAME");
            exportRequest.Data.INFORMATION_STORE.Data.CHECKBOX_SHOW_INFORMATION_STORE_ADDRESS = infoStore.ThenGetStringData("CHECKBOX_SHOW_INFORMATION_STORE_ADDRESS");
            exportRequest.Data.INFORMATION_STORE.Data.AJAX_DROPDOWN_INFORMATION_STORE_OFFICE_CODE_TEXT = infoStore.ThenGetStringData("AJAX_DROPDOWN_INFORMATION_STORE_OFFICE_CODE_TEXT");

            #endregion

            #region clinicOwnedOparetor
            int ownedTotal = clinicOwnedOparetor.ThenGetIntData("APP_CLINIC_OWNED_OPARETOR_SECTION_TOTAL");
            exportRequest.Data.APP_CLINIC_OWNED_OPARETOR_SECTION = new APP_CLINIC_OWNED_OPARETOR_SECTION();
            exportRequest.Data.APP_CLINIC_OWNED_OPARETOR_SECTION.Data = new APP_CLINIC_OWNED_OPARETOR_SECTION_DATA();
            exportRequest.Data.APP_CLINIC_OWNED_OPARETOR_SECTION.Data.APP_CLINIC_OWNED_OPARETOR_SECTION_TOTAL = ownedTotal;
            if (ownedTotal > 0)
            {
                var ownedList = new List<Owned>();
                for (int i = 0; i < ownedTotal; i++)
                {
                    var owned = new Owned()
                    {
                        APP_CLINIC_OWNED_OPARETOR_SECTION_DETAIL_OPTION = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_DETAIL_OPTION_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_TITLE = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_TITLE_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_FIRSTNAME = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_FIRSTNAME_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_LASTNAME = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_LASTNAME_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_AGE = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_AGE_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_NATIONALITY = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_NATIONALITY_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_ID = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_ID_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_PASSPORT = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_PASSPORT_" + i),
                        ADDRESS_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_MOO_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_MOO_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_VILLAGE_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_VILLAGE_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_SOI_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_SOI_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_BUILDING_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_BUILDING_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_ROOMNO_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_ROOMNO_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_FLOOR_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_FLOOR_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_ROAD_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_ROAD_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_PROVINCE_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_PROVINCE_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_AMPHUR_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_AMPHUR_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_TUMBOL_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_TUMBOL_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_POSTCODE_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_POSTCODE_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_TELEPHONE_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_TELEPHONE_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_TELEPHONE_EXT_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_TELEPHONE_EXT_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_FAX_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_FAX_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_EMAIL_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_EMAIL_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_TYPE = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_TYPE_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_BRANCH = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_BRANCH_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_LICENSE_NUMBER = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_LICENSE_NUMBER_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_LICENSING_DATE = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_LICENSING_DATE_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_DIPLOMA = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_DIPLOMA_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_OPARETOR_STATUS = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_OPARETOR_STATUS_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_EMPLOYEE_STATUS = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_EMPLOYEE_STATUS_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_WOKING_PLACE_NAME = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_WOKING_PLACE_NAME_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_WOKING_LICENSE_NUMBER = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_WOKING_LICENSE_NUMBER_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_HOSPITAL_TYPE_OPTION = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_HOSPITAL_TYPE_OPTION_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_CLINIC_DETAIL = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_CLINIC_DETAIL_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_CLINIC_TYPE = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_CLINIC_TYPE_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_CLINIC_OTHER = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_CLINIC_OTHER_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_HOSPITAL_DETAIL = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_HOSPITAL_DETAIL_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_HOSPITAL_CHOICE = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_HOSPITAL_CHOICE_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_HOSPITAL_OTHER = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_HOSPITAL_OTHER_" + i),
                        ADDRESS_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_MOO_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_MOO_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_VILLAGE_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_VILLAGE_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_SOI_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_SOI_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_BUILDING_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_BUILDING_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_ROOMNO_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_ROOMNO_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_FLOOR_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_FLOOR_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_ROAD_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_ROAD_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_PROVINCE_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_PROVINCE_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_AMPHUR_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_AMPHUR_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_TUMBOL_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_TUMBOL_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_POSTCODE_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_POSTCODE_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_TELEPHONE_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_TELEPHONE_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_TELEPHONE_EXT_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_TELEPHONE_EXT_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_FAX_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = clinicOwnedOparetor.ThenGetStringData("ADDRESS_FAX_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_DAY_TIME_WOKING = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_DAY_TIME_WOKING_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_QUIT_WOKING_DATE = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_QUIT_WOKING_DATE_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_CONFIRM_WOKING = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_CONFIRM_WOKING_" + i),
                        ARR_IDX = clinicOwnedOparetor.ThenGetStringData("ARR_IDX_" + i),
                        IS_EDIT = clinicOwnedOparetor.ThenGetStringData("IS_EDIT_" + i),
                        CUSREQ = clinicOwnedOparetor.ThenGetStringData("CUSREQ_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_DETAIL_OPTION__RADIO_TEXT = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_DETAIL_OPTION__RADIO_TEXT_" + i),
                        APP_CLINIC_OWNED_OPARETOR_SECTION_HOSPITAL_TYPE_OPTION__RADIO_TEXT = clinicOwnedOparetor.ThenGetStringData("APP_CLINIC_OWNED_OPARETOR_SECTION_HOSPITAL_TYPE_OPTION__RADIO_TEXT_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_TITLE_TEXT = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_TITLE_TEXT_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_NATIONALITY_TEXT = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_NATIONALITY_TEXT_" + i),
                        ADDRESS_PROVINCE_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_TEXT = clinicOwnedOparetor.ThenGetStringData("ADDRESS_PROVINCE_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_TEXT_" + i),
                        ADDRESS_AMPHUR_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_TEXT = clinicOwnedOparetor.ThenGetStringData("ADDRESS_AMPHUR_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_TEXT_" + i),
                        ADDRESS_TUMBOL_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_TEXT = clinicOwnedOparetor.ThenGetStringData("ADDRESS_TUMBOL_APP_CLINIC_OWNED_OPARETOR_SECTION_OWNER_ADDRESS_TEXT_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_TYPE_TEXT = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_TYPE_TEXT_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_BRANCH_TEXT = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_BRANCH_TEXT_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_OPARETOR_STATUS_TEXT = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_OPARETOR_STATUS_TEXT_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_EMPLOYEE_STATUS_TEXT = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_EMPLOYEE_STATUS_TEXT_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_CLINIC_DETAIL_TEXT = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_CLINIC_DETAIL_TEXT_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_CLINIC_TYPE_TEXT = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_CLINIC_TYPE_TEXT_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_HOSPITAL_DETAIL_TEXT = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_HOSPITAL_DETAIL_TEXT_" + i),
                        DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_HOSPITAL_CHOICE_TEXT = clinicOwnedOparetor.ThenGetStringData("DROPDOWN_APP_CLINIC_OWNED_OPARETOR_SECTION_HOSPITAL_CHOICE_TEXT_" + i),
                        ADDRESS_PROVINCE_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_TEXT = clinicOwnedOparetor.ThenGetStringData("ADDRESS_PROVINCE_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_TEXT_" + i),
                        ADDRESS_AMPHUR_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_TEXT = clinicOwnedOparetor.ThenGetStringData("ADDRESS_AMPHUR_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_TEXT_" + i),
                        ADDRESS_TUMBOL_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_TEXT = clinicOwnedOparetor.ThenGetStringData("ADDRESS_TUMBOL_APP_CLINIC_OWNED_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_TEXT_" + i),
                        ARR_ITEM_ID = clinicOwnedOparetor.ThenGetStringData("ARR_ITEM_ID_" + i),
                    };
                    ownedList.Add(owned);
                }
                exportRequest.Data.APP_CLINIC_OWNED_OPARETOR_SECTION.Data.Owners = ownedList;
            }
            #endregion

            exportRequest.Data.APP_CLINIC_OWNED_CONFIRM_SECTION = new APP_CLINIC_OWNED_CONFIRM_SECTION();
            exportRequest.Data.APP_CLINIC_OWNED_CONFIRM_SECTION.Data = new APP_CLINIC_OWNED_CONFIRM_SECTION_DATA();
            exportRequest.Data.APP_CLINIC_OWNED_CONFIRM_SECTION.GroupName = clinicOwnedConfirm.GroupName;
            exportRequest.Data.APP_CLINIC_OWNED_CONFIRM_SECTION.Visible = clinicOwnedConfirm.Visible;
            exportRequest.Data.APP_CLINIC_OWNED_CONFIRM_SECTION.Data.APP_CLINIC_OWNED_CONFIRM_SECTION_CONFIRM_TRUE_CLINIC_CHECKED_TRUE = clinicOwnedConfirm.ThenGetStringData("APP_CLINIC_OWNED_CONFIRM_SECTION_CONFIRM_TRUE_CLINIC_CHECKED_TRUE");

            exportRequest.Data.APP_CLINIC_LICENSE_INFO_SECTION = new APP_CLINIC_LICENSE_INFO_SECTION();
            exportRequest.Data.APP_CLINIC_LICENSE_INFO_SECTION.Data = new APP_CLINIC_LICENSE_INFO_SECTION_DATA();
            exportRequest.Data.APP_CLINIC_LICENSE_INFO_SECTION.GroupName = clinicLicenseInfo.GroupName;
            exportRequest.Data.APP_CLINIC_LICENSE_INFO_SECTION.Visible = clinicLicenseInfo.Visible;
            exportRequest.Data.APP_CLINIC_LICENSE_INFO_SECTION.Data.APP_CLINIC_LICENSE_INFO_SECTION_TYPE = clinicLicenseInfo.ThenGetStringData("APP_CLINIC_LICENSE_INFO_SECTION_TYPE");
            #region clinicLicenseDetail
            exportRequest.Data.APP_CLINIC_LICENSE_DETAIL_SECTION = new APP_CLINIC_LICENSE_DETAIL_SECTION();
            exportRequest.Data.APP_CLINIC_LICENSE_DETAIL_SECTION.Data = new APP_CLINIC_LICENSE_DETAIL_SECTION_DATA();
            exportRequest.Data.APP_CLINIC_LICENSE_DETAIL_SECTION.GroupName = clinicLicenseInfo.GroupName;
            exportRequest.Data.APP_CLINIC_LICENSE_DETAIL_SECTION.Visible = clinicLicenseInfo.Visible;
            int licenseTotal = clinicLicenseDetail.ThenGetIntData("APP_CLINIC_LICENSE_DETAIL_SECTION_TOTAL");
            exportRequest.Data.APP_CLINIC_LICENSE_DETAIL_SECTION.Data.APP_CLINIC_LICENSE_DETAIL_SECTION_TOTAL = licenseTotal;
            if (licenseTotal > 0)
            {
                var licenseList = new List<License>();
                for (int i = 0; i < licenseTotal; i++)
                {
                    var license = new License()
                    {
                        DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_DAY = clinicLicenseDetail.ThenGetStringData("DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_DAY_" + i),
                        DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_START_TIME = clinicLicenseDetail.ThenGetStringData("DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_START_TIME_" + i),
                        DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_TIME_OUT = clinicLicenseDetail.ThenGetStringData("DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_TIME_OUT_" + i),
                        ARR_IDX = clinicLicenseDetail.ThenGetStringData("ARR_IDX_" + i),
                        IS_EDIT = clinicLicenseDetail.ThenGetStringData("IS_EDIT_" + i),
                        CUSREQ = clinicLicenseDetail.ThenGetStringData("CUSREQ_" + i),
                        DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_DAY_TEXT = clinicLicenseDetail.ThenGetStringData("DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_DAY_TEXT_" + i),
                        DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_START_TIME_TEXT = clinicLicenseDetail.ThenGetStringData("DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_START_TIME_TEXT_" + i),
                        DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_TIME_OUT_TEXT = clinicLicenseDetail.ThenGetStringData("DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_TIME_OUT_TEXT_" + i),
                        ARR_ITEM_ID = clinicLicenseDetail.ThenGetStringData("ARR_ITEM_ID_" + i),
                    };
                    licenseList.Add(license);
                }
                exportRequest.Data.APP_CLINIC_LICENSE_DETAIL_SECTION.Data.Licenses = licenseList;
            }
            #endregion


            exportRequest.Data.APP_CLINIC_LICENSE_INFO_SECTION_2 = new APP_CLINIC_LICENSE_INFO_SECTION_2();
            exportRequest.Data.APP_CLINIC_LICENSE_INFO_SECTION_2.Data = new APP_CLINIC_LICENSE_INFO_SECTION_2_DATA();
            exportRequest.Data.APP_CLINIC_LICENSE_INFO_SECTION_2.GroupName = clinicLicenseInfo2.GroupName;
            exportRequest.Data.APP_CLINIC_LICENSE_INFO_SECTION_2.Visible = clinicLicenseInfo2.Visible;
            exportRequest.Data.APP_CLINIC_LICENSE_INFO_SECTION_2.Data.APP_CLINIC_LICENSE_INFO_SECTION_2_CONFIRM_INFO_TRUE_INFO_CHECKED_TRUE = clinicLicenseInfo2.ThenGetStringData("APP_CLINIC_LICENSE_INFO_SECTION_2_CONFIRM_INFO_TRUE_INFO_CHECKED_TRUE");
            #region clinicPlanInfo
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION = new APP_CLINIC_PLAN_INFO_SECTION();
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data = new APP_CLINIC_PLAN_INFO_SECTION_DATA();
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.GroupName = clinicPlanInfo.GroupName;
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Visible = clinicPlanInfo.Visible;
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_SERVICES_XRAY_ROOM = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_XRAY_ROOM");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_SERVICES_ARTIFICIAL_ROOM = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_ARTIFICIAL_ROOM");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_SERVICES_OTHER_TEXT = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_OTHER_TEXT");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_TYPE_OPTION = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_TYPE_OPTION");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_TYPE_OTHER = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_TYPE_OTHER");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_BOOTHS = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_BOOTHS");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_FLOORS = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_FLOORS");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_AREA = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_AREA");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_WIDTH = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_WIDTH");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_LENGTH = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_LENGTH");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_HIGH = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_HIGH");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_PROFESSIONALS = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_PROFESSIONALS");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_CONFIRM_TRUE = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_CONFIRM_TRUE");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_SERVICES_SMALL_ROOM = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_SMALL_ROOM");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_SERVICES_MAJOR_ROOM = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_MAJOR_ROOM");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_SERVICES_ACUPUNCTURE_ROOM = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_ACUPUNCTURE_ROOM");
            exportRequest.Data.APP_CLINIC_PLAN_INFO_SECTION.Data.APP_CLINIC_PLAN_INFO_SECTION_SERVICES_OTHER = clinicPlanInfo.ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_OTHER");

            #endregion


            //clinicInfo
            exportRequest.Data.APP_CLINIC_INFO_SECTION = new APP_CLINIC_INFO_SECTION();
            exportRequest.Data.APP_CLINIC_INFO_SECTION.Data = new APP_CLINIC_INFO_SECTION_DATA();
            exportRequest.Data.APP_CLINIC_INFO_SECTION.GroupName = clinicInfo.GroupName;
            exportRequest.Data.APP_CLINIC_INFO_SECTION.Visible = clinicInfo.Visible;
            exportRequest.Data.APP_CLINIC_INFO_SECTION.Data.DROPDOWN_APP_CLINIC_INFO_SECTION_TYPE = clinicInfo.ThenGetStringData("DROPDOWN_APP_CLINIC_INFO_SECTION_TYPE");
            exportRequest.Data.APP_CLINIC_INFO_SECTION.Data.DROPDOWN_APP_CLINIC_INFO_SECTION_TYPE_MEDICINE = clinicInfo.ThenGetStringData("DROPDOWN_APP_CLINIC_INFO_SECTION_TYPE_MEDICINE");
            exportRequest.Data.APP_CLINIC_INFO_SECTION.Data.APP_CLINIC_INFO_SECTION_OTHER = clinicInfo.ThenGetStringData("APP_CLINIC_INFO_SECTION_OTHER");
            exportRequest.Data.APP_CLINIC_INFO_SECTION.Data.APP_CLINIC_INFO_SECTION_CONFIRM_CONFIRM_TRUE_FIRST = clinicInfo.ThenGetStringData("APP_CLINIC_INFO_SECTION_CONFIRM_CONFIRM_TRUE_FIRST");
            exportRequest.Data.APP_CLINIC_INFO_SECTION.Data.DROPDOWN_APP_CLINIC_INFO_SECTION_TYPE_TEXT = clinicInfo.ThenGetStringData("DROPDOWN_APP_CLINIC_INFO_SECTION_TYPE_TEXT");
            exportRequest.Data.APP_CLINIC_INFO_SECTION.Data.DROPDOWN_APP_CLINIC_INFO_SECTION_TYPE_MEDICINE_TEXT = clinicInfo.ThenGetStringData("DROPDOWN_APP_CLINIC_INFO_SECTION_TYPE_MEDICINE_TEXT");


            return JsonConvert.SerializeObject(exportRequest, Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
        }


        //ใบขออนุญาตให้ประกอบกิจการสถานพยาบาล (ประเภทไม่รับผู้ป่วยไว้ค้างคืน) บุคคลธรรมดา
        public override JObject GenerateELicenseData(Guid applicationrequestId)
        {
            var data = (object)null;
            var request = ApplicationRequestEntity.Get(applicationrequestId);
            //var files = request.UploadedFiles.Where(e => e.Description == "APP_CLINIC_OWNED_FILE_SECTION").FirstOrDefault();
            //var phofilePhotoMeta = files.Files.Where(e => e.FileTypeCode == "APP_CLINIC_OWNED_PHOTO3M").FirstOrDefault();//.Adapt<BizPortal.ViewModels.V2.FileMetadata>();

            var licenseNumber = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("Identifier");
            var identityName = string.Empty;
            switch (request.IdentityType)
            {
                case UserTypeEnum.Citizen:
                    identityName = request.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("DROPDOWN_CITIZEN_TITLE_TEXT") + " " + request.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("CITIZEN_NAME") +
                                   " " + request.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("CITIZEN_LASTNAME");
                    break;
                case UserTypeEnum.Juristic:
                    var titleCompany = request.Data.TryGetData("GENERAL_INFORMATION").ThenGetStringData("GENERAL_INFORMATION__JURISTIC_TYPE").RemoveLTD();
                    identityName = titleCompany + request.IdentityName;
                    break;
            }
            var createDate = DateTime.Now.ToString("dd MMMM yyyy", new CultureInfo("th"));
            //var practitionerPhoto = Convert.ToBase64String(phofilePhotoMeta.GetBytes());
            //var practitionerPhotoContentType = "image/jpg";
            //var practitionerPhotoUrl = "";
            //var practitionerPhotoSize = phofilePhotoMeta.FileSize;
            //var practitionerPhotoTitle = phofilePhotoMeta.FileName;
            //var practitionerQualificationId = "";
            //var practitionerQualificationType = "";
            var practitionerQualificationStart = "";
            var professionallicenseNumber = "";
            var characteristicsSanatorium = request.Data.TryGetData("APP_CLINIC_INFO_SECTION").ThenGetStringData("DROPDOWN_APP_CLINIC_INFO_SECTION_TYPE_TEXT");
            var typeMedicalFacility = request.Data.TryGetData("APP_CLINIC_LICENSE_INFO_SECTION").ThenGetStringData("APP_CLINIC_LICENSE_INFO_SECTION_TYPE");
            //var clinicName = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("INFORMATION_STORE_NAME_TH");
            var clinicName = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("ClinicName");
            var organizationAddressNumber = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_INFORMATION_STORE__ADDRESS");
            var organizationMoo = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_MOO_INFORMATION_STORE__ADDRESS");
            var organizationSoi = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_SOI_INFORMATION_STORE__ADDRESS");
            var organizationStreet = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_ROAD_INFORMATION_STORE__ADDRESS");
            var organizationCity = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_TUMBOL_INFORMATION_STORE__ADDRESS");
            var organizationCityValue = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_TUMBOL_INFORMATION_STORE__ADDRESS_TEXT");
            var organizationDistrict = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_AMPHUR_INFORMATION_STORE__ADDRESS");
            var organizationDistrictValue = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_AMPHUR_INFORMATION_STORE__ADDRESS_TEXT");
            var organizationState = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_PROVINCE_INFORMATION_STORE__ADDRESS");
            var organizationStateValue = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_PROVINCE_INFORMATION_STORE__ADDRESS_TEXT");
            var organizationPostalCode = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_POSTCODE_INFORMATION_STORE__ADDRESS");
            var organizationCountry = "TH";
            var organizationPhone = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_TELEPHONE_INFORMATION_STORE__ADDRESS");
            var organizationFax = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_FAX_INFORMATION_STORE__ADDRESS");
            var organizationEmail = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_EMAIL_INFORMATION_STORE__ADDRESS");
            var permitStartDate = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("PermitStartDate");
            var permitEndDate = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("PermitEndDate");
            //var services = request.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetBooleanData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_XRAY_ROOM") == true ? "ห้องเอกซเรย์," : string.Empty;
            //services = services + (request.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetBooleanData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_ARTIFICIAL_ROOM") == true ? "ห้องไตเทียม," : string.Empty);
            //services = services + (request.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetBooleanData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_SMALL_ROOM") == true ? "ห้องผ่าตัดเล็ก," : string.Empty);
            //services = services + (request.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetBooleanData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_MAJOR_ROOM") == true ? "ห้องผ่าตัดใหญ่," : string.Empty);
            //services = services + (request.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetBooleanData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_ACUPUNCTURE_ROOM") == true ? "ห้องฝังเข็ม," : string.Empty);
            //services = services + (request.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetBooleanData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_OTHER") == true ? request.Data.TryGetData("APP_CLINIC_PLAN_INFO_SECTION").ThenGetStringData("APP_CLINIC_PLAN_INFO_SECTION_SERVICES_OTHER_TEXT") + "," : string.Empty);
            //services = services.Length > 0 ? services.Substring(0, services.Length - 1) : services;
            var services = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("ExtraService");
            var totalDate = 0;
            var getOpenServicesDate = request.Data.TryGetData("APP_CLINIC_LICENSE_DETAIL_SECTION").Data;
            var openServicesDate = string.Empty;

            //if (int.TryParse(getOpenServicesDate["APP_CLINIC_LICENSE_DETAIL_SECTION_TOTAL"], out totalDate))
            //{
            //    for (int i = 0; i < totalDate; i++)
            //    {
            //        openServicesDate = openServicesDate + getOpenServicesDate["DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_DAY_TEXT_" + i] + " " + getOpenServicesDate["DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_START_TIME_TEXT_" + i] + " - " + getOpenServicesDate["DROPDOWN_APP_CLINIC_LICENSE_DETAIL_SECTION_TIME_OUT_TEXT_" + i] + " น. , ";
            //    }
            //    openServicesDate = openServicesDate.Substring(0, openServicesDate.Length - 2);
            //}
            //else
            //{
            //    throw new NullReferenceException("Cannot parse APP_CLINIC_LICENSE_DETAIL_SECTION_TOTAL to int");
            //}
            openServicesDate = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("OperationTime");

            var permitEnd = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("PermitEndYear");
            var permitDate = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("PermitDate");

            data = new
            {
                Bundle = new
                {
                    id = new
                    {
                        attr_value = "Request-for-permission-to-operate-a-medical-facility-clinic"
                    },
                    identifier = new
                    {
                        system = new
                        {
                            attr_value = "https://oid.estandard.or.th"
                        },
                        value = new
                        {
                            attr_value = "2.16.764.1.4.100.16.5.1.1"
                        }
                    },
                    type = new
                    {
                        attr_value = "document"
                    },
                    timestamp = new
                    {
                        attr_value = createDate.ToArabicDigit()
                    },
                    entry = new object[] {
                        new {
                            fullUrl = new {
                                attr_value = "https://schemas.teda.th/Composition/1"
                            },
                            resource = new {
                                Composition = new {
                                    id = new {
                                        attr_value = "1"
                                    },
                                    extension = new {
                                        attr_url = "http://hl7.org/fhir/StructureDefinition/composition-clinicaldocument-versionNumber",
                                        valueString = new {
                                            attr_value = "1.0.0"
                                        }
                                    },
                                    identifier = new {
                                        attr_id = licenseNumber.ToArabicDigit()
                                    },
                                    status = new {
                                        attr_value = "final"
                                    },
                                    type = new {
                                        text = new {
                                            attr_value = "ส.พ.7"
                                        }
                                    },
                                    subject = new {
                                        reference = new {
                                            attr_value = "https://schemas.teda.th/practitionerrole/pr1"
                                        }
                                    },
                                    date = new {
                                        attr_value = createDate.ToArabicDigit()
                                    },
                                    author = new {
                                        reference = new {
                                            attr_value = "Practitioner/p2"
                                        }
                                    },
                                    title = new {
                                        attr_value = "ขอใบอนุญาตให้ประกอบกิจการสถานพยาบาล (คลินิก)"
                                    },
                                    relatesTo = new {
                                        code = new {
                                            attr_id = "NEW",
                                            attr_value = "signs"
                                        },
                                        targetIdentifier = new {
                                            attr_id = "-"
                                        }
                                    }
                                }
                            }
                        },
                        new {
                            fullUrl = new {
                                attr_value = "https://schemas.teda.th/practitionerrole/pr1"
                            },
                            resource = new {
                                PractitionerRole = new {
                                    id = new {
                                        attr_value = "pr1"
                                    },
                                    period = new {
                                        start = new {
                                            attr_value = permitDate.ToArabicDigit()
                                        },
                                        end = new {
                                            attr_value = permitEnd.ToArabicDigit()
                                        }
                                    },
                                    practitioner = new {
                                        reference = new {
                                            attr_value = "https://schemas.teda.th/Practitioner/p1"
                                        }
                                    },
                                    healthcareService = new {
                                        reference = new {
                                            attr_value = "https://schemas.teda.th/healthcareservice/h1"
                                        }
                                    },
                                    availableTime = new {
                                        modifierExtension = new {
                                            attr_url = "https://schemas.teda.th/availableTime/at1",
                                            valueString = new {
                                                attr_value = openServicesDate.ToArabicDigit()
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        new {
                            fullUrl = new {
                                attr_value = "https://schemas.teda.th/Practitioner/p1"
                            },
                            resource = new {
                                Practitioner = new {
                                    id = new {
                                        attr_value = "p1"
                                    },
                                    name = new {
                                        text = new {
                                            attr_value = identityName.ToArabicDigit()
                                        }
                                    },
                                    qualification = new {
                                        identifier = new {
                                            attr_id = "",//"เลขที่ใบประกอบวิชาชีพ",
                                            value = new {
                                            attr_value = ""//"เวชกรรม"
                                            }
                                        },
                                        code = new {
                                            text = new {
                                            attr_value = "CER"
                                            }
                                        },
                                        period = new {
                                            start = new {
                                            attr_value = "",//"2020-06-16"
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        new {
                            fullUrl = new {
                                attr_value = "https://schemas.teda.th/healthcareservice/h1"
                            },
                            resource = new {
                                HealthcareService = new {
                                    id = new {
                                        attr_value = "h1"
                                    },
                                    providedBy = new {
                                        reference = new {
                                            attr_value = "https://schemas.teda.th/Organization/o1"
                                        }
                                    },
                                    type = new {
                                        text = new {
                                            attr_value = typeMedicalFacility.ToArabicDigit()
                                        }
                                    },
                                    specialty = new {
                                        text = new {
                                            attr_value = "-" // จำนวนเตียง คลินิคไม่มีจำนวนเตียง
                                        }
                                    },
                                    comment = new {
                                        attr_value =  characteristicsSanatorium.ToArabicDigit()
                                    },
                                    eligibility = new {
                                        comment = new {
                                            attr_value = services.ToArabicDigit()
                                        }
                                    }
                                }
                            }
                        },
                        new {
                            fullUrl = new {
                            attr_value = "https://schemas.teda.th/Organization/o1"
                            },
                            resource = new {
                                Organization = new {
                                    id = new {
                                        attr_value = "o1"
                                    },
                                    name = new {
                                        attr_value = clinicName.ToArabicDigit()
                                    },
                                    address = new {
                                        text = new {
                                            attr_value = "",//"ระบุที่ตั้งสถานประกอบการแบบ Unstructure"
                                        },
                                        line = new object[] {
                                            new {
                                            attr_id = "No.",
                                            attr_value = organizationAddressNumber.ToArabicDigit()
                                            },
                                            new {
                                            attr_id = "Moo",
                                            attr_value = organizationMoo.ToArabicDigit()
                                            },
                                            new {
                                            attr_id = "Soi",
                                            attr_value = organizationSoi.ToArabicDigit()
                                            },
                                            new {
                                            attr_id = "Street",
                                            attr_value = organizationStreet.ToArabicDigit()
                                            }
                                        },
                                        city = new {
                                            attr_value = organizationCity,
                                            attr_valueString = organizationCityValue
                                        },
                                        district = new {
                                            attr_value = organizationDistrict,
                                            attr_valueString = organizationDistrictValue.RemoveTextDistrict()
                                        },
                                        state = new {
                                            attr_value = organizationState,
                                            attr_valueString = organizationStateValue
                                        },
                                        postalCode = new {
                                            attr_value = organizationPostalCode.ToArabicDigit()
                                        },
                                        country = new {
                                            attr_value = organizationCountry
                                        }
                                    },
                                    contact = new {
                                        telecom = new object[] {
                                            new {
                                            system = new {
                                                attr_value = "phone"
                                            },
                                            value = new {
                                                attr_value =  organizationPhone.ToArabicDigit()
                                            },
                                            use = new {
                                                attr_value = "work"
                                            }
                                            },
                                            new {
                                            system = new {
                                                attr_value = "fax"
                                            },
                                            value = new {
                                                attr_value = organizationFax.ToArabicDigit()
                                            },
                                            use = new {
                                                attr_value = "work"
                                            }
                                            },
                                            new {
                                            system = new {
                                                attr_value = "email"
                                            },
                                            value = new {
                                                attr_value = organizationEmail.ToArabicDigit()
                                            },
                                            use = new {
                                                attr_value = "work"
                                            }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        new {
                            fullUrl = new {
                                attr_value = "https://schemas.teda.th/Practitioner/p2"
                            },
                            resource = new {
                                Practitioner = new {
                                    id = new {
                                        attr_value = "p2"
                                    },
                                    name = new {
                                        text = new {
                                            attr_value = identityName.ToArabicDigit()
                                        }
                                    }
                                }
                            }
                        }
                    },

                }

            };

            var json = JObject.FromObject(data);

            return json;
        }
    }

}
