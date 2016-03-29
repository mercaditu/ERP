using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows;
namespace entity
{
  
    public class Execustionstrategy : DbExecutionStrategy
    {
        public bool IsHandeled { get; set; }

        /// <summary>
        /// The default retry limit is 5, which means that the total amount of time spent 
        /// between retries is 26 seconds plus the random factor.
        /// </summary>
        public Execustionstrategy()
        {
            IsHandeled = false;
        }

        /// <summary>
        /// Creates a new instance of "PharylonExecutionStrategy" with the specified limits for
        /// number of retries and the delay between retries.
        /// </summary>
        /// <param name="maxRetryCount"> The maximum number of retry attempts. </param>
        /// <param name="maxDelay"> The maximum delay in milliseconds between retries. </param>
        public Execustionstrategy(int maxRetryCount, TimeSpan maxDelay) : base (maxRetryCount, maxDelay)
        {
            IsHandeled = false;
        }

        protected override bool ShouldRetryOn(Exception ex)
        {
            bool retry = false;
           
                MySqlException sqlException = ex as MySqlException;

                if (sqlException != null)
                {
                    int[] errorsToRetry =
                {
                    1042  //Deadlock
                          //Timeout
                };

                    if (errorsToRetry.Contains(sqlException.Number))
                    {
                        if (IsHandeled == false)
                        {
                            AboutBox1 box = new AboutBox1();
                            box.ShowDialog();

                            IsHandeled = true;
                        }

                        retry = true;
                    }
                    else
                    {
                        //Add some error logging on this line for errors we aren't retrying.
                        //Make sure you record the Number property of sqlError. 
                        //If you see an error pop up that you want to retry, you can look in 
                        //your log and add that number to the list above.
                    }
                }

                if (ex is TimeoutException)
                {
                    
                    retry = true;
                }

            return retry;
        }
    } 
}
