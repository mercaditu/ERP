namespace entity
{
    public class DataConfiguration : System.Data.Entity.DbConfiguration
    {
        public DataConfiguration()
        {
            //in ctor, call the config methods

            //for Azure, retry common transient exceptions
            SetExecutionStrategy("MySql.Data.MySqlClient", () => new Execustionstrategy(20, new System.TimeSpan(30000)));
        }
    }
}