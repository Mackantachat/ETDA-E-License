using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizPortal.DAL.MongoDB;
using BizPortal.ViewModels.SingleForm;
using BizPortal.ViewModels.V2;
using BizPortal.Utils.Extensions;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Ocsp;
using BizPortal.Utils.Helpers;
namespace BizPortal.AppsHook
{
    class OCPBDirectSellAppHook : SingleFormRendererAppHook
    {
        public override decimal? CalculateFee(List<ISectionData> sectionData)
        {
            return 0;
            //throw new Exception("ไม่พบข้อมูลการจำหน่ายสุราใน SingleFormRequest");
        }

        public override InvokeResult Invoke(AppsStage stage, ApplicationRequestViewModel model, AppHookInfo appHookInfo, ref ApplicationRequestEntity request)
        {
            // หลังจากทำ Biz-api เสร็จแล้วจะต้องนำโค้ดชุดนี้ออก
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

            // TODO: Updated on 2020-05-08, do not submit data to third-party.
            return new InvokeResult() { Success = true };
        }

        public override decimal CalculateEMSFee(List<ISectionData> sectionData)
        {
            return 0;
        }

        public override string PrintFormTitle
        {
            get
            {
                return "คำขอจดทะเบียนประกอบธุรกิจขายตรง";
            }
        }

        public override string PrintFormHeaderRight
        {
            get
            {
                return "ขต.๑";
            }
        }

        public override bool IsEnabledChat()
        {
            return true;
        }

        public override JObject GenerateELicenseData(Guid applicationrequestId)
        {
            var request = ApplicationRequestEntity.Get(applicationrequestId);
            var specifiedPersonId = request.IdentityID.ToThaiDigit();
            var specifiedPersonName = request.IdentityName.ToThaiDigit();
            var addressNo = string.Empty;
            var addressSoi = string.Empty;
            var addressRoad = string.Empty;
            var addressSubDistrict = string.Empty;
            var addressDistrict = string.Empty;
            var addressProvince = string.Empty;
            var addressPostCode = string.Empty;
            var addressTel = string.Empty;
            var issueDate = request.CreatedDateTxt.ToThaiDigit(); // วันที่ยื่นคำขอ
            var licenseNo = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("Identifier").ToThaiDigit(); // เลขที่ใบอนุญาต
            var effectiveDate = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("PermitStartDate").ToThaiDigit(); // วันที่อนุมัติ
            var actualDate = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("PermitDate").ToThaiDigit(); // วันที่ออกใบอนุญาต
            var signer = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("Signer_0"); // ผู้อนุมัติ
            var signerDescription = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("SignerDecription_0"); // รายละเอียดผู้อนุมัติ

            switch (request.IdentityType)
            {
                case UserTypeEnum.Citizen:
                    addressNo = request.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_CITIZEN_ADDRESS").ToThaiDigit();
                    addressSoi = request.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_SOI_CITIZEN_ADDRESS").ToThaiDigit();
                    addressRoad = request.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_ROAD_CITIZEN_ADDRESS").ToThaiDigit();
                    addressSubDistrict = request.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_TUMBOL_CITIZEN_ADDRESS_TEXT").ToThaiDigit();
                    addressDistrict = request.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_AMPHUR_CITIZEN_ADDRESS_TEXT").ToThaiDigit();
                    addressProvince = request.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_PROVINCE_CITIZEN_ADDRESS_TEXT").ToThaiDigit();
                    addressPostCode = request.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_POSTCODE_CITIZEN_ADDRESS").ToThaiDigit();
                    addressTel = request.Data.TryGetData("CITIZEN_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_TELEPHONE_CITIZEN_ADDRESS").ToThaiDigit();
                    break;
                case UserTypeEnum.Juristic:
                    addressNo = request.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_JURISTIC_HQ_ADDRESS").ToThaiDigit();
                    addressSoi = request.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_SOI_JURISTIC_HQ_ADDRESS").ToThaiDigit();
                    addressRoad = request.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_ROAD_JURISTIC_HQ_ADDRESS").ToThaiDigit();
                    addressSubDistrict = request.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_TUMBOL_JURISTIC_HQ_ADDRESS_TEXT").ToThaiDigit();
                    addressDistrict = request.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_AMPHUR_JURISTIC_HQ_ADDRESS_TEXT").ToThaiDigit();
                    addressProvince = request.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_PROVINCE_JURISTIC_HQ_ADDRESS_TEXT").ToThaiDigit();
                    addressPostCode = request.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_POSTCODE_JURISTIC_HQ_ADDRESS").ToThaiDigit();
                    addressTel = request.Data.TryGetData("JURISTIC_ADDRESS_INFORMATION").ThenGetStringData("ADDRESS_TELEPHONE_JURISTIC_HQ_ADDRESS").ToThaiDigit();
                    break;
            }

            var data = new
            {
                rsm__ExchangedDocumentContext = new
                {
                    ram__MessageStandardSpecifiedDocumentContextParameter = new
                    {
                        ram__ID = "2.16.764.1.4.100.4.2.1.1",
                        ram__SpecifiedDocumentVersion = new
                        {
                            ram__ID = "1.0",
                            ram__IssueDateTime = new
                            {
                                udt__DateTime = issueDate
                            }
                        }
                    }
                },
                rsm__ExchangedDocument = new
                {
                    ram__Name = "ใบอนุญาตประกอบธุรกิจขายตรง",
                    ram__TypeCode = licenseNo,
                    ram__IssueDateTime = new
                    {
                        udt__DateTime = issueDate
                    },
                    ram__Remark = "",
                    ram__SubmitterParty = new
                    {
                        ram__SpecifiedPerson = new
                        {
                            ram__ID = specifiedPersonId,
                            ram__Name = specifiedPersonName,
                            ram__IssueDateTime = new
                            {
                                udt__DateTime = issueDate
                            }
                        },
                        ram__PostalAddress = new
                        {
                            ram__BuildingNumber = addressNo,
                            ram__AdditionalStreetName = addressSoi,
                            ram__StreetName = addressRoad,
                            ram__CitySubDivisionName = addressSubDistrict,
                            ram__CityName = addressDistrict,
                            ram__CountrySubDivisionName = addressProvince,
                            ram__Postcode = addressPostCode,
                            ram__CountryCode = "TH"
                        },
                        ram__TelephoneCommunication = new
                        {
                            ram__LocalNumber = addressTel
                        }
                    },
                    ram__Issuer = new
                    {
                        ram__SpecifiedOrganization = new
                        {
                            ram__Name = "สำนักงานคณะกรรมการคุ้มครองผู้บริโภค",
                            ram__ParentText = "สังกัดสำนักนายกรัฐมนตรี"
                        }
                    },
                    ram__ApplicablePeriod = new
                    {
                        ram__EffectiveDateTime = new
                        {
                            udt__DateTime = effectiveDate
                        }
                    },
                    ram__SignatoryAuthentication = new
                    {
                        ram__ActualDateTime = new
                        {
                            udt__DateTime = actualDate
                        },
                        ram__ProviderParty = new
                        {
                            ram__Name = signer,
                            ram__Description = signerDescription
                        }
                    }
                }
            };
           

            
            var json = JObject.FromObject(data);            
            return JObject.FromObject(data);
        }
    }
}
