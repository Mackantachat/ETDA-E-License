using BizPortal.DAL.MongoDB;
using BizPortal.Utils.Extensions;
using BizPortal.ViewModels;
using BizPortal.ViewModels.SingleForm;
using BizPortal.ViewModels.V2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using BizPortal.ViewModels.Apps.HSSStandard;
using BizPortal.Utils;
using Newtonsoft.Json.Linq;
using System.Data.Entity.Core.Metadata.Edm;
using BizPortal.ViewModels.Apps;
using Mapster;
using System.Linq.Expressions;

namespace BizPortal.AppsHook
{
    public class HSSHospitalNewAppHook : StoreBaseAppHook
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
            var hospitalPlanInfo = request.Data.TryGetData("APP_HOSPITAL_PLAN_INFO_SECTION");
            var hospitalPlanBussinessInvest = request.Data.TryGetData("APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION");
            var hospitalPlanBussinessService = request.Data.TryGetData("APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION");
            var hospitalPlanBussinessServiceType = request.Data.TryGetData("APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION");
            var hospitalPlanBussinessServiceProblem = request.Data.TryGetData("APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION");
            var hospitalPlanPersonnel = request.Data.TryGetData("APP_HOSPITAL_PLAN_PERSONNEL_SECTION");
            var hospitalPlanPersonnelDoctor = request.Data.TryGetData("APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION");
            var hospitalPlanConfirm = request.Data.TryGetData("APP_HOSPITAL_PLAN_CONFIRM_SIGNATURE");
            var hospitalLicense = request.Data.TryGetData("APP_HOSPITAL_LICENSE_INFO_SECTION");



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


