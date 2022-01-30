using BizPortal.DAL.MongoDB;
using BizPortal.Utils;
using BizPortal.Utils.Extensions;
using BizPortal.ViewModels;
using BizPortal.ViewModels.Apps.HSSStandard;
using BizPortal.ViewModels.SingleForm;
using BizPortal.ViewModels.V2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Data.Entity.Core.Metadata.Edm;
using Org.BouncyCastle.Crypto.Tls;

namespace BizPortal.AppsHook
{
    public class HSSHospitalPermissionNewAppHook : StoreBaseAppHook
    {
        public override decimal? CalculateFee(List<ISectionData> sectionData)
        {
            return 0;
        }

        public override bool IsEnabledChat()
        {
            return true;
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
            var hospitalPermission = request.Data.TryGetData("APP_HOSPITAL_PERMISSION_INFO_SECTION");
            var hospitalPermissionOwner = request.Data.TryGetData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION");
            var hospitalPermissionOwnerConfirm = request.Data.TryGetData("APP_HOSPITAL_PERMISSION_OWNER_CONFIRM_SECTION");


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
            #region hospitalPermission
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION = new APP_HOSPITAL_PERMISSION_INFO_SECTION();
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data = new APP_HOSPITAL_PERMISSION_INFO_SECTION_DATA();
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.GroupName = hospitalPermission.GroupName;
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Visible = hospitalPermission.Visible;
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_HEADER_TYPE = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_HEADER_TYPE");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_BED_AMOUNT = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_BED_AMOUNT");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.DROPDOWN_APP_HOSPITAL_PERMISSION_INFO_SECTION_HOSPITAL_DETAIL = hospitalPermission.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_INFO_SECTION_HOSPITAL_DETAIL");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.DROPDOWN_APP_HOSPITAL_PERMISSION_INFO_SECTION_HOSPITAL_CHOICE = hospitalPermission.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_INFO_SECTION_HOSPITAL_CHOICE");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_HOSPITAL_OTHER = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_HOSPITAL_OTHER");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_OBSTETRICS = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_OBSTETRICS");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_ORTHOPEDIC_DEPARTMENT = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_ORTHOPEDIC_DEPARTMENT");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_PHYSICAL_THERAPY = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_PHYSICAL_THERAPY");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_OTHER_TEXT = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_OTHER_TEXT");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_HEADER = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_HEADER");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_CONFIRM_TRUE = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_CONFIRM_TRUE");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_INTERNAL_MEDICINE = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_INTERNAL_MEDICINE");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_SURGERY = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_SURGERY");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_PEDIATRICS = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_PEDIATRICS");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_MEDICAL_TECHNOLOGY = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_MEDICAL_TECHNOLOGY");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_DERMATOLOGY_DEPARTMENT = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_DERMATOLOGY_DEPARTMENT");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_ARTIFICIAL_INSEMINATION = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_ARTIFICIAL_INSEMINATION");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_THAI_TRADITIONAL_MEDICINE = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_THAI_TRADITIONAL_MEDICINE");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_NUTRITION_DEPARTMENT = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_NUTRITION_DEPARTMENT");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_LAUNDRY_DEPARTMENT = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_LAUNDRY_DEPARTMENT");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_INTENSIVE_CARE_UNIT = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_INTENSIVE_CARE_UNIT");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_INTERNAL_EXAMINATION = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_INTERNAL_EXAMINATION");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_SMALL_OPERATING_ROOM = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_SMALL_OPERATING_ROOM");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_TREATMENT_ROOM = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_TREATMENT_ROOM");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_AFTER_BIRTH = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_AFTER_BIRTH");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_ORGAN_TRANSPLANT = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_ORGAN_TRANSPLANT");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_HEMODIALYSIS_ROOM = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_HEMODIALYSIS_ROOM");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_DENTAL_ROOM = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_DENTAL_ROOM");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_DIAGNOSTIC_RADIATION = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_DIAGNOSTIC_RADIATION");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_OPEN_HEART_SURGERY = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_OPEN_HEART_SURGERY");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_CARDIAC_CATHETERIZATION = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_CARDIAC_CATHETERIZATION");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_RADIATION_THERAPY = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_RADIATION_THERAPY");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_EXAMINATION_MAGNETIC_FIELD = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_EXAMINATION_MAGNETIC_FIELD");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_BREAKDOWN = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_BREAKDOWN");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_MORGUE = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_MORGUE");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_THAI_TRADITIONAL_MEDICINE_APPLIED = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_THAI_TRADITIONAL_MEDICINE_APPLIED");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_MASSAGE_DEPARTMENT = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_MASSAGE_DEPARTMENT");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_CHINESE_MEDICINE = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_CHINESE_MEDICINE");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_OTHER = hospitalPermission.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_ONLINE_SERVICE_OTHER");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.DROPDOWN_APP_HOSPITAL_PERMISSION_INFO_SECTION_HOSPITAL_DETAIL_TEXT = hospitalPermission.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_INFO_SECTION_HOSPITAL_DETAIL_TEXT");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_INFO_SECTION.Data.DROPDOWN_APP_HOSPITAL_PERMISSION_INFO_SECTION_HOSPITAL_CHOICE_TEXT = hospitalPermission.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_INFO_SECTION_HOSPITAL_CHOICE_TEXT");

            #endregion
            #region hospitalPermissionOwner
            exportRequest.Data.APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION = new APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION();
            exportRequest.Data.APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION.Data = new APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_DATA();
            exportRequest.Data.APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION.GroupName = hospitalPermissionOwner.GroupName;
            exportRequest.Data.APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION.Visible = hospitalPermissionOwner.Visible;
            //exportRequest.Data.APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION.Data. = hospitalPermissionOwner.ThenGetStringData("");
            int ownerTotal = hospitalPermissionOwner.ThenGetIntData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_TOTAL");
            exportRequest.Data.APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION.Data.APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_TOTAL = ownerTotal;
            if (ownerTotal > 0)
            {
                var ownerList = new List<OWNER>();
                for (int i = 0; i < ownerTotal; i++)
                {
                    var owner = new OWNER()
                    {
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_DETAIL_OPTION = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_DETAIL_OPTION_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_TITLE = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_TITLE_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_FIRSTNAME = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_FIRSTNAME_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_LASTNAME = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_LASTNAME_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_AGE = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_AGE_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_NATIONALITY = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_NATIONALITY_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_ID = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_ID_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_PASSPORT = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_PASSPORT_" + i),
                        ADDRESS_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_MOO_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_MOO_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_VILLAGE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_VILLAGE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_SOI_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_SOI_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_BUILDING_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_BUILDING_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_ROOMNO_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_ROOMNO_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_FLOOR_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_FLOOR_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_ROAD_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_ROAD_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_PROVINCE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_PROVINCE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_AMPHUR_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_AMPHUR_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_TUMBOL_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_TUMBOL_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_POSTCODE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_POSTCODE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_TELEPHONE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_TELEPHONE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_TELEPHONE_EXT_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_TELEPHONE_EXT_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_FAX_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_FAX_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        ADDRESS_EMAIL_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_EMAIL_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_TYPE = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_TYPE_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_BRANCH = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_BRANCH_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_LICENSE_NUMBER = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_LICENSE_NUMBER_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_LICENSING_DATE = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_LICENSING_DATE_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_DIPLOMA = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_DIPLOMA_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_DAY_TIME_WOKING = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_DAY_TIME_WOKING_" + i),


                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OPARETOR_STATUS = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OPARETOR_STATUS_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_EMPLOYEE_STATUS = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_EMPLOYEE_STATUS_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_WOKING_PLACE_NAME = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_WOKING_PLACE_NAME_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_WOKING_LICENSE_NUMBER = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_WOKING_LICENSE_NUMBER_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_HOSPITAL_TYPE_OPTION = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_HOSPITAL_TYPE_OPTION_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_CLINIC_DETAIL = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_CLINIC_DETAIL_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_CLINIC_TYPE = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_CLINIC_TYPE_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_CLINIC_OTHER = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_CLINIC_OTHER_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_HOSPITAL_DETAIL = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_HOSPITAL_DETAIL_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_HOSPITAL_CHOICE = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_HOSPITAL_CHOICE_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_HOSPITAL_OTHER = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_HOSPITAL_OTHER_" + i),
                        ADDRESS_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_MOO_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_MOO_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_VILLAGE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_VILLAGE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_SOI_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_SOI_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_BUILDING_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_BUILDING_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_ROOMNO_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_ROOMNO_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_FLOOR_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_FLOOR_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_ROAD_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_ROAD_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_PROVINCE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_PROVINCE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_AMPHUR_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_AMPHUR_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_TUMBOL_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_TUMBOL_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_POSTCODE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_POSTCODE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_TELEPHONE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_TELEPHONE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_TELEPHONE_EXT_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_TELEPHONE_EXT_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        ADDRESS_FAX_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS = hospitalPermissionOwner.ThenGetStringData("ADDRESS_FAX_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_QUIT_WOKING_DATE = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_QUIT_WOKING_DATE_" + i),
                        ARR_IDX = hospitalPermissionOwner.ThenGetStringData("ARR_IDX_" + i),
                        IS_EDIT = hospitalPermissionOwner.ThenGetStringData("IS_EDIT_" + i),
                        CUSREQ = hospitalPermissionOwner.ThenGetStringData("CUSREQ_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_DETAIL_OPTION__RADIO_TEXT = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_DETAIL_OPTION__RADIO_TEXT_" + i),
                        APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_HOSPITAL_TYPE_OPTION__RADIO_TEXT = hospitalPermissionOwner.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_HOSPITAL_TYPE_OPTION__RADIO_TEXT_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_TITLE_TEXT = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_TITLE_TEXT_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_NATIONALITY_TEXT = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_NATIONALITY_TEXT_" + i),
                        ADDRESS_PROVINCE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_TEXT = hospitalPermissionOwner.ThenGetStringData("ADDRESS_PROVINCE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_TEXT_" + i),
                        ADDRESS_AMPHUR_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_TEXT = hospitalPermissionOwner.ThenGetStringData("ADDRESS_AMPHUR_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_TEXT_" + i),
                        ADDRESS_TUMBOL_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_TEXT = hospitalPermissionOwner.ThenGetStringData("ADDRESS_TUMBOL_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OWNER_ADDRESS_TEXT_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_TYPE_TEXT = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_TYPE_TEXT_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_BRANCH_TEXT = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_BRANCH_TEXT_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OPARETOR_STATUS_TEXT = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_OPARETOR_STATUS_TEXT_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_EMPLOYEE_STATUS_TEXT = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_EMPLOYEE_STATUS_TEXT_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_CLINIC_DETAIL_TEXT = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_CLINIC_DETAIL_TEXT_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_CLINIC_TYPE_TEXT = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_CLINIC_TYPE_TEXT_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_HOSPITAL_DETAIL_TEXT = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_HOSPITAL_DETAIL_TEXT_" + i),
                        DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_HOSPITAL_CHOICE_TEXT = hospitalPermissionOwner.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_HOSPITAL_CHOICE_TEXT_" + i),
                        ADDRESS_PROVINCE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_TEXT = hospitalPermissionOwner.ThenGetStringData("ADDRESS_PROVINCE_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_TEXT_" + i),
                        ADDRESS_AMPHUR_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_TEXT = hospitalPermissionOwner.ThenGetStringData("ADDRESS_AMPHUR_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_TEXT_" + i),
                        ADDRESS_TUMBOL_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_TEXT = hospitalPermissionOwner.ThenGetStringData("ADDRESS_TUMBOL_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_MEDICAL_CENTER_ADDRESS_TEXT_" + i),
                        ARR_ITEM_ID = hospitalPermissionOwner.ThenGetStringData("ARR_ITEM_ID_" + i),

                    };
                    ownerList.Add(owner);
                }
                exportRequest.Data.APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION.Data.Owners = ownerList;
            }

            #endregion
            exportRequest.Data.APP_HOSPITAL_PERMISSION_OWNER_CONFIRM_SECTION = new APP_HOSPITAL_PERMISSION_OWNER_CONFIRM_SECTION();
            exportRequest.Data.APP_HOSPITAL_PERMISSION_OWNER_CONFIRM_SECTION.Data = new APP_HOSPITAL_PERMISSION_OWNER_CONFIRM_SECTION_DATA();
            exportRequest.Data.APP_HOSPITAL_PERMISSION_OWNER_CONFIRM_SECTION.GroupName = hospitalPermissionOwnerConfirm.GroupName;
            exportRequest.Data.APP_HOSPITAL_PERMISSION_OWNER_CONFIRM_SECTION.Visible = hospitalPermissionOwnerConfirm.Visible;
            exportRequest.Data.APP_HOSPITAL_PERMISSION_OWNER_CONFIRM_SECTION.Data.APP_HOSPITAL_PERMISSION_OWNER_CONFIRM_SECTION_CONFIRM_TRUE = hospitalPermissionOwnerConfirm.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_CONFIRM_SECTION_CONFIRM_TRUE");



            return JsonConvert.SerializeObject(exportRequest, Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
        }


        public override JObject GenerateELicenseData(Guid applicationrequestId)
        {
            var request = ApplicationRequestEntity.Get(applicationrequestId);

            var addressLine = new JArray();
            addressLine.Add(new
            {
                id = request.Data.TryGetData("INFORMATION_STORE").Data.ThenGetStringData("ADDRESS_INFORMATION_STORE__ADDRESS")
            });

            addressLine.Add(new
            {
                id = request.Data.TryGetData("INFORMATION_STORE").Data.ThenGetStringData("ADDRESS_MOO_INFORMATION_STORE__ADDRESS")
            });

            addressLine.Add(new
            {
                id = request.Data.TryGetData("INFORMATION_STORE").Data.ThenGetStringData("ADDRESS_SOI_INFORMATION_STORE__ADDRESS")
            });
            addressLine.Add(new
            {
                id = request.Data.TryGetData("INFORMATION_STORE").Data.ThenGetStringData("ADDRESS_ROAD_INFORMATION_STORE__ADDRESS")
            });

            var extensionArr = new JArray();
            extensionArr.Add(new
            {
                valueQuantity = new // จำนวนเตียง
                {
                    id = request.Data.TryGetData("APP_HOSPITAL_PERMISSION_INFO_SECTION").Data.ThenGetStringData("APP_HOSPITAL_PERMISSION_INFO_SECTION_BED_AMOUNT")
                }
            });
            extensionArr.Add(new
            {
                valueString = new
                {
                    value = "ประเภทรับไว้ค้างคืน"
                }
            });
            extensionArr.Add(new
            {
                valueString = new //ลักษณะ
                {
                    value = request.Data.TryGetData("APP_HOSPITAL_PERMISSION_INFO_SECTION").Data.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_INFO_SECTION_HOSPITAL_DETAIL_TEXT")
                }
            });
            var telecomContact = new JArray();
            telecomContact.Add(new
            {
                system = JObject.FromObject(new
                {
                    value = "phone"
                }),
                value = JObject.FromObject(new
                {
                    value = string.Empty
                }),
                use = JObject.FromObject(new
                {
                    value = "work"
                })
            });
            telecomContact.Add(new
            {
                system = JObject.FromObject(new
                {
                    value = "fax"
                }),
                value = JObject.FromObject(new
                {
                    value = string.Empty
                }),
                use = JObject.FromObject(new
                {
                    value = "work"
                })
            });
            telecomContact.Add(new
            {
                system = JObject.FromObject(new
                {
                    value = "email"
                }),
                value = JObject.FromObject(new
                {
                    value = string.Empty
                }),
                use = JObject.FromObject(new
                {
                    value = "work"
                })
            });


            return JObject.FromObject(new
            {
                //ใบขออนุญาตให้ประกอบกิจการสถานพยาบาล (ประเภทที่รับผู้ป่วยไว้ค้างคืน)
                HospitalSP7 = JObject.FromObject(new
                {
                    DocumentReference = JObject.FromObject(new
                    {
                        identifier = string.Empty,
                        subject = JObject.FromObject(new
                        {
                            display = JObject.FromObject(new
                            {
                                value = request.IdentityName,
                            }),
                        }),
                        date = string.Empty,
                        author = JObject.FromObject(new
                        {
                            practitioner = JObject.FromObject(new
                            {
                                qualification = JObject.FromObject(new
                                {
                                    identifier = JObject.FromObject(new
                                    {
                                        id = string.Empty,
                                        value = JObject.FromObject(new
                                        {
                                            value = string.Empty,
                                        }),
                                    }),
                                    peroid = JObject.FromObject(new
                                    {
                                        start = JObject.FromObject(new
                                        {
                                            value = string.Empty,
                                        })
                                    }),
                                }),
                            }),
                            organization = JObject.FromObject(new
                            {
                                type = JObject.FromObject(new
                                {
                                    text = JArray.FromObject(new
                                    {
                                        extension = extensionArr
                                    }),
                                }),
                                name = JObject.FromObject(new
                                {
                                    value = request.Data.TryGetData("INFORMATION_STORE").Data.ThenGetStringData("INFORMATION_STORE_NAME_TH")
                                }),
                                address = JObject.FromObject(new
                                {
                                    text = string.Empty,
                                    line = addressLine,
                                    city = JObject.FromObject(new
                                    {
                                        value = request.Data.TryGetData("INFORMATION_STORE").Data.ThenGetStringData("ADDRESS_TUMBOL_INFORMATION_STORE__ADDRESS")
                                    }),
                                    district = JObject.FromObject(new
                                    {
                                        value = request.Data.TryGetData("INFORMATION_STORE").Data.ThenGetStringData("ADDRESS_AMPHUR_INFORMATION_STORE__ADDRESS")
                                    }),
                                    state = JObject.FromObject(new
                                    {
                                        value = request.Data.TryGetData("INFORMATION_STORE").Data.ThenGetStringData("ADDRESS_PROVINCE_INFORMATION_STORE__ADDRESS")
                                    }),
                                    postalCode = JObject.FromObject(new
                                    {
                                        value = request.Data.TryGetData("INFORMATION_STORE").Data.ThenGetStringData("ADDRESS_POSTCODE_INFORMATION_STORE__ADDRESS")
                                    }),
                                    country = JObject.FromObject(new
                                    {
                                        value = "TH"
                                    }),

                                }),
                            }),
                            healthcareService = JObject.FromObject(new
                            {
                                display = JObject.FromObject(new
                                {
                                    value = string.Empty,
                                })
                            }),
                            telecom = telecomContact,
                            availableTime = JObject.FromObject(new
                            {
                                allDay = JObject.FromObject(new
                                {
                                    value = string.Empty
                                }),
                                availableStartTime = JObject.FromObject(new
                                {
                                    value = string.Empty
                                }),
                                availableEndTime = JObject.FromObject(new
                                {
                                    value = string.Empty
                                })
                            })
                        }),
                        relatesTo = JObject.FromObject(new
                        {
                            code = string.Empty,
                            target = JObject.FromObject(new
                            {
                                display = JObject.FromObject(new
                                {
                                    value = string.Empty
                                })
                            })
                        }),
                        context = JObject.FromObject(new
                        {
                            period = JObject.FromObject(new
                            {
                                start = JObject.FromObject(new
                                {
                                    value = string.Empty
                                }),
                                end = JObject.FromObject(new
                                {
                                    value = string.Empty
                                })
                            })
                        })
                    }),
                }),

                //ใบขอรับใบอนุญาตให้ดำเนินการสถานพยาบาล (ประเภทที่รับผู้ป่วยไว้ค้างคืน)
                HospitalSP19 = JObject.FromObject(new
                {
                    DocumentReference = JObject.FromObject(new
                    {
                        identifier = JObject.FromObject(new
                        {
                            id = string.Empty
                        }),
                        subject = JObject.FromObject(new
                        {
                            display = JObject.FromObject(new
                            {
                                value = request.Data.TryGetData("GENERAL_INFORMATION").Data.ThenGetStringData("CITIZEN_NAME") + request.Data.TryGetData("GENERAL_INFORMATION").Data.ThenGetStringData("CITIZEN_LASTNAME")
                            })
                        }),
                        date = JObject.FromObject(new
                        {
                            value = string.Empty
                        }),
                        author = JObject.FromObject(new
                        {
                            practitioner = JObject.FromObject(new
                            {
                                photo = JObject.FromObject(new
                                {
                                    contentType = JObject.FromObject(new
                                    {
                                        value = string.Empty
                                    }),
                                    size = JObject.FromObject(new
                                    {
                                        value = string.Empty
                                    }),
                                    title = JObject.FromObject(new
                                    {
                                        value = string.Empty
                                    })
                                }),
                                qualification = JObject.FromObject(new
                                {
                                    identifier = JObject.FromObject(new
                                    {
                                        id = request.Data.TryGetData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION").Data.ThenGetStringData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_LICENSE_NUMBER_0"),
                                        value = JObject.FromObject(new
                                        {
                                            value = request.Data.TryGetData("APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION").Data.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PERMISSION_OWNER_OPARETOR_SECTION_HOSPITAL_DETAIL_TEXT_0")
                                        })
                                    }),
                                    period = JObject.FromObject(new
                                    {
                                        start = JObject.FromObject(new
                                        {
                                            value = string.Empty
                                        })
                                    }),
                                })
                            }),
                            organization = JObject.FromObject(new
                            {
                                type = JObject.FromObject(new
                                {
                                    text = JArray.FromObject(new
                                    {
                                        extension = extensionArr
                                    })
                                }),
                                name = JObject.FromObject(new
                                {
                                    value = request.Data.TryGetData("INFORMATION_STORE").Data.ThenGetStringData("INFORMATION_STORE_NAME_TH")
                                }),
                                address = JObject.FromObject(new
                                {
                                    text = string.Empty,
                                    line = addressLine,
                                    city = JObject.FromObject(new
                                    {
                                        value = request.Data.TryGetData("INFORMATION_STORE").Data.ThenGetStringData("ADDRESS_TUMBOL_INFORMATION_STORE__ADDRESS")
                                    }),
                                    district = JObject.FromObject(new
                                    {
                                        value = request.Data.TryGetData("INFORMATION_STORE").Data.ThenGetStringData("ADDRESS_AMPHUR_INFORMATION_STORE__ADDRESS")
                                    }),
                                    state = JObject.FromObject(new
                                    {
                                        value = request.Data.TryGetData("INFORMATION_STORE").Data.ThenGetStringData("ADDRESS_PROVINCE_INFORMATION_STORE__ADDRESS")
                                    }),
                                    postalCode = JObject.FromObject(new
                                    {
                                        value = request.Data.TryGetData("INFORMATION_STORE").Data.ThenGetStringData("ADDRESS_POSTCODE_INFORMATION_STORE__ADDRESS")
                                    }),
                                    country = JObject.FromObject(new
                                    {
                                        value = "TH"
                                    }),

                                }),

                            }),
                            healthcareService = JObject.FromObject(new
                            {
                                display = JObject.FromObject(new
                                {
                                    value = string.Empty
                                })
                            }),
                            telecom = telecomContact,
                            availableTime = JObject.FromObject(new {
                                allDay = JObject.FromObject(new {
                                    value = string.Empty, // true or false
                                }),
                                availableStartTime = JObject.FromObject(new {
                                    value = string.Empty
                                }),
                                availableEndTime = JObject.FromObject(new {
                                    value = string.Empty
                                })
                            })
                        }),
                        relatesTo = JObject.FromObject(new {
                            code = string.Empty,
                            target = JObject.FromObject(new {
                                display = string.Empty
                            })
                        }),
                        context = JObject.FromObject(new {
                            period = JObject.FromObject(new {
                                end = JObject.FromObject(new {
                                    value = string.Empty
                                })
                            })
                        })
                    }),
                })
            });
        }

    }
}

