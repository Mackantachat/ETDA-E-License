using BizPortal.DAL.MongoDB;
using BizPortal.Utils;
using BizPortal.Utils.Extensions;
using BizPortal.Utils.Helpers;
using BizPortal.ViewModels.Apps.HSSStandard;
using BizPortal.ViewModels.SingleForm;
using Mapster;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BizPortal.AppsHook
{
    public class HSSClinicOperationEditAppHook : StoreBaseAppHook
    {
        public override decimal? CalculateFee(List<ISectionData> sectionData)
        {
            return 250;
        }
        public override bool IsEnabledChat()
        {
            return true;
        }

        public override JObject GenerateELicenseData(Guid applicationrequestId)
        {
            var data = (object)null;
            var request = ApplicationRequestEntity.Get(applicationrequestId);

            var files = request.UploadedFiles.Where(e => e.Description == "APP_CLINIC_OPERATION_EDIT_B").FirstOrDefault();
            var profilePhotoMeta = files.Files.Where(e => e.FileTypeCode == "APP_CLINIC_OPERATION_EDIT_B_DOCC").FirstOrDefault().Adapt<BizPortal.ViewModels.V2.FileMetadata>();
            var practitionerPhoto = profilePhotoMeta.ContentType.ToLower().Contains("image") ? Convert.ToBase64String(profilePhotoMeta.GetBytes()) : "";
            var practitionerPhotoContentType = profilePhotoMeta.ContentType;
            var practitionerPhotoUrl = "";
            var practitionerPhotoSize = profilePhotoMeta.FileSize;
            var practitionerPhotoTitle = profilePhotoMeta.FileName;

            var relateIdentityId = request.IdentityID;
            var relateLicenseNumber = request.Data.TryGetData("APP_HOSPITAL_OPERATION_EDIT_B_SECA").ThenGetStringData("APP_HOSPITAL_OPERATION_EDIT_B_SECA_ID"); // เลขที่ใบอนุญาตที่ใช้อ้างอิง (เลขที่ใบเก่า)
            var requestLicenseOriginal = ApplicationRequestEntity.GetRelateLicense(relateLicenseNumber, relateIdentityId);
            var permitName = request.PermitName;
            var dataLicensor = request.IdentityName;
            var createDocumentDate = DateTime.Now.ToString("dd MMMM yyyy", new CultureInfo("th"));
            var licenseNumber = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("Identifier");
            var clinicName = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("INFORMATION_STORE_NAME_TH");
            var clinicBuildingNumber = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_INFORMATION_STORE__ADDRESS");
            var clinicMoo = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_MOO_INFORMATION_STORE__ADDRESS");
            var clinicSoi = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_SOI_INFORMATION_STORE__ADDRESS");
            var clinicStreet = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_ROAD_INFORMATION_STORE__ADDRESS");
            var clinicTumbol = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_TUMBOL_INFORMATION_STORE__ADDRESS");
            var clinicDistrict = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_AMPHUR_INFORMATION_STORE__ADDRESS");
            var clinicProvince = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_PROVINCE_INFORMATION_STORE__ADDRESS");
            var clinicTumbolName = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_TUMBOL_INFORMATION_STORE__ADDRESS_TEXT");
            var clinicDistrictName = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_AMPHUR_INFORMATION_STORE__ADDRESS_TEXT");
            var clinicProvinceName = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_PROVINCE_INFORMATION_STORE__ADDRESS_TEXT");
            var clinicPostCode = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_POSTCODE_INFORMATION_STORE__ADDRESS");
            var phoneNo = string.IsNullOrEmpty(request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_TELEPHONE_EXT_INFORMATION_STORE__ADDRESS")) ?
                request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_TELEPHONE_INFORMATION_STORE__ADDRESS") :
                string.Format("{0} ต่อ {1}",
                    request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_TELEPHONE_INFORMATION_STORE__ADDRESS"),
                    request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_TELEPHONE_EXT_INFORMATION_STORE__ADDRESS"));
            var clinicPhone = phoneNo.ToArabicDigit();
            var clinicFax = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_FAX_INFORMATION_STORE__ADDRESS");
            var clinicEmail = request.Data.TryGetData("INFORMATION_STORE").ThenGetStringData("ADDRESS_EMAIL_INFORMATION_STORE__ADDRESS");
            var clinicRenewType = "ไม่รับผู้ป่วยค้างคืน";
            var clinicRenewCharacteristics = request.Data.TryGetData("APP_HOSPITAL_OPERATION_EDIT_B_SECD").ThenGetStringData("DROPDOWN_APP_HOSPITAL_OPERATION_EDIT_B_SECD_CONC_TEXT");

            var oparetorName = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("IdentifierName");
            var professionalLicenseNumber = request.Data.TryGetData("APP_HOSPITAL_OPERATION_EDIT_B_SECA").ThenGetStringData("APP_HOSPITAL_OPERATION_EDIT_B_SECA_CONJ");
            var fieldPractitionerBranch = request.Data.TryGetData("APP_HOSPITAL_OPERATION_EDIT_B_SECA").ThenGetStringData("DROPDOWN_APP_HOSPITAL_OPERATION_EDIT_B_SECA_CONH_TEXT") + " " +
                                          request.Data.TryGetData("APP_HOSPITAL_OPERATION_EDIT_B_SECA").ThenGetStringData("DROPDOWN_APP_HOSPITAL_OPERATION_EDIT_B_SECA_CONI_TEXT");
            var fieldPractitionerLicenseDate = request.Data.TryGetData("APP_HOSPITAL_OPERATION_EDIT_B_SECA").ThenGetStringData("APP_HOSPITAL_OPERATION_EDIT_B_SECA_CONK");

            int totalDate = 0;
            var getOpenServicesDate = request.Data.TryGetData("APP_HOSPITAL_OPERATION_EDIT_B_SECE").Data;
            var openServicesDate = string.Empty;
            if (int.TryParse(getOpenServicesDate["APP_HOSPITAL_OPERATION_EDIT_B_SECE_TOTAL"], out totalDate))
            {
                for (int i = 0; i < totalDate; i++)
                {
                    openServicesDate = openServicesDate + getOpenServicesDate["DROPDOWN_APP_HOSPITAL_OPERATION_EDIT_B_SECE_CONA_TEXT_" + i] + " " + getOpenServicesDate["DROPDOWN_APP_HOSPITAL_OPERATION_EDIT_B_SECE_CONB_TEXT_" + i] + " - " + getOpenServicesDate["DROPDOWN_APP_HOSPITAL_OPERATION_EDIT_B_SECE_CONC_TEXT_" + i] + " น. , ";
                }
                openServicesDate = openServicesDate.Substring(0, openServicesDate.Length - 2);
            }
            else
            {
                throw new NullReferenceException("Cannot parse APP_HOSPITAL_OPERATION_EDIT_B_SECE_TOTAL to int");
            }


            var permitDate = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("PeriodStart");
            var permitEnd = request.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("PeriodEnd");
            var permitDateOld = requestLicenseOriginal.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("PermitDate");
            var permitEndOld = "๓๑ ธันวาคม พ.ศ. " + requestLicenseOriginal.Data.TryGetData("ELICENSE_INFORMATION").ThenGetStringData("PermitEndYear");
            data = new
            {
                Bundle = new
                {
                    id = new
                    {
                        attr_value = "Request-to-edit-a-license-to-operate-a-medical-facility-clinic"
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
                        attr_value = createDocumentDate.ToArabicDigit()
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
                                    attr_value = "ส.พ.19"
                                }
                            },
                            subject = new {
                                reference = new {
                                    attr_value = "https://schemas.teda.th/practitionerrole/pr1"
                                }
                            },
                            date = new {
                                attr_value = createDocumentDate.ToArabicDigit()
                            },
                            author = new {
                                reference = new {
                                    attr_value = "Practitioner/p2"
                                }
                            },
                            title = new {
                                attr_value = permitName.ToArabicDigit()
                            },
                            relatesTo = new {
                                code = new {
                                    attr_id = "EDIT",
                                    attr_value = "replaces"
                                },
                                targetIdentifier = new {
                                    attr_id = relateLicenseNumber.ToArabicDigit(),
                                    peroid = new {
                                        start = new
                                        {
                                            attr_value = permitDateOld.ToArabicDigit()
                                        },
                                        end = new
                                        {
                                            attr_value = permitEndOld.ToArabicDigit()
                                        }
                                    }
                                }
                            }
                        }
                        }

                    },
                    new {
                        fullUrl = new
                        {
                            attr_value = "https://schemas.teda.th/practitionerrole/pr1"
                        },
                        resource = new
                        {
                            PractitionerRole = new
                            {
                                id = new
                                {
                                    attr_value = "pr1"
                                },
                                period = new
                                {
                                    start = new
                                    {
                                        attr_value = permitDate.ToArabicDigit()
                                    },
                                    end = new
                                    {
                                        attr_value = permitEnd.ToArabicDigit()
                                    }
                                },
                                practitioner = new
                                {
                                    reference = new
                                    {
                                        attr_value = "https://schemas.teda.th/Practitioner/p1"
                                    }
                                },
                                healthcareService = new
                                {
                                    reference = new
                                    {
                                        attr_value = "https://schemas.teda.th/healthcareservice/h1"
                                    }
                                },
                                availableTime = new
                                {
                                    modifierExtension = new
                                    {
                                        attr_url = "https://schemas.teda.th/availableTime/at1",
                                        valueString = new
                                        {
                                            attr_value = openServicesDate.ToArabicDigit()
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new {
                        fullUrl = new
                        {
                            attr_value = "https://schemas.teda.th/Practitioner/p1"
                        },
                        resource = new
                        {
                            Practitioner = new
                            {
                                id = new
                                {
                                    attr_value = "p1"
                                },
                                name = new
                                {
                                    text = new
                                    {
                                        attr_value = oparetorName.ToArabicDigit()
                                    }
                                },
                                photo = new
                                {
                                    contentType = new
                                    {
                                        attr_value = practitionerPhotoContentType.ToArabicDigit()
                                    },
                                    url = new
                                    {
                                        attr_value = practitionerPhotoUrl.ToArabicDigit()
                                    },
                                    size = new
                                    {
                                        attr_value = practitionerPhotoSize
                                    },
                                    title = new
                                    {
                                        attr_value = practitionerPhotoTitle.ToArabicDigit()
                                    }
                                },
                                qualification = new
                                {
                                    identifier = new
                                    {
                                        attr_id = professionalLicenseNumber.ToArabicDigit(),
                                        value = new
                                        {
                                            attr_value = fieldPractitionerBranch.ToArabicDigit()
                                        }
                                    },
                                    code = new
                                    {
                                        text = new
                                        {
                                            attr_value = "CER"
                                        }
                                    },
                                    period = new
                                    {
                                        start = new
                                        {
                                            attr_value = fieldPractitionerLicenseDate.ToArabicDigit().ToMonthNameThai()
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new {
                        fullUrl = new
                        {
                            attr_value = "https://schemas.teda.th/healthcareservice/h1"
                        },
                        resource = new
                        {
                            HealthcareService = new
                            {
                                id = new
                                {
                                    attr_value = "h1"
                                },
                                providedBy = new
                                {
                                    reference = new
                                    {
                                        attr_value = "https://schemas.teda.th/Organization/o1"
                                    }
                                },
                                type = new
                                {
                                    text = new
                                    {
                                        attr_value = clinicRenewType.ToArabicDigit() //ประเภทสถานพยาบาล
                                    }
                                },
                                specialty = new
                                {
                                    text = new
                                    {
                                        attr_value = "-" // จำนวนเตียง ถ้ามี
                                    }
                                },
                                comment = new
                                {
                                    attr_value = clinicRenewCharacteristics.ToArabicDigit()  //ลักษณะสถานพยาบาล
                                },
                                photo = new
                                {
                                    contentType = new
                                    {
                                        attr_value = "" //"image/gif"
                                    },
                                    data = new
                                    {
                                        attr_value = "" //"MTIzNA=="
                                    }
                                }
                            }
                        }
                    },
                    new {
                        fullUrl = new
                        {
                            attr_value = "https://schemas.teda.th/Organization/o1"
                        },
                        resource = new
                        {
                            Organization = new
                            {
                                id = new
                                {
                                    attr_value = "o1"
                                },
                                name = new
                                {
                                    attr_value = clinicName.ToArabicDigit()
                                },
                                address = new
                                {
                                    text = new
                                    {
                                        attr_value = "ระบุที่ตั้งสถานประกอบการแบบ Unstructure"
                                    },
                                    line = new object[] {
                                        new {
                                            attr_id = "BuildingNumber",
                                            attr_value = clinicBuildingNumber.ToArabicDigit()
                                        },
                                        new {
                                            attr_id = "Moo",
                                            attr_value = clinicMoo.ToArabicDigit()
                                        },
                                        new {
                                            attr_id = "Soi",
                                            attr_value = clinicSoi.ToArabicDigit()
                                        },
                                        new {
                                            attr_id = "Street",
                                            attr_value = clinicStreet.ToArabicDigit()
                                        }
                                    },
                                    city = new
                                    {
                                        attr_value = clinicTumbol.ToArabicDigit(),
                                        attr_valueString = clinicTumbolName.ToArabicDigit()
                                    },
                                    district = new
                                    {
                                        attr_value = clinicDistrict.ToArabicDigit(),
                                        attr_valueString = clinicDistrictName.RemoveTextDistrict().ToArabicDigit()
                                    },
                                    state = new
                                    {
                                        attr_value = clinicProvince.ToArabicDigit(),
                                        attr_valueString = clinicProvinceName.ToArabicDigit()
                                    },
                                    postalCode = new
                                    {
                                        attr_value = clinicPostCode.ToArabicDigit()
                                    },
                                    country = new
                                    {
                                        attr_value = "TH"
                                    }
                                },
                                contact = new
                                {
                                    telecom = new object[] {
                                        new {
                                            system = new {
                                                attr_value = "phone"
                                            },
                                            value = new {
                                                attr_value = clinicPhone.ToArabicDigit()
                                            },
                                            use = new {
                                                attr_value = "work"
                                            }
                                        },
                                        new {
                                            system = new
                                            {
                                                attr_value = "fax"
                                            },
                                            value = new
                                            {
                                                attr_value = clinicFax.ToArabicDigit()
                                            },
                                            use = new
                                            {
                                                attr_value = "work"
                                            }
                                        },
                                        new {
                                            system = new
                                            {
                                                attr_value = "email"
                                            },
                                            value = new
                                            {
                                                attr_value = clinicEmail.ToArabicDigit()
                                            },
                                            use = new
                                            {
                                                attr_value = "work"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new {
                        fullUrl = new
                        {
                            attr_value = "https://schemas.teda.th/Practitioner/p2"
                        },
                                resource = new
                                {
                                    Practitioner = new
                                    {
                                        id = new
                                        {
                                            attr_value = "p2"
                                        },
                                        name = new
                                        {
                                            text = new
                                            {
                                                attr_value = dataLicensor.ToArabicDigit()
                                            }
                                        }
                                    }
                                }
                            }
                    },
                    
                },//Bundle
                Images = new
                {
                    PersonPhoto = practitionerPhoto
                }
            }; // data

            var json = JObject.FromObject(data);

            return JObject.FromObject(data);
        }
    }
}
