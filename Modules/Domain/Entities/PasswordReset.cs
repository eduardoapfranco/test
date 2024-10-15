using Domain.Enum;
using Infra.CrossCutting.Repository;
using System;

namespace Domain.Entities
{
    public class PasswordReset : BaseEntityDates<long>
    {
        public PasswordReset(int userId, string userEmail)
        {
            UserId = userId;
            UserEmail = userEmail;
            Active = (byte) PasswordResetMobileEnum.YES;
            Used = (byte) PasswordResetMobileEnum.NO;
            CheckerNumber = GenerateCheckerNumber();
        }

        public PasswordReset(int userId, string userEmail, byte used, int checkerNumber)
        {
            UserId = userId;
            UserEmail = userEmail;
            Active = (byte)PasswordResetMobileEnum.YES;
            Used = used;
            CheckerNumber = checkerNumber;
        }

        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public int CheckerNumber { get; set; }
        public byte Active { get; set; }
        public byte Used { get; set; }

        private int GenerateCheckerNumber()
        {
            Random rnd = new Random();
            var result = "";
            for (int i = 0; i < 6; i++)
            {
                result += rnd.Next(1, 9);
            }
            return int.Parse(result);           
        }

    }
}
