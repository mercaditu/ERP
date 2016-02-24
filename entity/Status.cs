namespace entity
{
    public static class Status
    {

        public enum ReturnTypes
        {
            ItemDefect = 1,
            ItemExpired = 2,
            Discount = 3,
            Bonus = 4,
            Error = 5,
        }

        public enum Documents
        {
            Pending,
            Issued,
            Returned
        }

        public enum Documents_General
        {
            Pending = 1,
            Approved = 2,
            Annulled = 3,
            Rejected = 4
        }

        public enum Production
        {
            Pending = 1,
            Approved = 2,
            InProcess = 3,
            Executed = 4,
            QA_Check = 5,
            QA_Rejected = 6
        }

        public enum Project
        {
            Pending = 1,
            Approved = 2,
            InProcess = 3,
            Executed = 4,
            Rejected = 5
        }

        public enum Stock
        {
            OnTheWay = 1,
            InStock = 2,
            Reserved = 3
        }
    }
}
