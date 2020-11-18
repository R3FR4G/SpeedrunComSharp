﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace SpeedrunComSharp
{
    public class RunStatus
    {
        public RunStatusType Type { get; private set; }
        public string ExaminerUserID { get; private set; }
        public string Reason { get; private set; }
        public DateTime? VerifyDate { get; private set; }

        #region Links

        private Lazy<User> examiner;

        public User Examiner { get { return examiner.Value; } }

        #endregion

        private RunStatus() { }

        private static RunStatusType ParseType(string type)
        {
            switch (type)
            {
                case "new":
                    return RunStatusType.New;
                case "verified":
                    return RunStatusType.Verified;
                case "rejected":
                    return RunStatusType.Rejected;
            }

            throw new ArgumentException("type");
        }

        public static RunStatus Parse(SpeedrunComClient client, dynamic statusElement)
        {
            RunStatus status = new RunStatus();
            IDictionary<string, dynamic> properties = statusElement as IDictionary<string, dynamic>;

            status.Type = ParseType(statusElement.status as string);

            if (status.Type == RunStatusType.Rejected || status.Type == RunStatusType.Verified)
            {
                status.ExaminerUserID = properties["examiner"] as string;
                status.examiner = new Lazy<User>(() => client.Users.GetUser(status.ExaminerUserID));

                if (status.Type == RunStatusType.Verified)
                {
                    if(properties.TryGetValue("verify-date", out dynamic date))
                    {
                        status.VerifyDate = (DateTime)date;
                    }
                }
            }
            else
            {
                status.examiner = new Lazy<User>(() => null);
            }

            if (status.Type == RunStatusType.Rejected)
            {
                status.Reason = statusElement.reason as string;
            }

            return status;
        }

        public override string ToString()
        {
            if (Type == RunStatusType.Rejected)
            {
                return "Rejected:" + Reason;
            }
            else
            {
                return Type.ToString();
            }
        }
    }
}
