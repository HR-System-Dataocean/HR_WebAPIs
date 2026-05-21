using Microsoft.Data.SqlClient;
using System.Data;

namespace VenusHR.Infrastructure.Presistence.SelfService
{
    internal static class VacationBalanceCalculator
    {
        public static decimal CalculateAnnualVacationDays(
            System.Data.Common.DbConnection connection,
            int contractId,
            int employeeId,
            DateTime dateToCheck,
            int annualVacationTypeId = 1)
        {
            var joinDate = GetEmployeeJoinDate(connection, employeeId);
            if (joinDate == null)
                return 0;

            var use360DayYear = GetCompanyUses360DayYear(connection);
            var dueDate = joinDate.Value;
            double remainingBalance = 0;

            var openBalance = GetOpenBalance(connection, employeeId, annualVacationTypeId);
            if (openBalance.HasValue)
            {
                if (openBalance.Value.GBalanceDate >= dueDate)
                {
                    dueDate = openBalance.Value.GBalanceDate;
                    remainingBalance = openBalance.Value.Days;
                }
            }

            var lastVacation = GetLastAnnualVacation(connection, employeeId);
            if (lastVacation != null)
            {
                var lastVacationDate = lastVacation.ActualEndDate ?? lastVacation.ExpectedEndDate;
                if (lastVacationDate.HasValue && lastVacationDate.Value > DateTime.MinValue)
                {
                    if (openBalance == null || lastVacationDate.Value > openBalance.Value.GBalanceDate)
                    {
                        dueDate = lastVacationDate.Value;
                        remainingBalance = lastVacation.RemainingDays;
                    }
                }
            }

            var settlement = GetLastVacationSettlement(connection, employeeId);
            if (settlement != null && settlement.PaidDate > dueDate)
            {
                dueDate = settlement.PaidDate;
                remainingBalance = settlement.RemainVacDaySettlement;
            }

            int workingDaysSinceDue = use360DayYear
                ? Days360(dueDate, dateToCheck)
                : (int)(dateToCheck - dueDate).TotalDays;

            int allWorkingDays = use360DayYear
                ? Days360(joinDate.Value, dateToCheck)
                : (int)(dateToCheck - joinDate.Value).TotalDays;

            int workingDaysFromStart = use360DayYear
                ? Days360(joinDate.Value, dueDate)
                : (int)(dueDate - joinDate.Value).TotalDays;

            workingDaysSinceDue = Math.Max(workingDaysSinceDue, 0);

            var contractVacations = GetContractVacations(connection, contractId, dateToCheck);
            double dueDays = CalculateDueDaysFromTiers(
                contractVacations,
                workingDaysSinceDue,
                allWorkingDays,
                workingDaysFromStart);

            return Convert.ToDecimal(dueDays + remainingBalance);
        }

        public static decimal RoundVacationBalance(decimal balance)
        {
            var intPart = decimal.Truncate(balance);
            var decimalPart = balance - intPart;
            var roundedDecimal = decimalPart <= 0.499m ? 0m : 1m;
            return intPart + roundedDecimal;
        }

