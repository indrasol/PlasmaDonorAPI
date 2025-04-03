using NLog.Fluent;
using OfficeOpenXml;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Dto.Dashboard;
using Serilog;
using System.Linq;
using NewPlasmaDonorsAPI.utils;

namespace NewPlasmaDonorsAPI.Services.excel
{
    public class ExcelProcessor
    {
        // Excel MIME type for .xlsx files
        public static string TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        // Define headers as static list
        static List<string> HEADERS = new List<string>
            {
                "email", "first_name", "last_name", "phone_number", "gender", "dob",
                "is_donor", "is_influencer", "address_line1", "city", "state", "country", "postal_code",
                "latitude", "longitude", "education", "school_attended", "influenced_by", "relationship",
                "is_relationship_active", "race", "occupation", "language", "home_center"
            };

        // Method to check if the file is in the correct Excel format
        public bool HasExcelFormat(IFormFile file)
        {
            return file.ContentType.Equals(TYPE, StringComparison.OrdinalIgnoreCase);
        }

        // Method to convert Excel file rows to list of ProfileDto objects
        public List<ProfileDto> ExcelToProfiles(IFormFile xlFile)
        {
            var profiles = new List<ProfileDto>();

            using (var stream = new MemoryStream())
            {
                xlFile.CopyTo(stream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {

                    var worksheet = package.Workbook.Worksheets[0]; // Assume first worksheet
                    var headers = GetHeaderMap(worksheet);

                    for (int i = 2; i <= worksheet.Dimension.Rows; i++) // Skipping header row
                    {
                        var row = worksheet.Cells[i, 1, i, worksheet.Dimension.Columns];
                        var profile = new ProfileDto
                        {
                            email = GetCellValueAsString(row[headers["email"], 1]),
                            firstName = GetCellValueAsString(row[headers["first_name"], 1]),
                            lastName = GetCellValueAsString(row[headers["last_name"], 1]),
                            gender = GetCellValueAsString(row[headers["gender"], 1]),
                            dob = DateUtils.ToShortString(ConvertToDate(GetCellValueAsString(row[headers["dob"], 1]))),
                            phoneNumber = GetCellValueAsString(row[headers["phone_number"], 1]),
                            isDonor = ConvertToBoolean(GetCellValueAsString(row[headers["is_donor"], 1])),
                            isInfluencer = ConvertToBoolean(GetCellValueAsString(row[headers["is_influencer"], 1])),
                            addressLine1 = GetCellValueAsString(row[headers["address_line1"], 1]),
                            city = GetCellValueAsString(row[headers["city"], 1]),
                            state = GetCellValueAsString(row[headers["state"], 1]),
                            country = GetCellValueAsString(row[headers["country"], 1]),
                            postalCode = GetCellValueAsString(row[headers["postal_code"], 1]),
                            latitude = ConvertToDouble(GetCellValueAsString(row[headers["latitude"], 1])),
                            longitude = ConvertToDouble(GetCellValueAsString(row[headers["longitude"], 1])),
                            education = GetCellValueAsString(row[headers["education"], 1]),
                            schoolAttended = GetCellValueAsString(row[headers["school_attended"], 1]),
                            influencedBy = GetCellValueAsString(row[headers["influenced_by"], 1]),
                            relationship = GetCellValueAsString(row[headers["relationship"], 1]),
                            race = GetCellValueAsString(row[headers["race"], 1]),
                            occupation = GetCellValueAsString(row[headers["occupation"], 1]),
                            language = GetCellValueAsString(row[headers["language"], 1]),
                            homeCenter = GetCellValueAsString(row[headers["home_center"], 1])

                        };

                        Serilog.Log.Information(":: Profile Info: " + profile);
                        profiles.Add(profile);
                    }
                }
            }

            return profiles;
        }


        private Dictionary<string, int> GetHeaderMap(ExcelWorksheet worksheet)
        {
            var headers = new Dictionary<string, int>();

            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            {
                var header = worksheet.Cells[1, col].Text.ToLower();
                if (HEADERS.Contains(header))
                {
                    headers[header] = col;
                }
            }

            return headers;
        }

        private string GetCellValueAsString(ExcelRangeBase cell)
        {
            return cell?.Text ?? string.Empty;
        }

        private bool ConvertToBoolean(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            return bool.TryParse(value, out bool result) && result;
        }

        private double ConvertToDouble(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            return double.TryParse(value, out double result) ? result : 0;
        }

        private DateTime? ConvertToDate(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return DateTime.TryParse(value, out DateTime result) ? result : (DateTime?)null;
        }
    }
}