            #region hospitalPlanInfo
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION = new APP_HOSPITAL_PLAN_INFO_SECTION();
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data = new APP_HOSPITAL_PLAN_INFO_SECTION_DATA();
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.GroupName = hospitalPlanInfo.GroupName;
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Visible = hospitalPlanInfo.Visible;
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_1 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_1");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_2 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_2");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_4 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_4");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_7 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_7");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_10 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_10");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_11 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_11");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_12 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_12");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_13 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_13");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_14 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_14");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_15 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_15");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_OTHER = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_OTHER");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_BUILD_TYPE_OPTION = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_BUILD_TYPE_OPTION");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_BUILD_TYPE_OTHER = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_BUILD_TYPE_OTHER");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_INVESTMENT = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_INVESTMENT");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_3 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_3");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_5 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_5");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_6 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_6");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_8 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_8");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_9 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_9");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_16 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_16");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_17 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_17");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_18 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_18");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_19 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_19");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_20 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_20");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_21 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_21");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_22 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_22");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_23 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_23");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_24 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_24");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_25 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_25");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_26 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_26");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_27 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_27");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_28 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_28");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_29 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_29");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_30 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_30");
            exportRequest.Data.APP_HOSPITAL_PLAN_INFO_SECTION.Data.APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_31 = hospitalPlanInfo.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_31");

            #endregion
            #region hospitalPlanBussinessInvest
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION = new APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION();
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION.Data = new APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION_DATA();
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION.GroupName = hospitalPlanBussinessInvest.GroupName;
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION.Visible = hospitalPlanBussinessInvest.Visible;

            int bussinessInvestTotal = hospitalPlanBussinessInvest.ThenGetIntData("APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION_TOTAL");
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION.Data.APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION_TOTAL = bussinessInvestTotal;
            if (bussinessInvestTotal > 0)
            {
                var bussinessInvestList = new List<BUSSINESS_INVEST>();
                for (int i = 0; i < bussinessInvestTotal; i++)
                {
                    var bussinessInvest = new BUSSINESS_INVEST()
                    {
                        DROPDOWN_APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION_TYPE = hospitalPlanBussinessInvest.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION_TYPE_" + i),
                        APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION_PERCENT = hospitalPlanBussinessInvest.ThenGetStringData("APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION_PERCENT_" + i),
                        ARR_IDX = hospitalPlanBussinessInvest.ThenGetStringData("ARR_IDX_" + i),
                        IS_EDIT = hospitalPlanBussinessInvest.ThenGetStringData("IS_EDIT_" + i),
                        CUSREQ = hospitalPlanBussinessInvest.ThenGetStringData("CUSREQ_" + i),
                        DROPDOWN_APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION_TYPE_TEXT = hospitalPlanBussinessInvest.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION_TYPE_TEXT_" + i),
                        ARR_ITEM_ID = hospitalPlanBussinessInvest.ThenGetStringData("ARR_ITEM_ID_" + i),
                    };
                    bussinessInvestList.Add(bussinessInvest);
                }
                exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_INVEST_SECTION.Data.BussinessInvests = bussinessInvestList;
            }


            #endregion
            #region hospitalPlanBussinessService
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION = new APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION();
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION.Data = new APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION_DATA();
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION.GroupName = hospitalPlanBussinessService.GroupName;
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION.Visible = hospitalPlanBussinessService.Visible;
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION_LOCATION = hospitalPlanBussinessService.ThenGetStringData("APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION_LOCATION");
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION_PEOPLE_AMOUNT = hospitalPlanBussinessService.ThenGetStringData("APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION_PEOPLE_AMOUNT");
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION_NURSE_AMOUNT = hospitalPlanBussinessService.ThenGetStringData("APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_SECTION_NURSE_AMOUNT");


            #endregion
            #region hospitalPlanBussinessServiceType
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION = new APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION();
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION.Data = new APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION_DATA();
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION.GroupName = hospitalPlanBussinessServiceType.GroupName;
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION.Visible = hospitalPlanBussinessServiceType.Visible;
            // exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION_TOTAL = hospitalPlanBussinessServiceType.ThenGetStringData("APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION_TOTAL");
            int bussinessServiceTotal = hospitalPlanBussinessServiceType.ThenGetIntData("APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION_TOTAL");
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION_TOTAL = bussinessServiceTotal;
            if (bussinessServiceTotal > 0)
            {
                var bussinessServiceList = new List<BUSSINESS_SERVICE>();
                for (int i = 0; i < bussinessServiceTotal; i++)
                {
                    var bussinessService = new BUSSINESS_SERVICE()
                    {
                        DROPDOWN_APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION_CHOICE = hospitalPlanBussinessServiceType.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION_CHOICE_" + i),
                        APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION_AMOUNT = hospitalPlanBussinessServiceType.ThenGetStringData("APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION_AMOUNT_" + i),
                        ARR_IDX = hospitalPlanBussinessServiceType.ThenGetStringData("ARR_IDX_" + i),
                        IS_EDIT = hospitalPlanBussinessServiceType.ThenGetStringData("IS_EDIT_" + i),
                        CUSREQ = hospitalPlanBussinessServiceType.ThenGetStringData("CUSREQ_" + i),
                        DROPDOWN_APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION_CHOICE_TEXT = hospitalPlanBussinessServiceType.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION_CHOICE_TEXT_" + i),
                        ARR_ITEM_ID = hospitalPlanBussinessServiceType.ThenGetStringData("ARR_ITEM_ID_" + i),
                    };
                    bussinessServiceList.Add(bussinessService);
                }
                exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_TYPE_SECTION.Data.BussinessServices = bussinessServiceList;
            }
            #endregion
            #region hospitalPlanBussinessServiceProblem
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION = new APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION();
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION.Data = new APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION_DATA();
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION.GroupName = hospitalPlanBussinessServiceProblem.GroupName;
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION.Visible = hospitalPlanBussinessServiceProblem.Visible;
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION_REASON = hospitalPlanBussinessServiceProblem.ThenGetStringData("APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION_REASON");
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION_DURATION_YEAR = hospitalPlanBussinessServiceProblem.ThenGetStringData("APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION_DURATION_YEAR");
            exportRequest.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION.Data.APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION_DURATION_MONTH = hospitalPlanBussinessServiceProblem.ThenGetStringData("APP_HOSPITAL_PLAN_BUSSINESS_SERVICE_PROBLEM_SECTION_DURATION_MONTH");


            #endregion
            #region hospitalPlanPersonnel
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION = new APP_HOSPITAL_PLAN_PERSONNEL_SECTION();
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION.Data = new APP_HOSPITAL_PLAN_PERSONNEL_SECTION_DATA();
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION.GroupName = hospitalPlanPersonnel.GroupName;
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION.Visible = hospitalPlanPersonnel.Visible;

            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION_DOCTOR_AMOUNT = hospitalPlanPersonnel.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_SECTION_DOCTOR_AMOUNT");
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION_NURSE_AMOUNT = hospitalPlanPersonnel.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_SECTION_NURSE_AMOUNT");
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION_DENTIST_AMOUNT = hospitalPlanPersonnel.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_SECTION_DENTIST_AMOUNT");
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION_PHARMACIST_AMOUNT = hospitalPlanPersonnel.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_SECTION_PHARMACIST_AMOUNT");
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION_THERAPIST_AMOUNT = hospitalPlanPersonnel.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_SECTION_THERAPIST_AMOUNT");
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION_TECHNICAL_AMONT = hospitalPlanPersonnel.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_SECTION_TECHNICAL_AMONT");
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION_THAI_TRADITIONAL = hospitalPlanPersonnel.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_SECTION_THAI_TRADITIONAL");
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION_THAI_TRADITIONAL_APPLIED = hospitalPlanPersonnel.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_SECTION_THAI_TRADITIONAL_APPLIED");
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION_THAI_MEDICINE = hospitalPlanPersonnel.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_SECTION_THAI_MEDICINE");
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION_THAI_PHARMACY = hospitalPlanPersonnel.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_SECTION_THAI_PHARMACY");
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION.Data.APP_HOSPITAL_PLAN_PERSONNEL_SECTION_OTHER = hospitalPlanPersonnel.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_SECTION_OTHER");



            #endregion
            #region hospitalPlanPersonnelDoctor
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION = new APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION();
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION.Data = new APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_DATA();
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION.GroupName = hospitalPlanPersonnelDoctor.GroupName;
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION.Visible = hospitalPlanPersonnelDoctor.Visible;
            //exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION.Data.APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_TOTAL = hospitalPlanPersonnelDoctor.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_TOTAL");
            int PersonnelDoctorTotal = hospitalPlanPersonnelDoctor.ThenGetIntData("APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_TOTAL");
            exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION.Data.APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_TOTAL = PersonnelDoctorTotal;
            if (PersonnelDoctorTotal > 0)
            {
                var PersonnelDoctorList = new List<DOCTOR>();
                for (int i = 0; i < PersonnelDoctorTotal; i++)
                {
                    var PersonnelDoctor = new DOCTOR()
                    {
                        DROPDOWN_APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_POSITION = hospitalPlanPersonnelDoctor.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_POSITION_" + i),
                        APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_OTHER = hospitalPlanPersonnelDoctor.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_OTHER_" + i),
                        DROPDOWN_APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_TITLE = hospitalPlanPersonnelDoctor.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_TITLE_" + i),
                        APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_FIRSTNAME = hospitalPlanPersonnelDoctor.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_FIRSTNAME_" + i),
                        APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_LASTNAME = hospitalPlanPersonnelDoctor.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_LASTNAME_" + i),
                        APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_ID_CARD = hospitalPlanPersonnelDoctor.ThenGetStringData("APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_ID_CARD_" + i),

                        ARR_IDX = hospitalPlanPersonnelDoctor.ThenGetStringData("ARR_IDX_" + i),
                        IS_EDIT = hospitalPlanPersonnelDoctor.ThenGetStringData("IS_EDIT_" + i),
                        CUSREQ = hospitalPlanPersonnelDoctor.ThenGetStringData("CUSREQ_" + i),
                        DROPDOWN_APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_POSITION_TEXT = hospitalPlanPersonnelDoctor.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_POSITION_TEXT_" + i),
                        DROPDOWN_APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_TITLE_TEXT = hospitalPlanPersonnelDoctor.ThenGetStringData("DROPDOWN_APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION_TITLE_TEXT_" + i),

                        ARR_ITEM_ID = hospitalPlanPersonnelDoctor.ThenGetStringData("ARR_ITEM_ID_" + i),
                    };
                    PersonnelDoctorList.Add(PersonnelDoctor);
                }
                exportRequest.Data.APP_HOSPITAL_PLAN_PERSONNEL_DOCTOR_SECTION.Data.Dortors = PersonnelDoctorList;
            }


            #endregion
            #region hospitalPlanConfirm
            exportRequest.Data.APP_HOSPITAL_PLAN_CONFIRM_SIGNATURE = new APP_HOSPITAL_PLAN_CONFIRM_SIGNATURE();
            exportRequest.Data.APP_HOSPITAL_PLAN_CONFIRM_SIGNATURE.Data = new APP_HOSPITAL_PLAN_CONFIRM_SIGNATURE_DATA();
            exportRequest.Data.APP_HOSPITAL_PLAN_CONFIRM_SIGNATURE.GroupName = hospitalPlanConfirm.GroupName;
            exportRequest.Data.APP_HOSPITAL_PLAN_CONFIRM_SIGNATURE.Visible = hospitalPlanConfirm.Visible;

            #endregion
            #region hospitalLicense
            exportRequest.Data.APP_HOSPITAL_LICENSE_INFO_SECTION = new APP_HOSPITAL_LICENSE_INFO_SECTION();
            exportRequest.Data.APP_HOSPITAL_LICENSE_INFO_SECTION.Data = new APP_HOSPITAL_LICENSE_INFO_SECTION_DATA();
            exportRequest.Data.APP_HOSPITAL_LICENSE_INFO_SECTION.GroupName = hospitalLicense.GroupName;
            exportRequest.Data.APP_HOSPITAL_LICENSE_INFO_SECTION.Visible = hospitalLicense.Visible;

            exportRequest.Data.APP_HOSPITAL_LICENSE_INFO_SECTION.Data.APP_HOSPITAL_LICENSE_INFO_SECTION_BED_AMOUNT = hospitalLicense.ThenGetStringData("APP_HOSPITAL_LICENSE_INFO_SECTION_BED_AMOUNT");
            exportRequest.Data.APP_HOSPITAL_LICENSE_INFO_SECTION.Data.DROPDOWN_APP_HOSPITAL_LICENSE_INFO_SECTION_HOSPITAL_TYPE = hospitalLicense.ThenGetStringData("DROPDOWN_APP_HOSPITAL_LICENSE_INFO_SECTION_HOSPITAL_TYPE");
            exportRequest.Data.APP_HOSPITAL_LICENSE_INFO_SECTION.Data.DROPDOWN_APP_HOSPITAL_LICENSE_INFO_SECTION_HOSPITAL_CHOICE = hospitalLicense.ThenGetStringData("DROPDOWN_APP_HOSPITAL_LICENSE_INFO_SECTION_HOSPITAL_CHOICE");
            exportRequest.Data.APP_HOSPITAL_LICENSE_INFO_SECTION.Data.APP_HOSPITAL_LICENSE_INFO_SECTION_HOSPITAL_TEXT = hospitalLicense.ThenGetStringData("APP_HOSPITAL_LICENSE_INFO_SECTION_HOSPITAL_TEXT");
            exportRequest.Data.APP_HOSPITAL_LICENSE_INFO_SECTION.Data.APP_HOSPITAL_LICENSE_INFO_SECTION_HOSPITAL_CONFIRM_APP_HOSPITAL_CONFIRM_TRUE = hospitalLicense.ThenGetStringData("APP_HOSPITAL_LICENSE_INFO_SECTION_HOSPITAL_CONFIRM_APP_HOSPITAL_CONFIRM_TRUE");
            exportRequest.Data.APP_HOSPITAL_LICENSE_INFO_SECTION.Data.DROPDOWN_APP_HOSPITAL_LICENSE_INFO_SECTION_HOSPITAL_TYPE_TEXT = hospitalLicense.ThenGetStringData("DROPDOWN_APP_HOSPITAL_LICENSE_INFO_SECTION_HOSPITAL_TYPE_TEXT");
            exportRequest.Data.APP_HOSPITAL_LICENSE_INFO_SECTION.Data.DROPDOWN_APP_HOSPITAL_LICENSE_INFO_SECTION_HOSPITAL_CHOICE_TEXT = hospitalLicense.ThenGetStringData("DROPDOWN_APP_HOSPITAL_LICENSE_INFO_SECTION_HOSPITAL_CHOICE_TEXT");
            #endregion
            return JsonConvert.SerializeObject(exportRequest, Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
        }
        //ขออนุมัติแผนงานการจัดตั้งสถานพยาบาล(โรงพยาบาล)
        public override JObject GenerateELicenseData(Guid applicationrequestId)
        {
            var request = ApplicationRequestEntity.Get(applicationrequestId);

            var extensionVersion = new JArray();
            extensionVersion.Add(new
            {
                valueOid = JObject.FromObject(new
                {
                    id = "2.16.764.1.4.100.9.2.1.1"
                }),
                url = "https://oid.estandard.or.th"
            });
            extensionVersion.Add(new
            {
                valueString = JObject.FromObject(new
                {
                    value = "1.0.0"
                }),
                url = "VersionNumber"
            });


            var TypeHospital = new JArray();
            TypeHospital.Add(new
            {
                valueString = JObject.FromObject(new
                {
                    value = string.Empty   //สถานพยาบาลมีลักษณะเป็น
                })
            });
            TypeHospital.Add(new
            {

                valueString = JObject.FromObject(new
                {
                    value = string.Empty //ขนาดเล็ก, ขนาดกลาง, ขนาดใหญ่
                })
            });

            var TypeBuilding = new JArray();
            TypeBuilding.Add(new
            {
                valueString = JObject.FromObject(new
                {
                    value = string.Empty //"เป็นอาคารสถานพยาบาลสร้างใหม่"
                })
            });
            TypeBuilding.Add(new
            {
                valueString = JObject.FromObject(new
                {
                    value = string.Empty //อธิบายเพิ่มเติมกรณีเลือกอื่นๆ
                })
            });

            var SourceOfFunds = new JArray();
            SourceOfFunds.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "5.1.1"
                }),
                text = JArray.FromObject(new
                {
                    value = "ส่วนตัว"
                }),
                answer = JObject.FromObject(new
                {
                    valueDecimal = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            SourceOfFunds.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "5.1.2"
                }),
                text = JArray.FromObject(new
                {
                    value = "สถาบันการเงินภายในประเทศ"
                }),
                answer = JObject.FromObject(new
                {
                    valueDecimal = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            SourceOfFunds.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "5.1.3"
                }),
                text = JArray.FromObject(new
                {
                    value = "สถาบันการเงินต่างประเทศ"
                }),
                answer = JObject.FromObject(new
                {
                    valueDecimal = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            SourceOfFunds.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "5.1.4"
                }),
                text = JArray.FromObject(new
                {
                    value = "หุ้น"
                }),
                answer = JObject.FromObject(new
                {
                    valueDecimal = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });

            var ServicesArea = new JArray();
            ServicesArea.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "6.1"
                }),
                text = JArray.FromObject(new
                {
                    value = "ในเขตท้องที่การปกครองของกระทรวงมหาดไทย (อำเภอ/เขต จังหวัด)"
                }),
                answer = JObject.FromObject(new
                {
                    valueString = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            ServicesArea.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "6.2"
                }),
                text = JArray.FromObject(new
                {
                    value = "จำนวนประชากรภายในเขตรัศมี 5 กิโลเมตร โดยรอบสถานพยาบาล"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });

            var PublicHospital = new JArray();
            PublicHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.1.1"
                }),
                text = JArray.FromObject(new
                {
                    value = "จำนวน"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            PublicHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.1.2"
                }),
                text = JArray.FromObject(new
                {
                    value = "ผู้ป่วยใน"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            PublicHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.1.3"
                }),
                text = JArray.FromObject(new
                {
                    value = "ห้องผ่าตัด"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            PublicHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.1.4"
                }),
                text = JArray.FromObject(new
                {
                    value = "เครื่องเอ็กซ์เรย์คอมพิวเตอร์"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            PublicHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.1.5"
                }),
                text = JArray.FromObject(new
                {
                    value = "เครื่องตรวจอวัยวะภายในชนิดสนามแม่เหล็กไฟฟ้า"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            PublicHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.1.6"
                }),
                text = JArray.FromObject(new
                {
                    value = "เครื่องสลายนิ่ว"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            PublicHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.1.7"
                }),
                text = JArray.FromObject(new
                {
                    value = "เครื่องล้างไต"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            PublicHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.1.8"
                }),
                text = JArray.FromObject(new
                {
                    value = "อื่น ๆ"
                }),
                answer = JObject.FromObject(new
                {
                    valueString = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });

            var PrivateHospital = new JArray();
            PrivateHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.2.1"
                }),
                text = JArray.FromObject(new
                {
                    value = "จำนวน"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            PrivateHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.2.2"
                }),
                text = JArray.FromObject(new
                {
                    value = "ผู้ป่วยใน"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            PrivateHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.2.3"
                }),
                text = JArray.FromObject(new
                {
                    value = "ห้องผ่าตัด"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            PrivateHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.2.4"
                }),
                text = JArray.FromObject(new
                {
                    value = "เครื่องเอ็กซ์เรย์คอมพิวเตอร์"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            PrivateHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.2.5"
                }),
                text = JArray.FromObject(new
                {
                    value = "เครื่องตรวจอวัยวะภายในชนิดสนามแม่เหล็กไฟฟ้า"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            PrivateHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.2.6"
                }),
                text = JArray.FromObject(new
                {
                    value = "เครื่องสลายนิ่ว"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            PrivateHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.2.7"
                }),
                text = JArray.FromObject(new
                {
                    value = "เครื่องล้างไต"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            PrivateHospital.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.2.8"
                }),
                text = JArray.FromObject(new
                {
                    value = "อื่น ๆ"
                }),
                answer = JObject.FromObject(new
                {
                    valueString = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });

            var HealthcareInArea = new JArray();
            HealthcareInArea.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.1"
                }),
                text = JArray.FromObject(new
                {
                    value = "สถานพยาบาลของรัฐ"
                }),
                item = PublicHospital
            });
            HealthcareInArea.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7.2"
                }),
                text = JArray.FromObject(new
                {
                    value = "สถานพยาบาลของเอกชน"
                }),
                item = PrivateHospital
            });

            var ThaiTraditionalMedicine = new JArray();
            ThaiTraditionalMedicine.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.7.1"
                }),
                text = JArray.FromObject(new
                {
                    value = "เวชกรรมไทย"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            ThaiTraditionalMedicine.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.7.2"
                }),
                text = JArray.FromObject(new
                {
                    value = "เภสัชกรรมไทย"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            ThaiTraditionalMedicine.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.7.3"
                }),
                text = JArray.FromObject(new
                {
                    value = "แพทย์แผนไทยประยุกต์"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            ThaiTraditionalMedicine.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.7.4"
                }),
                text = JArray.FromObject(new
                {
                    value = "การผดุงครรภ์ไทย"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            ThaiTraditionalMedicine.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.7.5"
                }),
                text = JArray.FromObject(new
                {
                    value = "การนวดไทย"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            ThaiTraditionalMedicine.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.7.6"
                }),
                text = JArray.FromObject(new
                {
                    value = "การแพทย์พื้นบ้านไทย"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            ThaiTraditionalMedicine.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.7.7"
                }),
                text = JArray.FromObject(new
                {
                    value = "การแพทย์พื้นบ้านไทย"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });

            var ArtPractitioner = new JArray();
            ArtPractitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.8.1"
                }),
                text = JArray.FromObject(new
                {
                    value = "กิจกรรมบำบัด"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                }),
            });
            ArtPractitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.8.2"
                }),
                text = JArray.FromObject(new
                {
                    value = "การแก้ไขความผิดปกติของการสื่อความหมาย"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                }),
            });
            ArtPractitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.8.3"
                }),
                text = JArray.FromObject(new
                {
                    value = "เทคโนโลยีหัวใจและทรวงอก"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                }),
            });
            ArtPractitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.8.4"
                }),
                text = JArray.FromObject(new
                {
                    value = "รังสีเทคนิค"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                }),
            });
            ArtPractitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.8.5"
                }),
                text = JArray.FromObject(new
                {
                    value = "จิตวิทยาคลินิกอก"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                }),
            });
            ArtPractitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.8.6"
                }),
                text = JArray.FromObject(new
                {
                    value = "กายอุปกรณ์"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                }),
            });
            ArtPractitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.8.7"
                }),
                text = JArray.FromObject(new
                {
                    value = "การแพทย์แผนจีน"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                }),
            });
            ArtPractitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.8.8"
                }),
                text = JArray.FromObject(new
                {
                    value = "อื่น ๆ"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                }),
            });

            var practitioner = new JArray();
            practitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.1"
                }),
                text = JObject.FromObject(new
                {
                    value = "แพทย์"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            practitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.2"
                }),
                text = JObject.FromObject(new
                {
                    value = "พยาบาล"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            practitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.3"
                }),
                text = JObject.FromObject(new
                {
                    value = "ทันตแพทย์"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            practitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.4"
                }),
                text = JObject.FromObject(new
                {
                    value = "เภสัขกร"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            practitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.5"
                }),
                text = JObject.FromObject(new
                {
                    value = "นักกายภาพบำบัด"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            practitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.6"
                }),
                text = JObject.FromObject(new
                {
                    value = "นักเทคนิคการแพทย์"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            practitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.7"
                }),
                text = JObject.FromObject(new
                {
                    value = "แพทย์แผนไทย"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                }),
                item = ThaiTraditionalMedicine
            });
            practitioner.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9.8"
                }),
                text = JObject.FromObject(new
                {
                    value = "ผู้ประกอบโรคศิลปะ"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                }),
                item = ArtPractitioner
            });

            var Professionals = new JArray();
            Professionals.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "10.1"
                }),
                text = JArray.FromObject(new
                {
                    value = "ผู้ดำเนินการสถานพยาบาล"
                }),
                answer = JObject.FromObject(new
                {
                    valueString = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            Professionals.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "10.2"
                }),
                text = JArray.FromObject(new
                {
                    value = "ผู้อำนวยการฝ่ายการแพทย์"
                }),
                answer = JObject.FromObject(new
                {
                    valueString = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            Professionals.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "10.3"
                }),
                text = JArray.FromObject(new
                {
                    value = "ผู้อำนวยการฝ่ายการพยาบาล"
                }),
                answer = JObject.FromObject(new
                {
                    valueString = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            Professionals.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "10.4"
                }),
                text = JArray.FromObject(new
                {
                    value = "อื่น ๆ"
                }),
                answer = JObject.FromObject(new
                {
                    valueString = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });


            var itemArr = new JArray();
            itemArr.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "1"
                }),
                text = JArray.FromObject(new
                {
                    value = "สถานพยาบาลมีลักษณะเป็น"
                }),
                answer = TypeHospital
            });
            itemArr.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "2"
                }),
                text = JArray.FromObject(new
                {
                    value = "จำนวนเตียง"
                }),
                answer = JObject.FromObject(new
                {
                    valueInteger = JObject.FromObject(new
                    {
                        value = string.Empty // ขนาด
                    })
                })
            });
            itemArr.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "3"
                }),
                text = JArray.FromObject(new
                {
                    value = "บริการที่จัดให้มีเพิ่มเติม"
                }),
                answer = JObject.FromObject(new
                {
                    valueString = JObject.FromObject(new
                    {
                        value = request.Data.TryGetData("APP_HOSPITAL_PLAN_INFO_SECTION").Data.ThenGetStringData("APP_HOSPITAL_PLAN_INFO_SECTION_MED_TYPE_APP_HOSPITAL_PLAN_MED_TYPE_1")
                    })
                })
            });
            itemArr.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "4"
                }),
                text = JArray.FromObject(new
                {
                    value = "ลักษณะอาคารสถานพยาบาล"
                }),
                answer = TypeBuilding
            });
            itemArr.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "5"
                }),
                text = JArray.FromObject(new
                {
                    value = "งบลงทุน"
                }),
                answer = JObject.FromObject(new
                {
                    valueDecimal = JObject.FromObject(new
                    {
                        value = string.Empty // เงินลงทุนทั้งหมด
                    })
                }),
                item = JObject.FromObject(new
                {
                    linkId = JObject.FromObject(new
                    {
                        id = "5.1"
                    }),
                    text = JArray.FromObject(new
                    {
                        value = "แหล่งเงินทุนจาก"
                    }),
                    item = SourceOfFunds
                })
            });
            itemArr.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "6"
                }),
                text = JArray.FromObject(new
                {
                    value = "พื้นที่บริการครอบคลุม"
                }),
                item = ServicesArea
            });
            itemArr.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "7"
                }),
                text = JArray.FromObject(new
                {
                    value = "สถานพยาบาลของรัฐและเอกชนในพื้นที่บริการ"
                }),
                item = HealthcareInArea
            });
            itemArr.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "8"
                }),
                text = JArray.FromObject(new
                {
                    value = "ปัญหาการบริการรักษาพยาบาลในพื้นที่ที่ครอบคลุม ซึ่งเป็นเหตุให้สมควรลงทุน"
                }),
                answer = JObject.FromObject(new
                {
                    valueString = JObject.FromObject(new
                    {
                        value = string.Empty
                    })
                })
            });
            itemArr.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "9"
                }),
                text = JArray.FromObject(new
                {
                    value = "จำนวนของผู้ประกอบวิชาชีพที่จะมาปฏิบัติงาน"
                }),
                item = practitioner
            });
            itemArr.Add(new
            {
                linkId = JObject.FromObject(new
                {
                    id = "10"
                }),
                text = JArray.FromObject(new
                {
                    value = "ผู้ประกอบวิชาชีพที่จะมาปฏิบัติงานในตำแหน่งที่สำคัญ"
                }),
                item = Professionals
            });

            return JObject.FromObject(new
            {
                QuestionnaireResponse = JObject.FromObject(new
                {
                    extension = extensionVersion,
                    subject = JObject.FromObject(new
                    {
                        display = JObject.FromObject(new
                        {
                            value = request.PermitName
                        }),
                    }),
                    author = JObject.FromObject(new
                    {
                        display = JObject.FromObject(new
                        {
                            value = request.IdentityName
                        })
                    }),
                    item = itemArr,
                })
            });
        }
    }
}