        public static int? GetValidContractId(System.Data.Common.DbConnection connection, int employeeId, DateTime asOfDate)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT TOP 1 ID
                FROM hrs_Contracts
                WHERE EmployeeID = @EmployeeId
                  AND CancelDate IS NULL
                  AND StartDate <= @AsOfDate
                  AND (EndDate IS NULL OR EndDate >= @AsOfDate)
                ORDER BY StartDate DESC";
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));
            cmd.Parameters.Add(new SqlParameter("@AsOfDate", asOfDate));
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? null : Convert.ToInt32(result);
        }

        private static DateTime? GetEmployeeJoinDate(System.Data.Common.DbConnection connection, int employeeId)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT JoinDate FROM hrs_Employees WHERE ID = @EmployeeId AND CancelDate IS NULL";
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? null : Convert.ToDateTime(result);
        }

        private static bool GetCompanyUses360DayYear(System.Data.Common.DbConnection connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT TOP 1 ISNULL(RegComputerID, 0)
                FROM sys_Companies
                WHERE ID = (SELECT TOP 1 ID FROM sys_Companies ORDER BY ID)";
            var result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value && Convert.ToInt32(result) == 360;
        }

        private static (DateTime GBalanceDate, double Days)? GetOpenBalance(
            System.Data.Common.DbConnection connection, int employeeId, int vacationTypeId)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT TOP 1 GBalanceDate, ISNULL(Days, 0)
                FROM hrs_EmployeeVacationOpenBalance
                WHERE EmployeeID = @EmployeeId
                  AND VacationTypeID = @VacationTypeId
                  AND CancelDate IS NULL
                ORDER BY GBalanceDate DESC";
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));
            cmd.Parameters.Add(new SqlParameter("@VacationTypeId", vacationTypeId));
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;
            return (reader.GetDateTime(0), Convert.ToDouble(reader.GetValue(1)));
        }

        private static LastVacationRow? GetLastAnnualVacation(System.Data.Common.DbConnection connection, int employeeId)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT TOP 1 ActualStartDate, ActualEndDate, ExpectedEndDate,
                       ISNULL(RemainingDays, 0), ISNULL(ConsumDays, 0), ISNULL(TotalDays, 0)
                FROM hrs_EmployeesVacations
                WHERE EmployeeID = @EmployeeId
                  AND CancelDate IS NULL
                  AND VacationTypeID IN (SELECT ID FROM hrs_VacationsTypes WHERE IsAnnual = 1)
                ORDER BY ActualStartDate DESC";
            cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            return new LastVacationRow
            {
                ActualEndDate = reader.IsDBNull(1) ? null : reader.GetDateTime(1),
                ExpectedEndDate = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                RemainingDays = Convert.ToDouble(reader.GetValue(3))
            };
        }

        private static SettlementRow? GetLastVacationSettlement(System.Data.Common.DbConnection connection, int employeeId)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "GetEmployeeLastVacationSettlement";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@EmployeeID", employeeId));
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            return new SettlementRow
            {
                PaidDate = reader.IsDBNull(reader.GetOrdinal("PaidDate"))
                    ? DateTime.MinValue
                    : reader.GetDateTime(reader.GetOrdinal("PaidDate")),
                RemainVacDaySettlement = reader.IsDBNull(reader.GetOrdinal("RemainVacDaySettlement"))
                    ? 0
                    : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("RemainVacDaySettlement")))
            };
        }

        private static List<ContractVacationTier> GetContractVacations(
            System.Data.Common.DbConnection connection, int contractId, DateTime dateToCheck)
        {
            var tiers = new List<ContractVacationTier>();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "hrs_GetContractsVacations";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ContractId", contractId));
            cmd.Parameters.Add(new SqlParameter("@CheckDate", dateToCheck));
            cmd.Parameters.Add(new SqlParameter("@All", 1));

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tiers.Add(new ContractVacationTier
                {
                    DurationDays = reader.IsDBNull(reader.GetOrdinal("DurationDays"))
                        ? 0
                        : Convert.ToInt32(reader.GetValue(reader.GetOrdinal("DurationDays"))),
                    ToMonth = reader.IsDBNull(reader.GetOrdinal("ToMonth"))
                        ? 99999
                        : Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ToMonth"))),
                    RequiredWorkingMonths = reader.IsDBNull(reader.GetOrdinal("RequiredWorkingMonths"))
                        ? 0
                        : Convert.ToInt32(reader.GetValue(reader.GetOrdinal("RequiredWorkingMonths")))
                });
            }

            return tiers;
        }

        private static double CalculateDueDaysFromTiers(
            List<ContractVacationTier> tiers,
            int workingDaysSinceDue,
            int allWorkingDays,
            int workingDaysFromStart)
        {
            if (tiers.Count == 0)
                return 0;

            double dueDays = 0;
            int tDays = workingDaysSinceDue;
            int workingDayFromStart = allWorkingDays;
            int workingDaysToDueDate = workingDaysFromStart;

            for (int index = 0; index < tiers.Count; index++)
            {
                var tier = tiers[index];
                if (tier.RequiredWorkingMonths <= 0)
                    return 0;

                if (index == tiers.Count - 1)
                {
                    if (tDays > 0)
                        dueDays += ((double)tDays / tier.RequiredWorkingMonths) * tier.DurationDays;
                }
                else
                {
                    if (workingDayFromStart > tier.ToMonth)
                    {
                        if (workingDaysToDueDate > tier.ToMonth)
                            continue;

                        if (workingDaysToDueDate + tDays > tier.ToMonth)
                        {
                            var remainingDueDays = tier.ToMonth - workingDaysToDueDate;
                            dueDays += ((double)remainingDueDays / tier.RequiredWorkingMonths) * tier.DurationDays;
                            tDays -= remainingDueDays;
                            continue;
                        }

                        if (workingDayFromStart == tDays)
                        {
                            tDays -= tier.RequiredWorkingMonths;
                            dueDays = tier.DurationDays;
                        }
                        else if (workingDayFromStart > tier.ToMonth)
                        {
                            dueDays += tier.DurationDays;
                            tDays -= tier.RequiredWorkingMonths;
                        }

                        continue;
                    }

                    dueDays += ((double)tDays / tier.RequiredWorkingMonths) * tier.DurationDays;
                    tDays = 0;
                }
            }

            return dueDays;
        }

        private static int Days360(DateTime startDate, DateTime endDate)
        {
            var d1 = startDate.Day == 31 ? 30 : startDate.Day;
            var d2 = endDate.Day == 31 && (startDate.Day == 30 || startDate.Day == 31) ? 30 : endDate.Day;
            return (360 * (endDate.Year - startDate.Year)) + (30 * (endDate.Month - startDate.Month)) + (d2 - d1);
        }

        private sealed class LastVacationRow
        {
            public DateTime? ActualEndDate { get; set; }
            public DateTime? ExpectedEndDate { get; set; }
            public double RemainingDays { get; set; }
        }

        private sealed class SettlementRow
        {
            public DateTime PaidDate { get; set; }
            public double RemainVacDaySettlement { get; set; }
        }

        private sealed class ContractVacationTier
        {
            public int DurationDays { get; set; }
            public int ToMonth { get; set; }
            public int RequiredWorkingMonths { get; set; }
        }
    }
}
